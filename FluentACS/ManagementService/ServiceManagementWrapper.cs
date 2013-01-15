using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Services.Client;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.IdentityModel.Protocols.WSFederation.Metadata;
using EntityDescriptor = Microsoft.IdentityModel.Protocols.WSFederation.Metadata.EntityDescriptor;

namespace FluentACS.ManagementService
{
    public class ServiceManagementWrapper
    {
        private readonly string serviceIdentityPasswordForManagement;
        private readonly string serviceIdentityUsernameForManagement;
        private readonly string serviceNamespace;
        private static string cachedSwtToken;

        public ServiceManagementWrapper(string serviceNamespace, string serviceIdentityUsernameForManagement, string serviceIdentityPasswordForManagement)
        {
            this.serviceNamespace = serviceNamespace;
            this.serviceIdentityUsernameForManagement = serviceIdentityUsernameForManagement;
            this.serviceIdentityPasswordForManagement = serviceIdentityPasswordForManagement;
        }

        public Issuer AddFacebookIdentityProvider(string displayName, string facebookAppId, string facebookAppSecret)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                var issuer = new Issuer
                                 {
                                     Name = string.Format(CultureInfo.InvariantCulture, "{0}-{1}", "Facebook", facebookAppId)
                                 };

                client.AddToIssuers(issuer);
                client.SaveChanges(SaveChangesOptions.Batch);

                var facebook = new IdentityProvider
                                   {
                                       DisplayName = displayName, 
                                       LoginLinkName = "Facebook", 
                                       LoginParameters = "email", 
                                       WebSSOProtocolType = WebSSOProtocolType.Facebook.ToString(), 
                                       IssuerId = issuer.Id
                                   };

                client.AddObject("IdentityProviders", facebook);

                var facebookKeys = new[]
                                       {
                                           new IdentityProviderKey
                                               {
                                                   IdentityProvider = facebook, 
                                                   StartDate = DateTime.UtcNow, 
                                                   EndDate = DateTime.UtcNow.AddYears(1), 
                                                   Type = KeyType.ApplicationKey.ToString(), 
                                                   Usage = KeyUsage.ApplicationId.ToString(), 
                                                   Value = Encoding.Default.GetBytes(facebookAppId)
                                               }, 
                                           new IdentityProviderKey
                                               {
                                                   IdentityProvider = facebook, 
                                                   StartDate = DateTime.UtcNow, 
                                                   EndDate = DateTime.UtcNow.AddYears(1), 
                                                   Type = KeyType.ApplicationKey.ToString(), 
                                                   Usage = KeyUsage.ApplicationSecret.ToString(), 
                                                   Value = Encoding.Default.GetBytes(facebookAppSecret)
                                               }
                                       };

                foreach (var key in facebookKeys)
                {
                    client.AddRelatedObject(facebook, "IdentityProviderKeys", key);
                }

                client.SaveChanges(SaveChangesOptions.Batch);

                return issuer;
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public Issuer AddYahooIdentityProvider()
        {
            return this.AddIdentityProviderManually("Yahoo!", "https://open.login.yahooapis.com/openid/op/auth", WebSSOProtocolType.OpenId);
        }

        public Issuer AddGoogleIdentityProvider()
        {
            return this.AddIdentityProviderManually("Google", "https://www.google.com/accounts/o8/ud", WebSSOProtocolType.OpenId);
        }

        public Issuer AddIdentityProvider(string displayName, byte[] fedMetadata)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                string metadataImporter = string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}/{2}", this.serviceNamespace, Constants.AcsHostName, Constants.MetadataImportHead);

                var postRequest = (HttpWebRequest)WebRequest.Create(metadataImporter);
                postRequest.Method = "POST";

                this.AttachTokenWithWritePermissions(postRequest);

                using (Stream postStream = postRequest.GetRequestStream())
                {
                    for (int i = 0; i < fedMetadata.Length; i++)
                    {
                        postStream.WriteByte(fedMetadata[i]);
                    }
                }

                HttpWebResponse resp;
                try
                {
                    resp = (HttpWebResponse)postRequest.GetResponse();
                }
                catch (WebException e)
                {
                    resp = (HttpWebResponse)e.Response;

                    string responseHtml;
                    using (var stream = resp.GetResponseStream())
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            responseHtml = reader.ReadToEnd();
                        }
                    }

                    throw new WebException(responseHtml, e);
                }

                string identityProviderName;
                var serializer = new MetadataSerializer();

                using (var memoryStream = new MemoryStream(fedMetadata))
                {
                    var metadata = serializer.ReadMetadata(memoryStream) as EntityDescriptor;
                    identityProviderName = metadata.EntityId.Id;
                }

                var newIdentityProvider = client.IdentityProviders.Where(idp => idp.DisplayName == identityProviderName).FirstOrDefault();
                if (newIdentityProvider == null)
                {
                    throw new InvalidOperationException("Identity Provider: " + identityProviderName + " does not exist");
                }

                // Update display name
                newIdentityProvider.DisplayName = displayName;
                client.UpdateObject(newIdentityProvider);
                client.SaveChanges();

                // Update and return issuer
                Issuer issuer = client.Issuers.Where(i => i.Name == identityProviderName).FirstOrDefault();
                if (issuer == null)
                {
                    throw new InvalidOperationException("Issuer: " + identityProviderName + " does not exist");
                }

                return issuer;
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public Issuer AddIdentityProviderManually(string displayName, string federationUrl, WebSSOProtocolType protocolType, byte[] signingValidationCert = null, string[] allowedRelyingParties = null)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                var defaultStartDate = DateTime.UtcNow;
                var defaultEndDate = defaultStartDate.AddYears(1);

                var issuer = new Issuer
                {
                    Name = displayName
                };

                var oldIssuer = client.Issuers.Where(ip => ip.Name == issuer.Name).FirstOrDefault();
                if (oldIssuer != null)
                {
                    client.DeleteObject(oldIssuer);
                }

                client.AddToIssuers(issuer);
                client.SaveChanges(SaveChangesOptions.Batch);

                var identityProvider = new IdentityProvider
                {
                    DisplayName = displayName,
                    WebSSOProtocolType = protocolType.ToString(),
                    LoginLinkName = displayName,
                    IssuerId = issuer.Id
                };

                var oldIdentityProvider = client.IdentityProviders.Where(ip => ip.DisplayName.Equals(identityProvider.DisplayName, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();
                if (oldIdentityProvider != null)
                {
                    client.DeleteObject(oldIdentityProvider);
                    client.SaveChanges();
                }

                client.AddToIdentityProviders(identityProvider);
                client.SaveChanges(SaveChangesOptions.Batch);

                // Identity provider public key to verify the signature
                if (signingValidationCert != null)
                {
                    var key = new IdentityProviderKey
                    {
                        IdentityProviderId = identityProvider.Id,
                        DisplayName = "Signing Key for " + displayName,
                        StartDate = defaultStartDate,
                        EndDate = defaultEndDate,
                        Type = KeyType.X509Certificate.ToString(),
                        Usage = KeyUsage.Signing.ToString(),
                        Value = signingValidationCert
                    };

                    client.AddToIdentityProviderKeys(key);
                    client.SaveChanges(SaveChangesOptions.Batch);
                }

                // WS-Federation sign-in URL
                var federationSignInAddress = new IdentityProviderAddress
                {
                    IdentityProviderId = identityProvider.Id,
                    EndpointType = EndpointType.SignIn.ToString(),
                    Address = federationUrl
                };

                client.AddToIdentityProviderAddresses(federationSignInAddress);
                client.SaveChanges(SaveChangesOptions.Batch);

                return issuer;
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddPassThroughRuleToRuleGroup(string ruleGroupName, Issuer issuer, string inputClaimType = null, string outputClaimType = null)
        {
            try
            {
                var rule = new Rule
                {
                    InputClaimType = inputClaimType,
                    OutputClaimType = outputClaimType
                };

                this.AddRuleToRuleGroup(ruleGroupName, issuer, rule);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddPassThroughRuleToRuleGroup(string ruleGroupName, string identityProviderName, string inputClaimType = null, string outputClaimType = null)
        {
            try
            {
                var rule = new Rule
                               {
                                   InputClaimType = inputClaimType, 
                                   OutputClaimType = outputClaimType
                               };

                this.AddRuleToRuleGroup(ruleGroupName, identityProviderName, rule);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddRelyingParty(string relyingPartyName, string realmAddress, string replyAddress, byte[] signingCert = null, string signingCertPassword = null, byte[] encryptionCert = null, string ruleGroupName = "", string[] allowedIdentityProviders = null)
        {
            this.AddRelyingPartyWithAsymmetricKey(relyingPartyName, realmAddress, replyAddress, signingCert, signingCertPassword, encryptionCert, ruleGroupName, allowedIdentityProviders);
        }

        public void AddRelyingPartyWithAsymmetricKey(string relyingPartyName, string realmAddress, string replyAddress, 
            byte[] signingCert = null, string signingCertPassword = "", byte[] encryptionCert = null, string ruleGroupName = "", 
            string[] allowedIdentityProviders = null)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                var defaultStartDate = DateTime.UtcNow;
                var defaultEndDate = defaultStartDate.AddYears(1);
                var asymmetricTokenEncryptionRequired = encryptionCert != null;

                RelyingParty relyingParty;
                CreateRelyingParty(client, relyingPartyName, ruleGroupName, realmAddress, string.Empty, TokenType.SAML_2_0, Constants.RelyingPartyTokenLifetime, asymmetricTokenEncryptionRequired, out relyingParty);

                // Create the Reply for Relying Party
                var reply = new RelyingPartyAddress
                                {
                                    Address = replyAddress, 
                                    EndpointType = RelyingPartyAddressEndpointType.Reply.ToString(), 
                                    RelyingParty = relyingParty
                                };

                client.AddRelatedObject(relyingParty, "RelyingPartyAddresses", reply);

                client.SaveChanges();

                if (signingCert != null)
                {
                    AddSigningKeyToRelyingParty(client, relyingPartyName, signingCert, signingCertPassword, defaultStartDate, defaultEndDate, relyingParty);
                }

                if (asymmetricTokenEncryptionRequired)
                {
                    AddEncryptionKeyToRelyingParty(client, relyingPartyName, encryptionCert, defaultStartDate, defaultEndDate, relyingParty);
                }

                client.SaveChanges();

                AddIdentityProviderToRelyingParty(client, allowedIdentityProviders, relyingParty);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddRelyingPartyWithKey(string relyingPartyName, string realmAddress, string replyAddress, byte[] symmetricKey,
            TokenType tokenType, int tokenLifetime, 
            byte[] signingCert, string signingCertPassword, DateTime? signingStartDate, DateTime? signingEndDate,
            byte[] encryptionCert, string ruleGroupName, string[] allowedIdentityProviders)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                var defaultStartDate = DateTime.UtcNow;
                var defaultEndDate = defaultStartDate.AddYears(1);
                var asymmetricTokenEncryptionRequired = encryptionCert != null;

                RelyingParty relyingParty;
                CreateRelyingParty(client, relyingPartyName, ruleGroupName, realmAddress, string.Empty, tokenType, tokenLifetime, 
                    asymmetricTokenEncryptionRequired, out relyingParty);

                // Create the Reply for Relying Party
                var reply = new RelyingPartyAddress
                {
                    Address = replyAddress,
                    EndpointType = RelyingPartyAddressEndpointType.Reply.ToString(),
                    RelyingParty = relyingParty
                };

                client.AddRelatedObject(relyingParty, "RelyingPartyAddresses", reply);

                client.SaveChanges();

                if (signingCert != null)
                {
                    AddSigningKeyToRelyingParty(client, relyingPartyName, signingCert, signingCertPassword, signingStartDate.Value, signingEndDate.Value, relyingParty);
                }
                
                if (symmetricKey != null)
                {
                    AddSigningKeyToRelyingParty(client, relyingPartyName, symmetricKey, defaultStartDate, defaultEndDate, relyingParty);
                }

                if (asymmetricTokenEncryptionRequired)
                {
                    AddEncryptionKeyToRelyingParty(client, relyingPartyName, encryptionCert, defaultStartDate, defaultEndDate, relyingParty);
                }

                client.SaveChanges();

                AddIdentityProviderToRelyingParty(client, allowedIdentityProviders, relyingParty);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddRelyingPartyWithSymmetricKey(string relyingPartyName, string realmAddress, byte[] symmetricKey, string ruleGroupName = "", string[] allowedIdentityProviders = null)
        {
            this.AddRelyingPartyWithSymmetricKey(relyingPartyName, realmAddress, string.Empty, symmetricKey, TokenType.SAML_2_0, Constants.RelyingPartyTokenLifetime, ruleGroupName, allowedIdentityProviders);
        }

        public void AddRelyingPartyWithSymmetricKey(string relyingPartyName, string realmAddress, string replyAddress, 
            byte[] symmetricKey, TokenType tokenType, int tokenLifetime, string ruleGroupName = "", string[] allowedIdentityProviders = null)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                var defaultStartDate = DateTime.UtcNow;
                var defaultEndDate = defaultStartDate.AddYears(1);

                RelyingParty relyingParty;
                CreateRelyingParty(client, relyingPartyName, ruleGroupName, realmAddress, replyAddress, tokenType, tokenLifetime, false, out relyingParty);

                if (symmetricKey != null)
                {
                    AddSigningKeyToRelyingParty(client, relyingPartyName, symmetricKey, defaultStartDate, defaultEndDate, relyingParty);
                }

                client.SaveChanges();

                AddIdentityProviderToRelyingParty(client, allowedIdentityProviders, relyingParty);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddRuleGroup(string name)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                var ruleGroup = new RuleGroup
                                    {
                                        Name = name
                                    };

                client.AddToRuleGroups(ruleGroup);
                client.SaveChanges();
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddServiceIdentity(string name, string secret)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                var defaultStartDate = DateTime.UtcNow;
                var defaultEndDate = defaultStartDate.AddYears(1);

                var serviceIdentity = new ServiceIdentity
                                          {
                                              Name = name
                                          };

                client.AddToServiceIdentities(serviceIdentity);

                var serviceIdentityKey = new ServiceIdentityKey
                                             {
                                                 DisplayName = "Credentials for " + name, 
                                                 Value = Encoding.UTF8.GetBytes(secret), 
                                                 Type = IdentityKeyTypes.Password.ToString(), 
                                                 Usage = IdentityKeyUsages.Password.ToString(), 
                                                 StartDate = defaultStartDate, 
                                                 EndDate = defaultEndDate
                                             };

                client.AddRelatedObject(serviceIdentity, "ServiceIdentityKeys", serviceIdentityKey);
                client.SaveChanges(SaveChangesOptions.Batch);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddServiceKey(string displayName, byte[] keyValue, string protectionPassword, KeyType keyType, KeyUsage keyUsage)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                var defaultStartDate = DateTime.UtcNow;
                var defaultEndDate = defaultStartDate.AddYears(1);

                var serviceKey = new ServiceKey
                                     {
                                         DisplayName = displayName, 
                                         Type = keyType.ToString(), 
                                         Usage = keyUsage.ToString(), 
                                         Value = keyValue, 
                                         Password = string.IsNullOrEmpty(protectionPassword) ? null : new UTF8Encoding().GetBytes(protectionPassword), 
                                         StartDate = defaultStartDate, 
                                         EndDate = defaultEndDate, 
                                         IsPrimary = true
                                     };

                client.AddToServiceKeys(serviceKey);
                client.SaveChanges();
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddSimpleRuleToRuleGroup(string description, string ruleGroupName, string identityProviderName, string inputClaimType = null, string inputClaimValue = null, string outputClaimType = null, string outputClaimValue = null)
        {
            try
            {
                var rule = new Rule
                               {
                                   Description = description,
                                   InputClaimType = inputClaimType, 
                                   InputClaimValue = inputClaimValue, 
                                   OutputClaimType = outputClaimType, 
                                   OutputClaimValue = outputClaimValue
                               };

                this.AddRuleToRuleGroup(ruleGroupName, identityProviderName, rule);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void AddSimpleRuleToRuleGroupWithoutSpecifyInputClaim(string ruleGroupName, string identityProviderName, string outputClaimType, string outputClaimValue)
        {
            this.AddSimpleRuleToRuleGroup(string.Empty, ruleGroupName, identityProviderName, outputClaimType: outputClaimType, outputClaimValue: outputClaimValue);
        }

        public ManagementService CreateManagementServiceClient()
        {
           
                var managementServiceEndpoint = string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}/{2}", this.serviceNamespace, Constants.AcsHostName, Constants.ManagementServiceHead);

                var managementService = new ManagementService(new Uri(managementServiceEndpoint))
                                            {
                                                IgnoreMissingProperties = true
                                            };
                managementService.SendingRequest += this.AttachTokenWithWritePermissions;

            return managementService;
        }

        public void RemoveAllRulesInGroup(string ruleGroupName)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                RuleGroup ruleGroup = client.RuleGroups.Expand("Rules").AddQueryOption("$filter", "Name eq '" + ruleGroupName + "'").FirstOrDefault();

                if (ruleGroup != null)
                {
                    foreach (Rule rule in ruleGroup.Rules)
                    {
                        client.DeleteObject(rule);
                    }

                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void RemoveIdentityProvider(string name)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                this.RemoveIssuer(name);

                IdentityProvider identityProvider = client.IdentityProviders
                    .Where(m => m.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (identityProvider != null)
                {
                    client.DeleteObject(identityProvider);
                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void RemoveIssuer(string name)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                Issuer issuer = client.Issuers
                    .Where(m => m.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (issuer == null)
                {
                    var identityProvider = client.IdentityProviders
                        .Expand("Issuer")
                        .Where(ip => ip.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase))
                        .FirstOrDefault();

                    if (identityProvider != null)
                    {
                        issuer = identityProvider.Issuer;
                    }
                }

                if (issuer != null)
                {
                    client.DeleteObject(issuer);
                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void RemoveRelyingParty(string relyingPartyName)
        {
            try
            {
                var client = this.CreateManagementServiceClient();

                RelyingParty relyingParty = client.RelyingParties
                    .Where(m => m.Name == relyingPartyName)
                    .FirstOrDefault();

                if (relyingParty != null)
                {
                    client.DeleteObject(relyingParty);
                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void RemoveRuleGroup(string name)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                RuleGroup ruleGroup = client.RuleGroups.AddQueryOption("$filter", "Name eq '" + name + "'").FirstOrDefault();

                if (ruleGroup != null)
                {
                    client.DeleteObject(ruleGroup);
                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void RemoveServiceIdentity(string name)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                var serviceIdentities = client.ServiceIdentities.Where(e => e.Name == name);

                if (serviceIdentities.Count() != 0)
                {
                    foreach (ServiceIdentity id in serviceIdentities)
                    {
                        client.DeleteObject(id);
                    }

                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public void RemoveServiceKey(string displayName)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                var serviceKeys = client.ServiceKeys.Where(e => e.DisplayName == displayName);

                if (serviceKeys.Count() != 0)
                {
                    foreach (ServiceKey key in serviceKeys)
                    {
                        client.DeleteObject(key);
                    }

                    client.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<IdentityProvider> RetrieveIdentityProviders()
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                return client.IdentityProviders.Expand("IdentityProviderAddresses,IdentityProviderClaimTypes,IdentityProviderKeys,Issuer");
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<Issuer> RetrieveIssuers()
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                return client.Issuers.ToList();
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<RelyingParty> RetrieveRelyingParties()
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                var rps = client.RelyingParties;
                return client.RelyingParties.Expand("RelyingPartyKeys,RelyingPartyAddresses,RelyingPartyIdentityProviders,RelyingPartyRuleGroups/RuleGroup");
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<RelyingPartyRuleGroup> RetrieveRuleGroups()
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                return client.RelyingPartyRuleGroups;
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<Rule> RetrieveRules(string ruleGroupName)
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                return client.Rules.Expand("RuleGroup,Issuer").Where(r => r.RuleGroup.Name == ruleGroupName);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<ServiceIdentity> RetrieveServiceIdentities()
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                return client.ServiceIdentities.Expand("ServiceIdentityKeys");
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        public IEnumerable<ServiceKey> RetrieveServiceKeys()
        {
            try
            {
                var client = this.CreateManagementServiceClient();
                return client.ServiceKeys;
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        private static void AddEncryptionKeyToRelyingParty(ManagementService client, string relyingPartyName, byte[] encryptionCert, DateTime defaultStartDate, DateTime defaultEndDate, RelyingParty relyingParty)
        {
            var relyingPartyKey = new RelyingPartyKey
                                      {
                                          DisplayName = "Encryption Certificate for " + relyingPartyName, 
                                          Type = KeyType.X509Certificate.ToString(), 
                                          Usage = KeyUsage.Encrypting.ToString(), 
                                          Value = encryptionCert, 
                                          RelyingParty = relyingParty, 
                                          StartDate = defaultStartDate, 
                                          EndDate = defaultEndDate
                                      };

            client.AddRelatedObject(relyingParty, "RelyingPartyKeys", relyingPartyKey);

            client.SaveChanges();
        }

        private static void AddIdentityProviderToRelyingParty(ManagementService client, string[] allowedIdentityProviders, RelyingParty relyingParty)
        {
            // if no allowed identity providers were set, allow all
            if (allowedIdentityProviders == null)
            {
                allowedIdentityProviders = client.IdentityProviders.ToList().Select(idp => idp.DisplayName).ToArray();
            }

            foreach (var allowedIdp in allowedIdentityProviders)
            {
                var idp = client.IdentityProviders
                    .Where(i => i.DisplayName.Equals(allowedIdp, StringComparison.OrdinalIgnoreCase))
                    .FirstOrDefault();

                if (idp != null)
                {
                    var rpidp = new RelyingPartyIdentityProvider
                                    {
                                        IdentityProviderId = idp.Id, 
                                        RelyingPartyId = relyingParty.Id
                                    };

                    client.AddToRelyingPartyIdentityProviders(rpidp);
                    client.SaveChanges();
                }
            }
        }

        private static void AddSigningKeyToRelyingParty(ManagementService client, string relyingPartyName, byte[] signingCert, string signingCertPassword, DateTime defaultStartDate, DateTime defaultEndDate, RelyingParty relyingParty)
        {
            var relyingPartyKey = new RelyingPartyKey
                                      {
                                          DisplayName = "Signing Certificate for " + relyingPartyName, 
                                          Type = KeyType.X509Certificate.ToString(), 
                                          Usage = KeyUsage.Signing.ToString(), 
                                          Value = signingCert, 
                                          Password = string.IsNullOrEmpty(signingCertPassword) ? null : new UTF8Encoding().GetBytes(signingCertPassword), 
                                          RelyingParty = relyingParty, 
                                          StartDate = defaultStartDate, 
                                          EndDate = defaultEndDate, 
                                          IsPrimary = true
                                      };

            client.AddRelatedObject(relyingParty, "RelyingPartyKeys", relyingPartyKey);

            client.SaveChanges();
        }

        private static void AddSigningKeyToRelyingParty(ManagementService client, string relyingPartyName, byte[] symmetricKey, DateTime defaultStartDate, DateTime defaultEndDate, RelyingParty relyingParty)
        {
            var relyingPartyKey = new RelyingPartyKey
                                      {
                                          DisplayName = "Signing Key for " + relyingPartyName, 
                                          Type = KeyType.Symmetric.ToString(), 
                                          Usage = KeyUsage.Signing.ToString(), 
                                          Value = symmetricKey, 
                                          RelyingParty = relyingParty, 
                                          StartDate = defaultStartDate, 
                                          EndDate = defaultEndDate,
                                          IsPrimary = true
                                      };

            client.AddRelatedObject(relyingParty, "RelyingPartyKeys", relyingPartyKey);

            client.SaveChanges();
        }

        private static void CreateRelyingParty(ManagementService client, string relyingPartyName, string ruleGroupName, string realmAddress, string replyAddress, TokenType tokenType, int tokenLifetime, bool asymmetricTokenEncryptionRequired, out RelyingParty relyingParty)
        {
            // Create Relying Party
            relyingParty = new RelyingParty
                               {
                                   Name = relyingPartyName, 
                                   DisplayName = relyingPartyName, 
                                   Description = relyingPartyName, 
                                   TokenType = tokenType.ToString(), 
                                   TokenLifetime = tokenLifetime, 
                                   AsymmetricTokenEncryptionRequired = asymmetricTokenEncryptionRequired
                               };

            client.AddObject("RelyingParties", relyingParty);
            client.SaveChanges();

            if (!string.IsNullOrWhiteSpace(ruleGroupName))
            {
                RuleGroup ruleGroup = client.RuleGroups.Where(rg => rg.Name.Equals(ruleGroupName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (ruleGroup == null)
                {
                    ruleGroup = new RuleGroup
                                    {
                                        Name = ruleGroupName
                                    };

                    client.AddToRuleGroups(ruleGroup);
                    client.SaveChanges();
                }

                var relyingPartyRuleGroup = new RelyingPartyRuleGroup
                                                {
                                                    RuleGroupId = ruleGroup.Id, 
                                                    RelyingParty = relyingParty
                                                };

                client.AddRelatedObject(relyingParty, "RelyingPartyRuleGroups", relyingPartyRuleGroup);
            }

            // Create the Realm for Relying Party
            var realm = new RelyingPartyAddress
                            {
                                Address = realmAddress, 
                                EndpointType = RelyingPartyAddressEndpointType.Realm.ToString(), 
                                RelyingParty = relyingParty
                            };

            client.AddRelatedObject(relyingParty, "RelyingPartyAddresses", realm);

            if (!string.IsNullOrEmpty(replyAddress))
            {
                var reply = new RelyingPartyAddress
                                {
                                    Address = replyAddress, 
                                    EndpointType = RelyingPartyAddressEndpointType.Reply.ToString(), 
                                    RelyingParty = relyingParty
                                };

                client.AddRelatedObject(relyingParty, "RelyingPartyAddresses", reply);
            }

            client.SaveChanges(SaveChangesOptions.Batch);
        }
        
        private static Exception TryGetExceptionDetails(Exception ex)
        {
            return
                string.IsNullOrWhiteSpace(cachedSwtToken)
                    ? ex
                    : new ServiceManagementException(ex, cachedSwtToken);
        }

        private void AddRuleToRuleGroup(string ruleGroupName, Issuer issuer, Rule rule)
        {
            var client = this.CreateManagementServiceClient();

            RuleGroup ruleGroup = client.RuleGroups.AddQueryOption("$filter", "Name eq '" + ruleGroupName + "'").FirstOrDefault();
            if (ruleGroup == null)
            {
                throw new InvalidOperationException("Rule Group: " + ruleGroupName + " does not exist");
            }

            rule.IssuerId = issuer.Id;
            rule.RuleGroup = ruleGroup;

            client.AddRelatedObject(ruleGroup, "Rules", rule);
            client.SaveChanges();
        }

        private void AddRuleToRuleGroup(string ruleGroupName, string identityProviderName, Rule rule)
        {
            var client = this.CreateManagementServiceClient();

            RuleGroup ruleGroup = client.RuleGroups.AddQueryOption("$filter", "Name eq '" + ruleGroupName + "'").FirstOrDefault();
            if (ruleGroup == null)
            {
                throw new InvalidOperationException("Rule Group: " + ruleGroupName + " does not exist");
            }

            Issuer issuer;

            if (identityProviderName.Equals("LOCAL AUTHORITY"))
            {
                issuer = client.Issuers.Where(m => m.Name == "LOCAL AUTHORITY").Single();
            }
            else
            {
                IdentityProvider identityProvider = client.IdentityProviders.Expand("Issuer").Where(ip => ip.DisplayName.Equals(identityProviderName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                if (identityProvider == null)
                {
                    throw new InvalidOperationException("Identity Provider: " + identityProviderName + " does not exist");
                }

                issuer = identityProvider.Issuer;
            }

            rule.IssuerId = issuer.Id;
            rule.RuleGroup = ruleGroup;

            client.AddRelatedObject(ruleGroup, "Rules", rule);
            client.SaveChanges();
        }

        private void AttachTokenWithWritePermissions(object sender, SendingRequestEventArgs args)
        {
            this.AttachTokenWithWritePermissions((HttpWebRequest)args.Request);
        }

        private void AttachTokenWithWritePermissions(HttpWebRequest args)
        {
            if (cachedSwtToken == null)
            {
                cachedSwtToken = GetTokenFromACS();
            }

            args.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + cachedSwtToken);

            /*
            if (string.IsNullOrWhiteSpace(cachedSwtToken) ||
                DateTime.UtcNow.AddMinutes(5).CompareTo(GetExpiryTime(cachedSwtToken)) >= 0)
            {
                cachedSwtToken = this.GetTokenFromACS();
            }

            args.Headers.Add(HttpRequestHeader.Authorization, "WRAP access_token=\"" + HttpUtility.UrlDecode(cachedSwtToken) + "\"");
             * */
        }

        /// <summary>
        /// Obtains a SWT token from ACSv2. 
        /// </summary>
        /// <returns>A token  from ACS.</returns>
        public string GetTokenFromACS()
        {
            //
            // Request a token from ACS
            //
            WebClient client = new WebClient();
            client.BaseAddress = string.Format(CultureInfo.CurrentCulture, "https://{0}.{1}", this.serviceNamespace, Constants.AcsHostName);

            NameValueCollection values = new NameValueCollection();
            values.Add("grant_type", "client_credentials");
            values.Add("client_id", this.serviceIdentityUsernameForManagement);
            values.Add("client_secret", this.serviceIdentityPasswordForManagement);
            values.Add("scope", client.BaseAddress + "v2/mgmt/service/");

            byte[] responseBytes = client.UploadValues("/v2/OAuth2-13", "POST", values);

            //
            // Extract the access token and return it.
            //
            using (MemoryStream responseStream = new MemoryStream(responseBytes))
            {
                OAuth2TokenResponse tokenResponse = (OAuth2TokenResponse)new DataContractJsonSerializer(typeof(OAuth2TokenResponse)).ReadObject(responseStream);
                return tokenResponse.AccessToken;
            }
        }

        public void AddServiceIdentityWithCertificate(string name, byte[] encryptionCert, DateTime startdate, DateTime enddate)
        {
            try
            {
                var client = CreateManagementServiceClient();
                var serviceIdentity = new ServiceIdentity
                {
                    Name = name
                };

                client.AddToServiceIdentities(serviceIdentity);

                var serviceIdentityKey = new ServiceIdentityKey
                {
                    DisplayName = "Credentials for " + name,
                    Type = IdentityKeyTypes.X509Certificate.ToString(),
                    Usage = IdentityKeyUsages.Signing.ToString(),
                    Value = encryptionCert,
                    StartDate = startdate,
                    EndDate = enddate
                };

                client.AddRelatedObject(serviceIdentity, "ServiceIdentityKeys", serviceIdentityKey);
                client.SaveChanges(SaveChangesOptions.Batch);
            }
            catch (Exception ex)
            {
                throw TryGetExceptionDetails(ex);
            }
        }

        [DataContract]
        private class OAuth2TokenResponse
        {
            [DataMember(Name = "access_token")]
            public string AccessToken { get; set; }
        }
    }
}