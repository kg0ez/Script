namespace Sctipt
{
	public class Trade
	{
		//string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		string path = "transactions.txt";

		const double amount = 100; //USDT
		const double comission = 0.0015;

		const string startInstrument = "USDT";

		List<CCXT.NET.Shared.Coin.Public.Ticker> tickers;

		public async Task TransactionsAsync(CCXT.NET.Poloniex.Public.PublicApi poloniex,
			CCXT.NET.Binance.Public.PublicApi binance)
        {
			Dictionary<string, int> pairs = new Dictionary<string, int>()
			{
				{"BTC/USDT",2 },
				{"BNB/BTC",2 },
				{"BNB/UAH",2 },
				{"USDT/UAH",2 }
			};

			tickers = new List<CCXT.NET.Shared.Coin.Public.Ticker>();

            foreach (var pair in pairs)
            {
				string[] instruments = pair.Key.Split('/');

				if (pair.Value==1)
					tickers.Add(await poloniex.FetchTickerAsync(instruments[0], instruments[1]));
                
                else if (pair.Value == 2)
					tickers.Add(await binance.FetchTickerAsync(instruments[0], instruments[1]));
            }
			
			var workingInstrument = startInstrument;
			var workingInstrumentAmount = amount;

            for (int i = 0; i < pairs.Count; i++)
            {
				var type = GetType(workingInstrument, pairs.ElementAt(i).Key);

                if (type == 0)
					workingInstrument = pairs.ElementAt(i).Key.Split("/")[1];

				else if (type == 1)
					workingInstrument = pairs.ElementAt(i).Key.Split("/")[0];


                if (type==0)
					workingInstrumentAmount = GetComission(workingInstrumentAmount)
						* Convert.ToDouble(GetPrice(type, i));

				else if (type == 1)
					workingInstrumentAmount = GetComission(workingInstrumentAmount)
						/ Convert.ToDouble(GetPrice(type, i));

                if (i==pairs.Count-1)
                {
					using (StreamWriter writer = new StreamWriter(path, true))
					{
						await writer.WriteLineAsync($"{workingInstrumentAmount}");
					}
					Console.WriteLine(workingInstrumentAmount);
				}
			}
		}

		private int GetType(string instrument, string pair)
        {
			//0 - buy
			//1 - sell
			string[] instruments = pair.Split('/');

            if (instruments[0] == instrument)
				return 0;

			else if (instruments[1] == instrument)
				return 1;

			throw new Exception($"Type not found.");
		}

		private string GetOperation(int type)
        {
            if (type == 0)
				return "bid";

			else if (type == 1)
				return "ask";

			throw new Exception($"Operation type unknown");
		}

		private double GetComission(double amount)
        {
			return amount - amount * comission;
        }

		private decimal GetPrice(int type, int index)
        {
            if (GetOperation(type)=="bid")
				return tickers[index].result.bidPrice;

            else if (GetOperation(type) == "ask")
				return tickers[index].result.askPrice;

			throw new Exception($"Price not received.");
		}
	}
}

