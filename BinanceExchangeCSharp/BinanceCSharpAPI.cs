using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace BinanceExchangeCSharp
{
    /// <summary>
    /// Binance Exchange Get Request
    /// </summary>
    public class BinanceCSharpAPI
    {
        // API-keys, and secret-keys are case sensitive
        private static string _api_key = "";
                
        private static string _secret_key = "";
        
        // binance host
        private static string _binance_host = "https://www.binance.com";
        
        /// <summary>
        /// Request by WebClient
        /// </summary>
        /// <param name="url">url to request</param>
        /// <param name="strResult">request result</param>
        public static void publicRequest(string url, ref string strResult)
        {
            strResult = "";
            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; CSharp Binance API)");

            Stream data = client.OpenRead(url);
            
            StreamReader reader = new StreamReader(data);
            strResult = reader.ReadToEnd();
        }

        /// <summary>
        /// apiRequest
        /// </summary>
        /// <param name="url">url to post</param>
        /// <param name="action">GET/POST</param>
        /// <param name="strResult">post result</param>
        public static void apiRequest(string url, string action, ref string strResult)
        {
            strResult = "";
            
            HttpWebResponse httpWebResponse = null;
            StreamReader streamReader = null;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            
            httpWebRequest.Method = action;
            httpWebRequest.Headers.Add("X-MBX-APIKEY", _api_key);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            if (streamReader == null)
            {
                return;
            }
            strResult = streamReader.ReadToEnd();
        }

        /// <summary>
        /// signedRequest
        /// SIGNED Endpoint security
        /// </summary>
        /// <param name="url">url to request</param>
        /// <param name="queryString">query string</param>
        /// <param name="action">GET/POST</param>
        /// <param name="strResult">request result</param>
        public static void signedRequest(string url, string queryString, string signature, string action, ref string strResult)
        {
            strResult = "";
            string requestStr = url + queryString + "&signature=" + signature;

            HttpWebResponse httpWebResponse = null;
            StreamReader streamReader = null;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestStr);
            
            httpWebRequest.Method = action;
            httpWebRequest.Headers.Add("X-MBX-APIKEY", _api_key);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            streamReader = new StreamReader(httpWebResponse.GetResponseStream());
            if (streamReader == null)
            {
                return;
            }
            strResult = streamReader.ReadToEnd();
        }

        /// <summary>
        /// initial key and secretKey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="secretKey"></param>
        public static void setKey(string apiKey, string secretKey)
        {
            _api_key = apiKey;
            _secret_key = secretKey;
        }

        /// <summary>
        /// set log path
        /// </summary>
        /// <param name="logPath">set log </param>
        public static void setLogPath(string logPath)
        {
            Common.setLogPath(logPath);
        }

        /// <summary>
        /// Test connectivity to the Rest API.
        /// GET /api/v1/ping
        /// </summary>
        /// <param name="json_result">return json string</param>
        public static void get_ping(ref string json_result)
        {
            try 
	        {
                string url = _binance_host + "/api/v1/ping";
                publicRequest(url, ref json_result);
	        }
	        catch (Exception ex)
	        {
                Common.writeLog("get_ping:" + ex.Message);             
	        }
        }

        /// <summary>
        /// Test connectivity to the Rest API and get the current server time.
        /// GET /api/v1/time
        /// </summary>
        /// <param name="json_result">return json string</param>
        public static long get_serverTime()
        {
            try
            {
                string url = _binance_host + "/api/v1/time";

                string json_result = "";
                publicRequest(url, ref json_result);

                int pos1 = json_result.LastIndexOf('}');
                int pos2 = json_result.LastIndexOf(':');
                string timeStr = json_result.Substring(pos2 + 1, pos1 - pos2 - 1);
                return long.Parse(timeStr);
            }
	        catch (Exception ex)
	        {
                Common.writeLog("get_serverTime:" + ex.Message);             
                return 0;
	        }
        }

        /// <summary>
        /// Order book.
        /// GET /api/v1/depth
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param>
        /// <param name="limit">Default 100; legal range is '50, 20, 100, 500, 5, 200, 10'</param>
        public static void get_depth(
            string symbol,
            ref string json_result,
            int limit = 100)
        {
            string url = string.Format("{0}/api/v1/depth?symbol={1}&limit={2}",
                    _binance_host,
                    symbol,
                    limit);
            try
            {
                publicRequest(url,ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_depth:" + url + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// Get compressed, aggregate trades. 
        /// Trades that fill at the time, from the same order, 
        /// with the same price will have the quantity aggregated.
        /// GET /api/v1/aggTrades
        /// e.g.    https://www.binance.com/api/v1/aggTrades?symbol=BNBBTC&limit=50
        /// If both startTime and endTime are sent, limit should not be sent AND the distance between startTime and endTime must be less than 24 hours.
        /// If fromdId, startTime, and endTime are not sent, the most recent aggregate trades will be returned.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE.</param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 500.; legal range is 1-500.</param>
        public static void get_aggTrades(
            string symbol,
            ref string json_result,
            long startTime = 0,
            long endTime = 0,
            long fromId = 0,
            int limit = 500)
        {
            StringBuilder strUrl = new StringBuilder();
            strUrl.AppendFormat("{0}/api/v1/aggTrades?symbol={1}",_binance_host,symbol);
            if (fromId != 0)
            {
                strUrl.AppendFormat("&fromId={0}",fromId);
            }

            if (startTime > 0 && endTime > 0 && endTime > startTime)
            {
                strUrl.AppendFormat("&startTime={0}", startTime);
                strUrl.AppendFormat("&endTime={0}", endTime);
            }

            if (limit > 0)
            {
                strUrl.AppendFormat("&limit={0}", limit);
            }

            try
            {
                publicRequest(strUrl.ToString(), ref json_result);
            }
            catch (Exception ex)
            {
                Common.writeLog("get_aggTrades:" + strUrl + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// Kline/candlestick bars for a symbol. Klines are uniquely identified by their open time.
        /// GET /api/v1/klines
        /// e.g. https://www.binance.com/api/v1/klines?symbol=BNBBTC&interval=1d&limit=10
        /// 
        /// If startTime and endTime are not sent, the most recent klines are returned.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="interval">1m,3m,5m,15m,30m,1h,2h,4h,6h,8h,12h,1d,3d,1w,1M</param>
        /// <param name="json_result">return json string</param>
        /// <param name="limit">Default 500; max 500</param>        
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public static void get_klines(
            string symbol,
            string interval,
            ref string json_result,
            int limit = 500,
            long startTime = 0,
            long endTime = 0)
        {
            StringBuilder strURL = new StringBuilder();
            strURL.AppendFormat("{0}/api/v1/klines?symbol={1}&interval={2}", _binance_host, symbol, interval);

            if (startTime > 0 && endTime > 0)
            {
                strURL.AppendFormat("&startTime={0}", startTime);
                strURL.AppendFormat("&endTime={0}", endTime);
            }

            if (limit > 0)
            {
                strURL.AppendFormat("&limit={0}", limit);
            }

            try
            {
                publicRequest(strURL.ToString(), ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_klines:" + strURL + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// 24hr ticker price change statistics
        /// GET /api/v1/ticker/24hr
        /// e.g.    https://www.binance.com/api/v1/ticker/24hr?symbol=BNBBTC
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param>
        public static void get_24hr(string symbol,ref string json_result)
        {
            string url = _binance_host + "/api/v1/ticker/24hr?symbol=" + symbol;

            try
            {
                publicRequest(url.ToString(), ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_24hr:" + url + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// Best price/qty on the order book for all symbols.
        /// GET /api/v1/ticker/allPrices
        /// e.g.    https://www.binance.com/api/v1/ticker/allPrices
        /// </summary>
        /// <param name="json_result">return json string</param>
        public static void get_allPrices(ref string json_result)
        {
            string url = _binance_host + "/api/v1/ticker/allPrices";

            try
            {
                publicRequest(url.ToString(), ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_allPrices:" + url + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// Best price/qty on the order book for all symbols.
        /// GET /api/v1/ticker/allBookTickers 
        /// e.g.    https://www.binance.com/api/v1/ticker/allBookTickers
        /// </summary>
        /// <param name="json_result">return json string</param>
        public static void get_allBookTickers(ref string json_result)
        {
            string url = _binance_host + "/api/v1/ticker/allBookTickers";

            try
            {
                publicRequest(url.ToString(), ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_allBookTickers:" + url + "\t" + ex.Message);             
            }
        }

        //---------------------------------Account endpoints---------------------------------

        /// <summary>
        /// New order (SIGNED)
        /// POST /api/v3/order
        /// e.g.   
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="side">BUY,SELL</param>
        /// <param name="type">LIMIT,MARKET</param>
        /// <param name="timeInForce">GTC,IOC</param>
        /// <param name="quantity">quantity</param>
        /// <param name="price">price</param>        
        /// <param name="json_result">return json string</param> 
        /// <param name="newClientOrderId">A unique id for the order. Automatically generated if not sent.</param>
        /// <param name="stopPrice">Used with stop orders</param>
        /// <param name="icebergQty">Used with iceberg orders</param>
        public static void post_order(
            string symbol,
            string side,
            string type,
            string timeInForce,
            double quantity,
            double price,            
            ref string json_result,
            string newClientOrderId = "",
            double stopPrice = 0.0,
            double icebergQty = 0.0)
        {
            string url = _binance_host + "/api/v3/order?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}&side={1}&type={2}&timeInForce={3}&quantity={4:0.00000000}&price={5:0.00000000}",
                symbol, side, type, timeInForce, quantity, price);

            if (newClientOrderId.Length > 0)
            {
                strQuery.AppendFormat("&newClientOrderId={0}", newClientOrderId);
            }

            if (stopPrice > 0.0)
            {
                strQuery.AppendFormat("&stopPrice={0}", stopPrice);
            }

            if (icebergQty > 0.0)
            {
                strQuery.AppendFormat("&icebergQty={0}", icebergQty);
            }

            strQuery.AppendFormat("&recvWindow={0}", 5000);

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "POST", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("post_order:" + url + "\t" + strQuery + "\t" + ex.Message);             
            }            
        }

        /// <summary>
        /// New order (SIGNED)
        /// POST /api/v3/order/test
        /// e.g.    
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="side">BUY,SELL</param>
        /// <param name="type">LIMIT,MARKET</param>
        /// <param name="timeInForce">GTC,IOC</param>
        /// <param name="quantity">quantity</param>
        /// <param name="price">price</param>        
        /// <param name="json_result">return json string</param> 
        /// <param name="newClientOrderId">A unique id for the order. Automatically generated if not sent.</param>
        /// <param name="stopPrice">Used with stop orders</param>
        /// <param name="icebergQty">Used with iceberg orders</param>
        public static void post_order_test(
            string symbol,
            string side,
            string type,
            string timeInForce,
            double quantity,
            double price,
            ref string json_result,
            string newClientOrderId = "",
            double stopPrice = 0.0,
            double icebergQty = 0.0)
        {
            string url = _binance_host + "/api/v3/order/test?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}&side={1}&type={2}&timeInForce={3}&quantity={4:0.00000000}&price={5:0.00000000}",
                symbol, side, type, timeInForce, quantity, price);

            if (newClientOrderId.Length > 0)
            {
                strQuery.AppendFormat("&newClientOrderId={0}", newClientOrderId);
            }

            if (stopPrice > 0.0)
            {
                strQuery.AppendFormat("&stopPrice={0}", stopPrice);
            }

            if (icebergQty > 0.0)
            {
                strQuery.AppendFormat("&icebergQty={0}", icebergQty);
            }

            strQuery.AppendFormat("&recvWindow={0}", 5000);

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "POST", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("post_order_test:" + url + "\t" + strQuery + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// Query order (SIGNED)
        /// GET /api/v3/order
        /// Check an order's status.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param> 
        /// <param name="orderId">orderId can get from openOrders</param>
        /// <param name="origClientOrderId">origClientOrderId can get from openOrders</param>
        /// <param name="recvWindow"></param>
        public static void get_order(
            string symbol,
            ref string json_result,
            long orderId = 0,
            string origClientOrderId = "",
            long recvWindow = 5000)
        {
            //Either orderId or origClientOrderId must be sent.
            if (orderId == 0 && origClientOrderId == "")
            {
                return;
            }

            string url = _binance_host + "/api/v3/order?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}",symbol);

            if (orderId > 0)
            {
                strQuery.AppendFormat("&orderId={0}", orderId);
            }

            if (origClientOrderId.Length > 0)
            {
                strQuery.AppendFormat("&origClientOrderId={0}", origClientOrderId);
            }

            if (recvWindow > 0)
            {
                strQuery.AppendFormat("&recvWindow={0}", recvWindow);
            }

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "GET", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_order:" + url + "\t" + strQuery + "\t" + ex.Message);             
            }
        }

        /// <summary>
        /// Cancel order (SIGNED)
        /// DELETE /api/v3/order
        /// Cancel an active order.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param> 
        /// <param name="orderId">orderId can get from openOrders</param>
        /// <param name="origClientOrderId">orderId can get from openOrders</param>
        /// <param name="recvWindow"></param>
        public static void delete_order(
            string symbol,
            ref string json_result,
            long orderId = 0,
            string origClientOrderId = "",
            long recvWindow = 5000)
        {
            string url = _binance_host + "/api/v3/order?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}", symbol);

            if (orderId > 0)
            {
                strQuery.AppendFormat("&orderId={0}", orderId);
            }

            if (origClientOrderId.Length > 0)
            {
                strQuery.AppendFormat("&origClientOrderId={0}", origClientOrderId);
            }

            if (recvWindow > 0)
            {
                strQuery.AppendFormat("&recvWindow={0}", recvWindow);
            }

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "DELETE", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("delete_order:" + url + "\t" + strQuery + "\t" + ex.Message);             
            }
            
        }

        /// <summary>
        /// Current open orders (SIGNED)
        /// GET /api/v3/openOrders
        /// Get all open orders on a symbol.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param> 
        /// <param name="recvWindow"></param>
        public static void get_openOrders(string symbol, ref string json_result, long recvWindow = 5000)
        {
            string url = _binance_host + "/api/v3/openOrders?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}", symbol);

            if (recvWindow > 0)
            {
                strQuery.AppendFormat("&recvWindow={0}", recvWindow);
            }

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "GET", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_openOrders:" + url + "\t" + strQuery + "\t" + ex.Message);             
            }
            
        }

        /// <summary>
        /// All orders (SIGNED)
        /// GET /api/v3/allOrders
        /// Get all account orders; active, canceled, or filled.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param> 
        /// <param name="orderId"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        public static void get_allOrders(
            string symbol,
            ref string json_result,
            long orderId = 0,
            int limit = 500,
            long recvWindow = 5000)
        {
            string url = _binance_host + "/api/v3/allOrders?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}", symbol);

            if (orderId > 0)
            {
                strQuery.AppendFormat("&orderId={0}", orderId);
            }

            if (limit > 0 && limit < 500)
            {
                strQuery.AppendFormat("&limit={0}", limit);
            }

            if (recvWindow > 0)
            {
                strQuery.AppendFormat("&recvWindow={0}", recvWindow);
            }

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "GET", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_allOrders:" + url + "\t" + strQuery + "\t" + ex.Message);             
            }
            
        }

        /// <summary>
        /// Account trade list (SIGNED)
        /// GET /api/v3/myTrades
        /// Get trades for a specific account and symbol.
        /// </summary>
        /// <param name="symbol">e.g. BNBBTC,ETHBTC,BNBETH...</param>
        /// <param name="json_result">return json string</param> 
        /// <param name="limit"></param>
        /// <param name="fromId"></param>
        /// <param name="recvWindow"></param>
        public static void get_myTrades(
            string symbol,
            ref string json_result,
            int limit = 500,
            long fromId = 0,
            long recvWindow = 5000)
        {
            string url = _binance_host + "/api/v3/myTrades?";

            StringBuilder strQuery = new StringBuilder();
            strQuery.AppendFormat("symbol={0}", symbol);

            if (limit > 0 && limit < 500)
            {
                strQuery.AppendFormat("&limit={0}", limit);
            }

            if (fromId > 0)
            {
                strQuery.AppendFormat("&fromId={0}", fromId);
            }

            if (recvWindow > 0)
            {
                strQuery.AppendFormat("&recvWindow={0}", recvWindow);
            }

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "GET", ref json_result);
            }
            catch (Exception ex)
            {
                Common.writeLog("get_myTrades:" + url + "\t" + strQuery + "\t" + ex.Message);
            }
            
        }

        /// <summary>
        /// Account information (SIGNED)
        /// GET /api/v3/account
        /// Get current account information.
        /// </summary>
        /// <param name="json_result">return json string</param> 
        /// <param name="recvWindow"></param>
        public static void get_account(ref string json_result, long recvWindow = 5000)
        {
            string url = _binance_host + "/api/v3/account?";
            StringBuilder strQuery = new StringBuilder();

            // SIGNED endpoints require an additional parameter, timestamp,signature
            strQuery.AppendFormat("&timestamp={0}", get_serverTime());
            if (recvWindow > 0)
            {
                strQuery.AppendFormat("&recvWindow={0}", recvWindow);
            }

            string signature = Common.hash_hmac(strQuery.ToString(), _secret_key);

            try
            {
                signedRequest(url, strQuery.ToString(), signature, "GET", ref json_result);
            }
            catch (Exception ex)
            {                
                Common.writeLog("get_account:" + url + "\t" + strQuery + "\t" + ex.Message);                
            }
        }
    }
    
}
