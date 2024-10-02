using ProductionLineSimulation.Communication.Data;
using ProductionLineSimulation.LineCommunication.Data;
using ProductionLineSimulation.LineCommunication.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductionLineSimulation.LineCommunication.Services
{
    public class LineClient
    {
        private TcpClient? _tcpClient;
        private int _port;
        private string _host;
        object _locker = new object();
        public LineClient(string ip, int port)
        {
            _host = ip;
            _port = port;
        }
        public void Connect()
        {
            _tcpClient = new TcpClient();
            _tcpClient.Connect(_host, _port);
        }
        public void Disconnect()
        {
            _tcpClient!.Close();
        }
        public LinePacketReply? SendRequest(LinePacket packet)
        {
            lock (_locker)
            {
                if (_tcpClient == null || !_tcpClient.Connected) { return null; }
                NetworkStream stream = _tcpClient.GetStream();
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        Converters = { new LinePacketConverter() },
                        WriteIndented = true
                    };
                    string jsonString = JsonSerializer.Serialize(packet, options);
                    /// ez egy sebtapasz 
                    if (DEBUG)
                    {
                        jsonString= jsonString.Replace("PalettaPolizeiPro,", "ProductionLineSimulation,");
                    }
                    ///
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonString);

                    int length = jsonBytes.Length;
                    byte[] lengthBytes = BitConverter.GetBytes(length);

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(lengthBytes);
                    }

                    stream.Write(lengthBytes, 0, lengthBytes.Length);
                    stream.Write(jsonBytes, 0, jsonBytes.Length);
                    stream.Flush();

                    byte[] responseLengthBytes = new byte[4];
                    Task<int> readLengthTask = stream.ReadAsync(responseLengthBytes, 0, responseLengthBytes.Length);
                    if (Task.WhenAny(readLengthTask, Task.Delay(3000)).Result != readLengthTask || readLengthTask.Result < responseLengthBytes.Length)
                    {
                        return null;
                    }

                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(responseLengthBytes);
                    }

                    int responseLength = BitConverter.ToInt32(responseLengthBytes, 0);

                    byte[] responseBytes = new byte[responseLength];
                    int totalBytesRead = 0;
                    while (totalBytesRead < responseLength)
                    {
                        Task<int> readMessageTask = stream.ReadAsync(responseBytes, totalBytesRead, responseLength - totalBytesRead);
                        if (Task.WhenAny(readMessageTask, Task.Delay(3000)).Result != readMessageTask || readMessageTask.Result == 0)
                        {
                            return null;
                        }
                        totalBytesRead += readMessageTask.Result;
                    }

                    if (totalBytesRead == responseLength)
                    {
                        string responseString = Encoding.UTF8.GetString(responseBytes);
                        var options2 = new JsonSerializerOptions
                        {
                            Converters = { new LinePacketReplyConverter() },
                            WriteIndented = true
                        };
                        LinePacketReply? reply = JsonSerializer.Deserialize<LinePacketReply>(responseString, options2);

                        return reply;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while sending request and receiving reply: " + ex.Message);
                }

                return null;
            }
        }


    }
}
