using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class Tab1_Page5_script : SubPageHandler {

	public UIButton readyButton;
	public UILabel statusMessage;
	
	// Use this for initialization
	void Start () {
		readyButton.transform.Find("Label").GetComponent<UILabel>().text = PhotonNetwork.isMasterClient == true ? "START GAME!" : "READY!";
		UIEventListener.Get(readyButton.gameObject).onClick += ClickedReady;

		if(PhotonNetwork.isMasterClient)
		{
			statusMessage.text = "Waiting for player...";
			readyButton.isEnabled = false;
		}
		else
		{
			statusMessage.text = "Please tap on READY button when you are READY!";
			readyButton.isEnabled = true;
		}
	}

	public void PageLeaveRoom()
	{
		base.parent.OpenSubPage(4);
	}

	public override void OpenPopup(bool open = true)
	{
		if(popupObj != null)
		{
			base.popupObj.SetActive(open);
			//base.popupOpened = open;
			
			if(open)
			{
				UIButton[] gos = popupObj.transform.GetComponentsInChildren<UIButton>();
				
				foreach(UIButton obj in gos)
				{
					//UIEventListener.Get(obj.gameObject).onClick += parent.PageButtonClickHandler;
					UIEventListener.Get(obj.gameObject).onClick += PopupButtonHandler;
				}
			}
			else
			{
				UIButton[] gos = popupObj.transform.GetComponentsInChildren<UIButton>();
				
				foreach(UIButton obj in gos)
				{
					//UIEventListener.Get(obj.gameObject).onClick += parent.PageButtonClickHandler;
					UIEventListener.Get(obj.gameObject).onClick -= PopupButtonHandler;
				}
			}
		}
	}

	private void PopupButtonHandler(GameObject go)
	{
		if(go.name == "Yes Button")
		{
			//PageLeaveRoom();
			this.GetComponent<GameRoomHandler>().LeaveRoom();
		}
		else
		{
			OpenPopup(false);
		}
	}

	private void ClickedReady(GameObject go)
	{
		if(this.GetComponent<GameRoomHandler>().StartGameStatus){
			//GUI.Label(new Rect(100,100 * GlobalManager.ratioMultiplierY,150,150), someString + playerRole);
			
			if(PhotonNetwork.isMasterClient)
			{
				if(this.GetComponent<GameRoomHandler>().ClientReadyStatus)
				{
					Debug.Log("Start Game!");
					this.GetComponent<GameRoomHandler>().SendStartGame();
					//StartGame();
				}else{
					Debug.Log("Client NOT READY!");
					statusMessage.text = "Player 2 NOT READY!";
				}
			}else{
				//someString = "Please press READY to start game.";
				statusMessage.text = "Waiting for Game Host to Start the game.";
				readyButton.isEnabled = false;
				this.GetComponent<GameRoomHandler>().SendClientReady();
			}
		}
	}

	public void ClientPressedReady()
	{
		statusMessage.text = "Player 2 is READY! Start game now!";
		readyButton.isEnabled = true;
	}

	public void PlayerJoined()
	{
		statusMessage.text = "Player 2 joined room. Please wait awhile for them to get ready for battle!";
	}
}
