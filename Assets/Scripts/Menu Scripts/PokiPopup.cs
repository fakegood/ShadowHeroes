using UnityEngine;
using System.Collections;

public class PokiPopup : MonoBehaviour {

	public CharacterCard[] cardObj;
	public GameObject twirl;
	public GameObject shine1;
	public GameObject shine2;
	public GameObject brightStuff;
	public UILabel clickLabel;
	public GameObject card;
	private int cardCounter = 0;
	private bool showed = false;

	// Use this for initialization
	void Start () {

	}

	void OnEnable()
	{
		if(cardObj.Length > 0)
		{
			StartAnimation();
		}
	}

	void OnDisable()
	{
		showed = false;
		cardCounter = 0;
		cardObj = null;
	}

	public void NextCard()
	{
		if(showed)
		{
			if(cardCounter <= (cardObj.Length-1))
			{
				StartAnimation();
			}
			else
			{
				this.gameObject.SetActive(false);
			}
		}
	}

	private void StartAnimation()
	{
		showed = false;
		twirl.SetActive(true);
		twirl.transform.localScale = Vector3.one;
		shine1.SetActive(true);
		shine2.SetActive(true);
		brightStuff.SetActive(true);
		iTween.StopByName("clickLabel");
		clickLabel.gameObject.SetActive(false);
		card.SetActive(false);
		shine1.GetComponent<UISprite>().alpha = shine2.GetComponent<UISprite>().alpha = brightStuff.GetComponent<UISprite>().alpha = 0f;

		iTween.RotateAdd(twirl, iTween.Hash("z", -720, "time", 3f, "easetype", iTween.EaseType.linear));
		iTween.ScaleAdd(twirl, iTween.Hash("amount", new Vector3(-0.9f,-0.9f,-0.9f), "time", 1f, "delay", 2f, "easetype", iTween.EaseType.linear));
		iTween.ValueTo(shine1, iTween.Hash("from", 0f, "to", 1f, "time", 0.7f, "delay", 3f, "easetype", iTween.EaseType.linear, "onupdatetarget", this.gameObject, "onupdate", "UpdateShine1"));
		iTween.ValueTo(shine2, iTween.Hash("from", 0f, "to", 1f, "time", 0.7f, "delay", 3.5f, "easetype", iTween.EaseType.linear, "onupdatetarget", this.gameObject, "onupdate", "UpdateShine2"));
		iTween.ValueTo(brightStuff, iTween.Hash("from", 0f, "to", 1f, "time", 0.7f, "delay", 4f, "easetype", iTween.EaseType.linear, "onupdatetarget", this.gameObject, "onupdate", "UpdateBrightStuff"));
		iTween.ValueTo(brightStuff, iTween.Hash("from", 1f, "to", 0f, "time", 0.7f, "delay", 4.5f, "easetype", iTween.EaseType.linear, "onupdatetarget", this.gameObject, "onupdate", "UpdateBrightStuff"));
	}

	void UpdateShine1(float value)
	{
		shine1.GetComponent<UISprite>().alpha = value;
	}

	void UpdateShine2(float value)
	{
		shine2.GetComponent<UISprite>().alpha = value;
	}

	void UpdateBrightStuff(float value)
	{
		if(value >= 0.95f && !showed)
		{
			showed = true;
			twirl.SetActive(false);
			shine1.SetActive(false);
			shine2.SetActive(false);
			clickLabel.gameObject.SetActive(true);
			card.SetActive(true);

			iTween.ValueTo(this.gameObject, iTween.Hash("name", "clickLabel", "from", 0f, "to", 1f, "time", 0.5f, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.pingPong, "onupdatetarget", this.gameObject, "onupdate", "UpdateClickLabel"));

			card.GetComponent<UICardScript>().Card = cardObj[cardCounter];
			cardCounter ++;
		}
		brightStuff.GetComponent<UISprite>().alpha = value;
	}

	void UpdateClickLabel(float value)
	{
		clickLabel.alpha = value;
	}
}
