using Binance.Net;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace coinApi.Controllers
{
    [Route("api/[controller]")]
    public class CoinController : Controller
    {
        [HttpGet("{key}/{secret}")]
        public IActionResult Get(string key, string secret)
        {
            var client = new BinanceClient();
            client.SetApiCredentials(key, secret);
            var binanceAccuntInfo = client.General.GetAccountInfo().Data;
            var balances = binanceAccuntInfo.Balances.Where(e => e.Total > 0);
            return Ok(balances);
        }
    }
}
