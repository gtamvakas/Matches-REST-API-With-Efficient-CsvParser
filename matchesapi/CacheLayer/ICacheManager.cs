using System.Runtime.Caching;

namespace cw2backend.CacheLayer;

public interface ICacheManager
{
    public long GetCount();


    public T Get<T>(string key) where T : class;

    public bool Add(string key, object objectToBeCached, CacheItemPolicy cacheItemPolicy);

    public void Set(string key, object o, CacheItemPolicy cacheItemPolicy);
}