using System;
using Microsoft.Extensions.Caching.Memory;

namespace AutoTestMate.NUnit.Infrastructure.Core
{
	public class MemoryCache : IMemoryCache
	{
        public const int DefaultCacheMinutes = 5;

        private readonly Microsoft.Extensions.Caching.Memory.MemoryCache _memoryCache;

		public MemoryCache()
        {
            var cacheOptions = new MemoryCacheOptions();

            _memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(cacheOptions);
		}

		public object GetValue(string key)
		{
			return _memoryCache.Get(key);
		}

		public bool Add(string key, object value, DateTimeOffset absExpiration)
		{
			Delete(key);

            var newCacheEntry = _memoryCache.CreateEntry(key);
            newCacheEntry.Value = value;
            newCacheEntry.AbsoluteExpiration = absExpiration;

            return _memoryCache.TryGetValue(key, out _);

        }

		public bool Add(string key, object value)
		{
			Delete(key);
			var offset = DateTimeOffset.Now.AddMinutes(DefaultCacheMinutes);
            var newCacheEntry = _memoryCache.CreateEntry(key);
            newCacheEntry.Value = value;
            newCacheEntry.AbsoluteExpiration = offset;
            return _memoryCache.TryGetValue(key, out _);
        }

		public void Delete(string key)
		{
			if (_memoryCache.TryGetValue(key, out _))
			{
				_memoryCache.Remove(key);
			}
		}

        public bool Contains(string key)
        {
            var cacheValue = _memoryCache.TryGetValue(key, out string result);

            return cacheValue && !string.IsNullOrWhiteSpace(result);
        }
    }
}