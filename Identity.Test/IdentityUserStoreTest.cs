using InterSystems.AspNet.Identity.Cache;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Test
{
    public class IdentityUserStoreTest
    {
        [Fact]
        public void AddUserWithNoUserNameFailsTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);
            Assert.Throws<DbEntityValidationException>(
                () => AsyncHelper.RunSync(() => store.CreateAsync(new IdentityUser())));
        }

        [Fact]
        public async Task CanDisableAutoSaveChangesTest()
        {
            var db = new NoopIdentityDbContext();
            var store = new IdentityUserStore<IdentityUser>(db);
            store.AutoSaveChanges = false;
            var user = new IdentityUser("test");
            await store.CreateAsync(user);
            Assert.False(db.SaveChangesCalled);

            Clear(db);
        }

        [Fact]
        public async Task CreateAutoSavesTest()
        {
            var db = new NoopIdentityDbContext();
            db.Configuration.ValidateOnSaveEnabled = false;
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("test");
            await store.CreateAsync(user);
            Assert.True(db.SaveChangesCalled);
        }

        [Fact]
        public async Task UpdateAutoSavesTest()
        {
            var db = new NoopIdentityDbContext();
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("test");
            await store.UpdateAsync(user);
            Assert.True(db.SaveChangesCalled);

            Clear(db);
        }

        [Fact]
        public async Task AddDupeUserIdWithStoreFailsTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("dupemgmt");
            await store.CreateAsync(user);
            var u2 = new IdentityUser { Id = user.Id, UserName = "User" };
            try
            {
                await store.CreateAsync(u2);
                Assert.False(true);
            }
            catch (Exception e)
            {
                Assert.True(e.InnerException.InnerException.Message.Contains("Нарушение Ограничений UNIQUE"));
            }

            Clear(db);
        }

        [Fact]
        public void UserStoreMethodsThrowWhenDisposedTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);
            store.Dispose();
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.AddClaimAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.AddLoginAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.AddToRoleAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.GetClaimsAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.GetLoginsAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.GetRolesAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.IsInRoleAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.RemoveClaimAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.RemoveLoginAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(
                () => AsyncHelper.RunSync(() => store.RemoveFromRoleAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.RemoveClaimAsync(null, null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.FindAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.FindByIdAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.FindByNameAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.UpdateAsync(null)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.DeleteAsync(null)));
            Assert.Throws<ObjectDisposedException>(
                () => AsyncHelper.RunSync(() => store.SetEmailConfirmedAsync(null, true)));
            Assert.Throws<ObjectDisposedException>(() => AsyncHelper.RunSync(() => store.GetEmailConfirmedAsync(null)));
            Assert.Throws<ObjectDisposedException>(
                () => AsyncHelper.RunSync(() => store.SetPhoneNumberConfirmedAsync(null, true)));
            Assert.Throws<ObjectDisposedException>(
                () => AsyncHelper.RunSync(() => store.GetPhoneNumberConfirmedAsync(null)));
        }

        [Fact]
        public void UserStorePublicNullCheckTest()
        {
            var store = new IdentityUserStore<IdentityUser>();
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.CreateAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.UpdateAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.DeleteAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.AddClaimAsync(null, null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.RemoveClaimAsync(null, null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetClaimsAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetLoginsAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetRolesAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.AddLoginAsync(null, null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.RemoveLoginAsync(null, null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.AddToRoleAsync(null, null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.RemoveFromRoleAsync(null, null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.IsInRoleAsync(null, null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetPasswordHashAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.SetPasswordHashAsync(null, null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetSecurityStampAsync(null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.SetSecurityStampAsync(null, null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.AddClaimAsync(new IdentityUser("fake"), null)), "claim is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.RemoveClaimAsync(new IdentityUser("fake"), null)), "claim is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.AddLoginAsync(new IdentityUser("fake"), null)), "login is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.RemoveLoginAsync(new IdentityUser("fake"), null)), "login is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.FindAsync(null)), "login is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetEmailConfirmedAsync(null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.SetEmailConfirmedAsync(null, true)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetEmailAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.SetEmailAsync(null, null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetPhoneNumberAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.SetPhoneNumberAsync(null, null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.GetPhoneNumberConfirmedAsync(null)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.SetPhoneNumberConfirmedAsync(null, true)), "user is null");
            ExceptionHelper.ThrowsArgumentNull(() => AsyncHelper.RunSync(() => store.GetTwoFactorEnabledAsync(null)),
                "user is null");
            ExceptionHelper.ThrowsArgumentNull(
                () => AsyncHelper.RunSync(() => store.SetTwoFactorEnabledAsync(null, true)), "user is null");
            ExceptionHelper.ThrowsArgumentNullOrEmpty(
                () => AsyncHelper.RunSync(() => store.AddToRoleAsync(new IdentityUser("fake"), null)), "roleName is empty or null");
            ExceptionHelper.ThrowsArgumentNullOrEmpty(
                () => AsyncHelper.RunSync(() => store.RemoveFromRoleAsync(new IdentityUser("fake"), null)), "roleName is empty or null");
            ExceptionHelper.ThrowsArgumentNullOrEmpty(
                () => AsyncHelper.RunSync(() => store.IsInRoleAsync(new IdentityUser("fake"), null)), "roleName is empty or null");
        }

        [Fact]
        public async Task AddDupeUserNameWithStoreFailsTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var mgr = new UserManager<IdentityUser>(new IdentityUserStore<IdentityUser>(db));
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("dupe");
            UnitTestHelper.IsSuccess(await mgr.CreateAsync(user));
            var u2 = new IdentityUser("DUPe");
            Assert.Throws<DbEntityValidationException>(() => AsyncHelper.RunSync(() => store.CreateAsync(u2)));

            Clear(db);
        }

        [Fact]
        public async Task AddDupeEmailWithStoreFailsTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            db.RequireUniqueEmail = true;
            var mgr = new UserManager<IdentityUser>(new IdentityUserStore<IdentityUser>(db));
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("u1") { Email = "email" };
            UnitTestHelper.IsSuccess(await mgr.CreateAsync(user));
            var u2 = new IdentityUser("u2") { Email = "email" };
            Assert.Throws<DbEntityValidationException>(() => AsyncHelper.RunSync(() => store.CreateAsync(u2)));

            Clear(db);
        }

        [Fact]
        public async Task DeleteUserTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);
            var mgmt = new IdentityUser("deletemgmttest");
            await store.CreateAsync(mgmt);
            Assert.NotNull(await store.FindByIdAsync(mgmt.Id));
            await store.DeleteAsync(mgmt);
            Assert.Null(await store.FindByIdAsync(mgmt.Id));

            Clear(db);
        }
        
        [Fact]
        public async Task CreateLoadDeleteUserTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("Test");
            Assert.Null(await store.FindByIdAsync(user.Id));
            await store.CreateAsync(user);
            var loadUser = await store.FindByIdAsync(user.Id);
            Assert.NotNull(loadUser);

            Assert.Equal(user.Id, loadUser.Id);
            await store.DeleteAsync(loadUser);
            loadUser = await store.FindByIdAsync(user.Id);
            Assert.Null(loadUser);

            Clear(db);
        }

        [Fact]
        public async Task FindByUserName()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);
            var user = new IdentityUser("Hao");
            await store.CreateAsync(user);
            var found = await store.FindByNameAsync("hao");
            Assert.NotNull(found);
            Assert.Equal(user.Id, found.Id);

            found = await store.FindByNameAsync("HAO");
            Assert.NotNull(found);
            Assert.Equal(user.Id, found.Id);

            found = await store.FindByNameAsync("Hao");
            Assert.NotNull(found);
            Assert.Equal(user.Id, found.Id);

            Clear(db);
        }

        [Fact]
        public async Task GetAllUsersTest()
        {
            var db = UnitTestHelper.CreateDefaultDb();
            var store = new IdentityUserStore<IdentityUser>(db);

            var oldCount = store.Users.Count();

            var users = new[]
            {
                new IdentityUser("user1"),
                new IdentityUser("user2"),
                new IdentityUser("user3")
            };
            foreach (IdentityUser u in users)
            {
                await store.CreateAsync(u);
            }
            IQueryable<IUser> usersQ = store.Users;
            Assert.Equal(oldCount + 3, usersQ.Count());
            Assert.NotNull(usersQ.Where(u => u.UserName == "user1").FirstOrDefault());
            Assert.NotNull(usersQ.Where(u => u.UserName == "user2").FirstOrDefault());
            Assert.NotNull(usersQ.Where(u => u.UserName == "user3").FirstOrDefault());
            Assert.Null(usersQ.Where(u => u.UserName == "bogus").FirstOrDefault());

            Clear(db);
        }

        private void Clear(IdentityDbContext db)
        {
            db.Database.ExecuteSqlCommand("delete from DBO.AspNetUserRoles");
            db.Database.ExecuteSqlCommand("delete from DBO.AspNetUsers");
            db.Database.ExecuteSqlCommand("delete from DBO.AspNetRoles");
        }

        private class NoopIdentityDbContext : IdentityDbContext
        {
            public bool SaveChangesCalled { get; set; }

            public override Task<int> SaveChangesAsync(CancellationToken token)
            {
                SaveChangesCalled = true;
                return Task.FromResult(0);
            }
        }
    }
}