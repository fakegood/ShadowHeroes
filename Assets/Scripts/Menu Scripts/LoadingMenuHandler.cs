using UnityEngine;
using System.Collections;
using SimpleJSON;

public class LoadingMenuHandler : MonoBehaviour {

	public GameObject loader;
	public GameObject registerObj;
	public UILabel nicknameLabel;
	public UILabel errorLabel;
	public UIButton submitButton;

	// Use this for initialization
	void Start () {
		Debug.Log(SystemInfo.deviceUniqueIdentifier);

		registerObj.SetActive(false);
		loader.SetActive(true);

		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("deviceId", SystemInfo.deviceUniqueIdentifier);

		NetworkHandler.self.ResultDelegate += ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate += ServerRequestError;
		NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.INIT), form);
	}

	private void ShowRegisterPopup()
	{
		registerObj.SetActive(true);

		UIEventListener.Get(submitButton.gameObject).onClick += SubmitNickname;
	}

	private void SubmitNickname(GameObject go)
	{
		if(Validate(nicknameLabel.text))
		{
			WWWForm form = new WWWForm(); //here you create a new form connection
			form.AddField("deviceId", SystemInfo.deviceUniqueIdentifier);
			form.AddField("nickname", nicknameLabel.text);
			
			NetworkHandler.self.ResultDelegate += ServerRequestCallback;
			NetworkHandler.self.ErrorDelegate += ServerRequestError;
			NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.REGISTER), form);
		}
	}

	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		//loader.SetActive(false);

		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		int tempUserID = N["userId"].AsInt;
		if(tempUserID > 0)
		{
			// user exist -- go to LandingScene
			GlobalManager.LocalUser.UID = N["userId"].AsInt;
			GlobalManager.LocalUser.nickname = N["nickname"];
			GlobalManager.LocalUser.level = N["userLevel"].AsInt;
			GlobalManager.LocalUser.experience = N["userExperience"].AsInt;
			GlobalManager.LocalUser.gold = N["gold"].AsInt;
			GlobalManager.LocalUser.gem = N["gem"].AsInt;
			GlobalManager.LocalUser.victoryPoint = N["victoryPoint"].AsInt;
			GlobalManager.LocalUser.totalBattle = N["totalBattle"].AsInt;
			GlobalManager.LocalUser.totalWin = N["totalWin"].AsInt;

			Application.LoadLevel("LandingMenu");
		}
		else
		{
			// no user -- show register popup
			loader.SetActive(false);
			ShowRegisterPopup();
		}
	}

	private void ServerRequestError(string result)
	{
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		loader.SetActive(false);
		//var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
	}

	private bool Validate(string text)
	{
		if(text.Length == 0 || text == "Player")
		{
			errorLabel.text = "Please enter a nickname.";
			return false;
		}
		else if(text.Length >= 0 && text.Length <= 3)
		{
			errorLabel.text = "Nickname should be at least 3 characters.";
			return false;
		}

		errorLabel.text = "";
		return true;
	}
}
