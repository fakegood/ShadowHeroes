using UnityEngine;
using System.Collections;

public class GameRoomHandler : Photon.MonoBehaviour {
	
	private bool startGame = false;
	private string playerRole = "Noob";
	private bool isClientReady = false;
	
	// Use this for initialization
	void Start () {
		//Screen.sleepTimeout = SleepTimeout.SystemSetting;
		photonView.viewID = 1;

		CheckGameStatus();
	}
	
	public void LeaveRoom(){
		PhotonNetwork.LeaveRoom();
	}
	
	public void OnLeftRoom()
    {
		isClientReady = false;
        // back to main menu
        //Application.LoadLevel("LobbyScene");
		this.GetComponent<Tab1_Page5_script>().PageLeaveRoom();
    }

	public void SendClientReady()
	{
		photonView.RPC("ClientReady", PhotonTargets.Others);
	}

	public void SendStartGame()
	{
		photonView.RPC("StartGame", PhotonTargets.All);
	}
	
	[RPC]
	public void ClientReady(){
		Debug.Log("Client Ready Function Called!");
		SendMessage("ClientPressedReady", SendMessageOptions.DontRequireReceiver);
		isClientReady = true;
	}
	
	[RPC]
	public void StartGame(){
		GlobalManager.multiplyerGame = true;
		//GlobalManager.bossGame = GlobalManager.cherryGame = true;
		GlobalManager.specialCombination = true;
		Application.LoadLevel("PlayScene");
	}
	
	public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room.");
		SendMessage("PlayerJoined", SendMessageOptions.DontRequireReceiver);
		CheckGameStatus();
    }
	
	private void CheckGameStatus(){
		if(PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers){
			startGame = true;
		}else{
			startGame = false;
		}
		
		if(PhotonNetwork.isMasterClient){
			playerRole = "Master";
		}else{
			playerRole = "Slave";
		}
	}
	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
		
		CheckGameStatus();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
		isClientReady = false;
		CheckGameStatus();
    }

	public bool ClientReadyStatus
	{
		get{ return isClientReady; }
	}

	public bool StartGameStatus
	{
		get{ return startGame; }
	}
}
