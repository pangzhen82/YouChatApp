using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace YouChat.Net.IO
{
    internal class PacketBuilder : IDisposable
    {
        MemoryStream _ms;
        public PacketBuilder() 
        {
            _ms = new MemoryStream();
        }
        public void WriteOpCode(byte opcode)
        {
            _ms.WriteByte(opcode);
        }
        public void WriteMessage(string msg)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
            _ms.Write(BitConverter.GetBytes(messageBytes.Length));
            _ms.Write(messageBytes);
        }
        public byte[] GetPacketBytes()
        {
            return _ms.ToArray();
        }
        public void Dispose()
        {
            _ms?.Dispose();
        }
    }
}
