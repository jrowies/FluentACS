namespace FluentACSTest
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using FluentACS;

    using Microsoft.IdentityModel.Claims;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class FunctionalTests
    {
        // TODO: 
        // agregar en los commands error handling
        // habilitar opcion "verbose"
        // ext methods para assert?

        private readonly AcsNamespaceDescription namespaceDesc = new AcsNamespaceDescription(
            ConfigurationManager.AppSettings["acsNamespace"],
            ConfigurationManager.AppSettings["acsUserName"],
            ConfigurationManager.AppSettings["acsPassword"]);

        [TestMethod]
        public void AddGoogleAndYahooIdentityProviders()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.IdentityProviders
                .AddGoogle()
                .AddYahoo();

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckIdentityProviderExists(this.namespaceDesc, "Google"));
            Assert.IsTrue(AcsHelper.CheckIdentityProviderExists(this.namespaceDesc, "Yahoo!"));
        }

        [TestMethod]
        public void AddManInAVanServiceIdentity()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.ServiceIdentities.Add(
                si => si
                    .Name("Man in a Van")
                    .Password("Passw0rd!"));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckServiceIdentityExists(this.namespaceDesc, "Man in a Van"));
        }

        [TestMethod]
        public void AddOrdersWebsiteRelyingParty()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.RelyingParties.Add(
                rp => rp
                    .Name("Orders.Website")
                    .RealmAddress("http://127.0.0.1/")
                    .ReplyAddress("http://127.0.0.1/")
                    .AllowIdentityProvider("Google")
                    .AllowIdentityProvider("Windows Live ID"));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "Orders.Website"));
        }

        [TestMethod]
        public void AddOrdersWebsiteRelyingPartyWithSwtTokenDetails()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.RelyingParties.Add(
                rp => rp
                    .Name("Orders.Website")
                    .RealmAddress("http://127.0.0.1/")
                    .ReplyAddress("http://127.0.0.1/")
                    .AllowIdentityProvider("Google")
                    .AllowIdentityProvider("Windows Live ID")
                    .SwtToken()
                    .TokenLifetime(120)
                    .SymmetricKey(Convert.FromBase64String("yMryA5VQVmMwrtuiJBfyjMnAJwoT7//fCuM6NwaHjQ1=")));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "Orders.Website"));
            Assert.IsTrue(AcsHelper.CheckRelyingPartyHasKeys(this.namespaceDesc, "Orders.Website", 1));
        }

        [TestMethod]
        public void AddOrdersWebsiteRelyingPartyWithSamlTokenDetails()
        {
            var encryptionCert = new X509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert.cer"));
            var signingCertBytes = this.ReadBytesFromPfxFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"));
            var temp = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"), "xyz");
            var startDate = temp.NotBefore.ToUniversalTime();
            var endDate = temp.NotAfter.ToUniversalTime();

            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.RelyingParties.Add(
                rp => rp
                    .Name("Orders.Website")
                    .RealmAddress("http://127.0.0.1/")
                    .ReplyAddress("http://127.0.0.1/")
                    .AllowIdentityProvider("Google")
                    .AllowIdentityProvider("Windows Live ID")
                    .SamlToken()
                    .TokenLifetime(120)
                    .SigningCertificate(sc => sc.Bytes(signingCertBytes).Password("xyz").StartDate(startDate).EndDate(endDate)) 
                    .EncryptionCertificate(encryptionCert.GetRawCertData()));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "Orders.Website"));
            Assert.IsTrue(AcsHelper.CheckRelyingPartyHasKeys(this.namespaceDesc, "Orders.Website", 2));
        }

        [TestMethod]
        public void AddOrdersWebsiteRelyingPartyWithRuleGroup()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.RelyingParties.Add(
                rp => rp
                    .Name("Orders.Website")
                    .RealmAddress("http://127.0.0.1/")
                    .ReplyAddress("http://127.0.0.1/")
                    .AllowIdentityProvider("Google")
                    .AllowIdentityProvider("Windows Live ID")
                    .RemoveRelatedRuleGroups()
                    .AddRuleGroup(rg => rg.Name("Rule Group for Orders.Website Relying Party")));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "Orders.Website"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupExists(this.namespaceDesc, "Orders.Website", "Rule Group for Orders.Website Relying Party"));
        }

        [TestMethod]
        public void AddOrdersWebsiteRelyingPartyWithRuleGroupAndRules()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.RelyingParties.Add(
                rp => rp
                    .Name("Orders.Website")
                    .RealmAddress("http://127.0.0.1/")
                    .ReplyAddress("http://127.0.0.1/")
                    .AllowIdentityProvider("Google")
                    .AllowIdentityProvider("Yahoo!")
                    .AllowIdentityProvider("Windows Live ID")
                    .RemoveRelatedRuleGroups()
                    .AddRuleGroup(rg => rg
                                .Name("Rule Group for Orders.Website Relying Party")
                                .AddRule(
                                    rule => rule
                                        .Description("Google Passthrough")
                                        .IfInputClaimIssuer().Is("Google")
                                        .AndInputClaimType().IsOfType(ClaimTypes.Email)
                                        .AndInputClaimValue().IsAny()
                                        .ThenOutputClaimType().ShouldBe(ClaimTypes.Name)
                                        .AndOutputClaimValue().ShouldPassthroughFirstInputClaimValue())
                                .AddRule(
                                    rule => rule
                                        .Description("Yahoo! Passthrough")
                                        .IfInputClaimIssuer().Is("Yahoo!")
                                        .AndInputClaimType().IsAny()
                                        .AndInputClaimValue().IsAny()
                                        .ThenOutputClaimType().ShouldPassthroughFirstInputClaimType()
                                        .AndOutputClaimValue().ShouldPassthroughFirstInputClaimValue())
                                .AddRule(
                                    rule => rule
                                        .Description("Windows Live ID rule")
                                        .IfInputClaimIssuer().Is("Windows Live ID")
                                        .AndInputClaimType().IsOfType(ClaimTypes.Email)
                                        .AndInputClaimValue().Is("johndoe@hotmail.com")
                                        .ThenOutputClaimType().ShouldBe(ClaimTypes.NameIdentifier)
                                        .AndOutputClaimValue().ShouldBe("John Doe"))
                                .AddRule(
                                    rule => rule
                                        .Description("ACS rule")
                                        .IfInputClaimIssuer().IsAcs()
                                        .AndInputClaimType().IsAny()
                                        .AndInputClaimValue().IsAny()
                                        .ThenOutputClaimType().ShouldPassthroughFirstInputClaimType()
                                        .AndOutputClaimValue().ShouldPassthroughFirstInputClaimValue())));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "Orders.Website"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupExists(this.namespaceDesc, "Orders.Website", "Rule Group for Orders.Website Relying Party"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRules(this.namespaceDesc, "Orders.Website", "Rule Group for Orders.Website Relying Party", 4));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, "Orders.Website", 
                "Rule Group for Orders.Website Relying Party", "Google Passthrough"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, "Orders.Website", 
                "Rule Group for Orders.Website Relying Party", "Yahoo! Passthrough"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, "Orders.Website", 
                "Rule Group for Orders.Website Relying Party", "Windows Live ID rule"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, "Orders.Website", 
                "Rule Group for Orders.Website Relying Party", "ACS rule"));
        }

        [TestMethod]
        public void AddOrdersWebsiteLinkedToExistingRuleGroup()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.RelyingParties.Add(
                rp => rp
                    .Name("Orders.Website")
                    .RealmAddress("http://127.0.0.1/")
                    .ReplyAddress("http://127.0.0.1/")
                    .AllowIdentityProvider("Google")
                    .LinkToRuleGroup("Default Rule Group for Trey Research order acknowledgement for Man in a Van"));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "Orders.Website"));
        }

        public byte[] ReadBytesFromPfxFile(string pfxFileName)
        {
            byte[] signingCertificate;
            using (var stream = File.OpenRead(pfxFileName))
            {
                using (var br = new BinaryReader(stream))
                {
                    signingCertificate = br.ReadBytes((int)stream.Length);
                }
            }

            return signingCertificate;
        }
    }
}
