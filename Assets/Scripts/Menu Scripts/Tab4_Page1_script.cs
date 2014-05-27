using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Tab4_Page1_script : SubPageHandler {

	public GameObject rankingPrefab;
	public UIScrollView scrollView;
	private Vector2 dimension;
	private float gap = 15f;

	// Use this for initialization
	void Start () {
		dimension = rankingPrefab.GetComponent<UISprite>().localSize;

		base.parent.tabParent.OpenMainLoader(true);

		WWWForm form = new WWWForm(); //here you create a new form connection
		form.AddField("userId", GlobalManager.LocalUser.UID);
		
		NetworkHandler.self.ResultDelegate += ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate += ServerRequestError;
		NetworkHandler.self.ServerRequest(GlobalManager.NetworkSettings.GetFullURL(GlobalManager.RequestType.RANKING), form);
	}

	private void SpawnRankingObject()
	{

	}

	private void ServerRequestCallback(string result)
	{
		NetworkHandler.self.ResultDelegate -= ServerRequestCallback;
		NetworkHandler.self.ErrorDelegate -= ServerRequestError;
		base.parent.tabParent.OpenMainLoader(false);
		
		var N = JSONNode.Parse(result);
		//Debug.Log("callback: " + N["userId"]);
		int totalRank = N["rankList"].AsArray.Count;
		if(totalRank > 0)
		{
			// user exist -- go to LandingScene
			//N["totalWin"].AsInt;
			
			for(int i=0; i<totalRank; i++)
			{
				Vector3 pos = Vector3.zero;
				GameObject holder = Instantiate(rankingPrefab, Vector3.zero, Quaternion.identity) as GameObject;
				pos.y = ((i * dimension.y) + (dimension.y / 2) + (i * gap)) * -1;
				holder.transform.parent = scrollView.transform;
				holder.transform.localScale = holder.transform.lossyScale;
				holder.transform.localPosition = pos;
				holder.name = "Rank_" + (i+1);
				holder.GetComponent<RankingObjectScript>().nameLabel.text = N["rankList"][i]["nickname"];
				holder.GetComponent<RankingObjectScript>().levelLabel.text = N["rankList"][i]["userLevel"];
				holder.GetComponent<RankingObjectScript>().rankLabel.text = N["rankList"][i]["rank"];
				holder.GetComponent<RankingObjectScript>().battleStatusLabel.text = N["rankList"][i]["totalWin"] + " Wins " + (N["rankList"][i]["totalBattle"].AsInt - N["rankList"][i]["totalWin"].AsInt).ToString() + " loses";
				holder.GetComponent<RankingObjectScript>().victoryLabel.text = N["rankList"][i]["victoryPoint"];
				holder.GetComponent<RankingObjectScript>().backgroundSprite.spriteName = i == 0 ? "box_1" : "btn_bg";
				holder.GetComponent<RankingObjectScript>().Rank = i+1;
				//UIEventListener.Get(holder).onClick += StageAreaClickHandler;
			}

			scrollView.ResetPosition();
		}
		else
		{
			// no user -- show register popup
			base.parent.tabParent.OpenMainLoader(false);
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
