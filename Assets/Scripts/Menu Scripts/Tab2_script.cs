using UnityEngine;
using System.Collections;

public class Tab2_script : TabPageHandler {

	public override void PageBackHandler(GameObject go)
	{
		if(base.currentOpenedPageNum == 4)
		{
			if(base.pageSelected == 5)
			{
				base.OpenSubPage(5);
			}
			else
			{
				base.OpenSubPage(currentOpenedPageNum - 1);
			}
		}
		else if(base.currentOpenedPageNum == 5) // if its in enhance selection page
		{
			base.OpenSubPage(1);
		}
		else if(base.currentOpenedPageNum == 8) // if its in sell card page
		{
			base.OpenSubPage(1);
		}
		else
		{
			base.OpenSubPage(currentOpenedPageNum - 1);
		}
	}
}
