using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using StackExchangeAPI.Web.Services;

namespace StackExchangeAPI.Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly RedisService _redisService;

        protected readonly IDatabase db;

        public BaseController(RedisService redisService)
        {
            _redisService = redisService;
            _redisService.Connect();
            db = _redisService.GetDb(1);
        }
    }
}
