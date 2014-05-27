using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab6_Page1_script : SubPageHandler {

	public UIButton soundButton;
	public UIButton vibrateButton;
	public UIButton moreGamesButton;
	public UIButton rateUsButton;

	// Use this for initialization
	void Start () {
	
	}

	public void SoundClick()
	{

	}

	public void VibrateClick()
	{
		
	}

	public void ResetClick()
	{
		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("userId", GlobalManager.LocalUser.UID);
		
		NetworkHandler.self.ResultDelegate += ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate += ServerRequestError;
		NetworkHandler.self.ServerRequest("http://touchtouch.biz/shadow/resetuser.jsp", form);
	}

	public void MoreGamesClick()
	{
		Application.OpenURL("https://www.google.com");
	}

	public void RateUsClick()
	{
		Application.OpenURL("https://www.naver.com");
	}

	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		//loader.SetActive(false);
		
		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		//int tempUserID = N["userId"].AsInt;
		//loader.SetActive(false);
		PlayerPrefs.DeleteAll();
		Application.LoadLevel("LoadingScene");
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
