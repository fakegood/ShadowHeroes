using UnityEngine;
using System.Collections;

public class Tab1_Page3_script : SubPageHandler {

	public StageUISettings stage;
	public AIBehaviorSettings aiStage;
	public bool stageUISettings = true;
	public GameObject stageButton;
	public UIScrollView scrollView;
	private Vector2 dimension;
	private float gap = 3f;

	// Use this for initialization
	void Start () {
		dimension = stageButton.GetComponent<UISprite>().localSize;

		SpawnStageList();
		scrollView.ResetPosition();

		//base.StartSubPage();
	}

	private void SpawnStageList()
	{
		if(stageUISettings)
		{
			for(int i=0; i<stage.area[GlobalManager.GameSettings.chosenArea-1].subStage.Length; i++)
			{
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(stageButton, Vector3.zero, Quaternion.identity) as GameObject;
				pos.y = ((i * dimension.y) + (dimension.y / 2) + gap) * -1;
				holder.transform.parent = scrollView.transform;
				holder.transform.localScale = holder.transform.lossyScale;
				holder.transform.localPosition = pos;
				holder.name = "SubStage_" + (i+1);
				holder.GetComponent<StageUIButtonScript>().StageName = stage.area[GlobalManager.GameSettings.chosenArea-1].subStage[i].subStageName;
				holder.AddComponent<UIDragScrollView>();
				UIEventListener.Get(holder).onClick += StageAreaClickHandler;
			}
		}
		else
		{
			for(int i=0; i<aiStage.area[GlobalManager.GameSettings.chosenArea-1].subStage.Length; i++)
			{
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(stageButton, Vector3.zero, Quaternion.identity) as GameObject;
				pos.y = ((i * dimension.y) + (dimension.y / 2) + gap) * -1;
				holder.transform.parent = scrollView.transform;
				holder.transform.localScale = holder.transform.lossyScale;
				holder.transform.localPosition = pos;
				holder.name = "SubStage_" + (i+1);
				holder.GetComponent<StageUIButtonScript>().StageName = aiStage.area[GlobalManager.GameSettings.chosenArea-1].subStage[i].subStageName;
				holder.AddComponent<UIDragScrollView>();
				UIEventListener.Get(holder).onClick += StageAreaClickHandler;
			}
		}
	}

	private void StageAreaClickHandler(GameObject go)
	{
		GlobalManager.GameSettings.chosenStage = int.Parse(go.name.Split(new char[]{'_'})[1]);

		int levelCost = aiStage.area[GlobalManager.GameSettings.chosenArea-1].subStage[GlobalManager.GameSettings.chosenStage-1].energyCost;
		if((GlobalManager.LocalUser.actionPoint - levelCost) >= 0)
		{
			//base.parent.tabParent.mainPopup.GetComponent<MainPopupScript>().ResultDelegate += StartGameCallback;
			//base.parent.tabParent.OpenMainPopup("Single Player Game", "Are you sure to start game with " + levelCost + " gems?", true, MainPopupScript.PopupType.CONFIRMATION);

			GlobalManager.LocalUser.actionPoint -= levelCost;
			base.parent.tabParent.UpdateUserDetailBar();
			Application.LoadLevel("PlaySceneNGUI");
		}
		else
		{
			base.parent.tabParent.mainPopup.GetComponent<MainPopupScript>().ResultDelegate += ErrorHandler;
			base.parent.tabParent.OpenMainPopup("Not enough Energy", "You got not enough energy. Do you want to refill you energy bar with 1 gem?", true, MainPopupScript.PopupType.ERROR);
		}
	}

	private void StartGameCallback(bool result)
	{
		base.parent.tabParent.mainPopup.GetComponent<MainPopupScript>().ResultDelegate -= StartGameCallback;
		if(result)
		{
			int levelCost = aiStage.area[GlobalManager.GameSettings.chosenArea-1].subStage[GlobalManager.GameSettings.chosenStage-1].energyCost;
			GlobalManager.LocalUser.actionPoint -= levelCost;
			base.parent.tabParent.UpdateUserDetailBar();
			Application.LoadLevel("PlaySceneNGUI");
		}
	}

	private void ErrorHandler(bool result)
	{
		base.parent.tabParent.mainPopup.GetComponent<MainPopupScript>().ResultDelegate -= ErrorHandler;
	}
}
