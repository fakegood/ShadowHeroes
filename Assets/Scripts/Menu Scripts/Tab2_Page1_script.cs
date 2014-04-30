using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab2_Page1_script : SubPageHandler {

	private int whichPage = -1;

	// Use this for initialization
	void Start () {
		//base.StartSubPage();
		StartSubPage();
	}

	public virtual void StartSubPage()
	{
		UIButton[] gos = this.transform.GetComponentsInChildren<UIButton>();
		
		foreach(UIButton obj in gos)
		{
			//UIEventListener.Get(obj.gameObject).onClick += parent.PageButtonClickHandler;
			UIEventListener.Get(obj.gameObject).onClick += OverrideClickHandler;
		}
	}
	
	private void OverrideClickHandler(GameObject go)
	{
		if(go.name == "Edit Deck")
		{
			base.parent.pageSelected = whichPage = 2;
			parent.OpenSubPage(2);
		}
		else if(go.name == "Enhance" || go.name == "Sell Card")
		{
			base.parent.enhanceCardSelected = -1;
			base.parent.enhanceBaseCard = null;
			base.parent.enhanceCards = new CharacterCard[5];

			base.parent.pageSelected = whichPage = go.name == "Enhance" ? 5 : 8;
			base.parent.tabParent.OpenMainLoader(true);
			WWWForm form = new WWWForm(); //here you create a new form connection
			form.AddField("userId", GlobalManager.LocalUser.UID);
			
			NetworkHandler.self.ResultDelegate += InventoryServerRequestCallback;
			NetworkHandler.self.ErrorDelegate += InventoryServerRequestError;
			NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.GET_INVENTORY), form);
		}
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
			
			parent.OpenSubPage(whichPage);
			whichPage = -1;
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
