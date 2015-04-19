The scope of the project is to provide ASP.NET Identity based on Cache database.

#### Install necessary software
- Visual Studio 2012/2013
- Cache 2015.2

####Now you can test provided functionality by performing following steps:
- Download Project from GitHub
- Open in Visual Studio InterSystems.AspNet.Identity.Cache -> Build Project 
- Create new C# console application
- Add reference to InterSystems.AspNet.Identity.Cache.dll built in previous step
- Add extra references to:
	- InterSystems.Data.CacheClient.dll
	- InterSystems.Data.Entity6.dll
	- Microsoft.AspNet.Identity.Core.dll
	
- Resolve dependencies: install entity framework package from nuget. PackageManager-> Install-Package EntityFramework
- Run Cache. Create namespace and specify it in connection string as in example below.
- Add Cache connection string to App.config. Here is example. You can change params. 
```
    <add name="Cache" connectionString="Server=localhost; Port=1972; Namespace=USER;Password=SYS; User ID=_SYSTEM;" providerName="InterSystems.Data.CacheClient" />
```
- Add data providers to App.config
```
  <entityFramework>
    <providers>
          <provider invariantName="InterSystems.Data.CacheClient" type="InterSystems.Data.Entity.ProviderServices, InterSystems.Data.Entity6" />
    </providers>
  </entityFramework>
```
- Now you can create IdentityDbContext using connection string name specified in App.config and operate with its entities (such as users, roles etc).
```
IdentityDbContext db = new IdentityDbContext("Cache");
db.Database.Connection.Open();
```
