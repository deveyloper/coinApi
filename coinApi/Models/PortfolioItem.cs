using System;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;

namespace coinApi.Models
{
    public class PortfolioItem
    {
        public BinanceBalance binanceBalance { get; set; }

        public BinancePrice btcPrice { get; set; }
        public BinancePrice usdtPrice { get; set; }

    }
}
