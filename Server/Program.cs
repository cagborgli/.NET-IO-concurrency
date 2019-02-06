using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Core;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var requestSerializer = new XmlSerializer(typeof(QuoteRequest));
                var responseSerializer = new XmlSerializer(typeof(QuoteResponse));

                var port = 3000;
                var address = IPAddress.Parse("127.0.0.1");
                
                // Start TcpListener
                var listener = new TcpListener(address, port);
                listener.Start();

                // Await connection from client
                Console.WriteLine("Awaiting connection...");
                while (true)
                {
                    var client = listener.AcceptTcpClient();

                    Console.WriteLine("Received connection.");
                    using (NetworkStream stream = client.GetStream())
                    {
                        // Deserialize QuoteRequest
                        var request = (QuoteRequest)requestSerializer.Deserialize(stream);

                        // Fetch quotes from stock API
                        Console.WriteLine("Fetching quotes");
                        var quotes = await Stocks.Api.FetchQuotes(request);

                        // Convert response into formatted string
                        var quoteString = new QuoteResponse
                        {
                            QuoteString = Stocks.Utils.QuotesToString(quotes, request.Fields)
                        };

                        // Serialize response back to client
                        Console.WriteLine("Sending quote");
                        responseSerializer.Serialize(stream, quoteString);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}