using UnityEngine;
using System.Collections;

public class MainPopupScript : MonoBehaviour {

	public enum PopupType
	{
		INFORMATION,
		ERROR,
		CONFIRMATION
	}

	public delegate void CallbackDelegateHandler(bool result);
	public CallbackDelegateHandler ResultDelegate;
	public UILabel titleLabel;
	public UILabel informationLabel;
	public UIButton yesButton;
	public UIButton noButton;
	public UIButton okayButton;
	private PopupType popupType;

	void OnEnable()
	{
		switch(popupType)
		{
		case PopupType.INFORMATION:
			yesButton.gameObject.SetActive(false);
			noButton.gameObject.SetActive(false);
			okayButton.gameObject.SetActive(true);
			break;
		case PopupType.ERROR:
			yesButton.gameObject.SetActive(false);
			noButton.gameObject.SetActive(false);
			okayButton.gameObject.SetActive(true);
			break;
		case PopupType.CONFIRMATION:
			yesButton.gameObject.SetActive(true);
			noButton.gameObject.SetActive(true);
			okayButton.gameObject.SetActive(false);
			break;
		default:
			Debug.LogWarning("Popup Type not set.");
			this.gameObject.SetActive(false);
			break;
		}
	}

	public void YesHandler()
	{
		if(ResultDelegate != null) ResultDelegate(true);
		this.gameObject.SetActive(false);
	}

	public void NoHandler()
	{
		if(ResultDelegate != null) ResultDelegate(false);
		this.gameObject.SetActive(false);
	}

	public void OkayHandler()
	{
		if(ResultDelegate != null) ResultDelegate(true);
		this.gameObject.SetActive(false);
	}

#region SETTER & GETTER FUNCTIONS
	public string Title
	{
		set{ titleLabel.text = value; }
		private get{ return titleLabel.text; }
	}

	public string Information
	{
		set{ informationLabel.text = value; }
		private get{ return informationLabel.text; }
	}

	public PopupType CurrentPopupType
	{
		set{ popupType = value; }
		get{ return popupType; }
	}
#endregion
}
