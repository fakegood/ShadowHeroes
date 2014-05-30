using UnityEngine;
using System.Collections;

public class Tab1_Page1_script : SubPageHandler {

	// Use this for initialization
	void Start () {
		base.parent.SetSubTitle("Select play mode");
		UIButton[] gos = this.transform.GetComponentsInChildren<UIButton>();
		
		foreach(UIButton obj in gos)
		{
			UIEventListener.Get(obj.gameObject).onClick += PageButtonClickHandler;
		}

		//base.StartSubPage();
	}

	private void PageButtonClickHandler(GameObject go)
	{
		if(go.name == "First Button")
		{
			base.parent.OpenSubPage(2);
		}
		else
		{
			//Application.LoadLevel("LobbyScene");
			base.parent.OpenSubPage(4);
		}
	}
}
