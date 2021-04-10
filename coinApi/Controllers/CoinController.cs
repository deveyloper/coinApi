using Binance.Net;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;
using coinApi.entity;
using coinApi.Entity;
using coinApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;

namespace coinApi.Controllers
{
    [Route("api/[controller]")]
    public class CoinController : Controller
    {
        static string testGuid;
        private readonly IMemoryCache _memCache;

        public CoinController(IMemoryCache memoryCache)
        {
            _memCache = memoryCache;

        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] Credential credential)
        {
            string guid = System.Guid.NewGuid().ToString();
            credential.apiKey = "RTNcs2g2TPODObcuSOKjhLyhHb9PmT37n3VTO6RFbB0ioL4EeAHlrPGFL0gJP70U";
            credential.apiSecret = "LKAZLNyunmBEnKZKT9pPQBvcNV81TMPmRmyMWRBKIBr2mIE0Itumbbblu2F5n252";
            credential.authKey = guid;
            testGuid = guid;
            _memCache.Set<Credential>(guid, credential);
            return Ok(new { authGuid = guid });
        }

        private List<PortfolioItem> getBalances(HttpRequest request)
        {
            List<PortfolioItem> portfolioItems = new List<PortfolioItem>();
            var prices = getPrices();
            var binanceAccuntInfo = getClientwihCredential(request).General.GetAccountInfo().Data;
            var balances = binanceAccuntInfo.Balances.Where(e => e.Total > 0).ToList();
            foreach(BinanceBalance balance in balances)
            {
                portfolioItems.Add(
                    new PortfolioItem
                    {
                        binanceBalance = balance,
                        btcPrice = prices.SingleOrDefault(e => e.Symbol == balance.Asset + "BTC"),
                        usdtPrice = prices.SingleOrDefault(e => e.Symbol == balance.Asset + "USDT"),

                    }
                    );
            }
            return portfolioItems;
        }

        private BinanceClient getClientwihCredential(HttpRequest request)
        {
            //string guid = testGuid;// request.Headers["authGuid"].ToString();
            //var credential = _memCache.Get<Credential>(guid);
            var client = new BinanceClient();
            client.SetApiCredentials("RTNcs2g2TPODObcuSOKjhLyhHb9PmT37n3VTO6RFbB0ioL4EeAHlrPGFL0gJP70U", "LKAZLNyunmBEnKZKT9pPQBvcNV81TMPmRmyMWRBKIBr2mIE0Itumbbblu2F5n252");
            return client;
        }

        [HttpGet("balances")]
        public IActionResult Get()
        {
            return Ok(getBalances(Request));
        }

        [HttpGet("prices/{symbol}")]
        public IActionResult Price(string symbol)
        {
            string guid = Request.Headers["authGuid"].ToString();
            return Ok();
        }

        private List<BinancePrice> getPrices()
        {
            var client = new BinanceClient();
            return client.Spot.Market.GetPrices().Data.ToList();

        }

        [HttpPut("updatebalances")]
        public IActionResult UpdateBalances()
        {
            string guid = Request.Headers["authGuid"].ToString();
            var balances = getBalances(Request);
            _memCache.Set($"{guid}|balances", balances);
            return Ok();
        }

        [HttpGet("costs")]
        public IActionResult GetCosts()
        {
            Dictionary<string, List<CoinCost>> keyValuePairs = new Dictionary<string, List<CoinCost>>();
            string guid = Request.Headers["authGuid"].ToString();
            var balances = getBalances(Request);
            var client = getClientwihCredential(Request);
            foreach (var balance in balances)
            {
                string pair = balance.binanceBalance.Asset;
                string btcPair = $"{pair}BTC";
                string usdtPair = $"{pair}USDT";
                List<CoinCost> coinCosts = new List<CoinCost>();

                var btcTrades = client.Spot.Order.GetMyTrades(btcPair).Data;
                var usdtTrades = client.Spot.Order.GetMyTrades(usdtPair).Data;
                if (btcTrades != null)
                {
                    foreach (var trade in btcTrades)
                    {
                        coinCosts.Add(new CoinCost { costCurrency = "BTC", cost = trade.Price , quantity= trade.Quantity});
                    }

                }
                if (usdtTrades != null)
                {
                    foreach (var trade in usdtTrades)
                    {
                        coinCosts.Add(new CoinCost { costCurrency = "USDT", cost = trade.Price, quantity = trade.Quantity });
                    }
                }
                keyValuePairs.Add(balance.binanceBalance.Asset, coinCosts);
            }
            return Ok(keyValuePairs);
        }


        [HttpGet("coincosts")]
        public IActionResult GetCoinCosts()
        {
            Dictionary<string, List<BinanceTrade>> keyValuePairs = new Dictionary<string, List<BinanceTrade>>();
            string guid = Request.Headers["authGuid"].ToString();
            var balances = getBalances(Request);
            var client = getClientwihCredential(Request);
            foreach (var balance in balances)
            {
                string pair = balance.binanceBalance.Asset;
                string btcPair = $"{pair}BTC";
                string usdtPair = $"{pair}USDT";
                List<CoinCost> coinCosts = new List<CoinCost>();

                var btcTrades = client.Spot.Order.GetMyTrades(btcPair).Data;
                var usdtTrades = client.Spot.Order.GetMyTrades(usdtPair).Data;
                if (btcTrades != null)
                {
                    keyValuePairs.Add(balance.binanceBalance.Asset + "BTC", btcTrades.ToList());
                    foreach (var trade in btcTrades)
                    {
                        coinCosts.Add(new CoinCost { costCurrency = "BTC", cost = trade.Price, quantity = trade.Quantity });
                    }

                }
                if (usdtTrades != null)
                {
                    keyValuePairs.Add(balance.binanceBalance.Asset + "USDT", usdtTrades.ToList());
                    foreach (var trade in usdtTrades)
                    {
                        coinCosts.Add(new CoinCost { costCurrency = "USDT", cost = trade.Price, quantity = trade.Quantity });
                    }
                }
            }
            return Ok(keyValuePairs);
        }

        [HttpPut("updatecost")]
        public IActionResult Update()
        {
            string guid = Request.Headers["authGuid"].ToString();
            var client = new BinanceClient();
            var credential = _memCache.Get<Credential>(guid);
            client.SetApiCredentials(credential.apiKey, credential.apiSecret);
            return Ok();
        }


    }
}
