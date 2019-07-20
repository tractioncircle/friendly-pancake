using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace netmonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = "8.8.8.8";
            var timeout = 1024;
            var outputpath = "pinglog.csv";
            var intervalInSeconds = 15;

            while(true)
            {
                var reply = PingHost(host, timeout);
                WriteEntry(outputpath, reply);
                Thread.Sleep(intervalInSeconds * 1000);
            }
        }

        public static Data PingHost(string host, int timeout)
        {
            var output = new Data();
            output.TimeStamp = DateTime.Now;


            var pinger = new Ping();
            var options = new PingOptions
            {
                DontFragment = true
            };

            byte[] buffer = Encoding.ASCII.GetBytes("the quick brown fox jumped over the lazy dog");

            Log($"Pinging {host}");
            
            try
            {
                PingReply reply = pinger.Send(host, timeout, buffer, options);
                output.Status = reply.Status.ToString();
                output.Roundtrip = reply.RoundtripTime;

                if(reply.Status != IPStatus.Success)
                {
                    Log($"ERROR - {reply.Status}");
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                output.Status = "FATAL";
                output.Roundtrip = 0;
            }

            return output;
        }

        public static void WriteEntry(string path, Data data)
        {
            using (var writer = File.AppendText(path))
            {
                writer.WriteLine($"{data.TimeStamp},{data.Roundtrip},{data.Status}");
            }
        }

        public static void Log(string message)
        {
            Console.WriteLine($"{DateTime.Now} - {message}");
        }
    }
}
