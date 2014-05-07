using UnityEngine;
using System.Collections;

public class Tab2_Page4_script : SubPageHandler {

	public GameObject mainHolder;
	public UISprite categoryIcon;
	public UISprite unitIcon;
	public UILabel categoryLabel;
	public UILabel unitLabel;
	public UILabel levelLabel;
	public UILabel costLabel;
	public UILabel hpLabel;
	public UILabel attackLabel;
	public UILabel expLabel;
	public UIProgressBar expProgressBar;
	public UILabel unitNameLabel;
	public UISprite rarityIcon;
	public UILabel unitSkillLabel;
	public UILabel unitDescriptionLabel;
	public GameObject characterPrefab;
	public GameObject dragonPrefab;
	[System.NonSerialized]
	public int inventoryIndex = -1;
	private int selectedIndex = -1;

	private CharacterProperties.CategoryColour category;
	private CharacterProperties.UnitType unitType;
	private CharacterCard cardObj = null;

	// Use this for initialization
	void Start () {
		if(csObj == null) return;
		cardObj = base.parent.currentSelectedCard;
		if(cardObj == null) return;

		UISprite backgroundImage = GameObject.FindGameObjectWithTag("MainBackground").GetComponent<UISprite>();
		Vector3 pos = Vector3.zero;
		pos.y = backgroundImage.transform.localPosition.y + (backgroundImage.localSize.y/2) - 89f;
		mainHolder.transform.localPosition = pos;

		selectedIndex = base.parent.currentSelectedDeckNum-1;

		FillInformation();
	}

	private void FillInformation()
	{
		UnitCategory = base.csObj.characterProperties[cardObj.cardNumber-1].category;
		UnitType = base.csObj.characterProperties[cardObj.cardNumber-1].unitType;
		levelLabel.text = "Lv." + cardObj.level.ToString();
		costLabel.text = cardObj.cost.ToString();
		hpLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].maxHitPoint.ToString();
		attackLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].damage.ToString();
		expLabel.text = cardObj.experience.ToString() + "/" + GlobalManager.LocalUser.ComputeCardNeedLevelupExp(cardObj.level).ToString();
		expProgressBar.value = (float)cardObj.experience / (float)GlobalManager.LocalUser.ComputeCardNeedLevelupExp(cardObj.level);
		unitNameLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].characterName.ToString();
		rarityIcon.width = base.csObj.characterProperties[cardObj.cardNumber-1].rarity * 32;
		unitSkillLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].skillType.ToString().Replace("_", " ");
		unitDescriptionLabel.text = "None";

		SpawnCharacter(cardObj.cardNumber, UnitType);
	}

	public void SpawnCharacter(int cardNum, CharacterProperties.UnitType unit)
	{
		CharacterProperties.Team tempOwner = CharacterProperties.Team.LEFT;
		CharacterProperties.CategoryColour colour = GlobalManager.GameSettings.csObj.characterProperties[cardNum].category;
		int rankLevel = cardNum;
		
		if(unit != CharacterProperties.UnitType.DRAGON)
		{
			GameObject go = Instantiate(characterPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			go.transform.parent = this.transform.Find("Holder/Character Unit Holder").transform;
			go.transform.localScale = Vector3.one * 10;
			Vector3 pos = Vector3.zero;
			pos.y = 36f;
			go.transform.localPosition = pos;
			go.rigidbody2D.isKinematic = true;
			go.collider2D.enabled = false;
			
			switch(unit)
			{
			case CharacterProperties.UnitType.ARCHER:
			case CharacterProperties.UnitType.CANNON:
				Archer rangeObj = go.AddComponent<Archer>();
				rangeObj.characterSettings = GlobalManager.GameSettings.csObj;
				rangeObj.rank = rankLevel;
				rangeObj.Category = colour;
				rangeObj.UnitType = unit;
				rangeObj.Team = tempOwner;
				rangeObj.DisplayUnit = true;
				break;
			case CharacterProperties.UnitType.ONE_HANDED_WARRIOR:
			case CharacterProperties.UnitType.TWO_HANDED_WARRIOR:
			case CharacterProperties.UnitType.SPEAR_WARRIOR:
				Warrior meleeObj = go.AddComponent<Warrior>();
				meleeObj.characterSettings = GlobalManager.GameSettings.csObj;
				meleeObj.rank = rankLevel;
				meleeObj.Category = colour;
				meleeObj.UnitType = unit;
				meleeObj.Team = tempOwner;
				meleeObj.DisplayUnit = true;
				break;
			case CharacterProperties.UnitType.HEALER:
				Healer healerObj = go.AddComponent<Healer>();
				healerObj.characterSettings = GlobalManager.GameSettings.csObj;
				healerObj.rank = rankLevel;
				healerObj.Category = colour;
				healerObj.UnitType = unit;
				healerObj.Team = tempOwner;
				healerObj.DisplayUnit = true;
				break;
			}
		}
		else if(unit == CharacterProperties.UnitType.DRAGON)
		{
			GameObject go = Instantiate(dragonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			go.transform.parent = this.transform.Find("Holder/Character Unit Holder").transform;
			go.transform.localScale = Vector3.one * 6;
			Vector3 pos = Vector3.zero;
			pos.y = 20f;
			go.transform.localPosition = pos;
			go.rigidbody2D.isKinematic = true;
			go.collider2D.enabled = false;

			go.GetComponent<DragonRider>().characterSettings = GlobalManager.GameSettings.csObj;
			go.GetComponent<DragonRider>().rank = rankLevel;
			go.GetComponent<DragonRider>().Category = colour;
			go.GetComponent<DragonRider>().Team = tempOwner;
			go.GetComponent<DragonRider>().DisplayUnit = true;
		}
	}

	public CharacterProperties.CategoryColour UnitCategory
	{
		private set{
			string categoryString = "";
			category = value;
			categoryIcon.spriteName = "type_"+((int)value-1);
			categoryLabel.text = value.ToString();
		}

		get{ return category; }
	}

	public CharacterProperties.UnitType UnitType
	{
		private set{
			unitType = value;
			unitIcon.spriteName = "job_"+(int)value;
			unitLabel.text = value.ToString().Replace("_", " ");
		}

		get{ return unitType; }
	}
}
