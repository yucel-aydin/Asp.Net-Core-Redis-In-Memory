using IDustributedCacheRedisApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

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
            #region Basit data kaydetme
            //DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            //cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(30);
            //_distributedCache.SetString("name", "yücel", cacheEntryOptions);
            //await _distributedCache.SetStringAsync("surname", "aydın", cacheEntryOptions);
            #endregion

            #region Complex Data Kaydetme
            // Nesne Kaydetme
            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(30);
            Product product = new Product()
            {
                Id = 3,
                Name = "Kitap 3",
                Price = 500
            };
            string jsonProduct = JsonConvert.SerializeObject(product);

            await _distributedCache.SetStringAsync("product:2", jsonProduct, cacheEntryOptions);

            // Binary Kaydetme
            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);

            await _distributedCache.SetAsync("product:3", byteProduct, cacheEntryOptions);

            #endregion


            return View();
        }
        public async Task<IActionResult> Show()
        {
            #region Complex Data Kaydetme
            //Json datayı alma
            //string jsonProduct = _distributedCache.GetString("product:1");
            //Product product = JsonConvert.DeserializeObject<Product>(jsonProduct);
            //ViewBag.product = product;

            //// binary datayı alma (daha meşakatli iş olduğu için json tipini kullanmak daha iyidir.)
            //Byte[] byteProduct = _distributedCache.Get("product:3");
            //string json = Encoding.UTF8.GetString(byteProduct);
            //Product product3 = JsonConvert.DeserializeObject<Product>(json);


            #endregion

            #region Basit data kaydetme
            //ViewBag.name = _distributedCache.GetString("name");
            //ViewBag.surname = await _distributedCache.GetStringAsync("surname");
            #endregion



            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            return View();
        }
        #region Dosyaların cachelenmesi
        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/sample.png");
            Byte[] imageByte = System.IO.File.ReadAllBytes(path);

            _distributedCache.Set("image", imageByte);
            return View();
        }
        public IActionResult ImageUrl()
        {
            
            Byte[] imageByte = _distributedCache.Get("image");

            return File(imageByte,"image/png");
        }
        #endregion
    }
}

