using Sctipt;

var api_Poloniex = new CCXT.NET.Poloniex.Public.PublicApi();
var api_Binance = new CCXT.NET.Binance.Public.PublicApi();

Trade trade = new Trade();

var timer = new System.Timers.Timer();
timer.Start();
timer.Elapsed += async (o, e) =>
{
    timer.Interval = 4000;
    Task.Run(async () =>
    {
        await trade.TransactionsAsync(api_Poloniex, api_Binance);
        System.Console.WriteLine(DateTime.Now);
    });
};
Console.ReadLine();
