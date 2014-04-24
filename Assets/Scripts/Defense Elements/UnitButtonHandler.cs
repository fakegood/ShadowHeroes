using UnityEngine;
using System.Collections;

public class UnitButtonHandler : MonoBehaviour {
	
	public ButtonMainHandler buttonHandlerObj;
	//public int[] cost = new int[6]{-1, -1, -1, -1, -1, -1};
	public UIProgressBar costGauge;
	public int cost = 0;
	public int referenceNumber = 0;
	public int buttonNumber = -1;
	public int currentAmount = 0;
	public float coolDownTime = 1.5f;
	public bool canEnable = true;
	public Vector3 ownScale = Vector3.zero;
	public CharacterProperties.UnitType unitType;
	public int unitLevel;
	public CharacterProperties.SkillType skillType;
	public int skillLevel;
	public CharacterProperties.CategoryColour element;
	public int settingsReferer = -1;

	[HideInInspector]
	public Vector2 originalSize;

	/*public int rarity = 1;
	public CharacterProperties.UnitType unitType;
	public int unitLevel;
	public CharacterProperties.SkillType skillType;
	public int skillLevel;
	public CharacterProperties.CategoryColour element;
    public int cost;*/

	// Use this for initialization
	void Start () {
		if(ownScale == Vector3.zero){
			ownScale = this.gameObject.transform.localScale;
		}

		originalSize = this.transform.FindChild("Dim").GetComponent<UISprite>().localSize;
	}
	
	public void ActivateButtonCoolDown(){
		StartCoroutine(CoolDownButton());
	}
	
	private IEnumerator CoolDownButton(){
		canEnable = false;
		this.gameObject.collider.enabled = false;
		
		//iTween.ValueTo(this.gameObject, iTween.Hash("from", 120, "to", 0, "time", (coolDownTime - 0.1f), "onupdate", "UpdateFillerSize", "onupdatetarget", this.gameObject));
		iTween.ValueTo(this.gameObject, iTween.Hash("from", originalSize.y, "to", 0, "time", (coolDownTime - 0.1f), "onupdate", "UpdateFillerSize", "onupdatetarget", this.gameObject));
		
		yield return new WaitForSeconds(coolDownTime);
		
		canEnable = true;
		buttonHandlerObj.UpdateElementText();
	}
	
	private void UpdateFillerSize(float tempValue){
		//this.gameObject.transform.FindChild("Filler").GetComponent<tk2dTiledSprite>().dimensions = new Vector2(120, tempValue);
		this.gameObject.transform.FindChild("Dim").GetComponent<UISprite>().SetDimensions((int)originalSize.x, (int)tempValue);
	}

	public void DisableSelf()
	{
		costGauge.gameObject.SetActive(false);
	}

	public float GaugePercentage
	{
		set{ costGauge.value = value; }
		get{ return costGauge.value; }
	}
}
