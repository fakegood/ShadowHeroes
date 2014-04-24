using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SubPageHandler : MonoBehaviour {
	
	public TabPageHandler parent;
	public CharacterSettings csObj = null;
	public GameObject popupObj;
	public GameObject[] tabPages;
	private GameObject currentOpenedPage;
	private bool popupOpened = false;

	// Use this for initialization
	void Start () {
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

		if(parent.currentSelectedDeckNum != -1)
		{
			OpenPopup(true);
		}
		else
		{
			OpenPopup(false);
		}
	}
	
	private void OverrideClickHandler(GameObject go)
	{
		//Debug.Log("HELLO");

		if(go.GetComponent<UICardScript>() != null && parent.currentOpenedPageNum == 2)
		{
			// open deck selection page
			parent.currentOpenedDeckNum = int.Parse(go.name.Split(new char[]{'_'})[1]);

			WWWForm form = new WWWForm(); //here you create a new form connection
			form.AddField("userId", GlobalManager.LocalUser.UID);
			
			NetworkHandler.self.ResultDelegate += ServerRequestCallback;
			NetworkHandler.self.ErrorDelegate += ServerRequestError;
			NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.GET_INVENTORY), form);
		}
		else if(parent.currentOpenedPageNum == 3)
		{
			// chosen card action
			parent.currentSelectedDeckNum = int.Parse(go.name.Split(new char[]{'_'})[1]) - 1;

			if(parent.currentSelectedDeckNum == 0)
			{
				// "None" selected -- perform swap right away
				GlobalManager.UICard.SwapDeckAndInventory(parent.currentSelectedDeckNum, parent.currentOpenedDeckNum);
				
				parent.currentOpenedDeckNum = -1;
				parent.currentSelectedDeckNum = -1;
				
				parent.OpenSubPage(2);
			}
			else
			{
				// show popup
				OpenPopup();
			}
		}
		else
		{
			if(go.name == "First Button")
			{

			}
			else
			{
				parent.OpenSubPage(2);
			}
		}
	}

	private void PopupButtonHandler(GameObject go)
	{
		if(go.name == "Close Popup Button")
		{
			OpenPopup(false);
		}
		else if(go.name == "View Info Button")
		{
			parent.OpenSubPage(4);
		}
		else if(go.name == "Change Unit Button")
		{
			if(GlobalManager.LocalUser.deckCost + csObj.characterProperties[parent.currentSelectedDeckNum-1].unitCost <= GlobalManager.LocalUser.maxDeckCost)
			{
				GlobalManager.UICard.SwapDeckAndInventory(parent.currentSelectedDeckNum, parent.currentOpenedDeckNum);
				
				parent.currentOpenedDeckNum = -1;
				parent.currentSelectedDeckNum = -1;
				
				parent.OpenSubPage(2);
			}
			else
			{
				// over deck cost -- cannot change
				Debug.Log("Cannot swap -- not enough cost");
			}
		}
	}

	public void OpenPage(int pageNumber)
	{
		if(currentOpenedPage != null)
		{
			UIButton[] gos = this.transform.GetComponentsInChildren<UIButton>();

			foreach(UIButton obj in gos)
			{
				//UIEventListener.Get(obj.gameObject).onClick += parent.PageButtonClickHandler;
				UIEventListener.Get(obj.gameObject).onClick -= OverrideClickHandler;
			}

			Destroy(currentOpenedPage);
			currentOpenedPage = null;
		}

		if(tabPages.Length >= pageNumber)
		{
			GameObject holder = Instantiate(tabPages[pageNumber-1], Vector3.zero, Quaternion.identity) as GameObject;
			holder.transform.parent = this.transform;
			holder.transform.localScale = holder.transform.lossyScale;
			currentOpenedPage = holder;
		}
	}

	public virtual void OpenPopup(bool open = true)
	{
		if(popupObj != null)
		{
			popupObj.SetActive(open);
			popupOpened = open;
			
			if(open)
			{
				UIButton[] gos = popupObj.transform.GetComponentsInChildren<UIButton>();
				
				foreach(UIButton obj in gos)
				{
					//UIEventListener.Get(obj.gameObject).onClick += parent.PageButtonClickHandler;
					UIEventListener.Get(obj.gameObject).onClick += PopupButtonHandler;
				}
			}
			else
			{
				UIButton[] gos = popupObj.transform.GetComponentsInChildren<UIButton>();
				
				foreach(UIButton obj in gos)
				{
					//UIEventListener.Get(obj.gameObject).onClick += parent.PageButtonClickHandler;
					UIEventListener.Get(obj.gameObject).onClick -= PopupButtonHandler;
				}
			}
		}
	}

	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		//loader.SetActive(false);
		
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
			
			parent.OpenSubPage(3);
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
		//loader.SetActive(false);
		//var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
	}
}
