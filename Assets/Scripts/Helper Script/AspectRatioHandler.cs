using UnityEngine;
using System.Collections;

public class AspectRatioHandler : MonoBehaviour {

	void Awake()
	{
		if(GameObject.Find("Network Controller") == null)
		{
			Application.LoadLevel("LoadingScene");
		}

		float portraitAspectRatio = (float)Screen.width/(float)Screen.height;
		float landscapeAspectRatio = (float)Screen.height/(float)Screen.width;
		float aspectRatio = Mathf.Round(portraitAspectRatio * 100f) / 100f;
		
		if(aspectRatio == 0.56f) // 9:16 ratio [640:1136 / 720:1280 / 1080:1920]
		{
			this.GetComponent<UIRoot>().manualHeight = 1136;
			//this.GetComponent<UIRoot>().manualHeight = 1920;
		}
		else if(aspectRatio == 0.62f) // 5:8 ratio [640:1024 / 800:1280]
		{
			this.GetComponent<UIRoot>().manualHeight = 1280;
		}
		else if(aspectRatio == 0.67f) // 2:3 ratio [640:960]
		{
			this.GetComponent<UIRoot>().manualHeight = 960;
		}
		else if(aspectRatio == 0.75f) // 3:4 ratio [960:1280]
		{
			this.GetComponent<UIRoot>().manualHeight = 1024;
		}
	}
}
