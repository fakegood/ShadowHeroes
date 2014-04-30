using UnityEngine;
using System.Collections;

public class EnhancePopupHandler : MonoBehaviour {

	public GameObject card;
	public CharacterCard cardObj;

	void OnEnable()
	{
		if(cardObj == null) return;

		card.GetComponent<AddCardButtonHandler>().Card = cardObj;
	}

	public void PopupHandler()
	{
		this.gameObject.SetActive(false);
	}
}
