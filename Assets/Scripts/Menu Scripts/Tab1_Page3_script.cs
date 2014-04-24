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

		// TO-DO --- need to check for energy before starting game
		Application.LoadLevel("PlaySceneNGUI");
	}
}
