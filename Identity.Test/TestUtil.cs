using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using InterSystems.AspNet.Identity.Cache;

namespace Identity.Test
{
    public static class TestUtil
    {
        public static void SetupDatabase<TDbContext>(string dataDirectory) where TDbContext : IdentityDbContext
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDirectory);
        }

        public static UserManager<IdentityUser> CreateManager(IdentityDbContext db)
        {
            var options = new IdentityFactoryOptions<UserManager<IdentityUser>>
            {
                Provider = new TestProvider(db),
                DataProtectionProvider = new DpapiDataProtectionProvider()
            };
            return options.Provider.Create(options, new OwinContext());
        }

        public static UserManager<IdentityUser> CreateManager()
        {
            return CreateManager(UnitTestHelper.CreateDefaultDb());
        }

        public static async Task CreateManager(OwinContext context)
        {
            var options = new IdentityFactoryOptions<UserManager<IdentityUser>>
            {
                Provider = new TestProvider(UnitTestHelper.CreateDefaultDb()),
                DataProtectionProvider = new DpapiDataProtectionProvider()
            };
            var middleware =
                new IdentityFactoryMiddleware
                    <UserManager<IdentityUser>, IdentityFactoryOptions<UserManager<IdentityUser>>>(null, options);
            await middleware.Invoke(context);
        }
    }

    public class TestProvider : IdentityFactoryProvider<UserManager<IdentityUser>>
    {
        public TestProvider(IdentityDbContext db)
        {
            OnCreate = ((options, context) =>
            {
                var manager =
                    new UserManager<IdentityUser>(new IdentityUserStore<IdentityUser>(db));
                manager.UserValidator = new UserValidator<IdentityUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = true,
                    RequireUniqueEmail = false
                };
                manager.EmailService = new TestMessageService();
                manager.SmsService = new TestMessageService();
                if (options.DataProtectionProvider != null)
                {
                    manager.UserTokenProvider =
                        new DataProtectorTokenProvider<IdentityUser>(
                            options.DataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;
            });
        }
    }

    public class TestMessageService : IIdentityMessageService
    {
        public IdentityMessage Message { get; set; }

        public Task SendAsync(IdentityMessage message)
        {
            Message = message;
            return Task.FromResult(0);
        }
    }
}