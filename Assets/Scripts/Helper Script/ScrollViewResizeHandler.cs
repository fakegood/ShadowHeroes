using UnityEngine;
using System.Collections;

public class ScrollViewResizeHandler : MonoBehaviour {
	
	private UISprite backgroundImage;
	private float backgroundImageHeaderHeight = 90f;
	private float extraOffset = 12f;
	private float manualOffset = 200f;
	
	// Use this for initialization
	void Start () {
		backgroundImage = GameObject.FindGameObjectWithTag("MainBackground").GetComponent<UISprite>();
		float finalHeight = (backgroundImage.localSize.y - (backgroundImageHeaderHeight + extraOffset + manualOffset));
		Vector2 size = new Vector2(backgroundImage.localSize.x, finalHeight);
		Vector2 position = new Vector2(0, 0);
		
		this.GetComponent<UIPanel>().baseClipRegion = new Vector4(position.x, position.y, size.x, size.y);
	}
}
