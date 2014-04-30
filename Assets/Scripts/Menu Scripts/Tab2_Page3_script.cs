using UnityEngine;
using System.Collections;

public class Tab2_Page3_script : SubPageHandler {

	public GameObject deckObj = null;
	public UIScrollView scrollPanel = null;
	private int totalCol = 6;
	private Vector2 deckDimension;

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
		Transform parent = this.transform.Find("ScrollPanel/Inventory List Holder");
		int currentRow = 0;
		int currentCol = 0;

		for(int i = 0; i<GlobalManager.UICard.localUserCardInventory.Count+1; i++)
		{
			Vector3 pos = Vector3.zero;
			GameObject holder = Instantiate(deckObj, Vector3.zero, Quaternion.identity) as GameObject;
			holder.name = "Inventory_" + (i+1);
			holder.transform.parent = parent;
			
			if(i % 6 == 0 && i != 0){ currentRow++; }
			pos.x = ((currentCol * deckDimension.x) + (deckDimension.x / 2)) + 3;
			pos.y = (((currentRow * deckDimension.y) + (deckDimension.y / 2)) + 3) * -1;
			
			holder.transform.localPosition = pos;
			holder.transform.localScale = holder.transform.lossyScale;
			holder.AddComponent<UIDragScrollView>();
			UIEventListener.Get(holder).onClick += ButtonHandler;

			if(i == 0)
			{
				holder.GetComponent<UICardScript>().Card = null;
			}
			else
			{
				CharacterCard tempCardObj = GlobalManager.UICard.localUserCardInventory[i-1];
				holder.GetComponent<UICardScript>().Card = tempCardObj;
				holder.GetComponent<UICardScript>().inventoryIndex = i-1;

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
		parent.currentSelectedDeckNum = int.Parse(go.name.Split(new char[]{'_'})[1]) - 1;
		parent.currentSelectedCard = GlobalManager.UICard.localUserCardInventory[go.GetComponent<UICardScript>().inventoryIndex];
		
		if(parent.currentSelectedDeckNum == 0)
		{
			// "None" selected -- perform swap right away
			GlobalManager.UICard.SwapDeckAndInventory(parent.currentSelectedDeckNum, parent.currentOpenedDeckNum);
			
			parent.currentOpenedDeckNum = -1;
			parent.currentSelectedDeckNum = -1;
			
			parent.OpenSubPage(2);
		}
		else
		{
			// show popup
			base.OpenPopup(true);
		}
	}
}
