using UnityEngine;
using System.Collections;

public class PhoneButtonHandler : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Application.platform == RuntimePlatform.Android){
			if(Input.GetKey(KeyCode.Escape)){
				if(Application.loadedLevelName != "LandingMenu"){
					if(GlobalManager.multiplyerGame){
						PhotonNetwork.Disconnect();
						GlobalManager.multiplyerGame = false;
					}else{
						Application.LoadLevel("LandingMenu");
					}
				}else{
					Application.Quit();
				}
			}
		}
	}
}
