using UnityEngine;
using System.Collections;

public class DamageIndicator : MonoBehaviour {

	public bool criticalHit = false;
	[System.NonSerialized]
	public float lobDistance = -0.06f;
	private Vector3 punchSize = new Vector3(1.2f,1.2f,1.2f);
	private float lobHeight = 0.1f;
	private float lobTime = 1.0f;
	private float r = 0f;
	private float g = 0f;
	private float b = 0f;
	
	// Use this for initialization
	void Start () {
		if(criticalHit)
		{
			//this.gameObject.GetComponent<tk2dTextMesh>().color = new Color32(255,100,0,255);
			this.gameObject.GetComponent<UILabel>().color = new Color32(255,100,0,255);
		}
		else
		{
			if(float.Parse(this.gameObject.GetComponent<UILabel>().text) > 0f)
			{
				this.gameObject.GetComponent<UILabel>().color = Color.blue;
			}
		}

		r = this.gameObject.GetComponent<UILabel>().color.r;
		g = this.gameObject.GetComponent<UILabel>().color.g;
		b = this.gameObject.GetComponent<UILabel>().color.b;

		if(criticalHit)
		{
			punchSize = Vector3.one * 2f;
			iTween.PunchScale(this.gameObject, iTween.Hash("amount", punchSize, "time", 0.5f, "easeType", iTween.EaseType.easeInOutExpo));
			iTween.MoveBy(this.gameObject, iTween.Hash("y", lobHeight, "time", lobTime, "space", Space.Self, "easeType", iTween.EaseType.linear));
			iTween.ValueTo(this.gameObject, iTween.Hash("from", 1f, "to", 0f, "delay", 0.3f, "time", lobTime-0.5f, "space", Space.Self, "onupdatetarget", this.gameObject, "onupdate", "UpdateSelf"));
		}
		else
		{
			iTween.PunchScale(this.gameObject, iTween.Hash("amount", punchSize, "time", 0.5f, "easeType", iTween.EaseType.easeInOutExpo));
			iTween.MoveBy(this.gameObject, iTween.Hash("y", lobHeight, "time", lobTime/2, "easeType", iTween.EaseType.easeOutQuad));
			iTween.MoveBy(this.gameObject, iTween.Hash("y", -lobHeight, "time", lobTime/2, "delay", lobTime/2, "easeType", iTween.EaseType.easeInQuad));
			iTween.MoveAdd(this.gameObject, iTween.Hash("x", lobDistance, "time", lobTime, "delay", 0.2f, "easeType", iTween.EaseType.linear));
			iTween.ValueTo(this.gameObject, iTween.Hash("from", 1f, "to", 0f, "delay", 0.3f, "time", lobTime-0.5f, "onupdatetarget", this.gameObject, "onupdate", "UpdateSelf"));
		}
	}
	
	private void UpdateSelf(float tempValue){
		if(tempValue > 0){
			this.gameObject.GetComponent<UILabel>().color = new Color(r, g, b, tempValue);
		}else{
			Destroy(this.gameObject);
		}
	}
}
