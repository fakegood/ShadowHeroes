using UnityEngine;
using System.Collections;

public class ButtonMainHandler : MonoBehaviour {

	public DefenseHandler defObj;
	public GameObject characterSettingsPrefab;
	public GameObject[] skillButtonArray;
	private int totalNumberOfButtons = 0;

	private int[] bonusDamageDeck = new int[6];

	void Start () {
		InitSkillButton();
		UpdateElementText();
	}

	private void InitSkillButton(){
		string buttonName = "";
		string spriteName = "";
		int limit = 8;
		int val = 0;
		int tempReferenceNumber = 0;
		int buttonNumber = -1;
		CharacterProperties.CategoryColour category = CharacterProperties.CategoryColour.RED;
		CharacterProperties.UnitType unitType = CharacterProperties.UnitType.ARCHER;
		int unitLevel = 0;
		CharacterProperties.SkillType skillType = CharacterProperties.SkillType.NONE;
		int skillLevel = 0;
		int settingsAmt = 0;
		//int offset = 0;
		float buttonXPos = 0;
		
		for(int i=0; i<skillButtonArray.Length; i++){
			val = i;
			if(GlobalManager.UICard.localUserCardDeck[i] != null)
			{
				int deckValue = settingsAmt = GlobalManager.UICard.localUserCardDeck[i].cardNumber - 1;
				
				UIEventListener.Get(skillButtonArray[i]).onClick += ButtonHandler;
				
				// character deck
				if(GlobalManager.UICard.localUserCardDeck[i].cardNumber > 0){
					buttonName = spriteName = characterSettingsPrefab.GetComponent<CharacterSettings>().characterProperties[deckValue].iconSpriteName;
					tempReferenceNumber = i + 1;
					category = characterSettingsPrefab.GetComponent<CharacterSettings>().characterProperties[deckValue].category;
					unitType = characterSettingsPrefab.GetComponent<CharacterSettings>().characterProperties[deckValue].unitType;
					unitLevel = 1;
					skillType = characterSettingsPrefab.GetComponent<CharacterSettings>().characterProperties[deckValue].skillType;
					skillLevel = (int)characterSettingsPrefab.GetComponent<CharacterSettings>().characterProperties[deckValue].skillLevel;
					settingsAmt = deckValue;
					buttonNumber = i;
					
					skillButtonArray[i].transform.Find("Background").GetComponent<UISprite>().spriteName = spriteName;
					skillButtonArray[i].GetComponent<UIButton>().normalSprite = spriteName;
					skillButtonArray[i].GetComponent<UIButton>().hoverSprite = spriteName;
					skillButtonArray[i].GetComponent<UIButton>().pressedSprite = spriteName;
					skillButtonArray[i].GetComponent<UIButton>().disabledSprite = spriteName;

					UnitButtonHandler obj = skillButtonArray[i].GetComponent<UnitButtonHandler>();
					obj.referenceNumber = tempReferenceNumber;
					obj.element = category;
					obj.unitType = unitType;
					obj.unitLevel = unitLevel;
					obj.skillType = skillType;
					obj.skillLevel = skillLevel;
					obj.buttonNumber = buttonNumber;
					obj.settingsReferer = settingsAmt;
				}
				else
				{
					//skillButtonArray[i].collider.enabled = false;
					skillButtonArray[i].GetComponent<UIButton>().isEnabled = false;
					skillButtonArray[i].GetComponent<UnitButtonHandler>().DisableSelf();
				} 
			}
			
			//buttonXPos = ((i+1f)*31f)+(i*52.5f);
			
			//skillButtonArray[i] = CreateButton(tempParent, new Vector3(buttonXPos, 515, -1), tempReferenceNumber, category, spriteName, unitType, unitLevel, skillType, skillLevel, settingsAmt, buttonName, buttonNumber);
			//skillButtonArray[i].collider.enabled = false;
		}
		
		bonusDamageDeck[0] = 1;
	}

	public void IncreaseElement(string tempSpriteName){
		CharacterProperties.CategoryColour spriteNumber = CharacterProperties.CategoryColour.NONE;
		
		switch (tempSpriteName){
		case "red":
			spriteNumber = CharacterProperties.CategoryColour.RED;
			break;
		case "blue":
			spriteNumber = CharacterProperties.CategoryColour.BLUE;
			break;
		case "green":
			spriteNumber = CharacterProperties.CategoryColour.GREEN;
			break;
		case "yellow":
			spriteNumber = CharacterProperties.CategoryColour.ORANGE;
			break;
		case "purple":
			spriteNumber = CharacterProperties.CategoryColour.PURPLE;
			break;
		case "white":
			spriteNumber = CharacterProperties.CategoryColour.WHITE;
			break;
		}
		
		for(int i = 0; i<skillButtonArray.Length; i++)
		{
			if(skillButtonArray[i].GetComponent<UnitButtonHandler>().element == spriteNumber)
			{
				skillButtonArray[i].GetComponent<UnitButtonHandler>().currentAmount++;
			}
		}

		UpdateElementText();
	}

	public void ButtonHandler(GameObject go)
	{
		int currentButton = go.GetComponent<UnitButtonHandler>().buttonNumber;
		UnitButtonHandler buttonHandlerRef = go.GetComponent<UnitButtonHandler>();

		//int tempRequirement = skillButtonArray[currentButton].GetComponent<UnitButtonHandler>().cost;
		int tempRequirement = 5;
		int elementToCheck = buttonHandlerRef.referenceNumber;
		int settingsValue = buttonHandlerRef.settingsReferer;
		CharacterProperties.UnitType buttonUnitType = characterSettingsPrefab.GetComponent<CharacterSettings>().characterProperties[settingsValue].unitType;
		
		buttonHandlerRef.currentAmount -= 5;
		
		UpdateElementText();
		
		if(go.transform.FindChild("Dim").GetComponent<UISprite>().localSize.y != buttonHandlerRef.originalSize.y){
			go.GetComponent<UnitButtonHandler>().ActivateButtonCoolDown();
		}
		
		if(GlobalManager.multiplyerGame){
			this.gameObject.GetComponent<GameNetworkHandler>().NetworkMessage(GlobalManager.NetworkMessage.SpawnUnit, new object[4]{bonusDamageDeck, settingsValue + 1, 5, (int)buttonUnitType});
		}else{
			defObj.SpawnCharacter(bonusDamageDeck, settingsValue + 1, GlobalManager.UICard.localUserCardDeck[currentButton].level, buttonUnitType, CharacterProperties.Team.LEFT);
		}
	}
	
	public void UpdateElementText(){
		for(int i=0; i<skillButtonArray.Length; i++){
			bool canTurnOn = false;
			int tempRequirement = 5;
			int totalAmount = skillButtonArray[i].GetComponent<UnitButtonHandler>().currentAmount;
			int elementToCheck = skillButtonArray[i].GetComponent<UnitButtonHandler>().referenceNumber;
			int settingsValue = skillButtonArray[i].GetComponent<UnitButtonHandler>().settingsReferer;

			if(tempRequirement != -1 && settingsValue >= 0){
				skillButtonArray[i].GetComponent<UnitButtonHandler>().GaugePercentage = (float)totalAmount / (float)tempRequirement;

				if(skillButtonArray[i].GetComponent<UnitButtonHandler>().currentAmount >= tempRequirement){
					canTurnOn = true;
				}

				if(GlobalManager.ignoreCost)
				{
					canTurnOn = true;
				}
			}
			
			if(canTurnOn){
				if(skillButtonArray[i].GetComponent<UnitButtonHandler>().canEnable){
					skillButtonArray[i].collider.enabled = true;
					skillButtonArray[i].transform.FindChild("Dim").GetComponent<UISprite>().SetDimensions(80, 0);
				}
			}else{
				skillButtonArray[i].collider.enabled = false;
				skillButtonArray[i].transform.FindChild("Dim").GetComponent<UISprite>().SetDimensions(80, 80);
			}
		}
	}
	
	private int GetCurrentClickedButton(string buttonName){
		for(int i=0; i<skillButtonArray.Length; i++){
			if(skillButtonArray[i].GetComponentInChildren<UISprite>().name == buttonName){
				return i + 1;
			}
		}
		
		return -1;
	}
}
