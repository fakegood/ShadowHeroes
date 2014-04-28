using UnityEngine;
using System.Collections;

public class Tab2_script : TabPageHandler {

	public override void PageBackHandler(GameObject go)
	{
		if(base.currentOpenedPageNum == 5) // if its in multiplayer lobby
		{
			base.OpenSubPage(1);
		}
		else
		{
			base.OpenSubPage(currentOpenedPageNum - 1);
		}
	}
}
