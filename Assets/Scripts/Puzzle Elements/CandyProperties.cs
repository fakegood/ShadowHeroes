using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CandyProperties : MonoBehaviour {
	public bool activate = false;
	public bool garbage = false;
	public string waiting = "none";
	
	public string type = "normal";
	public int objxpos = 0;
	public int objypos = 0;
	
	public AudioSource flyTrailSource;
	
	private float shrinkSpeed = 0f;
	
	private GameObject mainObj;
	private GameObject trail;
	//private DefenseMainHandler defenseObj;
	public GameObject bombTimerObj;
	public int bombTimerCount = 10;
	
	private int row = 0;
	private int col = 0;
	private int gap = 0;
	//private float size = 20.0f;
	private float offsetX = 0;
	private float offsetY = 0;
	//private float totalWidth = 0;
	//private float totalHeight = 0;
	private string currentSpriteName = "";
	private string currentSpriteColour = "";
	private GameObject destroySelfEffect = null;
	
	// Use this for initialization
	void Start () {
		mainObj = GameObject.Find("Global Controller");
		trail = Resources.Load("Assets/CandyFlyEffect") as GameObject;
		destroySelfEffect = Resources.Load("Assets/PuzzleDestroyEffect") as GameObject;
		currentSpriteColour = this.gameObject.GetComponent<UISprite>().spriteName;
		//defenseObj = mainObj.GetComponent<DefenseMainHandler>();
		
		row = GlobalManager.row;
		col = GlobalManager.col;
		gap = GlobalManager.gap;
		//size = GlobalManager.size;
		offsetX = GlobalManager.offsetX;
		offsetY = GlobalManager.offsetY;
		//totalWidth = mainObj.GetComponent<PuzzleHandler>().totalWidth;
		//totalHeight = mainObj.GetComponent<PuzzleHandler>().totalHeight;
		
		if(GlobalManager.initCheckDone){
			shrinkSpeed = GlobalManager.oriShrinkSpeed;
		}else{
			shrinkSpeed = GlobalManager.shrinkSpeed;
		}
	}
	
	public void Activate(UISprite[][] tempMainArray, float delay, bool isDoubleCombo = false, bool tempAutoCheck = false){
		if(!activate){
			activate = true;
			
			int xlar = (int)this.transform.localPosition.x;
			int ylar = (int)this.transform.localPosition.y;
			
			if(!isDoubleCombo){
				string[] finalSplitStringArray = this.gameObject.GetComponent<UISprite>().spriteName.Split(new char[] {' '});
				
				if(finalSplitStringArray.Length == 1){
					if(finalSplitStringArray[0] != "cherry" && finalSplitStringArray[0] != "cherry2"){
						//mainObj.GetComponent<PuzzleHandler>().AddScore(50);
						ActivateDestroySequence(xlar, ylar, delay);
						//mainObj.GetComponent<PuzzleHandler>().BanColor(GetColorNumber(this.gameObject.GetComponent<UISprite>().spriteName));
					}
				}else{
					if(finalSplitStringArray[1] == "stripe"){
						//mainObj.GetComponent<PuzzleHandler>().AddScore(100);
						ActivateDestroySequence(xlar, ylar, delay);

						int finalx = Mathf.RoundToInt(GetArrayPosition(xlar, ylar).x);
						int finaly = Mathf.RoundToInt(GetArrayPosition(xlar, ylar).y);
						if(finalSplitStringArray[2] == "vert"){
							for(int j=0; j < row; j++){
								PreActivateCandy(tempMainArray[j][finalx], tempMainArray, 0f, true);
							}
						}else{
							for(int j=0; j < col; j++){
								PreActivateCandy(tempMainArray[finaly][j], tempMainArray, 0f, true);
							}
						}
						
						//mainObj.GetComponent<PuzzleHandler>().BanColor(GetColorNumber(finalSplitStringArray[0].ToString()));
						
						//Vector3 scale = new Vector3((175/5)*GlobalManager.ratioMultiplierX, (75/5)*GlobalManager.ratioMultiplierY, 1);
						//defenseObj.CreateFireball("blue", "fireball", scale, 1.5f, 60.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
						//defenseObj.CreateFireball("blue", "fireball", scale, 1.5f, 20.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
						//defenseObj.CreateFireball("blue", "fireball", scale, 1.5f, 20.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
						
						if(GlobalManager.initCheckDone)
							mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.Stripe);
						
						//if(GlobalManager.initCheckDone)
							//defenseObj.UpdateCandyPoint(500);
					}else if(finalSplitStringArray[1] == "packet"){
						int blastRadius = 3;
						int totalLoop = blastRadius * blastRadius;
						
						int finalX = Mathf.RoundToInt(GetArrayPosition(xlar, ylar).x);
						int finalY = Mathf.RoundToInt(GetArrayPosition(xlar, ylar).y);

						int startingX = finalX - 1; // minus one to move the calculation one column to the left
						int startingY = finalY - 1; // minus one to move the calculation one row below
						int tempStartingX = startingX;
						int tempStartingY = startingY;
						int addRadius = startingX + blastRadius;
						
						tempStartingX = startingX;
						tempStartingY = startingY;
						
						//mainObj.GetComponent<PuzzleHandler>().AddScore(50);
						
						for(int temp = 0; temp < totalLoop; temp++){
							if((startingX >= 0 && startingX < col) && (startingY >= 0 && startingY < row)){
								if(startingX != finalX || startingY != finalY){ // WEIRD COMPILE BUG? SUPPOSED TO BE && INSTEAD OF ||
									PreActivateCandy(tempMainArray[startingY][startingX], tempMainArray, delay, true);
								}else{
									tempMainArray[startingY][startingX].GetComponent<CandyProperties>().waiting = "packet";
									iTween.ScaleBy(this.gameObject, iTween.Hash("amount", new Vector3(0.8f, 0.8f, 0), "time", 0.2f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.linear));
								}
							}
							
							startingX ++;
							if(startingX >= addRadius){
								if(startingX > col){
									startingX = finalX;
								}
								
								startingX = tempStartingX;
								startingY++;
								if(startingY >= row){
									startingY = tempStartingY;
								}
							}
						}
						
						//mainObj.GetComponent<PuzzleHandler>().BanColor(GetColorNumber(finalSplitStringArray[0].ToString()));
						
						//Vector3 scale = new Vector3((100/3)*GlobalManager.ratioMultiplierX, (100/3)*GlobalManager.ratioMultiplierY, 1);
						//defenseObj.CreateBomb("blue", "bomb", scale, 1.5f, 60.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
						//defenseObj.CreateBomb("blue", "bomb", scale, 1.5f, 20.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
						//defenseObj.CreateBomb("blue", "bomb", scale, 1.5f, 20.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
						
						if(GlobalManager.initCheckDone){
							mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.Bomb);
							
							CreateSpecialEffects("bomb_anim1", "bomb_anim", 1);
						}
						
						//if(GlobalManager.initCheckDone)
							//defenseObj.UpdateCandyPoint(500);
					}else if(finalSplitStringArray[1] == "chocolate"){
						if(tempAutoCheck){
							int randNum = UnityEngine.Random.Range(1, 6) + 1;
							string colour = "red";
							/*bool canProceed = false;
							
							List<int> colorArray = mainObj.GetComponent<PuzzleHandler>().banColorArray;
							
							while(!canProceed){
								randNum = UnityEngine.Random.Range(1, 6) + 1;
								
								if(colorArray.Count <= 0){
									canProceed = true;
									break;
								}else{
									foreach(int tempRandnum in colorArray){
										if(randNum == tempRandnum){
											canProceed = false;
										}else{
											canProceed = true;
											break;
										}
									}
								}
							}
							
							print("direct choco " + randNum + " -- " + colorArray.Count);*/
							
							if(randNum == 1){
								colour = "red";
							}else if(randNum == 2){
								colour = "blue";
							}else if(randNum == 3){
								colour = "blue2";
							}else if(randNum == 4){
								colour = "green";
							}else if(randNum == 5){
								colour = "yellow";
							}else if(randNum == 6){
								colour = "shit";
							}
							
							//int finalX = (int)GetArrayPosition(xlar, ylar).x;
							//int finalY = (int)GetArrayPosition(xlar, ylar).y;
							
							//mainObj.GetComponent<PuzzleHandler>().AddScore(200);
							ActivateDestroySequence(xlar, ylar, delay);
							
							for(int j=0; j< row; j++){
								for(int k=0; k< col; k++){
									string[] tempSplitArray = tempMainArray[j][k].name.Split(new char[] {' '});
									
									if(tempSplitArray[0] == colour){
										PreActivateCandy(tempMainArray[j][k], tempMainArray, 0f, true);
									}
								}
							}
						}else{
							//mainObj.GetComponent<PuzzleHandler>().AddScore(200);
							ActivateDestroySequence(xlar, ylar, delay);
						}
						
						//if(GlobalManager.initCheckDone)
							//mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.Chocolate);
						
						//if(GlobalManager.initCheckDone)
							//defenseObj.UpdateCandyPoint(1000);
					}else if(finalSplitStringArray[1] == "bomb"){
						// timer bomb
						//mainObj.GetComponent<PuzzleHandler>().AddScore(500);
						ActivateDestroySequence(xlar, ylar, delay);
						
						//mainObj.GetComponent<PuzzleHandler>().BanColor(GetColorNumber(finalSplitStringArray[0].ToString()));
					}
				}
			}else{
				//mainObj.GetComponent<PuzzleHandler>().AddScore(50);
				ActivateDestroySequence(xlar, ylar, delay);
			}
		}
	}
	
	public void ActivateWaitingCandy(int tempXpos, int tempYpos, string type, UISprite[][] tempMainArray){
		int blastRadius = 3;
		int totalLoop = blastRadius * blastRadius;
		
		int finalX = tempXpos;
		int finalY = tempYpos;
		
		int startingX;
		int startingY;
		int tempStartingX;
		int tempStartingY;
		int addRadius;
		
		switch(type){
		case "stripe":
			int xlar = (int)this.transform.localPosition.x;
			int ylar = (int)this.transform.localPosition.y;
			
			//mainObj.GetComponent<PuzzleHandler>().AddScore(100);
			
			string[] finalSplitStringArray = this.gameObject.GetComponent<UISprite>().spriteName.Split(new char[] {' '});
			
			if(finalSplitStringArray[2] == "vert"){
				for(int j=0; j < row; j++){
					PreActivateCandy(tempMainArray[j][Mathf.RoundToInt(GetArrayPosition(xlar, ylar).x)], tempMainArray, 0f, true);
				}
			}else{
				for(int j=0; j < col; j++){
					PreActivateCandy(tempMainArray[Mathf.RoundToInt(GetArrayPosition(xlar, ylar).y)][j], tempMainArray, 0f, true);
				}
			}
			
			if(GlobalManager.initCheckDone)
				mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.Stripe);
			
			//if(GlobalManager.initCheckDone)
				//defenseObj.UpdateCandyPoint(500);
			break;
		case "packet":
			blastRadius = 3;
			totalLoop = blastRadius * blastRadius;
			
			finalX = tempXpos;
			finalY = tempYpos;

			startingX = finalX - 1; // minus one to move the calculation one column to the left
			startingY = finalY - 1; // minus one to move the calculation one row below
			tempStartingX = startingX;
			tempStartingY = startingY;
			addRadius = startingX + blastRadius;
			
			tempStartingX = startingX;
			tempStartingY = startingY;
			
			//mainObj.GetComponent<PuzzleHandler>().AddScore(50);
			
			for(int temp = 0; temp < totalLoop; temp++){
				
				if((startingX >= 0 && startingX < col) && (startingY >= 0 && startingY < row)){
					if(startingX != finalX || startingY != finalY){ // WEIRD COMPILE BUG? SUPPOSED TO BE && INSTEAD OF ||
						PreActivateCandy(tempMainArray[startingY][startingX], tempMainArray, 0f, true);
					}else{
						ActivateDestroySequence(startingX, startingY, 0f);
					}
				}
				
				startingX ++;
				if(startingX >= addRadius){
					if(startingX > col){
						startingX = finalX;
					}
					
					startingX = tempStartingX;
					startingY++;
					if(startingY >= row){
						startingY = tempStartingY;
					}
				}
			}
			
			//Vector3 scale = new Vector3((100/3)*GlobalManager.ratioMultiplierX, (100/3)*GlobalManager.ratioMultiplierY, 1);
			//defenseObj.CreateBomb("blue", "bomb", scale, 1.5f, 20.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
			
			//if(GlobalManager.initCheckDone)
				//mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.Bomb);
			
			//if(GlobalManager.initCheckDone)
				//defenseObj.UpdateCandyPoint(500);
			
			if(GlobalManager.initCheckDone){
				CreateSpecialEffects("bomb_anim1", "bomb_anim", 1);
			}
			break;
		case "double packet":
			blastRadius = 5;
			totalLoop = blastRadius * blastRadius;
			
			finalX = tempXpos;
			finalY = tempYpos;
			
			startingX = finalX - 2;
			startingY = finalY - 2;
			tempStartingX = startingX;
			tempStartingY = startingY;
			addRadius = startingX + blastRadius;
			
			tempStartingX = startingX;
			tempStartingY = startingY;
			
			//mainObj.GetComponent<PuzzleHandler>().AddScore(100);
			
			for(int temp = 0; temp < totalLoop; temp++){
				
				if((startingX >= 0 && startingX < col) && (startingY >= 0 && startingY < row)){
					if(startingX != finalX || startingY != finalY){ // WEIRD COMPILE BUG? SUPPOSED TO BE && INSTEAD OF ||
						PreActivateCandy(tempMainArray[startingY][startingX], tempMainArray, 0f, true);
					}else{
						ActivateDestroySequence(startingX, startingY, 0f);
					}
				}
				
				startingX ++;
				if(startingX >= addRadius){
					if(startingX > col){
						startingX = finalX;
					}
					
					startingX = tempStartingX;
					startingY++;
					if(startingY >= row){
						startingY = tempStartingY;
					}
				}
			}
			
			if(GlobalManager.initCheckDone)
				mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.BombAndBomb);
			break;
		case "colour packet":
			//int randNum = UnityEngine.Random.Range(1, 6) + 1;
			string colour = "red";
			/*bool canProceed = false;
			
			List<int> colorArray = mainObj.GetComponent<PuzzleHandler>().banColorArray;
			
			while(!canProceed){
				randNum = UnityEngine.Random.Range(1, 6) + 1;
				
				if(colorArray.Count <= 0){
					canProceed = true;
					break;
				}else{
					foreach(int tempRandnum in colorArray){
						if(randNum == tempRandnum){
							canProceed = false;
						}else{
							canProceed = true;
							break;
						}
					}
				}
			}
			
			print("waiting choco " + randNum + " -- " + colorArray.Count);*/
			
			//mainObj.GetComponent<PuzzleHandler>().AddScore(300);
			ActivateDestroySequence(tempXpos, tempYpos, 0f);
			
			for(int i=0; i< row; i++){
				for(int j=0; j< col; j++){
					string[] tempSplitArray = tempMainArray[i][j].name.Split(new char[] {' '});
					
					if(tempSplitArray[0] == colour){
						PreActivateCandy(tempMainArray[i][j], tempMainArray, 0f, true);
					}
				}
			}
			
			////if(GlobalManager.initCheckDone)
				//mainObj.GetComponent<PuzzleHandler>().ExecuteSpecialMove(GlobalManager.CombinationType.ChocolateAndBomb);
			
			//if(GlobalManager.initCheckDone)
				//defenseObj.UpdateCandyPoint(3000);
			break;
		default:
			
			break;
		}
	}
	
	public void ActivateDestroySequence(int tempxpos, int tempypos, float delay = 0f){
		Hashtable param = new Hashtable();
		param.Add("scale", new Vector3(0,0,0));
		param.Add("time", shrinkSpeed);
		param.Add("delay", delay);
		param.Add("easetype", iTween.EaseType.linear);
		param.Add("oncompletetarget", this.gameObject);
		param.Add("oncomplete", "DestroyCandy");
		param.Add("onstarttarget", GameObject.Find("Global Controller"));
		param.Add("onstart", "AnimationStart");
		
		Hashtable param2 = new Hashtable();
		param2.Add("xpos", Mathf.RoundToInt(GetArrayPosition(tempxpos, tempypos).x));
		param2.Add("ypos", Mathf.RoundToInt(GetArrayPosition(tempxpos, tempypos).y));
		//param2.Add("xpos", objxpos);
		//param2.Add("ypos", objypos);
		param2.Add("target", this.gameObject);
		
		param.Add("oncompleteparams", param2);
		
		currentSpriteName = this.gameObject.GetComponent<UISprite>().spriteName;
		//GlobalManager.totalCandyOnScreen[(GetColorArrayPosition(GetCandyType(currentSpriteName))-1)] --;
		
		if(GlobalManager.initCheckDone){
			if(GlobalManager.createTrail){
				float finalTrailXPos = 0;
				float finalTrailScaleSize = 0.36f;
				float finalTrailAnimationTime = 0.8f;
				
				if(currentSpriteName != "cherry" && currentSpriteName != "cherry2"){
					switch (currentSpriteColour){
					case "red":
						//finalTrailXPos = 15;
						finalTrailXPos = 35;
						break;
					case "blue":
						//finalTrailXPos = 95;
						finalTrailXPos = 95;
						break;
					case "green":
						//finalTrailXPos = 175;
						finalTrailXPos = 155;
						break;
					case "yellow":
						//finalTrailXPos = 255;
						finalTrailXPos = 215;
						break;
					case "purple":
						//finalTrailXPos = 336;
						finalTrailXPos = 265;
						break;
					case "white":
						//finalTrailXPos = 412;
						finalTrailXPos = 335;
						break;
					default:
						finalTrailXPos = 0;
						break;
					}
					
					if(finalTrailXPos != 0){
						//iTween.PunchScale(GlobalManager.elementTextArray[(GetColorNumber(currentSpriteColour)-1)].gameObject, iTween.Hash("amount", new Vector3(150,150,1), "delay", finalTrailAnimationTime - 0.2f, "time", finalTrailAnimationTime, "easeType", iTween.EaseType.easeInOutExpo, "onstarttarget", mainObj, "onstart", "IncreaseElement", "onstartparams", currentSpriteName));
						mainObj.SendMessage("IncreaseElement", currentSpriteName);
						// make candy fly with trail
						//trail = Instantiate(trail, new Vector3(tempxpos, tempypos, -2), Quaternion.identity) as GameObject;
						//trail.GetComponent<UISprite>().SetSprite(currentSpriteName);
						//float xrange = Random.Range(-200,200);
						//iTween.MoveBy(trail, iTween.Hash("x", xrange, "time", finalTrailAnimationTime - 0.2f, "easeType", iTween.EaseType.easeOutQuad));
						//iTween.MoveAdd(trail, iTween.Hash("x", finalTrailXPos - this.gameObject.transform.position.x - xrange, "y", 517 - this.gameObject.transform.position.y, "time", finalTrailAnimationTime, "easeType", iTween.EaseType.easeInOutExpo));
						//iTween.ScaleTo(trail, iTween.Hash("scale", new Vector3(finalTrailScaleSize, finalTrailScaleSize, finalTrailScaleSize), "time", finalTrailAnimationTime, "easeType", iTween.EaseType.easeInOutExpo));
					}
				}
			}

			if(destroySelfEffect != null)
			{
				Instantiate(destroySelfEffect, this.gameObject.transform.position, Quaternion.identity);
			}
			
			//CreateSpecialEffects("remove1", "remove_candy_anim");
			
			string[] tempName = currentSpriteName.Split(new char[]{' '});
			if(tempName.Length > 1){
				if(tempName[1] == "stripe"){
					//param.Add("delay", 1f);
					if(tempName[2] == "vert"){
						CreateSpecialEffects("stripe_anim1", "stripe_anim", 0, new Vector3(1,1,1), new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0), 90);
					}else{
						CreateSpecialEffects("stripe_anim1", "stripe_anim", 0, new Vector3(1,1,1), new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0));
					}
				}
			}
		}

		iTween.ScaleTo(this.gameObject, param);

		/*if(bombTimerObj != null){
			bombTimerObj.GetComponent<FollowTarget>().target = null;
			bombTimerObj.GetComponent<FollowTarget>().ActivateDestroySequence();
			bombTimerObj = null;
		}*/
	}
	
	private void DestroyCandy(Hashtable hashObj){
		/*if(GlobalManager.jellyGame){
			CheckObstacle((int)GetArrayPosition(xlar, ylar).x, (int)GetArrayPosition(xlar, ylar).y);
		}*/
		
		//NGUITools.Destroy((Object)hashObj["target"]);
		
		Destroy((Object)hashObj["target"]);
	}
	
	private void PreActivateCandy(UISprite tempGarbage, UISprite[][] tempMainArray, float delay, bool tempAutoCheck = false, bool tempIsDoubleCombo = false){
		if(tempGarbage != null){
			if(!tempGarbage.GetComponent<CandyProperties>().activate && tempGarbage.GetComponent<CandyProperties>().waiting == "none"){
				//removeCandySource.Play();
				tempGarbage.GetComponent<CandyProperties>().Activate(tempMainArray, delay, tempIsDoubleCombo, tempAutoCheck);
				
				/*if(GlobalManager.jellyGame){
					CheckObstacle((int)GetArrayPosition((int)tempGarbage.transform.position.x, (int)tempGarbage.transform.position.y).x, (int)GetArrayPosition((int)tempGarbage.transform.position.x, (int)tempGarbage.transform.position.y).y);
				}*/
			}
		}
	}
	
	private void CreateSpecialEffects(string firstFrameName, string animationName, int depth = 0, Vector3 tempScale = default(Vector3), Vector3 tempPos = default(Vector3), float rotate = 0f){
		GameObject go = new GameObject("Remove Candy Effect");
		go.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, (30 + depth));
		go.transform.position = tempPos == Vector3.zero ? new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, (30 + depth)) : new Vector3(tempPos.x, tempPos.y, (30 + depth));
		go.transform.parent = this.gameObject.transform.parent;
		/*UISprite sprite = tk2dBaseSprite.AddComponent<UISprite>(go, GlobalManager.mainSpriteCollectionData, firstFrameName);
		sprite.scale = tempScale == Vector3.zero ? new Vector3(0.6f, 0.6f, 0.6f) : tempScale;
		if(rotate != 0f){
			go.transform.Rotate(0, 0, rotate);
		}*/
		
		//UISpriteAnimator animator = go.AddComponent<UISpriteAnimator>();
		//animator.Library = GlobalManager.mainSpriteAnimation;
		//animator.Play(animationName);
		
		go.AddComponent<DestroyEffectObject>();
	}
	
	private void CheckObstacle(int tempxpos, int tempypos){
		/*UISprite [][] tempObstacleArray = mainObj.GetComponent<PuzzleHandler>().obstacleArray;
		if(tempObstacleArray[tempypos][tempxpos] != null){
			tempObstacleArray[tempypos][tempxpos].GetComponent<JellyProperties>().DestroySelf();
			mainObj.GetComponent<PuzzleHandler>().MinusObstacle(-1);
		}*/
	}
	
	// return mainArray position
	private Vector2 GetArrayPosition(int tempPosx, int tempPosy){
		//tempPosition.x = (j * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (j * GlobalManager.gap);
		float x = (tempPosx - (GlobalManager.sizeForCalc/2f + GlobalManager.gap/2f)) / (GlobalManager.sizeForCalc + GlobalManager.gap);
		float y = (tempPosy - (GlobalManager.sizeForCalc/2f + GlobalManager.gap/2f)) / (GlobalManager.sizeForCalc + GlobalManager.gap);
		Vector2 finalPos = new Vector2(x, y);

		return finalPos;
	}

	/*private Vector2 GetArrayPosition(int tempPosx, int tempPosy){
		Vector2 finalPos = new Vector2(((tempPosx - offsetX - gap + (GlobalManager.sizeForCalc/2)) / GlobalManager.sizeForCalc), ((tempPosy - offsetY - gap + (GlobalManager.sizeForCalc/2)) / GlobalManager.sizeForCalc));
		//Vector2 finalPos = new Vector2(((tempPosx - (offsetX*GlobalManager.ratioMultiplierX) - (gap*GlobalManager.ratioMultiplierX) + ((size/2)*GlobalManager.ratioMultiplierX)) / (size*GlobalManager.ratioMultiplierX)), ((tempPosy - (offsetY*GlobalManager.ratioMultiplierY) - (gap*GlobalManager.ratioMultiplierY) + ((size/2)*GlobalManager.ratioMultiplierY)) / (size*GlobalManager.ratioMultiplierY)));
		//Vector2 finalPos = new Vector2(((tempPosx - (offsetX*GlobalManager.ratioMultiplierX) - (gap*GlobalManager.ratioMultiplierX) + ((GlobalManager.sizeForCalc/2)*GlobalManager.ratioMultiplierX)) / (GlobalManager.sizeForCalc*GlobalManager.ratioMultiplierX)), ((tempPosy - (offsetY*GlobalManager.ratioMultiplierY) - (gap*GlobalManager.ratioMultiplierY) + ((GlobalManager.sizeForCalc/2)*GlobalManager.ratioMultiplierY)) / (GlobalManager.sizeForCalc*GlobalManager.ratioMultiplierY)));
		finalPos = new Vector2(finalPos.x-1, finalPos.y-1);
		
		return finalPos;
	}
	
	private Vector2 GetArrayPosition(Vector3 tempMousePos){
		//Vector2 finalPos = new Vector2(((tempMousePos.x - offsetX - gap + (size/2)) / size), ((tempMousePos.y - offsetY - gap + (size/2)) / size));
		Vector2 finalPos = new Vector2(((tempMousePos.x - (offsetX*GlobalManager.ratioMultiplierX) - (gap*GlobalManager.ratioMultiplierX) + ((size/2)*GlobalManager.ratioMultiplierX)) / (size*GlobalManager.ratioMultiplierX)), ((tempMousePos.y - (offsetY*GlobalManager.ratioMultiplierY) - (gap*GlobalManager.ratioMultiplierY) + ((size/2)*GlobalManager.ratioMultiplierY)) / (size*GlobalManager.ratioMultiplierY)));
		
		return finalPos;
	}*/
	
	private int GetColorNumber(string colour){
		int num = 0;
		
		if(colour == "red"){
			num = 1;
		}else if(colour == "blue"){
			num = 2;
		}else if(colour == "green"){
			num = 3;
		}else if(colour == "yellow"){
			num = 4;
		}else if(colour == "purple"){
			num = 5;
		}else if(colour == "white"){
			num = 6;
		}
		
		return num;
	}
	
	private int GetColorArrayPosition(string colour){
		int num = 0;
		
		if(colour == "red"){
			num = 1;
		}else if(colour == "blue"){
			num = 2;
		}else if(colour == "white"){
			num = 3;
		}else if(colour == "green"){
			num = 4;
		}else if(colour == "yellow"){
			num = 5;
		}else if(colour == "purple"){
			num = 6;
		}
		
		return num;
	}
	
	private string GetCandyType(string tempCandyName, int checkType = 0){
		switch(tempCandyName){
			case "red": case "green": case "blue": case "white": case "purple": case "yellow":
				if(checkType == 0){
					return tempCandyName;
				}else{
					return "false";
				}
				break;
			case "red stripe vert": case "red stripe hort": case "green stripe vert": case "green stripe hort": case "blue stripe vert": case "blue stripe hort":
			case "white stripe vert": case "white stripe hort": case "purple stripe vert": case "purple stripe hort": case "yellow stripe vert": case "yellow stripe hort":
			case "red packet": case "green packet": case "blue packet": case "white packet": case "purple packet": case "yellow packet":
				string[] candyName = tempCandyName.Split(new char[] {' '});
				
				if(checkType == 0){
					return candyName[0].ToString();
				}else{
					return "true";
				}
				break;
			case "red bomb": case "green bomb": case "blue bomb": case "blue2 bomb": case "shit bomb": case "yellow bomb":
				string[] candyName2 = tempCandyName.Split(new char[] {' '});
				
				if(checkType == 0){
					return candyName2[0].ToString();
				}else{
					return "false";
				}
				break;
			case "colourful chocolate":
				if(checkType == 0){
					return tempCandyName;
				}else{
					return "true";
				}
				break;
			default:
				if(checkType == 0){
					return tempCandyName;
				}else{
					return "false";
				}
				break;
		}
	}
	
	public float ShrinkSpeed{
		set {shrinkSpeed = value;}
		get {return shrinkSpeed;}
	}
}
