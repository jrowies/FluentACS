namespace FluentACSTest
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using FluentACS;

    using Microsoft.IdentityModel.Claims;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentACS.Specs;

    [TestClass]
    public class IntegrationTests
    {
        private readonly AcsNamespaceDescription namespaceDesc = new AcsNamespaceDescription(
            ConfigurationManager.AppSettings["acsNamespace"],
            ConfigurationManager.AppSettings["acsUserName"],
            ConfigurationManager.AppSettings["acsPassword"]);

        private readonly string facebookAppId = ConfigurationManager.AppSettings["facebookAppId"];

        private readonly string facebookAppSecret = ConfigurationManager.AppSettings["facebookAppSecret"];

        [TestMethod]
        public void AddGoogleAndYahooIdentityProviders()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace
                .AddGoogleIdentityProvider()
                .AddYahooIdentityProvider();

            acsNamespace.SaveChanges(logInfo => Trace.WriteLine(logInfo.Message));

            Assert.IsTrue(AcsHelper.CheckIdentityProviderExists(this.namespaceDesc, "Google"));
            Assert.IsTrue(AcsHelper.CheckIdentityProviderExists(this.namespaceDesc, "Yahoo!"));
        }

        [TestMethod]
        public void AddFacebookIdentityProvider()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace
                .AddFacebookIdentityProvider(
                    ip => ip
                        .AppId(facebookAppId)
                        .AppSecret(facebookAppSecret)
                );

            acsNamespace.SaveChanges(logInfo => Trace.WriteLine(logInfo.Message));

            Assert.IsTrue(AcsHelper.CheckIdentityProviderExists(this.namespaceDesc, "Facebook"));
        }

        [TestMethod]
        public void AddFacebookIdentityProviderWithAdditionalPermissions()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace
                .AddFacebookIdentityProvider(
                    ip => ip
                        .AppId(facebookAppId)
                        .AppSecret(facebookAppSecret)
                        .WithApplicationPermission(FacebookApplicationPermission.UserPhotos)
                        .WithApplicationPermission(FacebookApplicationPermission.PublishStream)
                );

            acsNamespace.SaveChanges(logInfo => Trace.WriteLine(logInfo.Message));

            Assert.IsTrue(AcsHelper.CheckIdentityProviderExists(this.namespaceDesc, "Facebook"));
        }

        [TestMethod]
        public void AddVandelayIndustriesServiceIdentity()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddServiceIdentity(
                si => si
                    .Name("Vandelay Industries")
                    .Password("Passw0rd!"));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckServiceIdentityExists(this.namespaceDesc, "Vandelay Industries"));
        }

        [TestMethod]
        [DeploymentItem("testCert.cer")]
        public void AddVandelayIndustriesServiceIdentityWithX509()
        {
            var encryptionCert = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert.cer"));
            var acsNamespace = new AcsNamespace(namespaceDesc);
            var name = "Vandelay Industries X509";

            acsNamespace.AddServiceIdentityWithX509Certificate(
                si => si
                    .Name(name)
                    .EncryptionCertificate(encryptionCert)
                );

            acsNamespace.SaveChanges(logInfo => Trace.WriteLine(logInfo.Message));

            Assert.IsTrue(AcsHelper.CheckServiceIdentityExists(this.namespaceDesc, name));
        }

        [TestMethod]
        [DeploymentItem("testCert.cer")]
        public void AddVandelayIndustriesServiceIdentityWithX509FromFile()
        {
            var encryptionCert = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert.cer");
            var acsNamespace = new AcsNamespace(namespaceDesc);
            var name = "Vandelay Industries X509";

            acsNamespace.AddServiceIdentityWithX509Certificate(
                si => si
                    .Name(name).EncryptionCertificate(encryptionCert));

            acsNamespace.SaveChanges(logInfo => Trace.WriteLine(logInfo.Message));

            Assert.IsTrue(AcsHelper.CheckServiceIdentityExists(this.namespaceDesc, name));
        }

        [TestMethod]
        public void AddVandelayIndustriesServiceIdentityWithX509FromStore()
        {
            var acsNamespace = new AcsNamespace(namespaceDesc);
            var name = "Vandelay Industries X509";

            acsNamespace.AddServiceIdentityWithX509Certificate(
                si => si
                    .Name(name).EncryptionCertificateIdentifiedBy("66e0bc68570e30fba6207b1050ac72dc5b48cf47"));

            acsNamespace.SaveChanges(logInfo => Trace.WriteLine(logInfo.Message));

            Assert.IsTrue(AcsHelper.CheckServiceIdentityExists(this.namespaceDesc, name));
        }

        [TestMethod]
        public void AddMyCoolWebsiteRelyingParty()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name("MyCoolWebsite")
                    .RealmAddress("http://mycoolwebsite.com/")
                    .ReplyAddress("http://mycoolwebsite.com/")
                    .AllowGoogleIdentityProvider()
                    .AllowWindowsLiveIdentityProvider());

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "MyCoolWebsite"));
        }

        [TestMethod]
        public void AddMyCoolWebsiteRelyingPartyWithSwtTokenDetails()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name("MyCoolWebsite")
                    .RealmAddress("http://mycoolwebsite.com/")
                    .ReplyAddress("http://mycoolwebsite.com/")
                    .AllowGoogleIdentityProvider()
                    .AllowWindowsLiveIdentityProvider()
                    .SwtToken()
                    .TokenLifetime(120)
                    .SymmetricKey(Convert.FromBase64String("yMryA5VQVmMwrtuiJBfyjMnAJwoT7//fCuM6NwaHjQ1=")));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "MyCoolWebsite"));
            Assert.IsTrue(AcsHelper.CheckRelyingPartyHasKeys(this.namespaceDesc, "MyCoolWebsite", 1));
        }

        [TestMethod]
        [DeploymentItem("testCert_xyz.pfx")]
        [DeploymentItem("testCert.cer")]
        public void AddMyCoolWebsiteRelyingPartyWithSamlTokenDetails()
        {
            var encryptionCert = new X509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert.cer"));
            var signingCertBytes = this.ReadBytesFromPfxFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"));
            var temp = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"), "xyz");
            var startDate = temp.NotBefore.ToUniversalTime();
            var endDate = temp.NotAfter.ToUniversalTime();

            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name("MyCoolWebsite")
                    .RealmAddress("http://mycoolwebsite.com/")
                    .ReplyAddress("http://mycoolwebsite.com/")
                    .AllowGoogleIdentityProvider()
                    .AllowWindowsLiveIdentityProvider()
                    .SamlToken()
                    .TokenLifetime(120)
                    .SigningCertificate(sc => sc.Bytes(signingCertBytes).Password("xyz").StartDate(startDate).EndDate(endDate)) 
                    .EncryptionCertificate(encryptionCert.GetRawCertData()));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "MyCoolWebsite"));
            Assert.IsTrue(AcsHelper.CheckRelyingPartyHasKeys(this.namespaceDesc, "MyCoolWebsite", 2));
        }

        [TestMethod]
        [DeploymentItem("testCert.cer")]
        public void AddMyCoolWebsiteRelyingPartyWithSamlTokenDetailsWithX509CertificateFromFile()
        {
            var encryptionCert = new X509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert.cer"));
            var temp = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"), "xyz");

            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name("MyCoolWebsite with X509")
                    .RealmAddress("http://mycoolwebsitewithx509.com/")
                    .ReplyAddress("http://mycoolwebsitewithx509.com/")
                    .AllowGoogleIdentityProvider()
                    .EncryptionCertificate(encryptionCert));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "MyCoolWebsite with X509"));
            Assert.IsTrue(AcsHelper.CheckRelyingPartyHasKeys(this.namespaceDesc, "MyCoolWebsite with X509", 1));
        }

        [TestMethod]
        public void AddMyCoolWebsiteRelyingPartyWithRuleGroup()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name("MyCoolWebsite")
                    .RealmAddress("http://mycoolwebsite.com/")
                    .ReplyAddress("http://mycoolwebsite.com/")
                    .AllowGoogleIdentityProvider()
                    .AllowWindowsLiveIdentityProvider()
                    .RemoveRelatedRuleGroups()
                    .AddRuleGroup(rg => rg.Name("Rule Group for MyCoolWebsite Relying Party")));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "MyCoolWebsite"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupExists(this.namespaceDesc, "MyCoolWebsite", "Rule Group for MyCoolWebsite Relying Party"));
        }

        [TestMethod]
        public void AddMyCoolWebsiteRelyingPartyWithRuleGroupAndRules()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            
            const string MyCoolWebsite = "MyCoolWebsite";
            const string RuleGroupForMyCoolWebsiteRelyingParty = "Rule Group for MyCoolWebsite Relying Party";

            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name(MyCoolWebsite)
                    .RealmAddress("http://mycoolwebsite.com/")
                    .ReplyAddress("http://mycoolwebsite.com/")
                    .AllowGoogleIdentityProvider()
                    .AllowYahooIdentityProvider()
                    .AllowWindowsLiveIdentityProvider()
                    .RemoveRelatedRuleGroups()
                    .AddRuleGroup(rg => rg
                                .Name(RuleGroupForMyCoolWebsiteRelyingParty)
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

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, MyCoolWebsite));
            Assert.IsTrue(AcsHelper.CheckRuleGroupExists(this.namespaceDesc, MyCoolWebsite, RuleGroupForMyCoolWebsiteRelyingParty));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRules(this.namespaceDesc, MyCoolWebsite, RuleGroupForMyCoolWebsiteRelyingParty, 4));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, MyCoolWebsite, 
                RuleGroupForMyCoolWebsiteRelyingParty, "Google Passthrough"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, MyCoolWebsite, 
                RuleGroupForMyCoolWebsiteRelyingParty, "Yahoo! Passthrough"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, MyCoolWebsite, 
                RuleGroupForMyCoolWebsiteRelyingParty, "Windows Live ID rule"));
            Assert.IsTrue(AcsHelper.CheckRuleGroupHasRule(this.namespaceDesc, MyCoolWebsite, 
                RuleGroupForMyCoolWebsiteRelyingParty, "ACS rule"));
        }

        [TestMethod]
        public void AddMyCoolWebsiteLinkedToExistingRuleGroup()
        {
            var acsNamespace = new AcsNamespace(this.namespaceDesc);
            acsNamespace.AddRelyingParty(
                rp => rp
                    .Name("MyCoolWebsite")
                    .RealmAddress("http://mycoolwebsite.com/")
                    .ReplyAddress("http://mycoolwebsite.com/")
                    .AllowGoogleIdentityProvider()
                    .LinkToRuleGroup("Rule Group for MyCoolWebsite Relying Party"));

            acsNamespace.SaveChanges();

            Assert.IsTrue(AcsHelper.CheckRelyingPartyExists(this.namespaceDesc, "MyCoolWebsite"));
        }

        #region Helpers

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

        #endregion
    }
}
