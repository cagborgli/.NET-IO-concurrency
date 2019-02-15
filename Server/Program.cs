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
        public static void  Main(string[] args)
        {
            try
            {
                var requestSerializer = new XmlSerializer(typeof(QuoteRequest));
                var responseSerializer = new XmlSerializer(typeof(QuoteResponse));
                var quotesResponse = new QuoteResponse();

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

                     Task.Run(() =>
                     {
                         Console.WriteLine("Received connection.");
                         using (var stream = client.GetStream())
                         {
                            // Deserialize QuoteRequest
                            var request = (QuoteRequest)requestSerializer.Deserialize(stream);

                            // Fetch quotes from stock API
                            Console.WriteLine("Fetching quotes");

                            var quotes = Stocks.Api.FetchQuotes(request).Result;

                             int j = 0;
                             foreach ( var i in quotes)
                             {
                                 
                                 quotesResponse.QuotesRes.Add(new QuoteAttributes());
                                 quotesResponse.QuotesRes[j].Symbol = i.Symbol;

                                 foreach(var k in request.Fields)
                                 {
                                     if(k == QuoteField.High)
                                     {
                                         quotesResponse.QuotesRes[j].High = i.High;
                                     }
                                     if (k == QuoteField.Low)
                                     {
                                         quotesResponse.QuotesRes[j].Low = i.Low;
                                     }
                                     if (k == QuoteField.Close)
                                     {
                                         quotesResponse.QuotesRes[j].Close = i.Close;
                                     }
                                 }


     
                             }
                        
                            // Convert response into formatted string
                            //var quoteString = new QuoteResponse
                            // {
                            //     QuoteString = Stocks.Utils.QuotesToString(quotes, request.Fields)
                            // };

                            // Serialize response back to client
                            Console.WriteLine("Sending quote");
                             responseSerializer.Serialize(stream, quotesResponse);
                         }
                     });
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}