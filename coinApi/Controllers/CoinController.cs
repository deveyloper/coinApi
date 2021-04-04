using Binance.Net;
using coinApi.entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace coinApi.Controllers
{
    [Route("api/[controller]")]
    public class CoinController : Controller
    {
        private readonly IMemoryCache _memCache;

        public CoinController(IMemoryCache memoryCache)
        {
            _memCache = memoryCache;

        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] Credential credential)
        {
            string guid = System.Guid.NewGuid().ToString();
            _memCache.Set<Credential>(guid, credential);
            return Ok(new { authGuid = guid });
        }

        [HttpGet]
        public IActionResult Get(string key, string secret)
        {
            string guid = Request.Headers["authGuid"].ToString();
            var credential = _memCache.Get<Credential>(guid);
            var client = new BinanceClient();
            client.SetApiCredentials(credential.apiKey, credential.apiSecret);
            var binanceAccuntInfo = client.General.GetAccountInfo().Data;
            var balances = binanceAccuntInfo.Balances.Where(e => e.Total > 0);
            return Ok(balances);
        }

    }
}
