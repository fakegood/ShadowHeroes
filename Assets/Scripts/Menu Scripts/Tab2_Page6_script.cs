using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab2_Page6_script : SubPageHandler {

	public GameObject baseCardObject;
	public UILabel baseLevelLabel;
	public UILabel baseExpLabel;
	public UILabel baseAttackLabel;
	public UILabel baseHPLabel;

	public UIButton[] cards;
	public UILabel totalExpLabel;
	public UIButton confirmEnhanceButton;

	public GameObject popup;

	private CharacterCard cardObj;
	
	// Use this for initialization
	void Start () {
		cardObj = parent.enhanceBaseCard;
		if(cardObj == null) return;

		for(int i=0; i<cards.Length; i++)
		{
			UIEventListener.Get(cards[i].gameObject).onClick += EnhanceCardButtonHandler;
		}

		FillInformation();
		//base.StartSubPage();
	}

	private void FillInformation()
	{
		baseCardObject.GetComponent<UICardScript>().Card = cardObj;
		//baseLevelLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1];

		baseLevelLabel.text = GlobalManager.UICard.localUserCardInventory[parent.currentSelectedDeckNum].level.ToString();
		baseExpLabel.text = GlobalManager.UICard.localUserCardInventory[parent.currentSelectedDeckNum].experience.ToString();
		baseAttackLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].damage.ToString();
		baseHPLabel.text = base.csObj.characterProperties[cardObj.cardNumber-1].maxHitPoint.ToString();

		int totalExp = 0;
		for(int i=0; i<cards.Length; i++)
		{
			if(parent.enhanceCards[i] != null)
			{
				totalExp += GlobalManager.LocalUser.ComputeTotalExpFromCard(parent.enhanceCards[i]);
			}
			cards[i].GetComponent<AddCardButtonHandler>().Card = parent.enhanceCards[i];
		}

		if(totalExp > 0)
		{
			confirmEnhanceButton.isEnabled = true;
		}
		else
		{
			confirmEnhanceButton.isEnabled = false;
		}

		totalExpLabel.text = totalExp.ToString();
	}

	private void EnhanceCardButtonHandler(GameObject go)
	{
		parent.enhanceCardSelected = int.Parse(go.name.Split(new char[1]{' '})[1]) - 1;
		parent.OpenSubPage(7);
	}

	public void ConfirmEnhance()
	{
		int totalExp = 0;
		string finalString = "";
		for(int i=0; i<cards.Length; i++)
		{
			CharacterCard tempCard = cards[i].GetComponent<AddCardButtonHandler>().Card;
			if(tempCard != null)
			{
				totalExp += GlobalManager.LocalUser.ComputeTotalExpFromCard(tempCard);
				finalString = finalString == "" ? finalString += tempCard.UID : finalString += "," + tempCard.UID;
			}
		}

		if(totalExp > 0)
		{
			base.parent.tabParent.OpenMainLoader(true);
			WWWForm form = new WWWForm(); //here you create a new form connection
			form.AddField("userId", GlobalManager.LocalUser.UID);
			form.AddField("cardId", parent.enhanceBaseCard.UID);
			form.AddField("enhanceCardIdList", finalString);
			form.AddField("goldReduce", 200);
			
			NetworkHandler.self.ResultDelegate += ServerRequestCallback;
			NetworkHandler.self.ErrorDelegate += ServerRequestError;
			NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.ENHANCE), form);
		}
		else
		{
			// show popup -- no card to enhance
		}
	}

	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		
		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		
		if(N["result"].AsBool)
		{
			// done enhancing
			WWWForm form = new WWWForm(); //here you create a new form connection
			form.AddField("userId", GlobalManager.LocalUser.UID);
			
			NetworkHandler.self.ResultDelegate += InventoryServerRequestCallback;
			NetworkHandler.self.ErrorDelegate += InventoryServerRequestError;
			NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.GET_INVENTORY), form);
		}
		else
		{
			base.parent.tabParent.OpenMainLoader(false);
		}
	}
	
	private void ServerRequestError(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);
		//var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
	}

	private void InventoryServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= InventoryServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= InventoryServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);
		
		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		
		if(N["cardDeck"].AsArray.Count > 0)
		{
			// save all inventory details
			GlobalManager.UICard.localUserCardInventory.Clear();
			for(int i = 0; i<N["cardDeck"].AsArray.Count; i++)
			{
				CharacterCard cardObj = new CharacterCard();
				cardObj.UID = N["cardDeck"][i]["cardId"].AsInt;
				cardObj.experience = N["cardDeck"][i]["cardExperience"].AsInt;
				cardObj.cardNumber = N["cardDeck"][i]["cardNumber"].AsInt;
				cardObj.level = N["cardDeck"][i]["cardLevel"].AsInt;
				
				GlobalManager.UICard.localUserCardInventory.Add(cardObj);
			}
			
			//int tempTotalExp = 0;
			for(int i=0; i<cards.Length; i++)
			{
				//tempTotalExp += GlobalManager.LocalUser.ComputeTotalExpFromCard(tempCard);
				parent.enhanceCards[i] = cards[i].GetComponent<AddCardButtonHandler>().Card = null;
			}
			totalExpLabel.text = "0";
			confirmEnhanceButton.isEnabled = false;
			
			FillInformation();
			
			popup.GetComponent<EnhancePopupHandler>().cardObj = parent.enhanceBaseCard;
			popup.SetActive(true);
		}
		else
		{
			// no user -- show register popup
			//loader.SetActive(false);
		}
	}
	
	private void InventoryServerRequestError(string result)
	{
		NetworkHandler.self.ResultDelegate -= InventoryServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= InventoryServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);
		//var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
	}
}
