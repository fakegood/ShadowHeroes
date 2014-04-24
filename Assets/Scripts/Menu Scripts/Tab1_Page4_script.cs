using UnityEngine;
using System.Collections;

public class Tab1_Page4_script : SubPageHandler {

	public GameObject roomObj;
	public UIScrollView scrollView;
	public UIButton createRoom;
	private string connectionString = "";
	private string gameScene = "GameRoomScene";
	private Vector2 dimension;
	private float gap = 3f;
	
	public virtual void Start()
	{
		//Screen.sleepTimeout = SleepTimeout.SystemSetting;
		//Screen.SetResolution(480, 800, true);

		dimension = roomObj.GetComponent<UISprite>().localSize;

		UIEventListener.Get(createRoom.gameObject).onClick = CreateRoomButtonHandler;

		if(!PhotonNetwork.connected){
			base.parent.tabParent.OpenMainLoader(true);

			createRoom.isEnabled = false;

			PhotonNetwork.NetworkStatisticsEnabled = true;
			PhotonNetwork.autoJoinLobby = true;    // we join randomly. always. no need to join a lobby to get the list of rooms.
			PhotonNetwork.ConnectUsingSettings("1");
		}
		else
		{
			createRoom.isEnabled = true;
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
		base.parent.tabParent.OpenMainLoader(false);

		Debug.LogError("Cause: " + cause);
		connectionString += "\nCause: " + cause;
	}
	
	public virtual void OnJoinedRoom()
	{
		base.parent.tabParent.OpenMainLoader(false);

		Debug.Log("OnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage");
		connectionString += "\nOnJoinedRoom() called by PUN. Now this client is in a room. From here on, your game would be running. For reference, all callbacks are listed in enum: PhotonNetworkingMessage";
		
		//Application.LoadLevel(gameScene);
		base.parent.OpenSubPage(5);
	}
	
	public virtual void OnJoinedLobby()
	{
		base.parent.tabParent.OpenMainLoader(false);

		Debug.Log("OnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().");
		connectionString += "\nOnJoinedLobby(). Use a GUI to show existing rooms available in PhotonNetwork.GetRoomList().";
		
		PhotonNetwork.playerName = "Player"+Random.Range(10, 99);

		createRoom.isEnabled = true;
	}
	
	public virtual void OnDisconnectFromPhoton(){
		/*if(Application.loadedLevelName != "MainMenu"){
			Application.LoadLevel("MainMenu");
		}*/
		Debug.Log("DISCONNECT FROM PHOTON1");
		parent.OpenSubPage(1);
	}
	
	private void CreateRoom(){
		base.parent.tabParent.OpenMainLoader(true);
		PhotonNetwork.CreateRoom("Room " + (PhotonNetwork.GetRoomList().Length + 1 + Random.Range(1, 99)), true, true, 2);
	}
	
	public void OnCreatedRoom()
	{
		base.parent.tabParent.OpenMainLoader(false);
		Debug.Log("OnCreatedRoom");
		//PhotonNetwork.LoadLevel(gameScene);
		base.parent.OpenSubPage(5);
	}

	private void CreateRoomButtonHandler(GameObject go)
	{
		if(PhotonNetwork.connected)
		{
			CreateRoom();	
		}
	}

	private void RoomButtonHandler(GameObject go)
	{
		base.parent.tabParent.OpenMainLoader(true);
		PhotonNetwork.JoinRoom(go.transform.Find("Room Name").GetComponent<UILabel>().text);
	}

	private void PopulateRoomList()
	{
		UIButton[] children = scrollView.transform.GetComponentsInChildren<UIButton>();

		foreach(UIButton child in children)
		{
			Destroy(child.gameObject);
		}

		if(PhotonNetwork.connected)
		{
			Debug.Log(PhotonNetwork.GetRoomList().Length);
			if(PhotonNetwork.GetRoomList().Length >= 0)
			{
				int i = 0;
				foreach(RoomInfo roomInfo in PhotonNetwork.GetRoomList())
				{
					Vector3 pos = Vector3.zero;
					GameObject holder = Instantiate(roomObj, Vector3.zero, Quaternion.identity) as GameObject;
					holder.transform.parent = scrollView.transform;
					pos.y = ((i * dimension.y) + (dimension.y / 2) + gap) * -1;
					holder.transform.localPosition = pos;
					holder.transform.localScale = holder.transform.lossyScale;
					holder.transform.Find("Room Name").GetComponent<UILabel>().text = roomInfo.name;
					holder.transform.Find("Room Allocation").GetComponent<UILabel>().text = roomInfo.playerCount + "/" + roomInfo.maxPlayers;
					UIEventListener.Get(holder).onClick += RoomButtonHandler;
					i++;
				}

				scrollView.ResetPosition();
			}
			else
			{
				// no room available
				Debug.Log("no room");
			}
		}
	}

	public virtual void OnReceivedRoomListUpdate()
	{
		PopulateRoomList();
	}
}
