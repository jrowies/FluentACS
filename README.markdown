Fluent API for setting up Windows Azure ACS
-------------------------------------------

### How can this API help me?

You can use this fluent API for setting up, very quickly, an ACS namespace from scratch. The ACS artifacts that can be configured are identity providers, service identities, relying parties, rule groups and rules.

### When should I use it?

If you are developing a sample application, a spike or a POC, this API may come in handy for quickly get your ACS namespace up and running without having to deal with the [ACS Management Service API](http://msdn.microsoft.com/en-us/library/windowsazure/hh278947.aspx) (directly or through wrappers) or even with [PowerShell cmdlets](http://wappowershell.codeplex.com/). However, if this API fits your needs, you could use it in any place you want!

### How does it look like?

```c#
	var namespaceDesc = new AcsNamespaceDescription(
		"somenamespace", "ManagementClient", "T+bQtqP21BaCLO/8D1hanRdKJF8ZYEV8t32odxP4pYk=");

	var acsNamespace = new AcsNamespace(namespaceDesc);

	acsNamespace
		.AddGoogleIdentityProvider()
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
				.SwtToken()
				.TokenLifetime(120)
				.SymmetricKey(Convert.FromBase64String("yMryA5VQVmMwrtuiJBfyjMnAJwoT7//fCuM6NwaHjQ1="))
				.AddRuleGroup(rg => rg
					.Name("Rule Group for MyCoolWebsite Relying Party")
					.AddRule(
						rule => rule
							.Description("Google Passthrough")
							.IfInputClaimIssuer().Is("Google")
							.AndInputClaimType().IsOfType(
								"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
							.AndInputClaimValue().IsAny()
							.ThenOutputClaimType().ShouldBe(
								"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
							.AndOutputClaimValue().ShouldPassthroughFirstInputClaimValue())
					.AddRule(
						rule => rule
							.Description("ACS rule")
							.IfInputClaimIssuer().IsAcs()
							.AndInputClaimType().IsAny()
							.AndInputClaimValue().IsAny()
							.ThenOutputClaimType().ShouldPassthroughFirstInputClaimType()
							.AndOutputClaimValue().ShouldPassthroughFirstInputClaimValue())));

	acsNamespace.SaveChanges(logInfo => Console.WriteLine(logInfo.Message));
```

![Running](https://github.com/jrowies/FluentACS/raw/master/docs/Running.png)

### Where can I get it?

You can include the NuGet packages ([FluentACS](https://nuget.org/packages/FluentACS) and [FluentACS.Samples](https://nuget.org/packages/FluentACS.Samples)) using the package manager in Visual Studio.

![NuGet package manager](https://github.com/jrowies/FluentACS/raw/master/docs/PackageManager.png)

Also, you can grab the source code from the [project repository](https://github.com/jrowies/FluentACS) on github.

### Is there more documentation available?

There is no detailed documentation about the API, but given the fluent nature of the interface, it’s self-explaining and very straightforward to use. You can also take a look at the [tests](https://github.com/jrowies/FluentACS/blob/master/FluentACSTest/IntegrationTests.cs) !

### Can I use it for commercial purposes?

Sure! As long as the [license](https://github.com/jrowies/FluentACS/blob/master/LICENSE) is met.

### Credits

For ACS management, this project uses a modified version of the wrapper used in the p&p's sample for the [Windows Azure hybrid application integration guide](http://wag.codeplex.com/releases/view/74838).

Enjoy!
