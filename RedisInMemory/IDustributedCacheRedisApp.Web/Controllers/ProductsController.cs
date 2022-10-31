using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IDustributedCacheRedisApp.Web.Controllers
{
    public class ProductsController : Controller
    {
        private IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(1);
            _distributedCache.SetString("name", "yücel", cacheEntryOptions);
            await _distributedCache.SetStringAsync("surname", "aydın", cacheEntryOptions);
            return View();
        }
        public async Task<IActionResult> Show()
        {
            ViewBag.name = _distributedCache.GetString("name");
            ViewBag.surname = await _distributedCache.GetStringAsync("surname");
            return View();
        }
        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            return View();
        }
    }
}
