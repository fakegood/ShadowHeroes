using UnityEngine;
using System.Collections;

public class MainBackgroundHandler : MonoBehaviour {

	public UIRoot root;
	private float headerHeight = 92;
	private float tabButtonHeight = 100;

	// Use this for initialization
	void Start () {
		Vector2 dimension = this.GetComponent<UISprite>().localSize; 
		this.GetComponent<UISprite>().SetDimensions((int)dimension.x, (int)(root.manualHeight - (headerHeight + tabButtonHeight)));
	}
}
