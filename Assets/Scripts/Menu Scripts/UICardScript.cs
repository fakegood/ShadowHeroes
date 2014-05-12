using UnityEngine;
using System.Collections;

public class UICardScript : MonoBehaviour {

	public CharacterSettings csObj = null;
	public UILabel cardName;
	public UISprite cardBackground;
	public UISprite tick;
	public int inventoryIndex = -1;
	private CharacterCard cardSettings = null;
	private GlobalManager.CardSortType cardSortType = GlobalManager.CardSortType.RARITY;

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
				//cardName.text = csObj.characterProperties[(cardSettings.cardNumber-1)].cardName;
				//cardName.text = csObj.characterProperties[(cardSettings.cardNumber-1)].rarity.ToString();
				//SortType = GlobalManager.CardSortType.RARITY;
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

	public GlobalManager.CardSortType SortType
	{
		set{
			cardSortType = value;

			if(cardSettings == null || cardSettings.cardNumber < 0)
			{
				cardName.text = "None";
			}
			else
			{
				if(cardSortType == GlobalManager.CardSortType.RARITY)
				{
					cardName.text = csObj.characterProperties[(Card.cardNumber-1)].rarity.ToString();
				}
				else if(cardSortType == GlobalManager.CardSortType.DAMAGE)
				{
					float totalDamage = GlobalManager.GameSettings.csObj.characterProperties[Card.cardNumber-1].damage + (Card.level * GlobalManager.GameSettings.csObj.characterProperties[Card.cardNumber-1].damageIncreament);
					cardName.text = totalDamage.ToString();
				}
				else if(cardSortType == GlobalManager.CardSortType.COST)
				{
					cardName.text = csObj.characterProperties[(Card.cardNumber-1)].unitCost.ToString();
				}
				else if(cardSortType == GlobalManager.CardSortType.HP)
				{
					cardName.text = csObj.characterProperties[(Card.cardNumber-1)].maxHitPoint.ToString();
				}
				else if(cardSortType == GlobalManager.CardSortType.LEVEL)
				{
					cardName.text = Card.level.ToString();
				}
				else if(cardSortType == GlobalManager.CardSortType.NAME)
				{
					cardName.text = csObj.characterProperties[(Card.cardNumber-1)].characterName;
				}
			}
		}

		get{ return cardSortType; }
	}
}
