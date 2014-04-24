using UnityEngine;
using System.Collections;

public class Tab1_script : TabPageHandler {

	public override void PageBackHandler(GameObject go)
	{
		if(base.currentOpenedPageNum == 4) // if its in multiplayer lobby
		{
			if(PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}

			base.OpenSubPage(1);
		}
		else if(base.currentOpenedPageNum == 5)
		{
			base.ActivePage.GetComponent<Tab1_Page5_script>().OpenPopup(true);
		}
		else
		{
			base.OpenSubPage(currentOpenedPageNum - 1);
		}
	}

	public virtual void OnDisconnectFromPhoton()
	{
		Debug.Log("DISCONNECT FROM PHOTON");
		base.OpenSubPage(1);
	}
}
