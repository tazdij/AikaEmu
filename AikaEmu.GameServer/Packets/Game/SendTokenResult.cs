using AikaEmu.GameServer.Network;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Packets.Game
{
    public class SendTokenResult : GamePacket
    {
        private readonly int _result;

        public SendTokenResult(int result)
        {
            _result = result;

            Opcode = (ushort) GameOpcode.SendTokenResult;
        }

        public override PacketStream Write(PacketStream stream)
        {
            stream.Write(_result);
            return stream;
        }
    }
}