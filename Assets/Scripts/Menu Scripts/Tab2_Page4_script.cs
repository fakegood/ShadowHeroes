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
	public UILabel unitDescriptionLabel;
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
		expLabel.text = "100/1000";
		expProgressBar.value = 100f/1000f;
		unitNameLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].characterName;
		unitDescriptionLabel.text = "None";
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
