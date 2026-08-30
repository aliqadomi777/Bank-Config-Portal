using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace WebPortal.ASP.Security
{
    public class MemoryCacheAuthenticationSessionStore : IAuthenticationSessionStore
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly string _keyPrefix = "AuthSession-";

        public Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = _keyPrefix + Guid.NewGuid().ToString();
            _cache.Set(key, ticket, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return Task.FromResult(key);
        }

        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _cache.Set(key, ticket, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(5)
            });

            return Task.FromResult(0);
        }


        public Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var ticket = _cache.Get(key) as AuthenticationTicket;
            return Task.FromResult(ticket);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.FromResult(0);
        }
    }
}
