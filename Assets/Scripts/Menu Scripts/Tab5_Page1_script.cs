using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab5_Page1_script : SubPageHandler {

	public UIButton button1;
	public UIButton button2;
	public UIButton button3;
	public UIButton button4;
	public UIButton button5;
	private int tempOrder = -1;

	// Use this for initialization
	void Start () {
		UIEventListener.Get(button1.gameObject).onClick += ButtonHandler;
		UIEventListener.Get(button2.gameObject).onClick += ButtonHandler;
		UIEventListener.Get(button3.gameObject).onClick += ButtonHandler;
		UIEventListener.Get(button4.gameObject).onClick += ButtonHandler;
		UIEventListener.Get(button5.gameObject).onClick += ButtonHandler;
	}

	public void ButtonHandler(GameObject go)
	{
		tempOrder = int.Parse(go.name.Split(new char[1]{' '})[1]);

		base.parent.tabParent.OpenMainLoader(true);
		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("userId", GlobalManager.LocalUser.UID);
		form.AddField("type", 2);
		form.AddField("order", tempOrder);
		
		NetworkHandler.self.ResultDelegate += ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate += ServerRequestError;
		NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.SHOP_PAYMENT), form);
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
			int totalAmount = 0;
			if(tempOrder == 1)
			{
				totalAmount = 5;
			}
			else if(tempOrder == 2)
			{
				totalAmount = 11;
			}
			else if(tempOrder == 3)
			{
				totalAmount = 30;
			}
			else if(tempOrder == 4)
			{
				totalAmount = 60;
			}
			else if(tempOrder == 5)
			{
				totalAmount = 140;
			}

			GlobalManager.LocalUser.gem += totalAmount;
			base.parent.tabParent.UpdateUserDetailBar();
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
}
