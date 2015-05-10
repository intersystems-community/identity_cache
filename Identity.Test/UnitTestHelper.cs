using InterSystems.AspNet.Identity.Cache;
using Microsoft.AspNet.Identity;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Identity.Test
{
    public static class UnitTestHelper
    {
        public static bool EnglishBuildAndOS
        {
            get
            {
                var englishBuild = String.Equals(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, "en",
                    StringComparison.OrdinalIgnoreCase);
                var englishOS = String.Equals(CultureInfo.CurrentCulture.TwoLetterISOLanguageName, "en",
                    StringComparison.OrdinalIgnoreCase);
                return englishBuild && englishOS;
            }
        }

        public static IdentityDbContext CreateDefaultDb()
        {
            return new IdentityDbContext();
        }

        public static void IsSuccess(IdentityResult result)
        {
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
        }

        public static void IsFailure(IdentityResult result)
        {
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
        }

        public static void IsFailure(IdentityResult result, string error)
        {
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Equal(error, result.Errors.First());
        }
    }

    public class AlwaysBadValidator<T> : IIdentityValidator<T>
    {
        public const string ErrorMessage = "Bad validation";

        public Task<IdentityResult> ValidateAsync(T item)
        {
            return Task.FromResult(IdentityResult.Failed(ErrorMessage));
        }
    }

    public class NoopValidator<T> : IIdentityValidator<T>
    {
        public Task<IdentityResult> ValidateAsync(T item)
        {
            return Task.FromResult(IdentityResult.Success);
        }
    }
}