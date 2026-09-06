using Microsoft.Owin.Security;
using System;
using System.Runtime.Caching;

namespace WebPortal.API.Security
{
    public sealed class MemoryCacheTokenStore
    {
        private readonly MemoryCache _cache = MemoryCache.Default;
        private readonly string _keyPrefix = "ApiToken-";
        public static MemoryCacheTokenStore Instance { get; } = new MemoryCacheTokenStore();
        private MemoryCacheTokenStore() { }

        public string Store(AuthenticationTicket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            string token = Guid.NewGuid().ToString("N");

            _cache.Set(_keyPrefix + token, ticket, new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromMinutes(AuthenticationConstants.SessionTimeoutMinutes)
            });

            return token;
        }

        public AuthenticationTicket Retrieve(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            return _cache.Get(_keyPrefix + token) as AuthenticationTicket;
        }

        public void Remove(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _cache.Remove(_keyPrefix + token);
            }
        }
    }
}