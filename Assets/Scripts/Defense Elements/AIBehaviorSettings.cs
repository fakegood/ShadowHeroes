using UnityEngine;
using System.Collections;

public class AIBehaviorSettings : MonoBehaviour {

	//public Vector2 interval;
	public AreaSettings[] area;
}

[System.Serializable]
public class AreaSettings
{
	public string areaName = "Dungeon 1";
	public bool locked = true;
	public UISprite icon;
	public AISettings[] subStage;
}

[System.Serializable]
public class AISettings
{
	public enum WaveType
	{
		NORMAL,
		BOSS
	}

	public enum ConditionType
	{
		MAIN_HIT_POINT,
		WAVE_AMOUNT,
		GAME_TIME
	}

	public enum ComparisonType
	{
		LESS_THAN,
		MORE_THAN,
		LESS_OR_EQUAL,
		MORE_OR_EQUAL,
		EQUAL
	}

	public string subStageName = "Sub Stage";
	public bool locked = true;
	public int energyCost = 5;
	public UISprite icon;
	public int playerCastleHitPoint = 1000;
	public int enemyCastleHitPoint = 1000;
	public StageSettings[] stageSettings;
}

[System.Serializable]
public class StageSettings
{
	public bool active = true;
	public AISettings.WaveType waveType;
	public Vector2 interval;
	public int maximumSpawnAmount = -1;
	public WaveConditions[] waveConditions;
	public UnitSettings[] unitSettings;
	[HideInInspector]
	public int spawnCounter = 0;
}

[System.Serializable]
public class UnitSettings
{
	public int unitCardNum = -1;
	public int unitLevel = 1;
	public CharacterProperties.UnitType unitType = CharacterProperties.UnitType.ONE_HANDED_WARRIOR;
	public int rarity = 1;
	public int amount;
	public CharacterProperties.Team unitTeam = CharacterProperties.Team.RIGHT;
}

[System.Serializable]
public class WaveConditions
{
	public AISettings.ConditionType condition;
	public AISettings.ComparisonType comparison;
	public float amount = 0f;
}