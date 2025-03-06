using ChatServer.Net.IO;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Client
    {
        public string Username  { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }
        PacketReader _packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());
            
            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with the username: {Username}");
            Program.BroadcastMessage($"{Username} is now connected...", "[System]");

            Task.Run(() => Process());
        }
        public void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = _packetReader.ReadMessage();
                            var name = _packetReader.ReadMessage();
                            Program.BroadcastMessage(msg, Username);
                            break;
                        case 6:
                            var whispermsg = _packetReader.ReadMessage();
                            var whisperName = _packetReader.ReadMessage();
                            var whisperReceiverName = _packetReader.ReadMessage();
                            Program.SendMessageTo(whispermsg, whisperName, whisperReceiverName);
                            break;
                        default:
                            Console.WriteLine($"Default opcode({opcode})............"); ////////// ---------> Test line
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now}] {Username}: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
