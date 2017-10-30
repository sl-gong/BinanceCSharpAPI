# BinanceCSharpAPI
BinanceCSharpAPI is a lightweight client C# library for the Binance REST API.

### // binance rest api

https://www.binance.com/restapipub.html

### // dependence Newtonsoft.Json

https://www.newtonsoft.com/json

## Example

```
### // api_key and secret_key can get from your binance account

BinanceExchangeCSharp.BinanceCSharpAPI.setKey("your api key",
"your secret key");

string json_result = "";

### // Test server connection

BinanceExchangeCSharp.BinanceCSharpAPI.get_ping(ref json_result);

### // Get Server Time

BinanceExchangeCSharp.BinanceCSharpAPI.get_serverTime();

### // Get Order book

BinanceExchangeCSharp.BinanceCSharpAPI.get_depth("BNBBTC", ref json_result);

JObject depth = JObject.Parse(json_result);

JArray bids = depth["bids"] as JArray;

JToken bid0Price = bids[0][0];

JToken bid0quantity = bids[0][1];

JArray asks = depth["asks"] as JArray;

### // Get 24hr ticker price change statistics

BinanceExchangeCSharp.BinanceCSharpAPI.get_24hr("BNBBTC", ref json_result);

### // Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.

BinanceExchangeCSharp.BinanceCSharpAPI.get_aggTrades("BNBBTC", ref json_result);

### // Best price/qty on the order book for all symbols.

BinanceExchangeCSharp.BinanceCSharpAPI.get_allBookTickers(ref json_result);

### // Get all account orders; active, canceled, or filled.

BinanceExchangeCSharp.BinanceCSharpAPI.get_allOrders("BNBBTC", ref json_result);

### // Get Latest price for all symbols.

BinanceExchangeCSharp.BinanceCSharpAPI.get_allPrices(ref json_result);

JArray allPrices = JArray.Parse(json_result);

for (int i = 0; i < allPrices.Count; i++)
{
	JToken symbol = allPrices[i]["symbol"];
	JToken price = allPrices[i]["price"];
}

### // Kline/candlestick bars for a symbol. Klines are uniquely identified by their open time.

BinanceExchangeCSharp.BinanceCSharpAPI.get_klines("BNBBTC", "1d", ref json_result);

### // Get trades for a specific account and symbol.

BinanceExchangeCSharp.BinanceCSharpAPI.get_myTrades("BNBBTC", ref json_result);

### // Get all open orders on a symbol.

BinanceExchangeCSharp.BinanceCSharpAPI.get_openOrders("BNBBTC", ref json_result);

### // Check an order's status.the orderId and origClientOrderId can get from openOrders

BinanceExchangeCSharp.BinanceCSharpAPI.get_order("BNBBTC", ref json_result, 0, "ZFb87ptNydZNBwVoTBeUdR");

### // Cancel an active order.
BinanceExchangeCSharp.BinanceCSharpAPI.delete_order("BNBBTC", ref json_result, 0, "ZFb87ptNydZNBwVoTBeUdR");

### // Get current account information.

BinanceExchangeCSharp.BinanceCSharpAPI.get_account(ref json_result);

### // Send in a new order.

BinanceExchangeCSharp.BinanceCSharpAPI.post_order("BNBBTC", "BUY", "LIMIT", "GTC", 100, 0.00001, ref json_result);

```
