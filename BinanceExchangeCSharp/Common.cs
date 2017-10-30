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
    public class Common
    {
        // log path
        private static string _log_path = System.Environment.CurrentDirectory + "\\rest_api.log";

        // log writer
        private static StreamWriter _log_writer = null;

        public static long microsec_time()
        {
            long nEpochTicks = 0;
            long nUnixTimeStamp = 0;
            long nNowTicks = 0;
            long nowMiliseconds = 0;
            string sNonce = "";
            DateTime DateTimeNow;


            nEpochTicks = new DateTime(1970, 1, 1).Ticks;
            DateTimeNow = DateTime.UtcNow;
            nNowTicks = DateTimeNow.Ticks;
            nowMiliseconds = DateTimeNow.Millisecond;

            nUnixTimeStamp = ((nNowTicks - nEpochTicks) / TimeSpan.TicksPerSecond);

            sNonce = nUnixTimeStamp.ToString() + nowMiliseconds.ToString("D03");

            return (Convert.ToInt64(sNonce));
        }

        public static string hash_hmac(string sData,string secret_key)
        {
            byte[] rgbyKey = Encoding.UTF8.GetBytes(secret_key);

            using (var hmacsha256 = new HMACSHA256(rgbyKey))
            {
                hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(sData));

                return (byte2string(hmacsha256.Hash));
            }
        }

        public static string byte2string(byte[] rgbyBuff)
        {
            string sHexStr = "";

            for (int nCnt = 0; nCnt < rgbyBuff.Length; nCnt++)
            {
                sHexStr += rgbyBuff[nCnt].ToString("x2"); // Hex format
            }

            return (sHexStr);
        }

        public static ulong get_current_epoch( ) 
        {
            return (ulong)DateTime.Now.TimeOfDay.TotalSeconds;
        }
        
        public static ulong get_current_ms_epoch( ) 
        {
            System.TimeSpan ts = DateTime.Now.TimeOfDay;
            return (ulong)ts.TotalMilliseconds;
        }

        /// <summary>
        /// set log path
        /// </summary>
        /// <param name="logPath">set log </param>
        public static void setLogPath(string logPath)
        {
            _log_path = logPath;
            if (_log_writer != null)
            {
                _log_writer.Close();
            }
            _log_writer = File.AppendText(_log_path);
        }

        public static void writeLog(string logString)
        {
            if (_log_writer == null)
            {
                _log_writer = File.AppendText(_log_path);
            }
            _log_writer.WriteLine(logString);
            _log_writer.Flush();
        }
    }
}
