using UnityEngine;
using System.Collections;

public class StageUIButtonScript : MonoBehaviour {

	private string name;
	public UILabel label;

	public string StageName
	{
		set{ name = value; label.text = name; }
		get{ return name; }
	}
}
