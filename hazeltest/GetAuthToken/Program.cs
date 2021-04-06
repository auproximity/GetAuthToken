using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

using Hazel;
using Hazel.Dtls;

namespace GetAuthToken
{
    class Program
    {

        static DtlsUnityConnection connection;

        static AutoResetEvent wait = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            var endpoint = new IPEndPoint(IPAddress.Parse(args[1]), 22025);

            var mode = IPMode.IPv4;
            var rsa = RSA.Create();
            var cert = new X509Certificate2(args[0]);
            var certs = new X509Certificate2Collection(cert);

            connection = new DtlsUnityConnection(new ConsoleLogger(false), endpoint, mode);
            connection.SetValidServerCertificates(certs);
            connection.DataReceived += DataReceived;
            connection.Disconnected += Disconnected;
            connection.Connect(BuildData());
            wait.WaitOne();
        }

        static private byte[] BuildData()
        {
            MessageWriter messageWriter = MessageWriter.Get();
            messageWriter.Write(50531650);
            messageWriter.Write((byte)5);
            messageWriter.Write("");
            var ret = messageWriter.ToByteArray(includeHeader: false);
            //Console.WriteLine(BitConverter.ToString(ret));
            return ret;
        }

        static void Disconnected(object sender, DisconnectedEventArgs e)
        {
            connection.Dispose();
            connection = null;
        }

        static void DataReceived(DataReceivedEventArgs args)
        {
            MessageReader message = args.Message;
            try
            {
                MessageReader messageReader = message.ReadMessage();
                if (messageReader.Tag == 1)
                {
                    Console.WriteLine("TOKEN:" + messageReader.ReadUInt32() + ":TOKEN");
                    connection.Disconnect("Job done");
                    wait.Set();
                }
            }
            finally
            {
                message.Recycle();
            }

        }
    }
}
