using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace TsdServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = new TcpListener(IPAddress.Any, 8888);
            server.Start();

            Console.WriteLine("Server started. Waiting for clients...");

            while(true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                Task.Run(() => HandleClient(client));
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead;

            while((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                ProcessData(data, stream);
            }

            client.Close();
            Console.WriteLine("Client disconnected.");
        }

        static void ProcessData(string data, NetworkStream stream)
        {
            if(data == "LOAD")
            {
                SendDataToClient(stream);
            } else if (data.StartsWith("+"))
            {
                string[] parts = data.Substring(1).Split(',');
                string productCode = parts[0];
                int quantity = int.Parse(parts[1]);
                AddOrUpdateProduct(productCode, quantity);
            } else if (data.StartsWith("-"))
            {
                string productCode = data.Substring(1);
                DeleteProduct(productCode);
            }
        }

        static void SendDataToClient(NetworkStream stream)
        {
            try
            {
                using(var context = new ProductContext())
                {
                    var products = context.Products.ToList();
                    StringBuilder sb = new StringBuilder();
                    foreach (var product in products)
                    {
                        sb.AppendLine($"{product.Code},{product.Quantity}");
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
                    stream.Write(bytes, 0, bytes.Length);
                }
                Console.WriteLine("Data sent to client.");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error sending data to client: " + ex.Message);
            }
        }

        static void AddOrUpdateProduct(string productCode, int quantity)
        {
            using(var context = new ProductContext())
            {
                var existingProduct = context.Products.FirstOrDefault(p => p.Code == productCode);
                if(existingProduct == null)
                {
                    context.Products.Add(new Product { Code = productCode, Quantity = quantity });
                } else
                {
                    existingProduct.Quantity += quantity;
                }
                context.SaveChanges();
            }

            Console.WriteLine($"Updated product {productCode}, quantity {quantity}");
        }

        static void DeleteProduct(string productCode)
        {
            using(var context = new ProductContext())
            {
                var existingProduct = context.Products.FirstOrDefault(p => p.Code == productCode);
                if(existingProduct != null)
                {
                    context.Products.Remove(existingProduct);
                    context.SaveChanges();
                    Console.WriteLine($"Removed product {productCode}");
                } else
                {
                    Console.WriteLine($"Product {productCode} not found.");
                }
            }
        }
    }
}