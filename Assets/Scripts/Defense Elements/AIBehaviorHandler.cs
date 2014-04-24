using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIBehaviorHandler : MonoBehaviour {

	public AIBehaviorSettings settings;
	public CharacterSettings characterSettings;
	public UIProgressBar healthBar;
	public bool debugMode = false;
	public bool ignoreCost = false;
	public bool specialCombination = false;
	public CharacterCard[] myDeck = new CharacterCard[6];
	public int currentArea = -1;
	public int currentLevel = -1;

	private DefenseHandler defObj;
	private CharacterProperties.Team team = CharacterProperties.Team.RIGHT;
	private int[] damageApplyDeck = new int[6]{0,0,0,0,0,0};
	private bool started = false;
	private int areaNum = -1;
	private int stageNum = -1;

	private float timestamp = 0f;
	private int waveCounter = 0;
	private float[] waveIntervalCounter;
	private float[] waveIntervalRandom;
	private int[] waveIntervalTracker;

	private void Awake()
	{
		if(debugMode)
		{
			GlobalManager.GameSettings.chosenArea = areaNum = currentArea - 1;
			GlobalManager.GameSettings.chosenStage = stageNum = currentLevel - 1;
			for(int i=0; i<myDeck.Length; i++)
			{
				GlobalManager.UICard.localUserCardDeck.Add(myDeck[i]);
			}
			GlobalManager.ignoreCost = ignoreCost;
			GlobalManager.specialCombination = specialCombination;
		}
	}

	// Use this for initialization
	private void Start () {
		if(!debugMode)
		{
			areaNum = GlobalManager.GameSettings.chosenArea - 1;
			stageNum = GlobalManager.GameSettings.chosenStage - 1;
		}

		defObj = this.gameObject.GetComponent<DefenseHandler>();
		waveIntervalCounter = new float[settings.area[areaNum].subStage[stageNum].stageSettings.Length];
		waveIntervalRandom = new float[settings.area[areaNum].subStage[stageNum].stageSettings.Length];
		waveIntervalTracker = new int[settings.area[areaNum].subStage[stageNum].stageSettings.Length];

		for(int j = 0; j<settings.area[areaNum].subStage[stageNum].stageSettings.Length; j++)
		{
			settings.area[areaNum].subStage[stageNum].stageSettings[j].spawnCounter = 0;
		}
		
		for(int i=0; i<waveIntervalRandom.Length; i++)
		{
			waveIntervalRandom[i] = Random.Range(settings.area[areaNum].subStage[stageNum].stageSettings[i].interval.x, settings.area[areaNum].subStage[stageNum].stageSettings[i].interval.y);
		}
	}

	private void Update()
	{
		if(GlobalManager.initCheckDone && !started)
		{
			started = true;
			//Invoke("SpawnUnit", 0.5f);
		}

		if(GlobalManager.initCheckDone && !GlobalManager.gameover)
		{
			timestamp += Time.deltaTime;

			for(int i=0; i<waveIntervalCounter.Length; i++)
			{
				waveIntervalCounter[i] += Time.deltaTime;
				//Debug.Log(waveIntervalCounter[i] + " : " + waveIntervalRandom[i] + " : " + waveIntervalTracker[i]);
				if(waveIntervalCounter[i] >= waveIntervalRandom[i] && waveIntervalTracker[i] == 0)
				{
					waveIntervalTracker[i] = 1;
					SpawnUnit(i);
					ActivateDelay(Random.Range(0.3f, 1f));
				}
			}
		}
	}

	private void SpawnUnit(int index)
	{
		bool tempActiveStat = settings.area[areaNum].subStage[stageNum].stageSettings[index].active;
		AISettings.WaveType tempWaveType = settings.area[areaNum].subStage[stageNum].stageSettings[index].waveType;
		int tempSpawnCounter = settings.area[areaNum].subStage[stageNum].stageSettings[index].spawnCounter;
		int tempMaxSpawnAmt = tempWaveType == AISettings.WaveType.NORMAL ? settings.area[areaNum].subStage[stageNum].stageSettings[index].maximumSpawnAmount : 1;

		if(tempActiveStat)
		{
			if(CanSpawnCharacter(tempMaxSpawnAmt, tempSpawnCounter))
			{
				if(settings.area[areaNum].subStage[stageNum].stageSettings[index].waveConditions.Length > 0)
				{
					if(ConditionChecker(settings.area[areaNum].subStage[stageNum].stageSettings[index].waveConditions[0].condition, settings.area[areaNum].subStage[stageNum].stageSettings[index].waveConditions[0].comparison, settings.area[areaNum].subStage[stageNum].stageSettings[index].waveConditions[0].amount))
					{
						settings.area[areaNum].subStage[stageNum].stageSettings[index].spawnCounter ++;
						
						for(int j = 0; j<settings.area[areaNum].subStage[stageNum].stageSettings[index].unitSettings.Length; j++)
						{
							int tempAmount = settings.area[areaNum].subStage[stageNum].stageSettings[index].unitSettings[j].amount;
							StartCoroutine(DelayCharacterSpawn(tempAmount, index, j));
						}
					}
				}
				else
				{
					settings.area[areaNum].subStage[stageNum].stageSettings[index].spawnCounter ++;

					for(int j = 0; j<settings.area[areaNum].subStage[stageNum].stageSettings[index].unitSettings.Length; j++)
					{
						int tempAmount = settings.area[areaNum].subStage[stageNum].stageSettings[index].unitSettings[j].amount;
						StartCoroutine(DelayCharacterSpawn(tempAmount, index, j));
					}
				}
			}
		}

		waveCounter ++;
		waveIntervalRandom[index] = Random.Range(settings.area[areaNum].subStage[stageNum].stageSettings[index].interval.x, settings.area[areaNum].subStage[stageNum].stageSettings[index].interval.y);
		waveIntervalTracker[index] = 0;
		waveIntervalCounter[index] = 0f;
		//Invoke("SpawnUnit", Random.Range(settings.interval.x, settings.interval.y));
	}

	private IEnumerator DelayCharacterSpawn(int amt, int i, int j)
	{
		for(int k = 0; k<amt; k++)
		{
			int tempCardNum = settings.area[areaNum].subStage[stageNum].stageSettings[i].unitSettings[j].unitCardNum;
			int tempCardLevel = settings.area[areaNum].subStage[stageNum].stageSettings[i].unitSettings[j].unitLevel;
			CharacterProperties.UnitType tempUnitType;
			team = settings.area[areaNum].subStage[stageNum].stageSettings[i].unitSettings[j].unitTeam;

			if(tempCardNum > 0)
			{
				tempUnitType = GetUnitTypeByCardNumber(tempCardNum);
			}
			else
			{
				CharacterProperties.UnitType tempUnitType2 = settings.area[areaNum].subStage[stageNum].stageSettings[i].unitSettings[j].unitType;
				int tempRarity = settings.area[areaNum].subStage[stageNum].stageSettings[i].unitSettings[j].rarity;
				CharacterProperties prop = GetUnitByClassAndRarity(tempUnitType2, tempRarity);

				tempCardNum = characterSettings.characterProperties.IndexOf(prop) + 1;
				tempUnitType = prop.unitType;
			}

			defObj.SpawnCharacter(damageApplyDeck, tempCardNum, tempCardLevel, tempUnitType, team);

			yield return new WaitForSeconds(Random.Range(0.3f, 1f));
		}
	}

	private bool CanSpawnCharacter(int maxAmount, int currentAmount)
	{
		if(maxAmount == -1)
		{
			return true;
		}

		if(currentAmount < maxAmount)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private bool ConditionChecker(AISettings.ConditionType condition, AISettings.ComparisonType comparison, float amount)
	{
		switch(condition)
		{
		case AISettings.ConditionType.MAIN_HIT_POINT:
			if(comparison == AISettings.ComparisonType.EQUAL)
			{
				if((healthBar.value * 100) == amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.LESS_THAN)
			{
				if((healthBar.value * 100) < amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.MORE_THAN)
			{
				if((healthBar.value * 100) >= amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.LESS_OR_EQUAL)
			{
				if((healthBar.value * 100) <= amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.MORE_OR_EQUAL)
			{
				if((healthBar.value * 100) >= amount)
				{
					return true;
				}
			}
			break;

		case AISettings.ConditionType.GAME_TIME:
			if(comparison == AISettings.ComparisonType.EQUAL)
			{
				if(timestamp == amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.LESS_THAN)
			{
				if(timestamp < amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.MORE_THAN)
			{
				if(timestamp > amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.LESS_OR_EQUAL)
			{
				if(timestamp <= amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.MORE_OR_EQUAL)
			{
				if(timestamp >= amount)
				{
					return true;
				}
			}
			break;
			
		case AISettings.ConditionType.WAVE_AMOUNT:
			if(comparison == AISettings.ComparisonType.EQUAL)
			{
				if(waveCounter == amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.LESS_THAN)
			{
				if(waveCounter < amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.MORE_THAN)
			{
				if(waveCounter > amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.LESS_OR_EQUAL)
			{
				if(waveCounter <= amount)
				{
					return true;
				}
			}
			else if(comparison == AISettings.ComparisonType.MORE_OR_EQUAL)
			{
				if(waveCounter >= amount)
				{
					return true;
				}
			}
			break;
		default:
			return false;
			break;
		}

		return false;
	}

	private CharacterProperties.UnitType GetUnitTypeByCardNumber(int cardNum)
	{
		return characterSettings.characterProperties[cardNum-1].unitType;
	}

	private CharacterProperties GetUnitByClassAndRarity(CharacterProperties.UnitType unittype, int rarity)
	{
		List<CharacterProperties> tempHolder = new List<CharacterProperties>();

		for(int i = 0; i < characterSettings.characterProperties.Count; i++)
		{
			if(characterSettings.characterProperties[i].unitType == unittype && characterSettings.characterProperties[i].rarity == rarity)
			{
				tempHolder.Add(characterSettings.characterProperties[i]);
			}
		}

		int chosenOne = (int)Random.Range(0, tempHolder.Count);
		return tempHolder[chosenOne];
	}

	private IEnumerator ActivateDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
	}
}
