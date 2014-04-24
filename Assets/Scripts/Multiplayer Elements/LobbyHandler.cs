using UnityEngine;
using System.Collections;

public class LobbyHandler : MonoBehaviour {
	
	private string connectionString = "";
	private string gameScene = "GameRoomScene";
	
    public virtual void Start()
    {
		Screen.sleepTimeout = SleepTimeout.SystemSetting;
		//Screen.SetResolution(480, 800, true);
		
		if(!PhotonNetwork.connected){
			PhotonNetwork.NetworkStatisticsEnabled = true;
	        PhotonNetwork.autoJoinLobby = true;    // we join randomly. always. no need to join a lobby to get the list of rooms.
			PhotonNetwork.ConnectUsingSettings("1");
		}
    }

    // to react to events "connected" and (expected) error "failed to join random room", we implement some methods. PhotonNetworkingMessage lists all available methods!

    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
		connectionString += "\nOnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();";
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        Debug.Log("OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, true, true, 2);");
		connectionString += "\nOnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one. Calling: PhotonNetwork.CreateRoom(null, true, true, 2);";
        PhotonNetwork.CreateRoom(null, true, true, 2);
    }

    // the following methods are implemented to give you some context. re-implement them as needed.

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
		connectionString += "\nCause: " + cause;
    }

    public virtual void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		connectionString += "\nOnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage";
		
		Application.LoadLevel(gameScene);
    }

    public virtual void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
		connectionString += "\nOnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().";
		
		PhotonNetwork.playerName = "Player"+Random.Range(10, 99);
    }
	
	public virtual void OnDisconnectFromPhoton(){
		if(Application.loadedLevelName != "MainMenu"){
			Application.LoadLevel("MainMenu");
		}
	}
	
	private void CreateRoom(){
		PhotonNetwork.CreateRoom("Room " + (PhotonNetwork.GetRoomList().Length + 1 + Random.Range(1, 99)), true, true, 2);
	}
	
	public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        PhotonNetwork.LoadLevel(gameScene);
    }
	
	private void OnGUI(){
		//GUI.Label(new Rect(10,10,480,500), connectionString);
		if(PhotonNetwork.connected){
			if(PhotonNetwork.insideLobby){
				GUI.Label(new Rect(10,10,150,30), "Lobby");
				
				if(GUI.Button(new Rect(10,40, 150 * GlobalManager.ratioMultiplierX, 50 * GlobalManager.ratioMultiplierY), "Create Room")){
					CreateRoom();
				}
				
				GUILayout.Space(100 * GlobalManager.ratioMultiplierY);
				
				if(PhotonNetwork.GetRoomList().Length != 0){
					GUILayout.BeginHorizontal();
					foreach(RoomInfo roomInfo in PhotonNetwork.GetRoomList()){
		                GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
		                if (GUILayout.Button("Join", GUILayout.Width(100 * GlobalManager.ratioMultiplierX), GUILayout.Height(50 * GlobalManager.ratioMultiplierX)))
		                {
							Debug.Log("join room: " + roomInfo.name);
		                    PhotonNetwork.JoinRoom(roomInfo.name);
		                }
					}
					GUILayout.EndHorizontal();
				}else{
					GUI.Label(new Rect(10,100 * GlobalManager.ratioMultiplierY,150,30), "No available rooms");
				}
			}
		}
	}
}
