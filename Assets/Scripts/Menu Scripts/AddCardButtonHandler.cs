using UnityEngine;
using System.Collections;

public class AddCardButtonHandler : MonoBehaviour {

	public UIButton button;
	private CharacterCard cardObj;

	public CharacterCard Card
	{
		set{
			cardObj = value;
			if(cardObj != null)
			{
				this.GetComponent<UISprite>().spriteName = GlobalManager.GameSettings.csObj.characterProperties[cardObj.cardNumber-1].iconSpriteName;
				if(button != null)
				{
					button.normalSprite = button.hoverSprite = button.pressedSprite = button.disabledSprite = GlobalManager.GameSettings.csObj.characterProperties[cardObj.cardNumber-1].iconSpriteName;
				}
			}
			else
			{
				this.GetComponent<UISprite>().spriteName = button.normalSprite = button.hoverSprite = button.pressedSprite = button.disabledSprite = "add_card";
				if(button != null)
				{
					button.normalSprite = button.hoverSprite = button.pressedSprite = button.disabledSprite = "add_card";
				}
			}
		}
		get{ return cardObj; }
	}
}
