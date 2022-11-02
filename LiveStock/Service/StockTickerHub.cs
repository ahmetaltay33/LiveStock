using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;

namespace LiveStock.Service
{
    [HubName("stockTickerMini")]
    public class StockTickerHub : Hub
    {
        private readonly StockTicker _stockTicker;

        public StockTickerHub() : this(StockTicker.Instance) { }
        
        public StockTickerHub(StockTicker stockTicker)
        {
            _stockTicker = stockTicker;
        }
        
        public IEnumerable<Models.Stock> GetAllStocks()
        {
            return _stockTicker.GetAllStocks();
        }
    }
}