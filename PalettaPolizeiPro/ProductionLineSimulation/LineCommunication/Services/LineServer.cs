using ProductionLineSimulation.Communication.Data;
using ProductionLineSimulation.LineCommunication.Data;
using ProductionLineSimulation.LineCommunication.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Services
{
    public class LineServer
    {
        private IPEndPoint _ipEndPoint;
        private TcpListener _listener;
        public event EventHandler<LinePacketReceivedEventArgs> PacketReceived = delegate { };
        Thread _listenThread;
        private bool _listening = false;
        public LineServer(int port)
        {
            _ipEndPoint = new IPEndPoint(IPAddress.Any, port);
            _listener = new(_ipEndPoint);
            _listenThread = new Thread(ListenJob);
        }
        public void Start()
        {
            _listening = true;
            _listenThread.Start();
        }
        public void Stop() { _listening = false; }
        private void ListenJob()
        {
            _listener.Start();
            while (_listening)
            {
                HandleClient();
            }
            _listener.Stop();
        }
        private void HandleClient()
        {
            ReceivePacket(_listener.AcceptTcpClient());
        }
        private void ReceivePacket(TcpClient client)
        {
            Console.WriteLine("a client connected");
            Task.Run(async () =>
            {
                NetworkStream stream = client.GetStream();
                try
                {
                    while (client.Connected)
                    {
                        if (stream.DataAvailable)
                        {
                            byte[] lengthBytes = new byte[4];
                            int bytesRead = await stream.ReadAsync(lengthBytes, 0, lengthBytes.Length);
                            if (bytesRead < lengthBytes.Length)
                            {
                                continue; 
                            }

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(lengthBytes);
                            }

                            int messageLength = BitConverter.ToInt32(lengthBytes, 0);

                            byte[] messageBytes = new byte[messageLength];
                            int totalBytesRead = 0;

                            while (totalBytesRead < messageLength)
                            {
                                bytesRead = await stream.ReadAsync(messageBytes, totalBytesRead, messageLength - totalBytesRead);
                                if (bytesRead == 0)
                                {
                                    break;
                                }
                                totalBytesRead += bytesRead;
                            }

                            if (totalBytesRead == messageLength)
                            {
                                string receivedData = Encoding.UTF8.GetString(messageBytes);
                                var options = new JsonSerializerOptions
                                {
                                    Converters = { new LinePacketConverter() },
                                    WriteIndented = true
                                };
                                LinePacket? packet = JsonSerializer.Deserialize<LinePacket>(receivedData, options);

                                if (packet != null)
                                {
                                    PacketReceived?.Invoke(this, new LinePacketReceivedEventArgs { Packet = packet, Stream = stream });
                                }
                            }
                        }
                        await Task.Delay(10); 
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    stream.Close();
                    stream.Dispose();
                    client.Dispose();
                }
            });
        }

        public void Reply(LinePacketReceivedEventArgs args, LinePacketReply packet)
        {
            try
            {
                
                var options = new JsonSerializerOptions
                {
                    Converters = { new LinePacketReplyConverter() },
                    WriteIndented = true
                };
                string jsonString = JsonSerializer.Serialize(packet,options);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);
                int length = jsonBytes.Length;
                byte[] lengthBytes = BitConverter.GetBytes(length);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBytes);
                }
                args.Stream.Write(lengthBytes, 0, lengthBytes.Length);
                args.Stream.Write(jsonBytes, 0, jsonBytes.Length);
                args.Stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while sending message: " + ex.Message);
            }
        }
    }
}
