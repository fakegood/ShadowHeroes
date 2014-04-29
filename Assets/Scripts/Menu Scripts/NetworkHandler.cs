using UnityEngine;
using System.Collections;

public class NetworkHandler : MonoBehaviour {

	public delegate void CallbackDelegateHandler(string result);
	public CallbackDelegateHandler ResultDelegate;
	public delegate void ErrorDelegateHandler(string result);
	public ErrorDelegateHandler ErrorDelegate;
	public static NetworkHandler self;

	void Awake()
	{
		self = this;
	}

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(this.gameObject);
	}
	
	// To start a coroutine to request for a web URL / API
	public void ServerRequest(string tempRequest, WWWForm post)
	{
		StartCoroutine(DoCall(tempRequest, post));
	}
	
	// To process the request from coroutine
	private IEnumerator DoCall(string tempRequest, WWWForm post)
	{
		//Debug.Log("NetworkManager call: " + tempRequest);
		//WWWForm form = new WWWForm(); //here you create a new form connection
		//form.AddField("train", uid);
		//form.AddField("request", tempRequest);
		//form.AddField("Content-Type", "application/json");

		WWW w = new WWW(tempRequest, post);
		yield return w;
		
		if (w.error != null) {
			Debug.Log("Error: " + w.error);
			if(ErrorDelegate != null) ErrorDelegate(w.text);
		} else {
			Debug.Log("Result: " + w.text);
			if(ResultDelegate != null) ResultDelegate(w.text);
			
			//var N = JSONNode.Parse(w.text);
			//RecordLog(N["coin"]);
			
			w.Dispose();
		}
	}
}
