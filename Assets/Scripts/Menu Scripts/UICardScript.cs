using UnityEngine;
using System.Collections;

public class UICardScript : MonoBehaviour {

	public CharacterSettings csObj = null;
	public int cardNum = -1;
	public int level = 1;
	public int deckCost = -1;
	public UILabel cardName;

	// Use this for initialization
	void Start () {
		if(csObj == null) return;

		if(cardNum < 0)
		{
			cardName.text = "None";
		}
		else
		{
			cardName.text = csObj.characterProperties[(cardNum-1)].cardName;
		}
	}
}
