using UnityEngine;
using System.Collections;

public class UICardScript : MonoBehaviour {

	public CharacterSettings csObj = null;
	public UILabel cardName;
	public UISprite cardBackground;
	public UISprite tick;
	public int inventoryIndex = -1;
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
				cardBackground.spriteName = csObj.characterProperties[(cardSettings.cardNumber-1)].iconSpriteName;
				if(cardBackground.GetComponent<UIButton>() != null)
				{
					cardBackground.GetComponent<UIButton>().normalSprite = cardBackground.GetComponent<UIButton>().hoverSprite = cardBackground.GetComponent<UIButton>().disabledSprite = csObj.characterProperties[(cardSettings.cardNumber-1)].iconSpriteName;
				}
			}
		}

		get{ return cardSettings; }
	}

	public bool Selected
	{
		set{ tick.gameObject.SetActive(value); }
		get{ return tick.gameObject.activeSelf; }
	}
}
