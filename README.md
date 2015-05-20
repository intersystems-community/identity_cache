The scope of the project is to provide ASP.NET Identity based on Cache database.
ASP.NET Identity is the new membership system for building ASP.NET web applications. ASP.NET Identity allows you to add login features to your application and makes it easy to customize data about the logged in user.

#### Install necessary software
- Visual Studio 2012/2013
- Cache 2015.2
- .NET Framework 4.5

####Second stage:
- AspNet Identity completely implemented for Cache database (with IdentityUserStore and IdentityRoleStore).
- Asp.Net MVC test project added to display provided functionality (AspNetIdentityTestProject).
- Integration tests added (Identity.Test).
- Nuget package created (Release/InterSystems.AspNet.Identity.Cache.1.0.0.nupkg).
- Binary library created (Release/InterSystems.AspNet.Identity.Cache.dll).

####Now you can test provided functionality by performing following steps:
- Download Project from GitHub.
- Open solution in visual studio.
- In AspNetIdentityTestProject Web.config in connectionStrings section find next string:
```    
<add name="DefaultConnection" connectionString="Server=localhost; Port=1972; Namespace=USER;Password=SYS; User ID=_SYSTEM;" providerName="InterSystems.Data.CacheClient" />
```
You should replace Namespace and Port to yours if needed.
- Then launch AspNetIdentityTestProject. This is the simple web application that demonstrates AspNet.Identity implementation for cache. You can register and login to site using facebook or google account. All the data is stored in your Cache database.

We provided unit testing (Identity.Test project).
####Testing with unit tests.
- In Identity.Test project App.config in connectionStrings section find next string:
```    
<add name="DefaultConnection" connectionString="Server=localhost; Port=1972; Namespace=USER;Password=SYS; User ID=_SYSTEM;" providerName="InterSystems.Data.CacheClient" />
```
You should replace Namespace and Port to yours if needed.
- Use next commangs to install needed package to Identity.Test:
	 - Install-Package xunit -Version 1.9.2
	 - Install-Package Microsoft.AspNet.Identity.Owin
	 - Install-Package Microsoft.AspNet.Identity.EntityFramework
- Then build Identity.Test project
- Download XUnit test enviroment from https://xunit.codeplex.com/releases/view/90058 (xunit-1.9.1.zip).
- Install XUnit enviroment. 
- Open. 
- Before running test you should clean all data from Identity tables in Cache (DBO.AspNetUsers, DBO.AspNetUserLogins, DBO.AspNetUserClaims, DBO.AspNetRoles, DBO.AspNetUserRoles).
- Add Identity.Test.dll (Assembly -> Open) and press RunAll tests. 

Nuget package.
We created package - InterSystems.AspNet.Identity.Cache.1.0.0.nupkg (Release folder).
Installation of nuget package to your project - http://stackoverflow.com/questions/10240029/how-to-install-a-nuget-package-nupkg-file-locally.
