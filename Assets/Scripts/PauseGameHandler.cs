using UnityEngine;
using System.Collections;

public class PauseGameHandler : MonoBehaviour {
	
	public PuzzleHandler parent;
	
	// Use this for initialization
	void Start () {
	
	}
	
	public void ResumeGame(){
		if(parent != null){
			parent.ResumeGame();
		}
	}
	
	public void ExitGame(){
		GlobalManager.paused = false;
		Time.timeScale = 1.0f;
		
		if(GlobalManager.multiplyerGame){
			GameObject.Find("Global Controller").GetComponent<GameNetworkHandler>().NetworkMessage(GlobalManager.NetworkMessage.QuitGame);
		}else{
			Application.LoadLevel("LandingMenu");
		}
	}
}
