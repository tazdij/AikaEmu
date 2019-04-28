namespace AikaEmu.GameServer.Network
{
	public enum GameOpcode : ushort
	{
		DespawnUnit = 0x1001,
		UpdateHpMp = 0x1003,
		UpdateStoreItem = 0x1006,
		UpdateExperience = 0x1008,
		UpdateAttributes = 0x1009,
		UpdateStatus = 0x100A,
		SetEffectOnHead = 0x100D,
		OpenNpcChat = 0x100E,
		CloseNpcChat = 0x100F,
		ResetChatOptions = 0x1010,
		SendNpcOption = 0x1012,
		PlaySound = 0x1015,
		UpdatePranExperience = 0x1016,
		PranEffect = 0x1017,
		SendXpGoldAnimation = 0x101B,
		UpdateEquipDur = 0x102B,
		CurNationInfo = 0x1028,
		UpdateNationGuild = 0x1030,
		UpdateReliques = 0x1036,
		UpdatePremiumStash = 0x1038,
		UpdateCash = 0x1039,
		UpdateAccountLevel = 0x104F,
		SendXpMessage = 0x1053,
		ApplyBuff = 0x106F,
		UpdateBuffs = 0x1070,
		UpdateTitleList = 0x107D,
		SendCharacterList = 0x1801,
		SendPranToWorld = 0x1807,
		UpdateSiegeInfo = 0x181A,
		SendToWorld = 0x1825,
		UpdateNationGovernment = 0x1836,
		UpdateDungeonTimer = 0x184C,
		SendMessage = 0x1884,
		RenamePranResult = 0x2F02,
		OpenNpcShop = 0x3010,
		UpdateCharGold = 0x3012,
		SendQuestInfo = 0x3031,
		SendUnitSpawn = 0x3049,
		SendMobSpawn = 0x305E,
		SendTokenResult = 0x309D,
		UpdatePosition = 0x30BF,
		UpdateItem = 0x3C0E,
		UpdateMapInfo = 0x3C79,
		SendChatMessage = 0x3C86,
		ResponseDeleteCharToken = 0x3F33,
		UpdatePuzzleEvent = 0x3F34,

		Unk101F = 0x101F,
		Unk1027 = 0x1027,
		Unk102C = 0x102C,
		Unk1031 = 0x1031,
		Unk1054 = 0x1054,
		Unk107E = 0x107E, // Seems to be related to quest completion
		UnkTitleLink2 = 0x1086,
		Unk1C41 = 0x1C41,
		InitialUnk202 = 0x2027,
		Unk303D = 0x303D,
		Unk3057 = 0x3057,
		Unk30A2 = 0x30A2,
		Unk30A5 = 0x30A5,
		UnkTitleLink = 0x30A6,
		Unk3C7C = 0x3C7C,
		FinishedInGameState = 0x3CBE,
		Unk3F1B = 0x3F1B,
		Unk3F34 = 0x3F34,
		Unk4756 = 0x4756,
	}

	public enum ClientOpcode : ushort
	{
		RequestUpdateCash = 0x200A,
		RequestDeleteChar = 0x2403,
		ReturnCharacterSelect = 0x2468,
		RequestEnterGame = 0x24C1,
		RenamePran = 0x2F02,
		CreateCharacter = 0x2F04,
		RotationUnit = 0x3005,
		UnkClientRequest = 0x3006,
		RequestNpcChat = 0x300F,
		RequestUseItem = 0x301D,
		RequestTokenResult = 0x309D,
		MoveUnit = 0x30BF,
		SetActiveTitle = 0x3061,
		MoveItem = 0x340F,
		RequestToken = 0x3C02,
		RequestMapInfo = 0x3C79,
		SendChatMessage = 0x3C86,
		InGameState = 0x3CBE,
		RequestDeleteCharToken = 0x3F33,
		PuzzleEventRequest = 0x3F34,
	}
}