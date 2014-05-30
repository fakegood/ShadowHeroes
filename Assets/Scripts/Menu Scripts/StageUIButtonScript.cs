using UnityEngine;
using System.Collections;

public class StageUIButtonScript : MonoBehaviour {

	private string name;
	private int apCost = -1;
	public UILabel label;
	public UILabel apLabel;

	public string StageName
	{
		set{ name = value; label.text = name; }
		get{ return name; }
	}

	public int APCost
	{
		set
		{
			apCost = value;
			if(apCost < 1)
			{
				apLabel.text = "";
			}
			else
			{
				apLabel.text = "AP: " + apCost.ToString();
			}
		}
		get{ return apCost; }
	}
}
