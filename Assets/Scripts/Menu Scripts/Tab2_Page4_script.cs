using UnityEngine;
using System.Collections;

public class Tab2_Page4_script : SubPageHandler {
	
	public UILabel someText;
	private int selectedIndex = -1;

	// Use this for initialization
	void Start () {
		if(csObj == null) return;

		selectedIndex = base.parent.currentSelectedDeckNum-1;
		someText.text = base.csObj.characterProperties[selectedIndex].cardName;
	}
}
