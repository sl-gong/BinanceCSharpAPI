# BinanceCSharpAPI
BinanceCSharpAPI is a lightweight client C# library for the Binance REST API

Example 

// dependence Newtonsoft.Json

https://www.newtonsoft.com/json

// api_key and secret_key can get from your binance account
BinanceExchangeCSharp.BinanceCSharpAPI.setKey("your api key",
"your secret key");

string json_result = "";

BinanceExchangeCSharp.BinanceCSharpAPI.get_ping(ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_serverTime();

BinanceExchangeCSharp.BinanceCSharpAPI.get_depth("BNBBTC", ref json_result);

JObject depth = JObject.Parse(json_result);

JArray bids = depth["bids"] as JArray;

JToken bid0Price = bids[0][0];

JToken bid0quantity = bids[0][1];

JArray asks = depth["asks"] as JArray;

BinanceExchangeCSharp.BinanceCSharpAPI.get_24hr("BNBBTC", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_aggTrades("BNBBTC", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_allBookTickers(ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_allOrders("BNBBTC", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_allPrices(ref json_result);

JArray allPrices = JArray.Parse(json_result);

for (int i = 0; i < allPrices.Count; i++)
{
	JToken symbol = allPrices[i]["symbol"];
	JToken price = allPrices[i]["price"];
}

BinanceExchangeCSharp.BinanceCSharpAPI.get_depth("BNBBTC", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_klines("BNBBTC", "1d", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_myTrades("BNBBTC", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_openOrders("BNBBTC", ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.get_order("BNBBTC", ref json_result, 0, "ZFb87ptNydZNBwVoTBeUdR");

BinanceExchangeCSharp.BinanceCSharpAPI.get_account(ref json_result);

BinanceExchangeCSharp.BinanceCSharpAPI.post_order("BNBBTC", "BUY", "LIMIT", "GTC", 100, 0.00001, ref json_result);


