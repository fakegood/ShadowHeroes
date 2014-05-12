using UnityEngine;
using System.Collections;

public class Tab6_Page1_script : SubPageHandler {

	public UIButton soundButton;
	public UIButton vibrateButton;
	public UIButton moreGamesButton;
	public UIButton rateUsButton;

	// Use this for initialization
	void Start () {
	
	}

	public void SoundClick()
	{

	}

	public void VibrateClick()
	{
		
	}

	public void MoreGamesClick()
	{
		Application.OpenURL("https://www.google.com");
	}

	public void RateUsClick()
	{
		Application.OpenURL("https://www.naver.com");
	}
}
