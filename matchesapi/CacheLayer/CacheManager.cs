using System.Collections.Specialized;
using System.Runtime.Caching;
using Microsoft.Extensions.Caching.Memory;
using MemoryCache = System.Runtime.Caching.MemoryCache;

namespace cw2backend.CacheLayer;

public class CacheManager : ICacheManager
{
    private MemoryCache _cache;
    private readonly string _name;
    private readonly int _memLimitInMb;
    public CacheManager(string name)
    {
        _name = name;
        _memLimitInMb = 1024;
        _cache = CreateCache();
    }

    private MemoryCache CreateCache()
    {
        NameValueCollection cacheSettings = new NameValueCollection(2)
        {
            {"cacheMemoryLimitMegabytes", Convert.ToString(_memLimitInMb)},
            {"pollingInterval", "00:00:30"},
        };
        _cache = MemoryCache.Default;
        return new MemoryCache(_name + Guid.NewGuid(), cacheSettings);
    }
    
    public long GetCount() => _cache.GetCount();
    
    
    public T Get<T>(string key) where T : class
    {
        
        var c = _cache.Get(key);
        if (c != null)
        {
            return c as T;
        }
        return null!;
    }
    
    public bool Add(string key, object objectToBeCached, CacheItemPolicy cacheItemPolicy)
    {
        return _cache.Add(new CacheItem(key, objectToBeCached), cacheItemPolicy);

    }
    
    public void Set(string key, object o, CacheItemPolicy cacheItemPolicy)
    {
        _cache.Set(key, new CacheItem(key, o), cacheItemPolicy);
    }

 
}