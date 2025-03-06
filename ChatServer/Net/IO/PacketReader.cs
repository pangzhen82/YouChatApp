using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatServer.Net.IO
{
    internal class PacketReader : BinaryReader
    {
        private NetworkStream _ns;

        public PacketReader(NetworkStream ns) : base(ns, Encoding.UTF8, leaveOpen: true)
        {
            _ns = ns;
        }
        public string ReadMessage()
        {
            byte[] msgBuffer;
            var length = ReadInt32();
            msgBuffer = ReadBytes(length);
            var msg = Encoding.UTF8.GetString(msgBuffer);

            return msg;
        }
    }
}
