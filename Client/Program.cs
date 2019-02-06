using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Core;

namespace Client
{
    class Program
    {
        public static void Main(string[] args)
        {
            var port = 3000;
            var address = "127.0.0.1";
            
            var requestSerializer = new XmlSerializer(typeof(QuoteRequest));
            var responseSerializer = new XmlSerializer(typeof(QuoteResponse));

            //string filename = "C:\\Users\ftd-09\\Desktop\\Ticker.xml";

            // Example QuoteRequest containing symbols and fields client wishes to fetch data about
            var request = new QuoteRequest
            {
                Fields = new List<QuoteField> { QuoteField.High, QuoteField.Low, QuoteField.Close },
                Symbols = new List<string> { "AAPL", "TSLA", "TWTR", "SNAP", "GOOGL", "AMZN" },
                Interval = 5
            };
            
            try
            {
                while (true)
                {

                    // Initialize connection to server
                    var client = new TcpClient(address, port);

                    using (var stream = client.GetStream())
                    {
                        // Serialize request to server
                        requestSerializer.Serialize(stream, request);

                        // Temporary hacky fix for data preventing read blocking
                        client.Client.Shutdown(SocketShutdown.Send);

                        // Receive QuoteResponse from server
                        QuoteResponse response = (QuoteResponse)responseSerializer.Deserialize(stream);

                        //String to txt file 
                        System.IO.File.WriteAllText(@"C:\Users\ftd-09\Desktop\Assignments\Test.txt", response.QuoteString);
                        //requestSerializer.Serialize(filename, response);

                        Console.WriteLine(response.QuoteString);
                    }
                 
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}