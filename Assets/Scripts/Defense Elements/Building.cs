using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : Character {

	private GameObject globalController;

	public int level = 1;
	private int priority;
	private string type = "sub";
	private GlobalManager.Player owner = GlobalManager.Player.One;
	
	// Use this for initialization
	void Start () {
		globalController = GameObject.Find("Global Controller");

		UnitType = CharacterProperties.UnitType.BUILDING;
		MovementState = CharacterProperties.CharacterState.IDLE;
		//HitPoint = 2000;

		base.Start ();

		if(Team == CharacterProperties.Team.LEFT)
		{
			base.hitPointProgressBar = GameObject.Find("Blue Healthbar").GetComponent<UIProgressBar>();
		}
		else
		{
			base.hitPointProgressBar = GameObject.Find("Red Healthbar").GetComponent<UIProgressBar>();
		}
	}

	public override void ApplyDamage (DamageApplyClass obj)
	{
		base.ApplyDamage (obj);

		if(base.CurrentHitPoint <= 0)
		{
			if(UnitType == CharacterProperties.UnitType.BUILDING){
				//globalController.GetComponent<DefenseMainHandler>().Status = "gameover";
				
				GlobalManager.Player winteam = GlobalManager.playerNumber == GlobalManager.Player.One ? GlobalManager.Player.Two : GlobalManager.Player.One;
				globalController.GetComponent<PuzzleHandler>().GameOver(winteam);
			}
		}
	}

	public IEnumerator DamageTint(){
		//UISprite spriteObj = this.gameObject.GetComponent<UISprite>();
		/*UISprite spriteObj = this.gameObject.GetComponentInChildren<UISprite>();
		spriteObj.color = Color.red;*/
		
		yield return new WaitForSeconds(0.2f);
		
		//spriteObj.color = Color.white;
	}
	
	public int Level{
		set {level = value;}
		get {return level;}
	}
	
	public GlobalManager.Player Parent{
		set {owner = value;}
		get {return owner;}
	}
	
	public string Type{
		set {type = value;}
		get {return type;}
	}
	
	public GameObject GlobalController{
		set {globalController = value;}
		get {return globalController;}
	}
	
	public int Priority{
		set { priority = value; }
		get { return priority; }
	}
}
