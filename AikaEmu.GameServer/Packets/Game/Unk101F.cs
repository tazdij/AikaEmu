using AikaEmu.GameServer.Network;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Packets.Game
{
    public class Unk101F : GamePacket
    {
        public Unk101F()
        {
            Opcode = (ushort) GameOpcode.Unk101F;
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write((ushort) 0); // id1
            stream.Write((ushort) 0); // id2

            stream.Write((ushort) 0); // unk
            stream.Write((ushort) 0); // unk
            return stream;
        }
    }
}