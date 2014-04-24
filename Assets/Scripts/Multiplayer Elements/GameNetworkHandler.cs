using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]

public class GameNetworkHandler : Photon.MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		PhotonView view = this.gameObject.GetComponent<PhotonView>();
		view.observed = this.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void NetworkMessage(GlobalManager.NetworkMessage msg, object[] obj = null){
		if(msg == GlobalManager.NetworkMessage.SpawnUnit)
		{
			photonView.RPC("SpawnUnit", PhotonTargets.All, obj[0], obj[1], obj[2], obj[3]);
		}
		else if(msg == GlobalManager.NetworkMessage.SpawnSkill)
		{
			//photonView.RPC("SpawnSkill", PhotonTargets.Others, obj[0]);
		}
		else if(msg == GlobalManager.NetworkMessage.PauseGame)
		{
			photonView.RPC("PauseGame", PhotonTargets.Others);
		}
		else if(msg == GlobalManager.NetworkMessage.ResumeGame)
		{
			photonView.RPC("ResumeGame", PhotonTargets.Others);
		}
		else if(msg == GlobalManager.NetworkMessage.QuitGame)
		{
			//photonView.RPC("ResumeGame", PhotonTargets.Others);
			PhotonNetwork.LeaveRoom();
		}
	}
	
	[RPC]
	public void SpawnUnit(int[] damageDeck, int tempAmt, int tempLevel, int tempType, PhotonMessageInfo info){
		CharacterProperties.Team teamToSpawn;

		if(info.sender.ID == PhotonNetwork.player.ID)
		{
			teamToSpawn = GlobalManager.playerNumber == GlobalManager.Player.One ? CharacterProperties.Team.LEFT : CharacterProperties.Team.RIGHT;
		}
		else
		{
			teamToSpawn = GlobalManager.playerNumber == GlobalManager.Player.One ? CharacterProperties.Team.RIGHT : CharacterProperties.Team.LEFT;
		}

		//int[] bonusDamageDeck = new int[6]{0,0,0,0,0,0};

		this.gameObject.GetComponent<PuzzleHandler>().defObj.SpawnCharacter(damageDeck, tempAmt, tempLevel, (CharacterProperties.UnitType)tempType, teamToSpawn);
	}
	
	[RPC]
	public void SkillSpawned(int tempAmt){
		//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(tempAmt, GlobalManager.playerNumber);
	}
	
	[RPC]
	public void SpawnSkill(int tempAmt){
		GlobalManager.Player enemy = GlobalManager.playerNumber == GlobalManager.Player.One ? GlobalManager.Player.Two : GlobalManager.Player.One;
		//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(tempAmt, enemy);
	}
	
	[RPC]
	public void PauseGame(){
		//if(!photonView.isMine)
			this.gameObject.GetComponent<PuzzleHandler>().PauseGame();
	}
	
	[RPC]
	public void ResumeGame(){
		//if(!photonView.isMine)
			this.gameObject.GetComponent<PuzzleHandler>().ResumeGame();
	}
	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
		
		//CheckGameStatus();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
		
		PhotonNetwork.LeaveRoom();
    }
	
	public virtual void OnJoinedLobby()
    {
        Application.LoadLevel("LandingMenu");
    }
}
