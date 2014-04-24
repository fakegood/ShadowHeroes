using UnityEngine;
using System.Collections;

public class StageUISettings : MonoBehaviour {
	public Area[] area;
}

[System.Serializable]
public class Area
{
	public string areaName = "Dungeon 1";
	public bool locked = true;
	public UISprite icon;
	public SubStage[] subStage;
}

[System.Serializable]
public class SubStage
{
	public string subStageName = "Sub Stage";
	public bool locked = true;
	public int energyCost = 5;
	public UISprite icon;
}
