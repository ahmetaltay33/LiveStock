using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using LiveStock.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace LiveStock.Service
{
    public class StockTicker
    {
        private readonly static Lazy<StockTicker> _instance = new Lazy<StockTicker>(() => new StockTicker(GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));
        public static StockTicker Instance => _instance.Value;
        private IHubConnectionContext<dynamic> Clients { get; set; }
        private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();
        private readonly Timer _timer;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);
        private readonly object _updateLock = new object();
        private volatile bool _updatingStocks = false;
        private readonly Random _updateOrNotRandom = new Random();

        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            _stocks.Clear();
            var stocks = new List<Stock>(3)
            {
                new Stock { Symbol = "USD", OpeningPrice = 18.69m, Price = 18.69m},
                new Stock { Symbol = "EUR", OpeningPrice = 18.40m, Price = 18.40m},
                new Stock { Symbol = "GBP", OpeningPrice = 21.10m, Price = 21.10m}
            };

            foreach (var stock in stocks)
                _stocks.TryAdd(stock.Symbol, stock);

            _timer = new Timer(UpdateStocks, null, _updateInterval, _updateInterval);
        }

        public IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }

        private void UpdateStocks(object state)
        {
            lock (_updateLock)
            {
                if (!_updatingStocks)
                {
                    _updatingStocks = true;
                    foreach (var stock in _stocks.Values)
                    {
                        if (TryUpdateStockPrice(stock))
                            BroadCastStock(stock);
                    }
                    _updatingStocks = false;
                }
            }
        }

        private bool TryUpdateStockPrice(Stock stock)
        {
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
                return false;
            
            var rnd = new Random();
            stock.Price = rnd.Next(17, 21) + (rnd.Next(1, 99) / 100m);
            stock.PercentChange = (stock.Price - stock.OpeningPrice) / stock.OpeningPrice * 100m;
            return true;
        }

        private void BroadCastStock(Stock stock)
        {
            Clients.All.updateStockPrice(stock);
        }
    }
}