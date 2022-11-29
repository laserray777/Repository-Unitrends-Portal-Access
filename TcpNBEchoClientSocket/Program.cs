using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;    // For IPEndpoint, Dns
using System.Net.Sockets;  // For TcpClient, NetworkStream, SocketException
using System.Threading;
using System.Runtime.CompilerServices;

namespace TcpNBEchoClientSocket
{
    class Program
    {
        private const int TIMEOUT = 3000;  // Resend timeout (milliseconds)
        private const int MAXTRIES = 5;    // Maximum re-transmissions

        static void Main(string[] args)
        {
            if ((args.Length < 2) || (args.Length > 3)) // Test for correct # of args
                throw new ArgumentException("Parameters: <Server>, <Word>, <Port>");

            String server = args[0];  // Server name or ip address
                                      // Convert input string to bytes.
            byte[] byteBuffer = Encoding.ASCII.GetBytes(args[1]);

            // Use port argument if supplied, otherwise default to 7.
            int servPort = (args.Length == 3) ? Int32.Parse(args[2]) : 7;

            /* experimental section*/
            IPAddress parsedIPAddress = IPAddress.Parse("64.124.176.192");

            // Create Socket and connect
            Socket sockUdp = null;
            try
            {
                Console.WriteLine("Try to create sockUdp and bidnd it to Unitrends las-ltr portal endpoint.");
                sockUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
             //   sockUdp.Bind(new IPEndPoint(IPAddress.Any, servPort));

                // Set Socket Options
                // set the receive timeout for this socket.
                sockUdp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TIMEOUT);
                // set the Debug socket option name
                sockUdp.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, true);


            }
            catch (SocketException se)
            {
                Console.WriteLine("Message: {0}", se.Message);
                Console.WriteLine("ErrorCode: {0}", se.ErrorCode);
            }

            //     Dns.GetHostEntry(server).AddressList;

            // convert input string to a packet of bytes.
            byte[] sendPacket = Encoding.ASCII.GetBytes(args[1]);
            byte[] rcvPacket = new byte[300];

            IPHostEntry ipHostInfo = Dns.GetHostEntry("na3-ltr-las-4c260959fb4d-msp.unitrendscloud.com");

            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteIPEndPoint = new IPEndPoint(ipAddress, servPort);
            EndPoint remoteEndPoint = (EndPoint)remoteIPEndPoint; 
         
            
            //   IPEndPoint remoteIPEndPoint = new IPEndPoint(Dns.GetHostEntry(server).AddressList[0], servPort);

           

            int tries = 0; // Packets may be lost, so we have to keep trying
            Boolean receivedResponse = false;
            do
            {
                sockUdp.SendTo(sendPacket,   remoteEndPoint); // Send the echo string


                Console.WriteLine("Sent {0} bytes to the server...", sendPacket.Length);

                try
                {
                  
                    //  Attempt echo reply received.
                  sockUdp.ReceiveFrom(rcvPacket,ref  remoteEndPoint);

                    receivedResponse = true;
                }
                catch (SocketException se)
                {
                    tries++;
                    if (se.ErrorCode == 10060)  // WSAETIMEDOUT: connection timed out.
                        Console.WriteLine("Timed out, {0} more tries", (MAXTRIES - tries));
                    else // we encountered an error other than a timeout, output error message.
                        Console.WriteLine(se.ErrorCode + ":" + se.Message);


                }

              

            } while ((!receivedResponse) && (tries < MAXTRIES));

            if (receivedResponse)
                Console.WriteLine("Received {0} bytes from {1}: {2}",
                                   rcvPacket.Length, remoteIPEndPoint,
                                   Encoding.ASCII.GetString(rcvPacket, 0, rcvPacket.Length));
            else
                Console.WriteLine("No response -- giving up");
            Console.ReadKey();
        }
     }
}
