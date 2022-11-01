using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using StackExchangeAPI.Web.Services;

namespace StackExchangeAPI.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _redisService.Connect();
            db = _redisService.GetDb(0);
        }
        public IActionResult Index()
        {
            _redisService.Connect();
            var db = _redisService.GetDb(0);
            db.StringSet("name", "Yücel Aydın");
            db.StringSet("ziyaretci", 100);

            // byte olarak dosyları kaydetme
            Byte[] imageByte = default(byte[]);
            db.StringSet("image", imageByte);
            return View();
        }
        public IActionResult Show()
        {
       
            var name = db.StringGet("name");
            var ziyaretci = db.StringGet("ziyaretci");
            //ziyaretciyi 1 er arttırır
            db.StringIncrement("ziyaretci", 2);
            //ziyaretciyi 1 er azaltır
            db.StringDecrement("ziyaretci", 1);
            // key değerinin uzunluğunu verir
            var nameLen = db.StringLength("name");
            if (name.HasValue)
                ViewBag.name = name;
            if (name.HasValue)
                ViewBag.ziyaretci = ziyaretci;
            ViewBag.length = nameLen;
            return View();
        }
    }
}
