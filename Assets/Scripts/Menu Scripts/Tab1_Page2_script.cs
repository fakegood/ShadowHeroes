using UnityEngine;
using System.Collections;

public class Tab1_Page2_script : SubPageHandler {

	public StageUISettings stage;
	public AIBehaviorSettings aiStage;
	public bool stageUISettings = true;
	public GameObject stageButton;
	public UIScrollView scrollView;
	private Vector2 dimension;
	private float gap = 15f;

	// Use this for initialization
	void Start () {
		base.parent.SetSubTitle("Select a dungeon");
		dimension = stageButton.GetComponent<UISprite>().localSize;

		SpawnStageList();
		scrollView.ResetPosition();

		//base.StartSubPage();
	}

	private void SpawnStageList()
	{
		if(stageUISettings)
		{
			for(int i=0; i<stage.area.Length; i++)
			{
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(stageButton, Vector3.zero, Quaternion.identity) as GameObject;
				pos.y = ((i * dimension.y) + (dimension.y / 2) + (i * gap)) * -1;
				holder.transform.parent = scrollView.transform;
				holder.transform.localScale = holder.transform.lossyScale;
				holder.transform.localPosition = pos;
				holder.name = "Area_" + (i+1);
				holder.GetComponent<StageUIButtonScript>().StageName = stage.area[i].areaName;
				holder.GetComponent<StageUIButtonScript>().APCost = 0;
				holder.AddComponent<UIDragScrollView>();
				UIEventListener.Get(holder).onClick += StageAreaClickHandler;
			}
		}
		else
		{
			for(int i=0; i<aiStage.area.Length; i++)
			{
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(stageButton, Vector3.zero, Quaternion.identity) as GameObject;
				pos.y = ((i * dimension.y) + (dimension.y / 2) + (i * gap)) * -1;
				holder.transform.parent = scrollView.transform;
				holder.transform.localScale = holder.transform.lossyScale;
				holder.transform.localPosition = pos;
				holder.name = "Area_" + (i+1);
				holder.GetComponent<StageUIButtonScript>().StageName = aiStage.area[i].areaName;
				holder.GetComponent<StageUIButtonScript>().APCost = 0;
				holder.AddComponent<UIDragScrollView>();
				UIEventListener.Get(holder).onClick += StageAreaClickHandler;
			}
		}
	}

	private void StageAreaClickHandler(GameObject go)
	{
		GlobalManager.GameSettings.chosenArea = int.Parse(go.name.Split(new char[]{'_'})[1]);
		base.parent.OpenSubPage(3);
	}
}
