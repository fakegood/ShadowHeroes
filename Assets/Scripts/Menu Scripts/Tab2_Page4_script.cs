using UnityEngine;
using System.Collections;

public class Tab2_Page4_script : SubPageHandler {
	
	public UILabel someText;

	// Use this for initialization
	void Start () {
		if(csObj == null) return;

		someText.text = base.csObj.characterProperties[base.parent.currentSelectedDeckNum-1].cardName;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
