using UnityEngine;
using System.Collections;

public class WarningHandler : MonoBehaviour {
	//0.002083333
	private float size = 0.002583333f;
	private float originalSize = 0.002083333f;
	private float duration = 0.3f;
	
	void OnEnable(){
		InvokeRepeating("Animate", 1f, 1.5f);
	}
	
	void OnDisable(){
		CancelInvoke();
	}
	
	private void Animate(){
		iTween.ScaleTo(this.gameObject, iTween.Hash("x", size, "y", size, "time", duration, "easetype", iTween.EaseType.easeInOutQuart));
		iTween.ScaleTo(this.gameObject, iTween.Hash("x", originalSize, "y", originalSize, "time", duration, "delay", duration, "easetype", iTween.EaseType.easeInOutQuart));
		//iTween.ScaleAdd(this.gameObject, iTween.Hash("x", size, "y", size, "time", duration, "easetype", iTween.EaseType.easeInOutQuart));
		//iTween.ScaleAdd(this.gameObject, iTween.Hash("x", -size, "y", -size, "time", duration, "delay", duration, "easetype", iTween.EaseType.easeInOutQuart));
	}
}
