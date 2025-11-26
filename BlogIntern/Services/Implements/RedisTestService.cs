using BlogIntern.Data;
using BlogIntern.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;



namespace BlogIntern.Services.Implements
{
    public class RedisTestService
    {
        private readonly AdventureContext _adv;
        private readonly IDistributedCache _cache;

        public RedisTestService(AdventureContext adv, IDistributedCache cache)
        {
            _adv = adv;
            _cache = cache;
        }

        public async Task<List<SalesOrderDetail>> GetFromDb()
        {
            return await _adv.SalesOrderDetails
                             .Take(120000)
                             .ToListAsync();
        }

        public async Task<List<SalesOrderDetail>> GetFromRedis()
        {
            var key = "sales_50k";

            var cached = await _cache.GetStringAsync(key);

            if (cached != null)
                return JsonConvert.DeserializeObject<List<SalesOrderDetail>>(cached);

            var data = await GetFromDb();

            await _cache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(data),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return data;
        }

        public async Task ClearCache()
        {
            await _cache.RemoveAsync("sales_50k");
        }

    }
}
