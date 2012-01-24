namespace SampleApp
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    using FluentACS;

    using Microsoft.IdentityModel.Claims;

    class Program
    {
        static void Main(string[] args)
        {
            var namespaceDesc = new AcsNamespaceDescription(
                ConfigurationManager.AppSettings["acsNamespace"],
                ConfigurationManager.AppSettings["acsUserName"],
                ConfigurationManager.AppSettings["acsPassword"]);

            var encryptionCert = new X509Certificate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert.cer"));
            var signingCertBytes = ReadBytesFromPfxFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"));
            var temp = new X509Certificate2(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testCert_xyz.pfx"), "xyz");
            var startDate = temp.NotBefore.ToUniversalTime();
            var endDate = temp.NotAfter.ToUniversalTime();

            var acsNamespace = new AcsNamespace(namespaceDesc);

            acsNamespace
                .AddGoogleIdentityProvider()
                .AddYahooIdentityProvider()
                .AddServiceIdentity(
                    si => si
                        .Name("Vandelay Industries")
                        .Password("Passw0rd!"))
                .AddRelyingParty(
                    rp => rp
                        .Name("MyCoolWebsite")
                        .RealmAddress("http://mycoolwebsite.com/")
                        .ReplyAddress("http://mycoolwebsite.com/")
                        .AllowGoogleIdentityProvider()
                        .AllowWindowsLiveIdentityProvider()
                        .SamlToken()
                        .TokenLifetime(120)
                        .SigningCertificate(sc => sc.Bytes(signingCertBytes).Password("xyz").StartDate(startDate).EndDate(endDate)) 
                        .EncryptionCertificate(encryptionCert.GetRawCertData())
                        .RemoveRelatedRuleGroups()
                        .AddRuleGroup(rg => rg
                            .Name("Rule Group for MyCoolWebsite Relying Party")
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

            acsNamespace.SaveChanges(logInfo => Console.WriteLine(logInfo.Message));

            Console.ReadKey();
        }

        public static byte[] ReadBytesFromPfxFile(string pfxFileName)
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
