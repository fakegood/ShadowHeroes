using UnityEngine;
using System.Collections;

public class LandingMenuHandler : MonoBehaviour {

	// player level details
	public UILabel levelLabel;
	public UILabel experienceLabel;
	public UIProgressBar experienceBar;
	
	// player fighting point details
	public UILabel battleLabel;
	public UIProgressBar battleBar;
	
	// player battle point details
	public UILabel actionLabel;
	public UIProgressBar actionBar;
	
	// player gold details
	public UILabel goldLabel;
	
	// player gem details
	public UILabel gemLabel;
	
	public UIPanel mainLoader;

	public UIButton[] tabButtons = null;
	public UIPanel[] tabPages = null;
	private GameObject currentOpenedPage = null;

	// Use this for initialization
	void Start () {
		TopBarHandler();

		GlobalManager.GameSettings.chosenArea = -1;
		GlobalManager.GameSettings.chosenStage = -1;

		Screen.sleepTimeout = SleepTimeout.SystemSetting;

		for(int i = 0; i < tabButtons.Length; i++)
		{
			UIEventListener.Get(tabButtons[i].gameObject).onClick += TabPageHandler;
		}

		for(int i = 0; i < tabPages.Length; i++)
		{
			tabPages[i].gameObject.GetComponent<TabPageHandler>().DeactivateTab();
		}

		if(GlobalManager.multiplyerGame)
		{
			JumpToPage(1, 4);
		}
		else
		{
			tabPages[0].gameObject.GetComponent<TabPageHandler>().ActivateTab();
		}
	}

	private void TabPageHandler(GameObject go)
	{
		if(PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}

		for(int i = 0; i < tabPages.Length; i++)
		{
			tabPages[i].gameObject.GetComponent<TabPageHandler>().DeactivateTab();
		}

		tabPages[int.Parse(go.name)-1].gameObject.GetComponent<TabPageHandler>().ActivateTab();
	}

	private void OpenTabPage(int tabNumber)
	{
		if(currentOpenedPage != null)
		{
			Destroy(currentOpenedPage);
			currentOpenedPage = null;
		}
		
		if(tabPages.Length >= tabNumber)
		{
			GameObject holder = Instantiate(tabPages[tabNumber-1], Vector3.zero, Quaternion.identity) as GameObject;
			holder.transform.parent = this.transform;
			holder.transform.localScale = holder.transform.lossyScale;
			currentOpenedPage = holder;
		}
	}

	private void TopBarHandler()
	{
		int userCurrentLevel = GlobalManager.LocalUser.level;
		float userCurrentExp = (float)GlobalManager.LocalUser.experience;
		float prevLevelMaxExp = (float)GlobalManager.LocalUser.ComputeNeedLevelupExp(userCurrentLevel-1);
		float nextLevelMaxExp = (float)GlobalManager.LocalUser.ComputeNeedLevelupExp(userCurrentLevel);

		levelLabel.text = "Lv " + userCurrentLevel;
		experienceLabel.text = userCurrentExp + "/" + nextLevelMaxExp;
		experienceBar.value = userCurrentExp / nextLevelMaxExp;
		
		battleLabel.text = GlobalManager.LocalUser.battlePoint + "/" + 188;
		battleBar.value = (float)(GlobalManager.LocalUser.battlePoint / 188);
		
		actionLabel.text = GlobalManager.LocalUser.actionPoint + "/" + GlobalManager.LocalUser.ComputeActionPoint(userCurrentLevel);
		actionBar.value = GlobalManager.LocalUser.actionPoint / GlobalManager.LocalUser.ComputeActionPoint(userCurrentLevel);
		
		goldLabel.text = GlobalManager.LocalUser.gold.ToString();
		
		gemLabel.text = GlobalManager.LocalUser.gem.ToString();
	}

	private void JumpToPage(int tabNum, int pageNum)
	{
		tabPages[tabNum-1].gameObject.GetComponent<TabPageHandler>().ActivateTab(pageNum);
	}

	public void OpenMainLoader(bool open = true)
	{
		mainLoader.gameObject.SetActive(open);
	}
}
