using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenseHandler : MonoBehaviour {

	public CharacterSettings characterSettingsObj;
	public AIBehaviorSettings stageSettingsObj;
	public SkillSettings skillSettingsObj;
	public GameObject indicatorPrefab;
	public GameObject damageEffectPrefab;
	public GameObject characterPrefab;
	public GameObject riderPrefab;
	public GameObject buildingPrefab;
	public GameObject spawnPointLeft;
	public GameObject spawnPointRight;
	public GameObject[] skillEffects;

	private GameObject defenseParent;
	private List<Character> enemies;
	private int counter = 1;

	private void Awake()
	{
		defenseParent = GameObject.Find("_DefensePart");

		//if(defenseParent != null) defenseParent.GetComponent<UIScrollView>().SetDragAmount(0,0,false);
		if(spawnPointLeft == null || spawnPointRight == null) Debug.LogError("Spawn point(s) not assigned. Please assign spawn point(s) before running the game.");
	}

	// Use this for initialization
	void Start () {
		Physics2D.IgnoreLayerCollision(14, 14, true);

		float widthOffset = 30f;
		float heightOffset = 130f;
		string blueMain = "castle";
		string redMain = "castle";

		int areaNum = GlobalManager.GameSettings.chosenArea;
		int stageNum = GlobalManager.GameSettings.chosenStage;
		int tempPlayerCastleHP = stageSettingsObj.area[areaNum].subStage[stageNum].playerCastleHitPoint;
		int tempEnemyCastleHP = stageSettingsObj.area[areaNum].subStage[stageNum].enemyCastleHitPoint;

		// spawn blue castle
		Vector3 startPos = new Vector3(spawnPointLeft.transform.localPosition.x, heightOffset, 0);
		Vector3 startScale = new Vector3(1, 1, 1);
		CreateCustomBuilding(GlobalManager.Player.One, "Player1 Building", blueMain, 1, "main", startPos, startScale, tempPlayerCastleHP, Vector3.zero, Vector3.zero);
		
		// spawn red castle
		Vector3 startPos2 = new Vector3(spawnPointRight.transform.localPosition.x, heightOffset, 0);
		Vector3 startScale2 = new Vector3(-1, 1, 1);
		CreateCustomBuilding(GlobalManager.Player.Two, "Player2 Building", redMain, 1, "main", startPos2, startScale2, tempEnemyCastleHP, Vector3.zero, Vector3.zero);

		if(GlobalManager.multiplyerGame)
		{
			this.GetComponent<AIBehaviorHandler>().enabled = false;
		}
		else
		{
			this.GetComponent<AIBehaviorHandler>().enabled = true;
		}

		defenseParent.GetComponent<UIScrollView>().ResetPosition();
		//Vector3 newPos = dragPanel.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position); // center on object
		//SpringPanel.Begin(defenseParent, Vector3.zero, 8f);
	}
	
	// Update is called once per frame
	void Update () {
		if(!GlobalManager.gameover)
		{
			for(int i=0; i< Character.characterLeft.Count; i++)
			{
				if(Character.characterLeft[i] != null && Character.characterLeft[i].EnemyObject == null && Character.characterLeft[i].UnitType != CharacterProperties.UnitType.BUILDING && Character.characterLeft[i].ExternalStatus == CharacterProperties.ExternalStatus.NONE && Character.characterLeft[i].MovementState != CharacterProperties.CharacterState.DEAD)
				{
					Character nextEnemy = GetNextEnemy(Character.characterLeft[i].Team, Character.characterLeft[i].transform.localPosition.x, Character.characterLeft[i].AttackType, Character.characterLeft[i].AttackRange, Character.characterLeft[i].UnitType);
					if(Character.characterLeft[i].UnitType == CharacterProperties.UnitType.HEALER)
					{
						if(nextEnemy != null)
						{
							// if we got somebody in front of us
							if(nextEnemy.CurrentHitPoint < nextEnemy.HitPoint)
							{
								// if somebody in front of us is not full health -- nextEnemy = somebodyInFront
								Character.characterLeft[i].EnemyObject = nextEnemy;
							}
							else
							{
								// if somebody in front of us is full health -- set null
								Character.characterLeft[i].EnemyObject = null;
							}
						}
						else
						{
							// if there's nobody in front of us -- nextEnemy = null
							Character.characterLeft[i].EnemyObject = nextEnemy;
						}
					}
					else
					{
						Character.characterLeft[i].EnemyObject = nextEnemy;
					}
				}
			}

			for(int j=0; j< Character.characterRight.Count; j++)
			{
				if(Character.characterRight[j] != null && Character.characterRight[j].EnemyObject == null && Character.characterRight[j].UnitType != CharacterProperties.UnitType.BUILDING && Character.characterRight[j].ExternalStatus == CharacterProperties.ExternalStatus.NONE && Character.characterRight[j].MovementState != CharacterProperties.CharacterState.DEAD)
				{
					Character nextEnemy = GetNextEnemy(Character.characterRight[j].Team, Character.characterRight[j].transform.localPosition.x, Character.characterRight[j].AttackType, Character.characterRight[j].AttackRange, Character.characterRight[j].UnitType);
					
					if(Character.characterRight[j].UnitType == CharacterProperties.UnitType.HEALER)
					{
						if(nextEnemy != null)
						{
							if(nextEnemy.CurrentHitPoint < nextEnemy.HitPoint)
							{
								Character.characterRight[j].EnemyObject = nextEnemy;
							}
							else
							{
								Character.characterLeft[j].EnemyObject = null;
							}
						}
						else
						{
							Character.characterRight[j].EnemyObject = nextEnemy;
						}
					}
					else
					{
						Character.characterRight[j].EnemyObject = nextEnemy;
					}
				}
			}
		}
	}

	public Character GetNextEnemy(CharacterProperties.Team tempPlayer, float currentPosition, CharacterProperties.AttackType tempAttackType, float tempAttackRange, CharacterProperties.UnitType tempUnitType){
		if(tempUnitType == CharacterProperties.UnitType.HEALER)
		{
			enemies = tempPlayer == CharacterProperties.Team.LEFT ? Character.characterLeft : Character.characterRight;
		}
		else
		{
			enemies = tempPlayer == CharacterProperties.Team.LEFT ? Character.characterRight : Character.characterLeft;
		}
		
		for(int i=enemies.Count-1; i>=0; i--){
			if(enemies[i].MovementState != CharacterProperties.CharacterState.DEAD)
			{
				float distance = tempPlayer == CharacterProperties.Team.LEFT ? enemies[i].transform.localPosition.x - currentPosition : currentPosition - enemies[i].transform.localPosition.x;
				//Debug.Log(distance);
				if(distance <= tempAttackRange && distance > 0){
					//enemyTarget = enemies[i];
					//status = "attacking";
					
					bool willAttack = false;
					
					Character charObj = enemies[i].gameObject.GetComponent<Character>();
					if(charObj){
						if(tempAttackType == CharacterProperties.AttackType.AIR && charObj.PositionType == CharacterProperties.PositionType.AIR){
							willAttack = true;
						}else if(tempAttackType == CharacterProperties.AttackType.GROUND && charObj.PositionType == CharacterProperties.PositionType.GROUND){
							willAttack = true;
						}else if(tempAttackType == CharacterProperties.AttackType.GROUND_AND_AIR && (charObj.PositionType == CharacterProperties.PositionType.GROUND || charObj.PositionType == CharacterProperties.PositionType.AIR)){
							willAttack = true;
						}
					}else{
						willAttack = true;
					}
					
					if(willAttack){
						return enemies[i];
						break;
					}
				}
			}
		}
		
		return null;
	}

	public void SetBuildingHitPoint(GlobalManager.Player tempOwner, float tempValue)
	{
		if(tempOwner == GlobalManager.Player.One){
			//blueHealthBar.GetComponent<tk2dUIProgressBar>().Value = tempValue;
		}else if(tempOwner == GlobalManager.Player.Two){
			//redHealthBar.GetComponent<tk2dUIProgressBar>().Value = tempValue;
		}
	}

	public void SpawnCharacter(int[] damageDeck, CharacterProperties.SkillType skill, CharacterProperties.UnitType unit, CharacterProperties.CategoryColour colour, int characterLevel, int rankLevel, CharacterProperties.Team tempOwner = CharacterProperties.Team.LEFT)
	{
		Vector3 spawnPosition = tempOwner == CharacterProperties.Team.LEFT ? spawnPointLeft.transform.position : spawnPointRight.transform.position;

		if(unit != CharacterProperties.UnitType.DRAGON)
		{
			GameObject go = Instantiate(characterPrefab, spawnPosition, Quaternion.identity) as GameObject;
			go.transform.parent = defenseParent.transform;
			Debug.Log(unit);
			switch(unit)
			{
			case CharacterProperties.UnitType.ARCHER:
			case CharacterProperties.UnitType.CANNON:
				Archer rangeObj = go.AddComponent<Archer>();
				rangeObj.characterSettings = characterSettingsObj;
				rangeObj.skillSettings = skillSettingsObj;
				rangeObj.indicatorPrefab = indicatorPrefab;
				rangeObj.damageEffectPrefab = damageEffectPrefab;
				rangeObj.bonusDamageDeck = damageDeck;
				rangeObj.unitLevel = characterLevel;
				rangeObj.rank = rankLevel;
				rangeObj.Category = colour;
				rangeObj.SkillType = skill;
				rangeObj.UnitType = unit;
				rangeObj.Team = tempOwner;
				rangeObj.effectSettings = skillEffects;
				break;

			case CharacterProperties.UnitType.ONE_HANDED_WARRIOR:
			case CharacterProperties.UnitType.TWO_HANDED_WARRIOR:
			case CharacterProperties.UnitType.SPEAR_WARRIOR:
				Warrior meleeObj = go.AddComponent<Warrior>();
				meleeObj.characterSettings = characterSettingsObj;
				meleeObj.skillSettings = skillSettingsObj;
				meleeObj.indicatorPrefab = indicatorPrefab;
				meleeObj.damageEffectPrefab = damageEffectPrefab;
				meleeObj.bonusDamageDeck = damageDeck;
				meleeObj.unitLevel = characterLevel;
				meleeObj.rank = rankLevel;
				meleeObj.Category = colour;
				meleeObj.SkillType = skill;
				meleeObj.UnitType = unit;
				meleeObj.Team = tempOwner;
				meleeObj.effectSettings = skillEffects;
				break;
			case CharacterProperties.UnitType.HEALER:
				Healer healerObj = go.AddComponent<Healer>();
				healerObj.characterSettings = characterSettingsObj;
				healerObj.skillSettings = skillSettingsObj;
				healerObj.indicatorPrefab = indicatorPrefab;
				healerObj.damageEffectPrefab = damageEffectPrefab;
				healerObj.bonusDamageDeck = damageDeck;
				healerObj.unitLevel = characterLevel;
				healerObj.rank = rankLevel;
				healerObj.Category = colour;
				healerObj.SkillType = skill;
				healerObj.UnitType = unit;
				healerObj.Team = tempOwner;
				healerObj.effectSettings = skillEffects;
				break;
			}
		}
		else if(unit == CharacterProperties.UnitType.DRAGON)
		{
			GameObject go = Instantiate(riderPrefab, spawnPosition, Quaternion.identity) as GameObject;
			go.transform.parent = defenseParent.transform;
			go.GetComponent<DragonRider>().characterSettings = characterSettingsObj;
			go.GetComponent<DragonRider>().skillSettings = skillSettingsObj;
			go.GetComponent<DragonRider>().indicatorPrefab = indicatorPrefab;
			go.GetComponent<DragonRider>().damageEffectPrefab = damageEffectPrefab;
			go.GetComponent<DragonRider>().bonusDamageDeck = damageDeck;
			go.GetComponent<DragonRider>().unitLevel = characterLevel;
			go.GetComponent<DragonRider>().rank = rankLevel;
			go.GetComponent<DragonRider>().Category = colour;
			go.GetComponent<DragonRider>().SkillType = skill;
			go.GetComponent<DragonRider>().Team = tempOwner;
			go.GetComponent<DragonRider>().effectSettings = skillEffects;
		}
	}

	public void SpawnCharacter(int[] damageDeck, int cardNum, int cardLevel, CharacterProperties.UnitType unit, CharacterProperties.Team tempOwner = CharacterProperties.Team.LEFT)
	{
		Vector3 spawnPosition = tempOwner == CharacterProperties.Team.LEFT ? spawnPointLeft.transform.position : spawnPointRight.transform.position;
		CharacterProperties.CategoryColour colour = characterSettingsObj.characterProperties[cardNum].category;
		CharacterProperties.SkillType skill = characterSettingsObj.characterProperties[cardNum].skillType;
		int rankLevel = cardNum;

		if(unit != CharacterProperties.UnitType.DRAGON)
		{
			GameObject go = Instantiate(characterPrefab, spawnPosition, Quaternion.identity) as GameObject;
			go.transform.parent = defenseParent.transform;
			go.transform.localScale = Vector3.one * 1000;
			
			switch(unit)
			{
			case CharacterProperties.UnitType.ARCHER:
			case CharacterProperties.UnitType.CANNON:
				Archer rangeObj = go.AddComponent<Archer>();
				rangeObj.characterSettings = characterSettingsObj;
				rangeObj.skillSettings = skillSettingsObj;
				rangeObj.indicatorPrefab = indicatorPrefab;
				rangeObj.damageEffectPrefab = damageEffectPrefab;
				rangeObj.bonusDamageDeck = damageDeck;
				rangeObj.unitLevel = cardLevel;
				rangeObj.rank = rankLevel;
				rangeObj.Category = colour;
				rangeObj.SkillType = skill;
				rangeObj.UnitType = unit;
				rangeObj.Team = tempOwner;
				rangeObj.effectSettings = skillEffects;
				break;
				
			case CharacterProperties.UnitType.ONE_HANDED_WARRIOR:
			case CharacterProperties.UnitType.TWO_HANDED_WARRIOR:
			case CharacterProperties.UnitType.SPEAR_WARRIOR:
				Warrior meleeObj = go.AddComponent<Warrior>();
				meleeObj.characterSettings = characterSettingsObj;
				meleeObj.skillSettings = skillSettingsObj;
				meleeObj.indicatorPrefab = indicatorPrefab;
				meleeObj.damageEffectPrefab = damageEffectPrefab;
				meleeObj.bonusDamageDeck = damageDeck;
				meleeObj.unitLevel = cardLevel;
				meleeObj.rank = rankLevel;
				meleeObj.Category = colour;
				meleeObj.SkillType = skill;
				meleeObj.UnitType = unit;
				meleeObj.Team = tempOwner;
				meleeObj.effectSettings = skillEffects;
				break;
			case CharacterProperties.UnitType.HEALER:
				Healer healerObj = go.AddComponent<Healer>();
				healerObj.characterSettings = characterSettingsObj;
				healerObj.skillSettings = skillSettingsObj;
				healerObj.indicatorPrefab = indicatorPrefab;
				healerObj.damageEffectPrefab = damageEffectPrefab;
				healerObj.bonusDamageDeck = damageDeck;
				healerObj.unitLevel = cardLevel;
				healerObj.rank = rankLevel;
				healerObj.Category = colour;
				healerObj.SkillType = skill;
				healerObj.UnitType = unit;
				healerObj.Team = tempOwner;
				healerObj.effectSettings = skillEffects;
				break;
			}
		}
		else if(unit == CharacterProperties.UnitType.DRAGON)
		{
			GameObject go = Instantiate(riderPrefab, spawnPosition, Quaternion.identity) as GameObject;
			go.transform.parent = defenseParent.transform;
			go.GetComponent<DragonRider>().characterSettings = characterSettingsObj;
			go.GetComponent<DragonRider>().skillSettings = skillSettingsObj;
			go.GetComponent<DragonRider>().indicatorPrefab = indicatorPrefab;
			go.GetComponent<DragonRider>().damageEffectPrefab = damageEffectPrefab;
			go.GetComponent<DragonRider>().bonusDamageDeck = damageDeck;
			go.GetComponent<DragonRider>().unitLevel = cardLevel;
			go.GetComponent<DragonRider>().rank = rankLevel;
			go.GetComponent<DragonRider>().Category = colour;
			go.GetComponent<DragonRider>().SkillType = skill;
			go.GetComponent<DragonRider>().Team = tempOwner;
			go.GetComponent<DragonRider>().effectSettings = skillEffects;
		}
	}

	public void CreateCustomBuilding(GlobalManager.Player tempOwner, string tempName, string tempSpriteName, int tempLevel, string tempType, Vector3 tempPosition, Vector3 tempScale, int tempHitPoint, Vector3 tempColliderSize, Vector3 tempColliderPos){
		GameObject unit = Instantiate(buildingPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		unit.transform.parent = defenseParent.transform;
		unit.transform.localScale = tempScale;
		unit.transform.localPosition = tempPosition;
		unit.name = tempName;
		
		UISprite spriteObj = unit.GetComponent<UISprite>();
		//spriteObj.SetSprite(tempSpriteName);
		spriteObj.spriteName = tempSpriteName;
		//spriteObj.SortingOrder = 420;
		
		Building buildingObj = unit.GetComponent<Building>();
		//buildingObj.DefenseScreenSettings(0, 0, 100, 1050);
		//buildingObj.Parent = tempOwner;
		buildingObj.HitPoint = tempHitPoint;
		buildingObj.Type = tempType;
		//buildingObj.Level = tempLevel;
		buildingObj.enabled = true;

		buildingObj.level = 1;
		buildingObj.Team = tempOwner == GlobalManager.Player.One ? CharacterProperties.Team.LEFT : CharacterProperties.Team.RIGHT;
		
		//CharacterManager.Instance.AddCharacter(unit, tempOwner);
	}

	public void SpawnSkill(int type, CharacterProperties.Team playerNumber)
	{

	}
}
