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
	}
	
	private void OverrideClickHandler(GameObject go)
	{

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
}
