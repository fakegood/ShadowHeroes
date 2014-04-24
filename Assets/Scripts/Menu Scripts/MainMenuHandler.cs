using UnityEngine;
using System.Collections;

public class MainMenuHandler : MonoBehaviour {
	
	//public GameObject mainPanel;
	
	// Use this for initialization
	void Start () {
		//InitCharacterDetails();
	}
	
	private void InitCharacterDetails(){
		/*if(GlobalManager.characterDetails == null){
			TextAsset txt = (TextAsset)Resources.Load("Character_Details", typeof(TextAsset));
			
			if(txt){
				GlobalManager.characterDetails = GlobalManager.SplitCsvGrid(txt.text);
			}else{
				Debug.Log("Error loading text file.");
			}
		}*/
		
		if(GlobalManager.skillDetails == null){
			TextAsset txt = (TextAsset)Resources.Load("Skill_Details", typeof(TextAsset));
			
			if(txt){
				//GlobalManager.skillDetails = GlobalManager.SplitCsvGrid(txt.text);
			}else{
				Debug.Log("Error loading text file.");
			}
		}
	}
	
	void OnClick(){
		GlobalManager.bossGame = GlobalManager.cherryGame = false;
		GlobalManager.specialCombination = true;
		GlobalManager.multiplyerGame = false;
		Application.LoadLevel("PlayScene");
	}
	
	void OnSecondClick(){
		GlobalManager.bossGame = GlobalManager.cherryGame = false;
		GlobalManager.specialCombination = false;
		GlobalManager.multiplyerGame = false;
		Application.LoadLevel("PlayScene");
	}
	
	void OnBossClick(){
		/*GlobalManager.bossGame = GlobalManager.cherryGame = true;
		GlobalManager.specialCombination = true;
		GlobalManager.multiplyerGame = false;*/
		Application.LoadLevel("EditScene");
	}
	
	void OnMultiplayerClick(){
		//GlobalManager.multiplyerGame = true;
		//GlobalManager.bossGame = GlobalManager.cherryGame = true;
		//GlobalManager.specialCombination = true;
		Application.LoadLevel("LobbyScene");
	}
}
