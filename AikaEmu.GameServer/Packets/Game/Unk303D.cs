using AikaEmu.GameServer.Models;
using AikaEmu.GameServer.Network;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.Shared.Network;

namespace AikaEmu.GameServer.Packets.Game
{
	public class Unk303D : GamePacket
	{
		public Unk303D(Character character)
		{
			Opcode = (ushort) GameOpcode.Unk303D;
			SenderId = character.ConnectionId;
		}

		public override PacketStream Write(PacketStream stream)
		{
			stream.Write(0);

			for (var i = 0; i < 12; i++)
			{
				stream.Write((ushort) 0);
			}

			for (var i = 0; i < 12; i++)
			{
				stream.Write((byte) 0);
			}

			return stream;
		}
	}
}