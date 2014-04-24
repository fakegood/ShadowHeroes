using UnityEngine;
using System.Collections;

public class UICardScript : MonoBehaviour {

	public CharacterSettings csObj = null;
	public UILabel cardName;
	private CharacterCard cardSettings = null;

	public CharacterCard Card
	{
		set{
			cardSettings = value;

			if(csObj == null) return;

			if(cardSettings == null || cardSettings.cardNumber < 0)
			{
				cardName.text = "None";
			}
			else
			{
				cardName.text = csObj.characterProperties[(cardSettings.cardNumber-1)].cardName;
			}
		}

		get{ return cardSettings; }
	}
}
