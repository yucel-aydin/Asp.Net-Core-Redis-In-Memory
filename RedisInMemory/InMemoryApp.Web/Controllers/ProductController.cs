using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;
        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            #region Set()
            //IMemoryCache Set() zaman keyine sahip bir cache oluşturuyoruz
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            #endregion

            #region IsNullOrEmpty, TryGetValue, GetOrCreate
            // Bir key değerinin memoryde olup olmadığını kontrol etmek için yolar
            //1. Yöntem
            if (String.IsNullOrEmpty(_memoryCache.Get<string>("zaman")))
            {
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString());
            }
            //2. Yöntem
            // TryGetValue ile zaman key ine sahip bir cache varsa alır ve out keywordü ile zamanCache atar
            // Yoksa if içerisinde değer ataması yapar
            if (_memoryCache.TryGetValue("zaman", out string zamanCache))
            {
                _memoryCache.Set<string>("zaman", DateTime.Now.ToString());

            }
            //3. Yöntem 
            // GetOrCreate metodu yine verilen zaman keyine ait bir data varsa alır alamazsa bu keyi oluşturur ve zaman bilgisini atar.
            // entry ile expritaion vs. özellikleride değiştirebilir.
            _memoryCache.GetOrCreate<string>("zaman", entry =>
            {
                return DateTime.Now.ToString();
            });

            #endregion

            #region Expiration
            // Expiration
            // 1- AbsoluteExpiration
            // 10 dk cache te tutar ve 10 dk sonra silinir.
            MemoryCacheEntryOptions optionsAbsolute = new MemoryCacheEntryOptions();
            optionsAbsolute.AbsoluteExpiration = DateTime.Now.AddMinutes(10);
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), optionsAbsolute);

            //2- SlidingExpiration
            // 10 saniye içersinde zaman cache ine erişilirse 10 saniye arttırarak devam eder. 10sn erişilmezse memoryden silinir.
            MemoryCacheEntryOptions optionsSliding = new MemoryCacheEntryOptions();
            optionsSliding.SlidingExpiration = TimeSpan.FromSeconds(10);
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), optionsSliding);

            //SlidingExpiration kullanıldığında sürekli aynı datayı alma olasığına karşı absoluteexpiration ile birlikte kullanılması önerilir.
            // 10 sn içerisinde erişilirse 10 sn eklenerek slidingexpression devam eder ama absolute expression da tanımladığımız 1 dk. yüzünde en fazla 1 dk aynı data ulaşırız sonra silinir.

            MemoryCacheEntryOptions optionsSlidingAndAbsolute = new MemoryCacheEntryOptions();
            optionsAbsolute.AbsoluteExpiration = DateTime.Now.AddMinutes(1);

            optionsSliding.SlidingExpiration = TimeSpan.FromSeconds(10);
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), optionsSliding);
            #endregion

            #region Cache Priority
            // Cache Priority
            // Bu property memory dolduğunda yeni bir data kaydedileceğinde hangi data silinecek buna karar vermek için cache priorty (yani önemliliğini) belirliyoruz.
            MemoryCacheEntryOptions optionsCachePriority = new MemoryCacheEntryOptions();
            // High         : Yüksek önemlilik
            // Normal       : orta önemlilik
            // Low          : Düşük önemlilik
            // NeverRemove  : Asla silme
            // Önce low belirlenenler sonra normal sonra high silinir ihtiyaç olursa.
            // NeverRemove silinmez. Ama bunu kullanırsak tüm datalarda, memory dolarsa ve yeni bir cache eklemeye çalışırsak exception fırlatır. Dikkatli kullanılmalı.

            optionsCachePriority.Priority = CacheItemPriority.Normal;
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), optionsCachePriority);
            #endregion

            #region RegisterPostEvictionCallback
            // RegisterPostEvictionCallback 
            //Memoryden silinen datanın hangi sabeple silindiğini bu metotla belirliyoruz.
            MemoryCacheEntryOptions optionsRegisterPostEvictionCallback = new MemoryCacheEntryOptions();
            optionsCachePriority.RegisterPostEvictionCallback((key, value, reason, state) =>
            {
                //key   :Silinecek olan key
                //value :key değeri
                //reason: silinme sebebi
                _memoryCache.Set("callback", $"{key} -->{value} => sebep: {reason}");
            });
            _memoryCache.Set<string>("zaman", DateTime.Now.ToString(), optionsCachePriority);
            #endregion

            #region Complex Types Cache
            Product product = new Product()
            {
                Id = 1,
                Name = "Kalem",
                Price = 200
            };
            _memoryCache.Set<Product>("product:1", product);
            Product cacheProduct = _memoryCache.Get<Product>("product:1");
            #endregion

            return View();
        }
        public IActionResult Show()
        {


            //IMemoryCache Get() zaman keyine sahip değeri alıyoruz
            ViewBag.Zaman = _memoryCache.Get<string>("zaman");

            // Key değerinden cachei silme
            _memoryCache.Remove("zaman");
            return View();
        }
    }
}
