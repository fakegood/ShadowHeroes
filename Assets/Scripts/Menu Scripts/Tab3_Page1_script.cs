using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab3_Page1_script : SubPageHandler {

	public GameObject getCardPopup;
	public UIButton pokiOne;
	public UIButton pokiOneOne;
	private int type = -1;

	// Use this for initialization
	void Start () {
		UIEventListener.Get(pokiOne.gameObject).onClick += PokeHandler;
		UIEventListener.Get(pokiOneOne.gameObject).onClick += PokeHandler;
	}

	private void PokeHandler(GameObject go)
	{
		type = int.Parse(go.name.Split(new char[]{' '})[1]);
		base.parent.tabParent.OpenMainLoader(true);
		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("userId", GlobalManager.LocalUser.UID);
		form.AddField("type", type);
		
		NetworkHandler.self.ResultDelegate += ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate += ServerRequestError;
		NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.POKI), form);
	}

	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);
		
		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		CharacterCard[] tempCards = new CharacterCard[1];
		int arrayLength = N["cardDeck"].AsArray.Count;
		if(arrayLength > 0)
		{
			if(type == 1)
			{
				GlobalManager.LocalUser.gem -= 10;
				tempCards = new CharacterCard[1];
			}
			else if(type == 2)
			{
				GlobalManager.LocalUser.gem -= 100;
				tempCards = new CharacterCard[11];
			}

			for(int i = 0; i<tempCards.Length; i++)
			{
				CharacterCard cardObj = new CharacterCard();
				cardObj.UID = N["cardDeck"][i]["cardId"].AsInt;
				cardObj.experience = N["cardDeck"][i]["cardExperience"].AsInt;
				cardObj.cardNumber = N["cardDeck"][i]["cardNumber"].AsInt;
				cardObj.level = N["cardDeck"][i]["cardLevel"].AsInt;
				
				tempCards[i] = cardObj;
			}

			getCardPopup.GetComponent<PokiPopup>().cardObj = tempCards;
			getCardPopup.SetActive(true);

			base.parent.tabParent.UpdateUserDetailBar();
		}
		else
		{
			// no user -- show register popup
			//loader.SetActive(false);
		}

		type = -1;
	}
	
	private void ServerRequestError(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);
		type = -1;
		//var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
	}
}
