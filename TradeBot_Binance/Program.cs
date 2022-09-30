using Binance.API.Csharp.Client;
using Binance.API.Csharp.Client.Models.Market;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading;

namespace TradeBot_Binance
{
    internal static class Program
    {
        private static string _discordUrl;
        private static string _apiKey;
        private static string _apiSecret;
        private static string _relativeСryptocurrency = "USDT";
        
        private static BinanceClient _binanceClient = new BinanceClient(new ApiClient(_apiKey, _apiSecret));

        private static void Main(string[] args) // apiKey, apiSecret, discordWebHookUrl, optional relativeСryptocurrency
        {
            if (args.Length < 3) return;
            
            _apiKey = args[0];
            _apiSecret = args[1];
            _discordUrl = args[2];

            try { _relativeСryptocurrency = args[3]; }
            catch {}
            GetChangePrices();
        }

        private static void GetChangePrices(List<SymbolPrice> oldPriceList = null)
        {
            var symbolList = new List<ArrayList>();
            var loop = 0;
            while (true)
            {
                var resultSymbolPrice = new List<SymbolPrice>();
                var allPrices = _binanceClient.GetAllPrices().Result.ToArray();
                var pricesList = allPrices.Where(t => t.Symbol.Contains(_relativeСryptocurrency)).ToList();
                loop++;

                if (oldPriceList != null)
                {
                    symbolList.AddRange(oldPriceList.Select(t => new ArrayList { t.Symbol, 0M, 0M, 0M }));
                    for (int i = 0; i < oldPriceList.Count; i++)
                    {
                        decimal a = Math.Round((pricesList[i].Price - oldPriceList[i].Price) / oldPriceList[i].Price * 100, 2);

                        if (a < 0.25M) continue;
                        symbolList[i][loop] = a;
                    }
                }

                if (loop == 3)
                {
                    foreach (var t in symbolList)
                    {
                        decimal p1 = (decimal)t[1], p2 = (decimal)t[2], p3 = (decimal)t[3];

                        if (p1 == 0 || p2 == 0 || p3 == 0) continue;
                        var price = new SymbolPrice {Symbol = (string) t[0], Price = p1 + p2 + p3};
                        resultSymbolPrice.Add(price);
                    }
                    resultSymbolPrice.Sort(new PriceComparer());
                    foreach (var t in resultSymbolPrice)
                    {
                        if (t.Price > 6.0M)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + t.Symbol.Insert(t.Symbol.LastIndexOf('U'), "/") + " = " + t.Price + "%");
                            Console.ResetColor();
                            SendDiscordWebhook($@"```{DateTime.Now.ToLongTimeString()} Сильный рост!!! {t.Symbol.Insert(t.Symbol.LastIndexOf('U'), "/")} = {t.Price}%``` https://www.binance.com/ru/trade/{t.Symbol.Insert(t.Symbol.LastIndexOf('U'), "_")}?layout=pro");
                        }
                        else if (t.Price > 3.0M)
                        {
                            Console.WriteLine(DateTime.Now.ToLongTimeString() + " " + t.Symbol.Insert(t.Symbol.LastIndexOf('U'), "/") + " = " + t.Price + "%");
                        }
                    }

                    loop = 0;
                    symbolList = new List<ArrayList>();
                }
                
                Thread.Sleep(20000);
                oldPriceList = pricesList;
            }
        }

        private static void SendDiscordWebhook(string message)
        {
            try { new WebClient().UploadValues(_discordUrl, new NameValueCollection { { "content", message } }); }
            catch (Exception) {}
        }
    }

    internal class PriceComparer : IComparer<SymbolPrice>
    {
        public int Compare(SymbolPrice p1, SymbolPrice p2)
        {
            if (p2 != null && p1 != null && p1.Price < p2.Price)
                return 1;
            if (p2 != null && p1 != null && p1.Price > p2.Price)
                return -1;
            return 0;
        }
    }
}
