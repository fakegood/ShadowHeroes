using UnityEngine;
using System.Collections;

public class ScrollViewHandler : MonoBehaviour {

	private UISprite backgroundImage;
	private float backgroundImageHeaderHeight = 90f;
	private float extraOffset = 12f;

	// Use this for initialization
	void Start () {
		backgroundImage = GameObject.FindGameObjectWithTag("MainBackground").GetComponent<UISprite>();
		Debug.Log(backgroundImage.localSize.y);
		Vector2 size = new Vector2(backgroundImage.localSize.x, (backgroundImage.localSize.y - (backgroundImageHeaderHeight + extraOffset)));
		Vector2 position = new Vector2(0, -39);

		this.GetComponent<UIPanel>().baseClipRegion = new Vector4(position.x, position.y, size.x, size.y);
	}
}
