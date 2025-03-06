using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YouChat.MVVM.Model;
using YouChat.Net.IO;

namespace YouChat.Net
{
    class Server
    {
        TcpClient _client;
        public PacketReader PacketReader;
        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action UserDisconnectEvent;
        public event Action whisperMessageReceivedEvent;

        public Server()
        {
            _client = new TcpClient();
        }
        public void ConnectToServer(string username)
        {
            if (!_client.Connected)
            {
                _client.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(_client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _client.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPacket();
            }
        }
        private void ReadPacket()
        {
            Task.Run(() =>
            { 
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 6:
                            whisperMessageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            UserDisconnectEvent?.Invoke();
                            break;
                        default:
                            break;
                    }
                }
            });
        }
        public void SendMessageToServer(string message, string senderName)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            messagePacket.WriteMessage(senderName);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
        public void SendMessageToUser(string message, string senderName, string receiverName)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(6);
            messagePacket.WriteMessage(message);
            messagePacket.WriteMessage(senderName);
            messagePacket.WriteMessage(receiverName);
            _client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
