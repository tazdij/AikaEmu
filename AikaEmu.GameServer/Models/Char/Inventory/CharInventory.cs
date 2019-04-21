using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AikaEmu.GameServer.Managers.Configuration;
using AikaEmu.GameServer.Network.GameServer;
using AikaEmu.GameServer.Packets.Game;
using NLog;

namespace AikaEmu.GameServer.Models.Char.Inventory
{
	public class CharInventory
	{
		private readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly Character _character;
		private readonly Dictionary<SlotType, Item[]> _items;
		private readonly object _lockObject = new object();

		public CharInventory(Character character)
		{
			_character = character;

			_items = new Dictionary<SlotType, Item[]>();
			foreach (SlotType type in Enum.GetValues(typeof(SlotType)))
			{
				switch (type)
				{
					case SlotType.Equipments:
						_items.Add(type, new Item[15]);
						break;
					case SlotType.Inventory:
						_items.Add(type, new Item[85]);
						break;
					case SlotType.Bank:
						_items.Add(type, new Item[1]); // 84 & 85 (Pran Stone) (2 per account)
						break;
					case SlotType.PranEquipments:
						_items.Add(type, new Item[15]);
						break;
					case SlotType.PranInventory:
						_items.Add(type, new Item[41]);
						break;
					default:
						// TODO - Implement
						break;
				}
			}
		}

		public ConcurrentDictionary<ushort, Item> GetItemsBySlotType(SlotType slot)
		{
			var list = new ConcurrentDictionary<ushort, Item>();

			foreach (var item in _items[slot])
				if (item?.ItemId > 0)
					list.TryAdd(item.Slot, item);

			return list;
		}

		public Item GetItem(SlotType slotType, ushort slot)
		{
			foreach (var item in _items[slotType])
			{
				if (item?.Slot == slot) return item;
			}

			return null;
		}

		public bool SwapSameType(SlotType slotType, ushort slotFrom, ushort slotTo)
		{
			var item1 = GetItem(slotType, slotFrom);
			var item2 = GetItem(slotType, slotTo);

			// TODO - Check if can swap
			lock (_lockObject)
			{
				item1.Slot = slotTo;
				item2.Slot = slotFrom;
				_items[slotType][slotFrom] = item2;
				_items[slotType][slotTo] = item1;
			}

			_character.Connection.SendPacket(new UpdateItem(_items[slotType][slotTo], false));
			_character.Connection.SendPacket(new UpdateItem(_items[slotType][slotFrom], false));

			return true;
		}

		public void Init()
		{
			using (var sql = GameServer.Instance.DatabaseManager.GetConnection())
			using (var command = sql.CreateCommand())
			{
				command.CommandText = "SELECT * FROM items WHERE char_id=@char_id";
				command.Parameters.AddWithValue("@char_id", _character.Id);
				command.Prepare();
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var item = new Item
						{
							Id = reader.GetUInt32("id"),
							ItemId = reader.GetUInt16("item_id"),
							SlotType = (SlotType) reader.GetByte("slot_type"),
							Slot = reader.GetUInt16("slot"),
							Effect1 = reader.GetByte("effect1"),
							Effect2 = reader.GetByte("effect2"),
							Effect3 = reader.GetByte("effect3"),
							Effect1Value = reader.GetByte("effect1value"),
							Effect2Value = reader.GetByte("effect2value"),
							Effect3Value = reader.GetByte("effect3value"),
							Durability = reader.GetByte("dur"),
							DurMax = reader.GetByte("dur_max"),
							Quantity = reader.GetByte("refinement"),
							ItemTime = reader.GetUInt16("time")
						};

						// Check if item exists in json data
						if (!DataManager.Instance.ItemsData.Exists(item.ItemId)) continue;

						if (item.SlotType == SlotType.Equipments && item.Slot < 16 ||
						    item.SlotType == SlotType.Inventory && item.Slot < 86 ||
						    item.SlotType == SlotType.Bank && item.Slot < 2)
						{
							if (item.SlotType == SlotType.Bank)
								item.Slot = item.Slot == 0 ? (ushort) 84 : (ushort) 85;

							_items[item.SlotType][item.Slot] = item;
						}
					}
				}
			}

			foreach (var (key, value) in _items)
			{
				for (ushort i = 0; i < value.Length; i++)
				{
					if (_items[key][i] == null) _items[key][i] = new Item(key, i);
				}
			}
		}

		public bool Save()
		{
			using (var sql = GameServer.Instance.DatabaseManager.GetConnection())
			using (var transaction = sql.BeginTransaction())
			{
				foreach (var (key, items) in _items)
				{
					if (key == SlotType.PranInventory || key == SlotType.PranEquipments) continue;

					foreach (var item in items)
					{
						using (var command = sql.CreateCommand())
						{
							command.CommandText =
								"REPLACE INTO `items`" +
								"(`id`, `item_id`, `char_id`, `slot_type`, `slot`, `effect1`, `effect2`, `effect3`, `effect1value`, `effect2value`, `effect3value`, `dur`, `dur_max`, `refinement`, `time`)" +
								"VALUES (@id, @item_id, @char_id, @slot_type, @slot, @effect1, @effect2, @effect3, @effect1value, @effect2value, @effect3value, @dur, @dur_max, @refinement, @time);";

							command.Parameters.AddWithValue("@id", item.Id);
							command.Parameters.AddWithValue("@item_id", item.ItemId);
							command.Parameters.AddWithValue("@char_id", _character.Id);
							command.Parameters.AddWithValue("@slot_type", (byte) item.SlotType);
							command.Parameters.AddWithValue("@slot", item.Slot);
							command.Parameters.AddWithValue("@effect1", item.Effect1);
							command.Parameters.AddWithValue("@effect2", item.Effect2);
							command.Parameters.AddWithValue("@effect3", item.Effect3);
							command.Parameters.AddWithValue("@effect1value", item.Effect1Value);
							command.Parameters.AddWithValue("@effect2value", item.Effect2Value);
							command.Parameters.AddWithValue("@effect3value", item.Effect3Value);
							command.Parameters.AddWithValue("@dur", item.Durability);
							command.Parameters.AddWithValue("@dur_max", item.DurMax);
							command.Parameters.AddWithValue("@refinement", item.Quantity);
							command.Parameters.AddWithValue("@time", item.ItemTime);
							command.ExecuteNonQuery();
						}
					}
				}

				try
				{
					transaction.Commit();
					return true;
				}
				catch (Exception e)
				{
					_log.Error(e);
					try
					{
						transaction.Rollback();
					}
					catch (Exception exception)
					{
						_log.Error(exception);
					}

					return false;
				}
			}
		}
	}
}