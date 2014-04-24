using UnityEngine;
using System.Collections;

public class Tab2_Page2_script : SubPageHandler {

	public UILabel costLabel;
	public GameObject deckObj = null;
	private Vector2 deckDimension;

	// Use this for initialization
	void Start () {
		if(deckObj == null) return;

		GlobalManager.LocalUser.deckCost = 0;
		deckDimension = deckObj.GetComponent<UISprite>().localSize;
		SpawnLocalUserDeck();
		UpdateDeckCost();
		base.StartSubPage();
	}

	private void SpawnLocalUserDeck()
	{
		Transform parent = this.transform.Find("Deck List Holder");

		for(int i = 0; i<6; i++)
		{
			int cardDeckNum = GlobalManager.UICard.localUserCardDeck[i];
			Vector3 pos = Vector3.zero;
			GameObject holder = Instantiate(deckObj, Vector3.zero, Quaternion.identity) as GameObject;
			holder.name = "Deck_" + (i+1);
			holder.transform.parent = parent;
			pos.x = ((i * deckDimension.x) + (deckDimension.x / 2)) + 3;
			holder.transform.localPosition = pos;
			holder.transform.localScale = holder.transform.lossyScale;
			holder.GetComponent<UICardScript>().cardNum = cardDeckNum;

			if(cardDeckNum >= 0)
			{
				GlobalManager.LocalUser.deckCost += base.csObj.characterProperties[cardDeckNum].unitCost;
			}
		}
	}

	private void UpdateDeckCost()
	{
		costLabel.text = GlobalManager.LocalUser.deckCost + "/" + GlobalManager.LocalUser.maxDeckCost;
	}
}
