using UnityEngine;
using System.Collections;

public class Tab2_Page7_script : SubPageHandler {

	public GameObject deckObj = null;
	public UIScrollView scrollPanel = null;
	private int totalCol = 6;
	private Vector2 deckDimension;
	private float gap = 50f;
	
	// Use this for initialization
	void Start () {
		if(deckObj == null) return;
		
		deckDimension = deckObj.GetComponent<UISprite>().localSize;
		SpawnLocalUserInventory();
		scrollPanel.ResetPosition();
		
		//base.StartSubPage();
	}
	
	private void SpawnLocalUserInventory()
	{
		Transform parent = this.transform.Find("ScrollView/Inventory List Holder");
		int currentRow = 0;
		int currentCol = 0;
		
		for(int i = 0; i<GlobalManager.UICard.localUserCardInventory.Count; i++)
		{
			Vector3 pos = Vector3.zero;
			float startingGap = 0f;
			GameObject holder = Instantiate(deckObj, Vector3.zero, Quaternion.identity) as GameObject;
			holder.name = "Inventory_" + (i+1);
			holder.transform.parent = parent;
			
			if(i % 6 == 0 && i != 0){ currentRow++; }
			if(currentRow == 0) startingGap = 25f;
			pos.x = ((currentCol * deckDimension.x) + (deckDimension.x / 2)) + 3f;
			pos.y = (((currentRow * deckDimension.y) + (deckDimension.y / 2)) + (gap * currentRow) + 6f + startingGap) * -1;
			
			holder.transform.localPosition = pos;
			holder.transform.localScale = holder.transform.lossyScale;
			holder.AddComponent<UIDragScrollView>();
			UIEventListener.Get(holder).onClick += ButtonHandler;

			CharacterCard tempCardObj = GlobalManager.UICard.localUserCardInventory[i];
			holder.GetComponent<UICardScript>().Card = tempCardObj;
			
			for(int j=0; j<6; j++)
			{
				if(GlobalManager.UICard.localUserCardDeck[j] != null)
				{
					if(tempCardObj.UID == GlobalManager.UICard.localUserCardDeck[j].UID)
					{
						holder.GetComponent<UIButton>().isEnabled = false;
						break;
					}
				}
			}
			
			currentCol++;
			if(currentCol >= totalCol)
			{
				currentCol = 0;
			}
		}
	}
	
	private void ButtonHandler(GameObject go)
	{
		// chosen card action
		go.GetComponent<UICardScript>().Selected = !go.GetComponent<UICardScript>().Selected;
	}
}
