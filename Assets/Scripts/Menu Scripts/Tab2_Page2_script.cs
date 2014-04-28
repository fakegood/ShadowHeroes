using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab2_Page2_script : SubPageHandler {

	public UILabel costLabel;
	public UIButton saveButton;
	public GameObject deckObj = null;
	private Vector2 deckDimension;

	// Use this for initialization
	void Start () {
		if(deckObj == null) return;

		UIEventListener.Get(saveButton.gameObject).onClick += SaveDeckHandler;
		saveButton.isEnabled = GlobalManager.UICard.changed;
		GlobalManager.LocalUser.deckCost = 0;
		deckDimension = deckObj.GetComponent<UISprite>().localSize;
		SpawnLocalUserDeck();
		UpdateDeckCost();
		//base.StartSubPage();

		if(parent.currentSelectedDeckNum != -1)
		{
			base.OpenPopup(true);
		}
		else
		{
			base.OpenPopup(false);
		}
	}

	private void ButtonHandler(GameObject go)
	{
		if(go.GetComponent<UICardScript>() != null && parent.currentOpenedPageNum == 2)
		{
			// open deck selection page
			parent.currentOpenedDeckNum = int.Parse(go.name.Split(new char[]{'_'})[1]);
			
			parent.tabParent.OpenMainLoader(true);
			WWWForm form = new WWWForm(); //here you create a new form connection
			form.AddField("userId", GlobalManager.LocalUser.UID);
			
			NetworkHandler.self.ResultDelegate += InventoryServerRequestCallback;
			NetworkHandler.self.ErrorDelegate += InventoryServerRequestError;
			NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.GET_INVENTORY), form);
		}
	}

	private void SpawnLocalUserDeck()
	{
		Transform parent = this.transform.Find("Deck List Holder");

		for(int i = 0; i<6; i++)
		{
			CharacterCard cardDeckNum = null;
			if(i < GlobalManager.UICard.localUserCardDeck.Count)
			{
				cardDeckNum = GlobalManager.UICard.localUserCardDeck[i];
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(deckObj, Vector3.zero, Quaternion.identity) as GameObject;
				holder.name = "Deck_" + (i+1);
				holder.transform.parent = parent;
				pos.x = ((i * deckDimension.x) + (deckDimension.x / 2)) + 3;
				holder.transform.localPosition = pos;
				holder.transform.localScale = holder.transform.lossyScale;
				holder.GetComponent<UICardScript>().Card = cardDeckNum;
				UIEventListener.Get(holder).onClick += ButtonHandler;
			}
			else
			{
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(deckObj, Vector3.zero, Quaternion.identity) as GameObject;
				holder.name = "Deck_" + (i+1);
				holder.transform.parent = parent;
				pos.x = ((i * deckDimension.x) + (deckDimension.x / 2)) + 3;
				holder.transform.localPosition = pos;
				holder.transform.localScale = holder.transform.lossyScale;
				UIEventListener.Get(holder).onClick += ButtonHandler;
			}

			if(cardDeckNum != null && cardDeckNum.cardNumber >= 0)
			{
				GlobalManager.LocalUser.deckCost += base.csObj.characterProperties[cardDeckNum.cardNumber-1].unitCost;
			}
		}
	}

	private void UpdateDeckCost()
	{
		costLabel.text = GlobalManager.LocalUser.deckCost + "/" + GlobalManager.LocalUser.maxDeckCost;
	}

	public void SaveDeckHandler(GameObject go)
	{
		string deckSaveString = "";
		for(int i=0; i<GlobalManager.UICard.localUserCardDeck.Count; i++)
		{
			if(GlobalManager.UICard.localUserCardDeck[i] != null)
			{
				int val = GlobalManager.UICard.localUserCardDeck[i].UID;
				deckSaveString = deckSaveString == "" ? deckSaveString += val : deckSaveString += "," + val;
			}
			else
			{
				int val = -1;
				deckSaveString = deckSaveString == "" ? deckSaveString += val : deckSaveString += "," + val;
			}
		}

		base.parent.tabParent.OpenMainLoader(true);
		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("userId", GlobalManager.LocalUser.UID);
		form.AddField("cardDeckList", deckSaveString);
		
		NetworkHandler.self.ResultDelegate += ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate += ServerRequestError;
		NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.SAVE_DECK), form);
	}
	
	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);

		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		bool saveStat = N["result"].AsBool;
		if(saveStat)
		{
			// TODO - disable save button (some problem with missing reference)
			saveButton.isEnabled = GlobalManager.UICard.changed = false;
		}
		else
		{
			// no user -- show register popup
			//loader.SetActive(false);
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
		parent.tabParent.OpenMainLoader(false);
		
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
				cardObj.cardNumber = N["cardDeck"][i]["cardNumber"].AsInt + 1;
				cardObj.level = N["cardDeck"][i]["cardLevel"].AsInt;
				
				GlobalManager.UICard.localUserCardInventory.Add(cardObj);
			}
			
			parent.OpenSubPage(3);
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
		parent.tabParent.OpenMainLoader(false);
		//var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
	}
}
