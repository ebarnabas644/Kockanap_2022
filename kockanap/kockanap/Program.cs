using System;
using Grpc.Net.Client;
using NikTS.GrpcService;

namespace Kockanap.Grpc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://10.8.11.150:7777");
            var client = new NikTsService.NikTsServiceClient(channel);
            client.Regiser(new UserMessage
            {
                Blue = 139,
                Green = 172,
                Red = 15,
                IpAddress = "10.8.11.162",
                NickName = "XToDoubt",
                Userid = 1,
                PortNumber = 11000,
                Password = "helppls"
            });
            //client.Unregister(new RemoveMessage
            //{
            //    NickName = "Jozsi",
            //    Password = "almafa"
            //});

        }
    }
}