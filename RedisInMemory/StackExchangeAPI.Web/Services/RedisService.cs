using StackExchange.Redis;

namespace StackExchangeAPI.Web.Services
{
    public class RedisService
    {
        private readonly string _redisHost;
        private readonly string _redisPort;

        //Redis serverla haberleşecek classımız 
        private ConnectionMultiplexer _redis;
        public IDatabase db { get; set; }
        public RedisService(IConfiguration configuration)
        {
            //appsetting.json dosyasında tanımladığımız redis host ve portu alıyoruz.
            _redisHost = configuration["Redis:Host"];
            _redisPort = configuration["Redis:Port"];
        }

        public void Connect()
        {
            var configString = $"{_redisHost}:{_redisPort}";
            //appsettings.json dosyasındaki redis host ve portu 
            _redis = ConnectionMultiplexer.Connect(configString);
        }

        public IDatabase GetDb(int db)
        {
            return _redis.GetDatabase(db);  

        }
    }
}
