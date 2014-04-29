using UnityEngine;
using System.Collections;

public class PuzzleHandler : MonoBehaviour {
	
	//public UISpriteCollectionData spriteCollectionData;
	//public UISpriteAnimation spriteAnimation;
	public UIRoot root;
	public UIAtlas mainAtlas;
	public GameObject loadingScreen;
	public GameObject puzzleParent;
	public DefenseHandler defObj;
	public ButtonMainHandler buttonHandlerObj;
	public GameObject PopupPrefab;
	public GameObject PausePrefab;
	public GameObject characterSettingsPrefab;

	public GameObject introBackground;
	public GameObject introLabel;
	public GameObject introBlock;

	public AudioClip mainBgm;
	public AudioClip removeCandySfx;
	public AudioClip dropCandySfx;
	public AudioClip flyTrailSfx;

	private float switchSpeed = 0.2f;
	private float dropSpeed = 0.8f;
	//private float shrinkSpeed = 0.2f;
	
	private int row = 7;
	private int col = 8;
	private int gap = 0;
	private float size = 60f;
	private float scale;
	private float offsetX = 0f;
	private float offsetY = 0f;
	
	private UISprite[][] mainArray;
	private UISprite tempSpriteHolder;
	private int[] elementsNeededArray;
	
	private int xpos = 0;
	private	int ypos = 0;
	private int diffx = 0;
	private int diffy = 0;
	private int orix = 0;
	private int oriy = 0;
	
	private bool disablePuzzle = false;
	private bool canTouch = false;
	private bool toFillCandy = false;
	private bool secondCheck = true;
	private bool swipeEnabled = false;
	private bool initCheckDone = false;
	private bool waitingActivated = true;
	private bool checkGameStatus = true;
	
	private int mouseX = 0;
	private int mouseY = 0;
	private int xpostocheck = -1;
	private int ypostocheck = -1;
	private float currentMouseX = 0.0f;
	private float currentMouseY = 0.0f;
	private float swipeAmountThreshold = 15.0f;
	private string targetSpriteName;
	private string secondSpriteName;
	
	private int[] emptyslot;
	private int[] tempEmptySlot;
	private int[] totalEmpty;
	private bool newCandyDropping = false;
	private int animationCounter = 0;
	
	private float timerCounter;
	private float delay = 30.0f;
	private float hintCounter;
	private float hintDelay = 5.0f;
	private string hintString = "";

	//private AudioSource mainBgmSource;
	private AudioSource removeCandySource;
	private AudioSource dropCandySource;
	private AudioSource flyTrailSource;
	
	void Awake(){
//#if UNITY_EDITOR

//#else
		/*if(GlobalManager.characterDetails == null){
			Application.LoadLevel("LandingMenu");
		}else{
			AddAudio(mainBgm, true, true, 0.5f);
			removeCandySource = AddAudio(removeCandySfx, false, false, 0.8f);
			dropCandySource = AddAudio(dropCandySfx, false, false, 0.8f);
			flyTrailSource = AddAudio(flyTrailSfx, false, false, 0.8f);
		}*/
		AddAudio(mainBgm, true, true, 0.5f);
		removeCandySource = AddAudio(removeCandySfx, false, false, 0.8f);
		dropCandySource = AddAudio(dropCandySfx, false, false, 0.8f);
		flyTrailSource = AddAudio(flyTrailSfx, false, false, 0.8f);
//#endif
	}

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		GlobalManager.gameover = false;
		GlobalManager.initCheckDone = initCheckDone = false;
		
		row = GlobalManager.row;
		col = GlobalManager.col;
		
		mainArray = new UISprite[row][];
		//GlobalManager.pebbleCounter = new int[6]{0,0,0,0,0,0};
		size = GlobalManager.size;
		offsetX = GlobalManager.offsetX;
		offsetY = GlobalManager.offsetY;
		scale = size/100;
		//shrinkSpeed = GlobalManager.shrinkSpeed;
		dropSpeed = GlobalManager.dropSpeed;
		swipeAmountThreshold *= GlobalManager.ratioMultiplierX;
		
		emptyslot = new int[col];
		tempEmptySlot = new int[col];
		
		//defObj = this.gameObject.GetComponent<DefenseMainHandler>();
		//GlobalManager.mainSpriteCollectionData = spriteCollectionData;
		//GlobalManager.mainSpriteAnimation = spriteAnimation;
		
		//Camera.main.nearClipPlane = 99f;
		//Time.timeScale = 100f;
		
		if(GlobalManager.multiplyerGame){
			defObj.gameObject.GetComponent<AIBehaviorHandler>().enabled = false;
			this.gameObject.AddComponent<GameNetworkHandler>();
			
			if(PhotonNetwork.isMasterClient){
				GlobalManager.playerNumber = GlobalManager.Player.One;
			}else{
				GlobalManager.playerNumber = GlobalManager.Player.Two;
			}
		}else{
			GlobalManager.playerNumber = GlobalManager.Player.One;
		}
		
		StartCoroutine(Init());
	}
	
	private IEnumerator Init(){
		int totalNumberOfElements = 6;
		GlobalManager.elementTextArray = new UILabel[totalNumberOfElements];

		loadingScreen.SetActive(true);
		PuzzleStatus = true;
		CreateLevel();
		
		yield return new WaitForSeconds(0.3f);
		CheckComboCandy();
	}

	private void SpawnStageIntro()
	{
		introBlock.SetActive(true);
		if(!GlobalManager.multiplyerGame)
		{
			introLabel.GetComponent<UILabel>().text = "Stage " + GlobalManager.GameSettings.chosenArea + "-" + GlobalManager.GameSettings.chosenStage;
		}
		else
		{
			introLabel.GetComponent<UILabel>().text = "Battle Start!";
		}

		Vector3 pos = Vector3.zero;
		pos.x = 320;
		pos.y = GameObject.Find("Skill Button Area").transform.localPosition.y + 250f;
		introBackground.transform.localPosition = pos;

		Vector3 pos2 = Vector3.zero;
		pos2.x = 400;
		pos2.y = pos.y;
		introLabel.transform.localPosition = pos2;

		iTween.MoveTo(introBackground, iTween.Hash("x", 0, "time", 0.8f, "islocal", true));
		iTween.MoveTo(introLabel, iTween.Hash("x", 0, "time", 0.8f, "delay", 0.2f, "islocal", true));
		iTween.MoveTo(introBackground, iTween.Hash("x", -640, "time", 0.8f, "delay", 1.7f, "islocal", true, "oncompletetarget", this.gameObject, "oncomplete", "DoneIntroAnimation"));
		iTween.MoveTo(introLabel, iTween.Hash("x", -640, "time", 0.8f, "delay", 1.5f, "islocal", true));
	}
	
	private void InitCheckComplete(){
		//GlobalManager.dropSpeed = GlobalManager.oriDropSpeed;
		//GlobalManager.shrinkSpeed = GlobalManager.oriShrinkSpeed;
		
		//shrinkSpeed = GlobalManager.oriShrinkSpeed;
		dropSpeed = GlobalManager.oriDropSpeed;

		// spawn stage intro
		SpawnStageIntro();
		
		//Camera.main.nearClipPlane = 0.3f;
		Time.timeScale = 1f;
		
		//Vector2 tempPosition;
		
		for(int i = 0; i < row; i++){
			for(int j = 0; j < col; j++){
				/*tempPosition.x = (size/2) + (j * size) + (j * gap);
				tempPosition.y = (size/2) + (i * size) + (i * gap);
				
				UISprite sprite = NGUITools.AddWidget<UISprite>(HUDPanel);
				sprite.atlas = mainAtlas;
				
				sprite.name = "background tile";
				sprite.spriteName = "selector";
				sprite.transform.localScale = new Vector3(size, size, 1);
				sprite.transform.localPosition = new Vector3(tempPosition.x, tempPosition.y, 0);*/
				
				mainArray[i][j].GetComponent<CandyProperties>().ShrinkSpeed = GlobalManager.oriShrinkSpeed;
			}
		}
		
		//startCounting = true;
		timerCounter = delay;
		hintCounter = hintDelay;

		loadingScreen.SetActive(false);
	}

	private void DoneIntroAnimation()
	{
		PuzzleStatus = false;
		introBlock.SetActive(false);
		initCheckDone = true;
		GlobalManager.initCheckDone = true;
	}
	
	private void CreateLevel(){
		Vector2 tempPosition;
		
		for(int i = 0; i < row; i++){
			
			mainArray[i] = new UISprite[col];
			
			for(int j = 0; j < col; j++){
				//string[] collayout = tempMapLayout[((tempMapLayout.Length-1)-i)].ToString().Trim().Split(new char[] {'-'});

				tempPosition.x = (j * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (j * GlobalManager.gap);
				tempPosition.y = (i * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (i * GlobalManager.gap);
				
				mainArray[i][j] = CreateCandy(tempPosition.x, tempPosition.y);
				
				//mainArray[i][j].GetComponent<CandyProperties>().objxpos = j;
				//mainArray[i][j].GetComponent<CandyProperties>().objypos = i;
			}
		}
	}
	
	private UISprite CreateCandy(float tempXpos, float tempYpos, bool spawnBomb = false, bool spawnCherry = false, int type = 0){
		int randNum = type;
		//int bombNum = 0;
		
		if(type == 0){
			randNum = Mathf.RoundToInt(Random.Range(0.0f, 6.0f));
		}
		
		if(randNum >= 1 && randNum <= 6){
			GlobalManager.totalCandyOnScreen[(randNum-1)] ++;
		}
		
		//Debug.Log(GlobalManager.totalCandyOnScreen[0] + " : " + GlobalManager.totalCandyOnScreen[1] + " : " + GlobalManager.totalCandyOnScreen[2] + " : " + GlobalManager.totalCandyOnScreen[3] + " : " + GlobalManager.totalCandyOnScreen[4] + " : " + GlobalManager.totalCandyOnScreen[5]);
		
		string tempSpriteName = "red";
		
		if(randNum == 1){
			tempSpriteName = "red";
		}else if(randNum == 2){
			tempSpriteName = "blue";
		}else if(randNum == 3){
			tempSpriteName = "green";
		}else if(randNum == 4){
			tempSpriteName = "purple";
		}else if(randNum == 5){
			tempSpriteName = "yellow";
		}else if(randNum == 6){
			tempSpriteName = "white";
		}else if(randNum == 7){
			tempSpriteName = "green sprite vert";
		}else if(randNum == 8){
			tempSpriteName = "green packet";
		}else if(randNum == 9){
			tempSpriteName = "colourful chocolate";
		}else if(randNum == 10){
			tempSpriteName = "cherry";
		}else if(randNum == 11){
			tempSpriteName = "cherry2";
		}

		//GameObject spriteGameObj = new GameObject(tempSpriteName);
		//spriteGameObj.transform.position = new Vector3(tempXpos,tempYpos,40);
		//spriteGameObj.transform.parent = puzzleParent.transform;
		//UISprite sprite = tk2dBaseSprite.AddComponent<UISprite>(spriteGameObj, spriteCollectionData, tempSpriteName);
		UISprite sprite = NGUITools.AddSprite(puzzleParent, mainAtlas, tempSpriteName);
		sprite.gameObject.name = tempSpriteName;
		sprite.SetDimensions((int)GlobalManager.sizeForCalc, (int)GlobalManager.sizeForCalc);
		sprite.transform.localPosition = new Vector3(tempXpos,tempYpos,40);
		//sprite.SortingOrder = -2;
		//sprite.transform.localScale = new Vector3(scale,scale,1);
		sprite.gameObject.AddComponent<CandyProperties>().flyTrailSource = flyTrailSource;
		
		return sprite;
	}
	
	// Update is called once per frame
	void Update () {
		/*if(Input.GetMouseButtonDown(0)){
			print(Mathf.Round(GetArrayPosition(Input.mousePosition).x) + " : " + Mathf.Round(GetArrayPosition(Input.mousePosition).y));
		}*/
		
		if(!GlobalManager.gameover){
			if(GlobalManager.initCheckDone && GlobalManager.bossGame){
				timerCounter -= Time.deltaTime;
				if(timerCounter <= 0){
					timerCounter = delay;
					BossModifyPuzzle();
				}
			}
			
			if(GlobalManager.initCheckDone){
				hintCounter -= Time.deltaTime;
				if(hintCounter <= 0){
					hintCounter = hintDelay;
					ShowHint();
				}
			}

			if(Input.GetButton("Fire1") && !swipeEnabled && canTouch && !disablePuzzle){
				//mouseXtoCheck = (int)Input.mousePosition.x;
                                                 				//mouseYtoCheck = (int)Input.mousePosition.y;
				//print (mouseXtoCheck + " : " + mouseYtoCheck + " -- " + (int)GetArrayPosition(mouseXtoCheck, mouseYtoCheck).x + " : " + (int)GetArrayPosition(mouseXtoCheck, mouseYtoCheck).y);

				Vector3 position = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
				position = puzzleParent.transform.InverseTransformPoint(position);
				Vector2 pos2d = GetArrayPosition(position);
				xpostocheck = Mathf.RoundToInt(pos2d.x);
				ypostocheck = Mathf.RoundToInt(pos2d.y);
				
				if((xpostocheck >= 0 && xpostocheck <= (col-1)) && (ypostocheck >= 0 && ypostocheck <= (row-1))){
				//if((mouseXtoCheck >= ((offsetX*GlobalManager.ratioMultiplierX) - (size/2)) && mouseXtoCheck <= totalWidth) && (mouseYtoCheck >= ((offsetY*GlobalManager.ratioMultiplierY) - (size/2)) && mouseYtoCheck <= totalHeight)){
				//if((mouseXtoCheck >= (size/2) && mouseXtoCheck <= totalWidth) && (mouseYtoCheck >= (size/2) && mouseYtoCheck <= totalHeight)){
					if(tempSpriteHolder == null){
						mouseX = (int)position.x;
						mouseY = (int)position.y;

						xpos = Mathf.RoundToInt(pos2d.x);
						ypos = Mathf.RoundToInt(pos2d.y);
						
						//print (mouseX + " : " + mouseY + " -- " + xpos + " : " + ypos);
						
						if(xpos > col - 1 || ypos > row - 1){
							return;
						}
						
						tempSpriteHolder = mainArray[ypos][xpos];
						
						/*tempSpriteSelector = NGUITools.AddWidget<UISprite>(mainPanel);
						tempSpriteSelector.atlas = mainAtlas;
						tempSpriteSelector.name = "selector";
						tempSpriteSelector.spriteName = "selector";
						tempSpriteSelector.transform.localScale = new Vector3(size, size, 1);
						tempSpriteSelector.transform.localPosition = tempSpriteHolder.transform.localPosition;*/
						
						orix = xpos;
						oriy = ypos;
					}else{
						currentMouseX = position.x;
						currentMouseY = position.y;
						
						float horizontalThreshold = currentMouseX - mouseX;
						float verticalThreshold = currentMouseY - mouseY;
						
						if(Mathf.Abs(horizontalThreshold) > swipeAmountThreshold || Mathf.Abs(verticalThreshold) > swipeAmountThreshold){
							swipeEnabled = true;
							int newx = orix;
							int newy = oriy;
							
							if(Mathf.Abs(horizontalThreshold) > swipeAmountThreshold){
								if(horizontalThreshold > 0){
									newx = orix + 1 < col ? orix + 1 : orix;
								}else{
									newx = orix - 1 >= 0 ? orix - 1 : orix;
								}
							}else{
								if(verticalThreshold > 0){
									newy = oriy + 1 < row ? oriy + 1 : oriy;
								}else{
									newy = oriy - 1 >= 0 ? oriy - 1 : oriy;
								}
							}
							
							diffx = orix - newx;
							diffy = oriy - newy;
							
							if((Mathf.Abs(diffx) == 1 || Mathf.Abs(diffy) == 1) && (Mathf.Abs(diffx+diffy) == 1)){
								//canTouch = false;
								xpos = newx;
								ypos = newy;
								//NGUITools.Destroy(tempSpriteSelector);
								
								//print (orix + " : " + oriy + " -- " + newx + " : " + newy);
								SwapCandy(orix, oriy, newx, newy, true);
							}
						}
					}
				}
			}
			
			// clear mouse swipe variable
			if(Input.GetMouseButtonUp(0)){
				Vector3 position = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
				position = puzzleParent.transform.InverseTransformPoint(position);
				float horizontalThreshold = position.x - mouseX;
				float verticalThreshold = position.y - mouseY;
				
				if((Mathf.Abs(horizontalThreshold) < swipeAmountThreshold || Mathf.Abs(verticalThreshold) < swipeAmountThreshold) && !swipeEnabled){
					tempSpriteHolder = null;
					//NGUITools.Destroy(tempSpriteSelector);
				}
				
				currentMouseX = currentMouseY = 0.0f;
				mouseX = mouseY = 0;
				swipeEnabled = false;
			}
			
			// fill up empty spaces
			if(toFillCandy){
				FillCandy();
			}
			
			// clear pending candy
			if(canTouch && !waitingActivated){
				waitingActivated = true;
				
				if(!CheckWaitingCandy()){
					toFillCandy = false;
				}
				checkGameStatus = false;
				
				if(!GlobalManager.initCheckDone){
					InitCheckComplete();
				}
			}
			
			// check game status
			if(canTouch && !checkGameStatus){
				checkGameStatus = true;
				if(GlobalManager.initCheckDone){
					hintString = CheckAvailableMove();
					if(CheckAvailableMove() == ""){
						RandomizeArray();
						CheckComboCandy();
					}
				}
				
				//CheckGameOverStatus();
			}
		}
		
		if(Application.platform == RuntimePlatform.Android){
			if(Input.GetKey(KeyCode.Escape) && !GlobalManager.paused){
				PauseGame();
			}
		}
	}
	
	// swap 2 pebbles' position
	private void SwapCandy(int fromX, int fromY, int toX, int toY, bool onFinishCallback){
		UISprite tempFinalMCHolder = mainArray[toY][toX];
		
		//tempSpriteDepth	= mainArray[fromY][fromX].depth;
		//mainArray[fromY][fromX].depth = (row * col) + 1;

		float tempXpos = (toX * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (toX * GlobalManager.gap);
		float tempYpos = (toY * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (toY * GlobalManager.gap);
		
		Hashtable param2 = new Hashtable();
		param2.Add("position", new Vector3(tempXpos, tempYpos, tempFinalMCHolder.gameObject.transform.position.z));
		param2.Add("time", switchSpeed);
		param2.Add("islocal", true);
		param2.Add("easetype", iTween.EaseType.linear);
		
		iTween.MoveTo(mainArray[fromY][fromX].gameObject, param2);
		mainArray[toY][toX] = mainArray[fromY][fromX];

		tempXpos = (fromX * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (fromX * GlobalManager.gap);
		tempYpos = (fromY * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (fromY * GlobalManager.gap);
		
		Hashtable param = new Hashtable();
		param.Add("position", new Vector3(tempXpos, tempYpos, tempFinalMCHolder.gameObject.transform.position.z));
		param.Add("time", switchSpeed);
		param.Add("islocal", true);
		param.Add("easetype", iTween.EaseType.linear);
		param.Add("onstarttarget", this.gameObject);
		param.Add("onstart", "AnimationStart");

		//param.Add("onupdatetarget", this.gameObject);
		//param.Add("onupdate", "AnimationStart");
		
		if(onFinishCallback){
			param.Add("oncompletetarget", this.gameObject);
			param.Add("oncomplete", "AnimationDone");
		}else{
			param.Add("oncompletetarget", this.gameObject);
			param.Add("oncomplete", "ActivateTouch");
		}
		
		iTween.MoveTo(tempFinalMCHolder.gameObject, param);
		
		mainArray[fromY][fromX] = tempFinalMCHolder;
	}
	
	// move pebble from 1 tile to another
	private bool MoveCandy(int fromX, int fromY, int toX, int toY, bool onFinishCallback){
		if(mainArray[fromY][fromX] == null) return false;
		
		float tempXpos = (toX * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (toX * GlobalManager.gap);
		float tempYpos = (toY * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (toY * GlobalManager.gap);;
		
		Vector3 pos2 = new Vector3(tempXpos, tempYpos, mainArray[fromY][fromX].gameObject.transform.position.z);
		iTween.MoveTo(mainArray[fromY][fromX].gameObject, iTween.Hash("position", pos2, "time", dropSpeed, "islocal", true, "easetype", GlobalManager.dropTweenEaseType, "onstarttarget", this.gameObject, "onstart", "AnimationStart"));
		
		/*if(onFinishCallback){
			
		}else{
			
		}*/
		
		mainArray[toY][toX] = mainArray[fromY][fromX];
		mainArray[fromY][fromX] = null;
		
		return true;
	}
	
	// fill up empty spaces on screen
	private void FillCandy(){
		//int up = 0;
		//int destination = 0;
		
		//int totalMove = 0;
		bool candyDropping = false;
		
		for(int i=0; i<row; i++){
			for(int j=0; j<col; j++){
				if(mainArray[i][j] == null){
					emptyslot = new int[col];
					canTouch = false;
					
					if(i < row-1){
						if(mainArray[(i+1)][j] != null){
							//print("empty here! -- " + i + " : " + j);
							
							MoveCandy(j, (i+1), j, i, false);
							emptyslot[j]++;
							
							candyDropping = true;
							newCandyDropping = true;
						}
					}
					
					emptyslot[j]++;
					if(!newCandyDropping){
						newCandyDropping = true;
					}
				}
			}
		}
		
		if(!candyDropping && newCandyDropping){
			for(int q=0; q<col; q++){
				tempEmptySlot[q] = emptyslot[q];
			}
			//print("total empty " + totalEmpty[0] + " : " + totalEmpty[1] + " : " + totalEmpty[2] + " : " + totalEmpty[3] + " : " + totalEmpty[4] + " : " + totalEmpty[5] + " : " + totalEmpty[6] + " : " + totalEmpty[7] + " : " + totalEmpty[8]);
			for(int i=0; i<row; i++){
				for(int j=0; j<col; j++){
					if(mainArray[i][j] == null){
						//int tileDifference = (row + emptyslot[j] - tempEmptySlot[j]);
						Vector2 tempPosition;

						tempPosition.x = (j * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (j * GlobalManager.gap);
						tempPosition.y = (GlobalManager.sizeForCalc/2) + ((row + emptyslot[j] - tempEmptySlot[j]) * GlobalManager.sizeForCalc) + ((row + emptyslot[j] - tempEmptySlot[j]) * GlobalManager.gap);
						mainArray[i][j] = CreateCandy(tempPosition.x, tempPosition.y, true, true);

						tempPosition.y = (i * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (i * GlobalManager.gap);
						Vector3 pos2 = new Vector3(tempPosition.x, tempPosition.y, mainArray[i][j].gameObject.transform.position.z);
						iTween.MoveTo(mainArray[i][j].gameObject, iTween.Hash("position", pos2, "time", dropSpeed, "islocal", true, "easetype", GlobalManager.dropTweenEaseType, "onCompleteTarget", this.gameObject, "onComplete", "DropDone", "onstarttarget", this.gameObject, "onstart", "AnimationStart"));
						iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 2.0f, "delay", dropSpeed / 3, "onstarttarget", this.gameObject, "onstart", "DropStart", "onupdatetarget", this.gameObject, "onupdate", "DropUpdate"));
						
						animationCounter++;
						tempEmptySlot[j]--;
						newCandyDropping = false;
						
						//print("after call " + i + " : " + j + " -- " + emptyslot[j]);
					}
				}
			}
		}
	}
	
	// check for combo
	private bool CheckCombo(int toX, int toY, int fromX, int fromY, string type, bool isSecondCheck = false){
		int totalCols = 1;
		int totalRows = 1;
		string colString = System.String.Empty;
		string rowString = System.String.Empty;
		int toCheck = toX;
		bool autoCheck = tempSpriteHolder != null ? false : true;
		
		//waitingActivated = false;
		toFillCandy = true;
		
		// check matching of 2 special candy
		
		// BUG HERE -- FIX IF STATEMENT
		// if either 1 of the candy is color bomb
		if((targetSpriteName == "colourful chocolate" || secondSpriteName == "colourful chocolate") && !autoCheck && !isSecondCheck){
			// if both color bomb combined
			if(targetSpriteName == "colourful chocolate" && secondSpriteName == "colourful chocolate"){
				for(int i=0; i< row; i++){
					for(int j=0; j< col; j++){
						PreActivateCandy(mainArray[i][j], 0f, autoCheck, true);
					}
				}
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.ChocolateAndChocolate);
				
			}else if((targetSpriteName == "colourful chocolate" && GetCandyType(secondSpriteName, 1) == "false") || (secondSpriteName == "colourful chocolate" && GetCandyType(targetSpriteName, 1) == "false")){
				// if color bomb and normal candy combined
				
				if((targetSpriteName != "cherry" && targetSpriteName != "cherry2") && (secondSpriteName != "cherry" && secondSpriteName != "cherry2")){
					int checkX = -1;
					int checkY = -1;
					
					if(targetSpriteName == "colourful chocolate"){
						checkX = toX;
						checkY = toY;
						
						PreActivateCandy(mainArray[fromY][fromX], 0f, autoCheck, true);
					}else{
						checkX = fromX;
						checkY = fromY;
						
						PreActivateCandy(mainArray[toY][toX], 0f, autoCheck, true);
					}
					
					for(int i=0; i< row; i++){
						for(int j=0; j< col; j++){
							string[] candy = mainArray[i][j].spriteName.Split(new char[] {' '});
							string[] candynext = mainArray[checkY][checkX].spriteName.Split(new char[] {' '});
							
							if(mainArray[i][j].spriteName == candynext[0] || candy[0] == candynext[0]){
								PreActivateCandy(mainArray[i][j], 0f, autoCheck);
							}
							
							/*if(mainArray[i][j].spriteName == mainArray[checkY][checkX].spriteName || candy[0] == mainArray[checkY][checkX].spriteName){
								PreActivateCandy(mainArray[i][j], autoCheck);
							}*/
						}
					}
					
					if(initCheckDone)
						ExecuteSpecialMove(GlobalManager.CombinationType.ChocolateAndNormal);
					
				}else{
					// if color bomb and count down candy combined
					if(!initCheckDone){
						canTouch = true;
					}else{
						//if(startCounting)
							canTouch = true;
					}
					
					return false;
				}
				
				secondCheck = false;
			}else if((targetSpriteName == "colourful chocolate" && GetCandyType(secondSpriteName, 1) == "true") || (secondSpriteName == "colourful chocolate" && GetCandyType(targetSpriteName, 1) == "true")){
				// if color bomb and special candy combined
				string[] candyone = targetSpriteName.Split(new char[] {' '});
				string[] candytwo = secondSpriteName.Split(new char[] {' '});
				
				if(candyone[1] == "stripe" || candytwo[1] == "stripe"){
					// if stripe and color bomb combined
					
					toFillCandy = false;
					string colour = candyone[1] == "stripe" ? candyone[0] : candytwo[0];
					
					for(int i=0; i< row; i++){
						for(int j=0; j< col; j++){
							if(mainArray[i][j].spriteName == colour){
								int randNum = UnityEngine.Random.Range(1, 10);
								
								string dir = randNum % 2 == 0 ? "vert" : "hort";
								
								//mainArray[i][j].spriteName = colour + " stripe " + dir;
								mainArray[i][j].spriteName = colour + " stripe " + dir;
								
								StartCoroutine(DelayFunctionCall(0.7f, j, i));
							}
						}
					}
					
					animationCounter -= 2; // neutralize FillCandy() amount to re-enable check combo sequence
					
					PreActivateCandy(mainArray[toY][toX], 0f, autoCheck, true);
					PreActivateCandy(mainArray[fromY][fromX], 0f, autoCheck, true);
					
					if(initCheckDone)
						ExecuteSpecialMove(GlobalManager.CombinationType.ChocolateAndStripe);
					
				}else if(candyone[1] == "packet" || candytwo[1] == "packet"){
					// if packet and color bomb combined
					
					string colour = candyone[1] == "packet" ? candyone[0] : candytwo[0];
					
					int finalX = candyone[1] == "packet" ? fromX : toX;
					int finalY = candyone[1] == "packet" ? fromY : toY;
					int anotherX = candyone[1] == "packet" ? toX : fromX;
					int anotherY = candyone[1] == "packet" ? toY : fromY;
					
					PreActivateCandy(mainArray[finalY][finalX], 0f, autoCheck, true);
					
					mainArray[anotherY][anotherX].GetComponent<CandyProperties>().waiting = "colour packet";
					iTween.ScaleBy(mainArray[anotherY][anotherX].gameObject, iTween.Hash("amount", new Vector3(0.8f, 0.8f, 0), "time", 0.2f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.linear));
					
					for(int i=0; i< row; i++){
						for(int j=0; j< col; j++){
							string[] candy = mainArray[i][j].spriteName.Split(new char[] {' '});
							
							if(candy[0] == colour){
								PreActivateCandy(mainArray[i][j], 0f, autoCheck);
							}
						}
					}
					
					if(initCheckDone)
						ExecuteSpecialMove(GlobalManager.CombinationType.ChocolateAndBomb);
					
				}
				
				secondCheck = false;
			}
			
			canTouch = false;
			return true;
		}else if((GetCandyType(targetSpriteName, 1) == "true" && GetCandyType(secondSpriteName, 1) == "true") && !autoCheck){
			string[] candyone = targetSpriteName.Split(new char[] {' '});
			string[] candytwo = secondSpriteName.Split(new char[] {' '});
			
			// if both stripe candy combined
			
			if(candyone[1] == "stripe" && candytwo[1] == "stripe"){
				PreActivateCandy(mainArray[fromY][fromX], 0f, autoCheck, true);
				
				for(int j=0; j< col; j++){
					PreActivateCandy(mainArray[toY][j], 0f, autoCheck);
				}
				
				for(int i=0; i< row; i++){
					PreActivateCandy(mainArray[i][toX], 0f, autoCheck);
				}
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.StripeAndStripe);
				
			}else if((candyone[1] == "stripe" && candytwo[1] == "packet") || (candyone[1] == "packet" && candytwo[1] == "stripe")){
				// if stripe and packet combined
				// clear 3x3 vertical & horizontal
				
				PreActivateCandy(mainArray[toY][toX], 0f, autoCheck, true);
				PreActivateCandy(mainArray[fromY][fromX], 0f, autoCheck, true);
				int constantValue;
				
				for(int count=0; count<3; count++){
					if(count == 0){
						constantValue = toY - 1;
					}else if(count == 2){
						constantValue = toY + 1;
					}else{
						constantValue = toY;
					}
					for(int j=0; j< col; j++){
						if((constantValue >= 0 && constantValue < row) && (j >= 0 && j < col)){
							PreActivateCandy(mainArray[constantValue][j], 0f, autoCheck);
						}
					}
				}
				
				for(int count=0; count<3; count++){
					if(count == 0){
						constantValue = toX - 1;
					}else if(count == 2){
						constantValue = toX + 1;
					}else{
						constantValue = toX;
					}
					for(int i=0; i< row; i++){
						if((i >= 0 && i < row) && (constantValue >= 0 && constantValue < col)){
							PreActivateCandy(mainArray[i][constantValue], 0f, autoCheck, true);
						}
					}
				}
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.StripeAndBomb);
				
			}else if(candyone[1] == "packet" && candytwo[1] == "packet"){
				// if double packet combined
				// 5x5 bomb
				
				int blastRadius = 5;
				int totalLoop = blastRadius * blastRadius;
				int i = 0;
				
				while(i < 2){
					int finalX = i == 0 ? toX : fromX;
					int finalY = i == 0 ? toY : fromY;
					
					int startingX = finalX - 2;
					int startingY = finalY - 2;
					int tempStartingX = startingX;
					int tempStartingY = startingY;
					int addRadius = startingX + blastRadius;
					
					tempStartingX = startingX;
					tempStartingY = startingY;
					
					for(int temp = 0; temp < totalLoop; temp++){
						if((startingX >= 0 && startingX < col) && (startingY >= 0 && startingY < row)){
							if((startingX != toX || startingY != toY) && (startingX != fromX || startingY != fromY)){ // WEIRD COMPILE BUG? SUPPOSED TO BE && INSTEAD OF ||
								PreActivateCandy(mainArray[startingY][startingX], 0f, autoCheck, true);
							}else{
								if(mainArray[startingY][startingX].GetComponent<CandyProperties>().waiting == "none"){
									mainArray[startingY][startingX].GetComponent<CandyProperties>().waiting = "double packet";
									iTween.ScaleBy(mainArray[startingY][startingX].gameObject, iTween.Hash("amount", new Vector3(0.8f, 0.8f, 0), "time", 0.2f, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.linear));
								}
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
					
					i++;
				}
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.BombAndBomb);
				
			}
			
			secondCheck = false;
			canTouch = false;
			
			return true;
		}else{
			toCheck = toX;
			while(true){
				toCheck++;
				if(toCheck >= 0 && toCheck < col){
					if(mainArray[toY][toCheck].spriteName != "cherry" && mainArray[toY][toCheck].spriteName != "cherry2"){
						if(GetCandyType(mainArray[toY][toCheck].spriteName).Equals(GetCandyType(type)) && mainArray[toY][toCheck].GetComponent<CandyProperties>().waiting == "none" && !mainArray[toY][toCheck].GetComponent<CandyProperties>().activate){
							totalCols++;
							
							if(colString == System.String.Empty){
								colString += toCheck.ToString();
							}else{
								colString += "_" + toCheck.ToString();
							}
						}else{
							break;
						}
					}else{
						break;
					}
				}else{
					break;
				}
			}
			
			toCheck = toX;
			while(true){
				toCheck--;
				if(toCheck >= 0 && toCheck < col){
					if(mainArray[toY][toCheck].spriteName != "cherry" && mainArray[toY][toCheck].spriteName != "cherry2"){
						if(GetCandyType(mainArray[toY][toCheck].spriteName).Equals(GetCandyType(type)) && mainArray[toY][toCheck].GetComponent<CandyProperties>().waiting == "none" && !mainArray[toY][toCheck].GetComponent<CandyProperties>().activate){
							totalCols++;
							
							if(colString == System.String.Empty){
								colString += toCheck.ToString();
							}else{
								colString += "_" + toCheck.ToString();
							}
						}else{
							break;
						}
					}else{
						break;
					}
				}else{
					break;
				}
			}
			
			toCheck = toY;
			while(true){
				toCheck--;
				if(toCheck >= 0 && toCheck < row){
					if(mainArray[toCheck][toX].spriteName != "cherry" && mainArray[toCheck][toX].spriteName != "cherry2"){
						if(GetCandyType(mainArray[toCheck][toX].spriteName).Equals(GetCandyType(type)) && mainArray[toCheck][toX].GetComponent<CandyProperties>().waiting == "none" && !mainArray[toCheck][toX].GetComponent<CandyProperties>().activate){
							totalRows++;
							
							if(rowString == System.String.Empty){
								rowString += toCheck.ToString();
							}else{
								rowString += "_" + toCheck.ToString();
							}
						}else{
							break;
						}
					}else{
						break;
					}
				}else{
					break;
				}
			}
			
			toCheck = toY;
			while(true){
				toCheck++;
				if(toCheck >= 0 && toCheck < row){
					if(mainArray[toCheck][toX].spriteName != "cherry" && mainArray[toCheck][toX].spriteName != "cherry2"){
						if(GetCandyType(mainArray[toCheck][toX].spriteName).Equals(GetCandyType(type)) && mainArray[toCheck][toX].GetComponent<CandyProperties>().waiting == "none" && !mainArray[toCheck][toX].GetComponent<CandyProperties>().activate){
							totalRows++;
							
							if(rowString == System.String.Empty){
								rowString += toCheck.ToString();
							}else{
								rowString += "_" + toCheck.ToString();
							}
						}else{
							break;
						}
					}else{
						break;
					}
				}else{
					break;
				}
			}
		}
		
		//print("amt -- " + totalRows + " : " + totalCols);
		
		if(totalRows > 2 || totalCols > 2){
			if((totalCols >= 5 || totalRows >= 5) && GlobalManager.specialCombination){
				//print("become chocolate");
				
				//mainArray[toY][toX].spriteName = "colourful chocolate";
				
				string[] candy = mainArray[toY][toX].spriteName.Split(new char[] {' '});
				
				if(candy.Length > 1){
					if(candy[1] == "packet" || candy[1] == "stripe"){
						if(totalCols >= 5){
							colString = FindNextAvailable(colString, toX, toY, candy[0], "chocolate", 1);
						}else if(totalRows >= 5){
							rowString = FindNextAvailable(rowString, toX, toY, candy[0], "chocolate", 2);
						}
					}else if(candy[1] == "bomb"){
						//mainArray[toY][toX].spriteName = "colourful chocolate";
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerObj.GetComponent<FollowTarget>().DestroySelf();
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerObj = null;
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerCount = 10;
						
						/*if(GlobalManager.jellyGame){
							CheckObstacle(toX, toY);
						}*/
					}
				}else{
					mainArray[toY][toX].spriteName = "colourful chocolate";
					
					/*if(GlobalManager.jellyGame){
						CheckObstacle(toX, toY);
					}*/
				}
				
				if(totalCols >= 5){
					string[] finalToCheck = colString.Split(new char[] {'_'});
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[toY][int.Parse(finalToCheck[i])], 0f, autoCheck);
					}
				}else if(totalRows >= 5){
					string[] finalToCheck = rowString.Split(new char[] {'_'});
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[int.Parse(finalToCheck[i])][toX], 0f, autoCheck);
					}
				}
				
				//this.gameObject.GetComponent<DefenseMainHandler>().CreateFinalDragon("blue");
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.Chocolate);
				
			}else if((totalCols > 2 && totalRows > 2) && GlobalManager.specialCombination){
				//print("become packet " + " -- " + totalRows + " : " + totalCols);
				
				/*if(colArray.Contains(mainArray[toY][toX])){
					mainArray[toY][toX].GetComponent<CandyProperties>().garbage = false;
					colArray.Remove(mainArray[toY][toX]);
				}*/
				
				//mainArray[toY][toX].spriteName = mainArray[toY][toX].spriteName + " packet";
				
				string[] candy = mainArray[toY][toX].spriteName.Split(new char[] {' '});
				
				if(candy.Length > 1){
					if(candy[1] == "packet" || candy[1] == "stripe"){
						if(totalCols >= 2){
							colString = FindNextAvailable(colString, toX, toY, candy[0], "packet", 1);
						}else if(totalRows >= 2){
							rowString = FindNextAvailable(rowString, toX, toY, candy[0], "packet", 2);
						}
					}else if(candy[1] == "bomb"){
						//mainArray[toY][toX].spriteName = candy[0] + " packet";
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerObj.GetComponent<FollowTarget>().DestroySelf();
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerObj = null;
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerCount = 10;
						
						/*if(GlobalManager.jellyGame){
							CheckObstacle(toX, toY);
						}*/
					}
				}else{
					mainArray[toY][toX].spriteName = candy[0] + " packet";
					
					/*if(GlobalManager.jellyGame){
						CheckObstacle(toX, toY);
					}*/
				}
				
				if(totalCols >= 2){
					string[] finalToCheck = colString.Split(new char[] {'_'});
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[toY][int.Parse(finalToCheck[i])], 0f, autoCheck);
					}
				}
				
				if(totalRows >= 2){
					string[] finalToCheck = rowString.Split(new char[] {'_'});
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[int.Parse(finalToCheck[i])][toX], 0f, autoCheck);
					}
				}
				
				//DefenseMainHandler defObj = this.gameObject.GetComponent<DefenseMainHandler>();
				//Vector3 scale = new Vector3((100/3)*GlobalManager.ratioMultiplierX, (100/3)*GlobalManager.ratioMultiplierY, 1);
				//defObj.CreateBomb("blue", "bomb", scale, 1.5f, 20.0f, new Vector3(2.0f,1.6f,1), new Vector3(0, 0.5f, 0));
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.Bomb);
				
			}else if((totalCols >= 4 || totalRows >= 4) && GlobalManager.specialCombination){
				//print("become stripe " + " -- " + totalRows + " : " + totalCols);
				
				string dirString = "none";
				if(Mathf.Abs(diffx) == 1 && Mathf.Abs(diffy) == 0){
					dirString = "hort";
				}else{
					dirString = "vert";
				}
				
				string[] candy = mainArray[toY][toX].spriteName.Split(new char[] {' '});
				
				if(candy.Length > 1){
					if(candy[1] == "packet" || candy[1] == "stripe"){
						if(totalCols >= 4){
							colString = FindNextAvailable(colString, toX, toY, candy[0], "stripe", 1);
						}else if(totalRows >= 4){
							rowString = FindNextAvailable(rowString, toX, toY, candy[0], "stripe", 2);
						}
					}else if(candy[1] == "bomb"){
						//mainArray[toY][toX].spriteName = candy[0] + " stripe " + dirString;
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerObj.GetComponent<FollowTarget>().DestroySelf();
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerObj = null;
						//mainArray[toY][toX].GetComponent<CandyProperties>().bombTimerCount = 10;
						
						/*if(GlobalManager.jellyGame){
							CheckObstacle(toX, toY);
						}*/
					}
				}else{
					mainArray[toY][toX].spriteName = candy[0] + " stripe " + dirString;
					
					/*if(GlobalManager.jellyGame){
						CheckObstacle(toX, toY);
					}*/
				}
				
				if(totalCols >= 4){
					string[] finalToCheck = colString.Split(new char[] {'_'});
					
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[toY][int.Parse(finalToCheck[i])], 0f, autoCheck);
					}
				}else if(totalRows >= 4){
					string[] finalToCheck = rowString.Split(new char[] {'_'});
					
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[int.Parse(finalToCheck[i])][toX], 0f, autoCheck);
					}
				}
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.Stripe);
				
			}else{
				// normal combination
				if(totalCols > 2){
					PreActivateCandy(mainArray[toY][toX], 0f, autoCheck);
					
					string[] finalToCheck = colString.Split(new char[] {'_'});
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[toY][int.Parse(finalToCheck[i])], 0f, autoCheck);
					}
				}
				
				if(totalRows > 2){
					PreActivateCandy(mainArray[toY][toX], 0f, autoCheck);
					
					string[] finalToCheck = rowString.Split(new char[] {'_'});
					for(int i=0; i< finalToCheck.Length; i++){
						PreActivateCandy(mainArray[int.Parse(finalToCheck[i])][toX], 0f, autoCheck);
					}
				}
				
				if(initCheckDone)
					ExecuteSpecialMove(GlobalManager.CombinationType.Normal);
				
			}
			canTouch = false;
			
			return true;
		}else{
			if(!initCheckDone){
				canTouch = true;
			}else{
				//if(startCounting)
					canTouch = true;
			}
			
			return false;
		}
	}
	
	private void PreActivateCandy(UISprite tempGarbage, float delay, bool tempAutoCheck = false, bool tempIsDoubleCombo = false){
		if(!tempGarbage.GetComponent<CandyProperties>().activate && tempGarbage.GetComponent<CandyProperties>().waiting == "none"){
			if(initCheckDone){
				removeCandySource.Play();
				flyTrailSource.Play();
			}
			
			tempGarbage.GetComponent<CandyProperties>().Activate(mainArray, delay, tempIsDoubleCombo, tempAutoCheck);
			
			/*if(GlobalManager.jellyGame && GlobalManager.initCheckDone){
				CheckObstacle((int)GetArrayPosition((int)tempGarbage.transform.localPosition.x, (int)tempGarbage.transform.localPosition.y).x, (int)GetArrayPosition((int)tempGarbage.transform.localPosition.x, (int)tempGarbage.transform.localPosition.y).y);
			}*/
		}
	}
	
	private void CheckComboCandy(){
		bool comboStatus = false;
		
		for(int i = row-1; i >= 0; i--){
			for(int j = 0; j < col; j++){
				canTouch = false;
				if(!mainArray[i][j].GetComponent<CandyProperties>().garbage){
					if(CheckCombo(j, i, j, i, mainArray[i][j].spriteName)){
						comboStatus = true;
					}
				}
			}
		}
		
		if(!comboStatus){
			waitingActivated = false;
		}
	}
	
	private void CheckCherryStatus(){
		for(int j = 0; j < col; j++){
			canTouch = false;
			if(mainArray[0][j].spriteName == "cherry" || mainArray[0][j].spriteName == "cherry2"){
				mainArray[0][j].GetComponent<CandyProperties>().ActivateDestroySequence(j, 0);
				
				/*if(mainArray[0][j].spriteName == "cherry"){
					MinusCherry(1, 1);
				}else{
					MinusCherry(1, 2);
				}*/
			}
		}
	}
	
	private bool CheckWaitingCandy(){
		bool gotWaitingCandy = false;
		
		for(int i = row-1; i >= 0; i--){
			for(int j = 0; j < col; j++){
				//canTouch = false;
				if(mainArray[i][j].GetComponent<CandyProperties>().waiting != "none"){
					if(!gotWaitingCandy)
						gotWaitingCandy = true;
					
					mainArray[i][j].GetComponent<CandyProperties>().ActivateWaitingCandy(j, i, mainArray[i][j].GetComponent<CandyProperties>().waiting, mainArray);
				}
			}
		}
		
		return gotWaitingCandy;
	}
	
	private string CheckAvailableMove(){
		bool matchFound = false;
		string matchString = "";
		
		for(int i = 0; i < row; i++){
			for(int j = 0; j < col; j++){
				//canTouch = false;
				if(mainArray[i][j].spriteName != "cherry" && mainArray[i][j].spriteName != "cherry2"){
					string type = mainArray[i][j].spriteName;
					
					// CHECK RIGHT
					// xxo
					// oox
					if(j+1 < col && j+2 < col && i+1 < row){
						if(GetCandyType(mainArray[i][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i+1)][(j+2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + i + " : " + (j+1) + " - " + (i+1) + " : " + (j+2));
								matchString = i + "_" + j + ":" + i + "_" + (j+1) + ":" + (i+1) + "_" + (j+2);
								break;
							}
						}
					}
					// oox
					// xxo
					if(j+1 < col && j+2 < col && i-1 >= 0){ 
						if(GetCandyType(mainArray[i][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i-1)][(j+2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + i + " : " + (j+1) + " - " + (i-1) + " : " + (j+2));
								matchString = i + "_" + j + ":" + i + "_" + (j+1) + ":" + (i-1) + "_" + (j+2);
								break;
							}
						}
					}
					// ooxo
					if(j+1 < col && j+3 < col){
						if(GetCandyType(mainArray[i][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i][(j+3)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + i + " : " + (j+1) + " - " + i + " : " + (j+3));
								matchString = i + "_" + j + ":" + i + "_" + (j+1) + ":" + i + "_" + (j+3);
								break;
							}
						}
					}
					// oxo
					// xox
					if(i-1 >= 0 && j+2 < col){
						if(GetCandyType(mainArray[(i-1)][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i][(j+2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + (j+1) + " - " + i + " : " + (j+2));
								matchString = i + "_" + j + ":" + (i-1) + "_" + (j+1) + ":" + i + "_" + (j+2);
								break;
							}
						}
					}
					// xox
					// oxo
					if(i+1 < row && j+2 < col){
						if(GetCandyType(mainArray[(i+1)][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i][(j+2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + (j+1) + " - " + i + " : " + (j+2));
								matchString = i + "_" + j + ":" + (i+1) + "_" + (j+1) + ":" + i + "_" + (j+2);
								break;
							}
						}
					}
					
					// CHECK LEFT
					// xxo
					// oox
					if(j-1 >= 0 && j-2 >= 0 && i+1 < row){
						if(GetCandyType(mainArray[i][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i+1)][(j-2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + i + " : " + (j-1) + " - " + (i+1) + " : " + (j-2));
								matchString = i + "_" + j + ":" + i + "_" + (j-1) + ":" + (i+1) + "_" + (j-2);
								break;
							}
						}
					}
					// oox
					// xxo
					if(j-1 >= 0 && j-2 >= 0 && i-1 >= 0){ 
						if(GetCandyType(mainArray[i][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i-1)][(j-2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + i + " : " + (j-1) + " - " + (i-1) + " : " + (j-2));
								matchString = i + "_" + j + ":" + i + "_" + (j-1) + ":" + (i-1) + "_" + (j-2);
								break;
							}
						}
					}
					// ooxo
					if(j-1 >= 0 && j-3 >= 0){
						if(GetCandyType(mainArray[i][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i][(j-3)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + i + " : " + (j-1) + " - " + i + " : " + (j-3));
								matchString = i + "_" + j + ":" + i + "_" + (j-1) + ":" + i + "_" + (j-3);
								break;
							}
						}
					}
					// oxo
					// xox
					if(i-1 >= 0 && j-2 >= 0){
						if(GetCandyType(mainArray[(i-1)][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i][(j-2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + (j-1) + " - " + i + " : " + (j-2));
								matchString = i + "_" + j + ":" + (i-1) + "_" + (j-1) + ":" + i + "_" + (j-2);
								break;
							}
						}
					}
					// xox
					// oxo
					if(i+1 < row && j-2 >= 0){
						if(GetCandyType(mainArray[(i+1)][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i][(j-2)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + (j-1) + " - " + i + " : " + (j-2));
								matchString = i + "_" + j + ":" + (i+1) + "_" + (j-1) + ":" + i + "_" + (j-2);
								break;
							}
						}
					}
					
					// CHECK TOP
					// xo
					// ox
					// ox
					if(i+1 < row && i+2 < row && j+1 < row){
						if(GetCandyType(mainArray[(i+1)][j].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i+2)][(j+1)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + j + " - " + (i+2) + " : " + (j+1));
								matchString = i + "_" + j + ":" + (i+1) + "_" + j + ":" + (i+2) + "_" + (j+1);
								break;
							}
						}
					}
					// ox
					// xo
					// xo
					if(i+1 < row && i+2 < row && j-1 >= 0){
						if(GetCandyType(mainArray[(i+1)][j].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i+2)][(j-1)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + j + " - " + (i+2) + " : " + (j-1));
								matchString = i + "_" + j + ":" + (i+1) + "_" + j + ":" + (i+2) + "_" + (j-1);
								break;
							}
						}
					}
					// o
					// x
					// o
					// o
					if(i+1 < row && i+3 < row){
						if(GetCandyType(mainArray[(i+1)][j].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i+3)][j].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + j + " - " + (i+3) + " : " + j);
								matchString = i + "_" + j + ":" + (i+1) + "_" + j + ":" + (i+3) + "_" + j;
								break;
							}
						}
					}
					// ox
					// xo
					// ox
					if(i+2 < row && j+1 < col){
						if(GetCandyType(mainArray[(i+1)][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i+2][j].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + (j+1) + " - " + (i+2) + " : " + j);
								matchString = i + "_" + j + ":" + (i+1) + "_" + (j+1) + ":" + (i+2) + "_" + j;
								break;
							}
						}
					}
					// xo
					// ox
					// xo
					if(i+2 < row && j-1 >= 0){
						if(GetCandyType(mainArray[(i+1)][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i+2][j].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i+1) + " : " + (j-1) + " - " + (i+2) + " : " + j);
								matchString = i + "_" + j + ":" + (i+1) + "_" + (j-1) + ":" + (i+2) + "_" + j;
								break;
							}
						}
					}
					
					// CHECK BOTTOM
					// ox
					// ox
					// xo
					if(i-1 >= 0 && i-2 >= 0 && j+1 < row){
						if(GetCandyType(mainArray[(i-1)][j].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i-2)][(j+1)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + j + " - " + (i-2) + " : " + (j+1));
								matchString = i + "_" + j + ":" + (i+1) + "_" + j + ":" + (i-2) + "_" + (j+1);
								break;
							}
						}
					}
					// xo
					// xo
					// ox
					if(i-1 >= 0 && i-2 >= 0 && j-1 >= 0){
						if(GetCandyType(mainArray[(i-1)][j].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i-2)][(j-1)].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + j + " - " + (i-2) + " : " + (j-1));
								matchString = i + "_" + j + ":" + (i-1) + "_" + j + ":" + (i-2) + "_" + (j-1);
								break;
							}
						}
					}
					// o
					// o
					// x
					// o
					if(i-1 >= 0 && i-3 >= 0){
						if(GetCandyType(mainArray[(i-1)][j].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[(i-3)][j].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + j + " - " + (i-3) + " : " + j);
								matchString = i + "_" + j + ":" + (i-1) + "_" + j + ":" + (i-3) + "_" + j;
								break;
							}
						}
					}
					// ox
					// xo
					// ox
					if(i-2 >= 0 && j+1 < col){
						if(GetCandyType(mainArray[(i-1)][(j+1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i-2][j].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + (j+1) + " - " + (i-2) + " : " + j);
								matchString = i + "_" + j + ":" + (i-1) + "_" + (j+1) + ":" + (i-2) + "_" + j;
								break;
							}
						}
					}
					// xo
					// ox
					// xo
					if(i-2 >= 0 && j-1 >= 0){
						if(GetCandyType(mainArray[(i-1)][(j-1)].spriteName).Equals(GetCandyType(type)) && GetCandyType(mainArray[i-2][j].spriteName).Equals(GetCandyType(type))){
							if(!matchFound){
								matchFound = true;
								//Debug.Log(i + " : " + j + " - " + (i-1) + " : " + (j-1) + " - " + (i-2) + " : " + j);
								matchString = i + "_" + j + ":" + (i-1) + "_" + (j-1) + ":" + (i-2) + "_" + j;
								break;
							}
						}
					}
				}
			}
		}
		
		return matchString;
	}
	
	private void ShowHint(){
		if(hintString == "")
		{
			hintString = CheckAvailableMove();
			if(hintString == "")
			{
				// no available move, need to reshuffle
				return;
			}
		}

		string[] position = hintString.Split(new char[]{':'});
		float size = 1.05f;
		float duration = 1f;
		
		for(int i=0; i<position.Length; i++){
			string[] xypos = position[i].Split(new char[]{'_'});
			
			int tempxpos = int.Parse(xypos[0]);
			int tempypos = int.Parse(xypos[1]);
			iTween.PunchScale(mainArray[tempxpos][tempypos].gameObject, iTween.Hash("x", size, "y", size, "time", duration));
			//iTween.ScaleAdd(mainArray[int.Parse(xypos[0])][int.Parse(xypos[1])].gameObject, iTween.Hash("x", size, "y", size, "time", duration));
			//iTween.ScaleAdd(mainArray[int.Parse(xypos[0])][int.Parse(xypos[1])].gameObject, iTween.Hash("x", -(size), "y", -(size), "time", duration, "delay", duration));
		}
	}
	
	private void RandomizeArray()
	{
		Vector3 tempPosition;
		
		for (var i = row - 1; i >= 0; i--) {
			for(int j = 0; j < col; j++){
				int r = Random.Range(0,j);
				UISprite tmp = mainArray[i][j];
				mainArray[i][j] = mainArray[i][r];
				mainArray[i][r] = tmp;
			}
		}
		
		for (var i = row - 1; i >= 0; i--) {
			for(int j = 0; j < col; j++){
				tempPosition.x = (GlobalManager.sizeForCalc/2) + (j * GlobalManager.sizeForCalc) + (j * gap);
				tempPosition.y = (GlobalManager.sizeForCalc/2) + (i * GlobalManager.sizeForCalc) + (i * gap);
				mainArray[i][j].gameObject.transform.position = new Vector3(tempPosition.x, tempPosition.y, mainArray[i][j].gameObject.transform.position.z);
			}
		}
		
		Debug.Log("randomize!");
	}
	
	private void BossModifyPuzzle(){
		int amt = Random.Range(1, 3);
		for(int i=0; i<amt; i++){
			int x = Random.Range(0, col);
			int y = Random.Range(1, row);
			
			mainArray[y][x].GetComponent<UISprite>().spriteName = "cherry";
		}
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
			case "red bomb": case "green bomb": case "blue bomb": case "white bomb": case "purple bomb": case "yellow bomb":
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
	
	private string RemoveAndReform(string ori, string val){
		string[] toCheck = ori.Split(new char[] {'_'});
		string finalString = "";
		
		for(int i=0; i<toCheck.Length; i++){
			if(toCheck[i] != val){
				finalString = finalString == "" ? finalString += toCheck[i] : finalString += "_" + toCheck[i];
			}
		}
		
		return finalString;
	}
	
	private string FindNextAvailable(string tempString, int tempX, int tempY, string color, string type, int dir){
		string[] toCheck = tempString.Split(new char[] {'_'});
		string finalString = "";
		//string dirString = "";
		//string newStringName = "";
		int finalCandyPos = -1;
		
		if(dir == 1){
			//dirString = "vert";
			finalCandyPos = tempX;
		}else{
			//dirString = "hort";
			finalCandyPos = tempY;
		}
		
		/*if(type == "stripe"){
			newStringName = color + " stripe " + dirString;
		}else if(type == "packet"){
			newStringName = color + " packet";
		}else if(type == "chocolate"){
			newStringName = "colourful chocolate";
		}*/
		
		for(int i=0; i<toCheck.Length; i++){
			if(dir == 1){
				if(mainArray[tempY][int.Parse(toCheck[i])].spriteName == color){
					//mainArray[tempY][int.Parse(toCheck[i])].spriteName = newStringName;
					
					/*if(GlobalManager.jellyGame){
						CheckObstacle(int.Parse(toCheck[i]), tempY);
					}*/
					
					finalString = RemoveAndReform(tempString, toCheck[i]);
					break;
				}
			}else{
				if(mainArray[int.Parse(toCheck[i])][tempX].spriteName == color){
					//mainArray[int.Parse(toCheck[i])][tempX].spriteName = newStringName;
					
					/*if(GlobalManager.jellyGame){
						CheckObstacle(tempX, int.Parse(toCheck[i]));
					}*/
					
					finalString = RemoveAndReform(tempString, toCheck[i]);
					break;
				}
			}
		}
		
		finalString += "_" + finalCandyPos.ToString();
		
		return finalString;
	}
	
	private Vector2 GetArrayPosition(Vector3 tempMousePos){
		//tempPosition.x = (j * GlobalManager.sizeForCalc) + (GlobalManager.sizeForCalc/2) + (j * GlobalManager.gap);
		float x = (tempMousePos.x - (GlobalManager.sizeForCalc/2f + GlobalManager.gap/2f)) / (GlobalManager.sizeForCalc + GlobalManager.gap);
		float y = (tempMousePos.y - (GlobalManager.sizeForCalc/2f + GlobalManager.gap/2f)) / (GlobalManager.sizeForCalc + GlobalManager.gap);
		Vector2 finalPos = new Vector2(x, y);

		return finalPos;
	}
	
	private IEnumerator DelayFunctionCall(float duration, int tempxpos, int tempypos){
		yield return new WaitForSeconds(duration);
		//print("delayed callback");
		toFillCandy = true;
		PreActivateCandy(mainArray[tempypos][tempxpos], 0f, false);
	}
	
	private void ActivateTouch(){
		if(!initCheckDone){
			canTouch = true;
		}else{
			//if(startCounting)
				canTouch = true;
		}
	}
	
	private void AnimationStart(){
		canTouch = false;
	}
	
	private void DropStart(){
		if(initCheckDone)
			dropCandySource.Play();
	}
	
	private void DropUpdate( float val ){
		
	}
	
	private void AnimationDone(){
		//mainArray[oriy][orix].depth = tempSpriteDepth;
		
		targetSpriteName = mainArray[oriy][orix].spriteName;
		secondSpriteName = mainArray[ypos][xpos].spriteName;
		
		bool comboAvailable = CheckCombo(xpos, ypos, orix, oriy, tempSpriteHolder.spriteName);
		bool comboAvailable2 = false;
		
		if(secondCheck){
			comboAvailable2 = CheckCombo(orix, oriy, xpos, ypos, mainArray[oriy][orix].spriteName, true);
		}
		
		if(comboAvailable || comboAvailable2){
			//RemoveCandy();
			//MinusMove(-1);
			/*if(GlobalManager.bombGame){
				MinusBomb(-1);
			}*/
			hintCounter = hintDelay;
		}else{
			SwapCandy(xpos, ypos, orix, oriy, false);
		}
		
		targetSpriteName = "";
		secondSpriteName = "";
		secondCheck = true;
		tempSpriteHolder = null;
	}
	
	private void DropDone(){
		animationCounter--;
		
		if(animationCounter == 0){
			if(GlobalManager.cherryGame){
				CheckCherryStatus();
			}
			CheckComboCandy();
		}
	}
	
	public void ExecuteSpecialMove(GlobalManager.CombinationType combinationType){
		switch(combinationType){
		case GlobalManager.CombinationType.Normal:
			
			break;
		case GlobalManager.CombinationType.Stripe:
			
			break;
		case GlobalManager.CombinationType.Bomb:
			
			break;
		case GlobalManager.CombinationType.Chocolate:
			
			break;
		case GlobalManager.CombinationType.StripeAndNormal:
			break;
		case GlobalManager.CombinationType.StripeAndStripe:
			//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(1, GlobalManager.playerNumber);
			break;
		case GlobalManager.CombinationType.StripeAndBomb:
			//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(2, GlobalManager.playerNumber);
			break;
		case GlobalManager.CombinationType.BombAndNormal:
			
			break;
		case GlobalManager.CombinationType.BombAndBomb:
			//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(3, GlobalManager.playerNumber);
			break;
		case GlobalManager.CombinationType.ChocolateAndNormal:
			
			break;
		case GlobalManager.CombinationType.ChocolateAndStripe:
			//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(4, GlobalManager.playerNumber);
			break;
		case GlobalManager.CombinationType.ChocolateAndBomb:
			//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(5, GlobalManager.playerNumber);
			break;
		case GlobalManager.CombinationType.ChocolateAndChocolate:
			//this.gameObject.GetComponent<DefenseMainHandler>().SpawnSkill(6, GlobalManager.playerNumber);
			break;
		}
		
	}
	
	public void GameOver(GlobalManager.Player winTeam){
		if(!GlobalManager.gameover)
		{
			bool win = false;

			if(winTeam == GlobalManager.Player.One)
			{
				win = true;
			}
			
			defObj.CalculateEndGameReward(win, PopupPrefab, root.transform);
			
			GlobalManager.gameover = true;
			//Time.timeScale = 0f;
			
			StartCoroutine(BackToMainMenu());
		}
	}

	private IEnumerator BackToMainMenu(){
		yield return new WaitForSeconds(5.0f);
		if(GlobalManager.multiplyerGame){
			this.gameObject.GetComponent<GameNetworkHandler>().NetworkMessage(GlobalManager.NetworkMessage.QuitGame);
		}else{
			Application.LoadLevel("LandingMenu");
		}
	}
	
	public void PauseGame(){
		if(!GlobalManager.paused){
			if(GlobalManager.multiplyerGame){
				this.gameObject.GetComponent<GameNetworkHandler>().NetworkMessage(GlobalManager.NetworkMessage.PauseGame);
			}
			
			GameObject gameoverPopup = Instantiate(PausePrefab, Vector3.zero, Quaternion.identity) as GameObject;
			gameoverPopup.name = "Pause Game Popup";
			gameoverPopup.transform.parent = root.transform;
			gameoverPopup.transform.localScale = Vector3.one;
			gameoverPopup.GetComponent<PauseGameHandler>().parent = this;
			
			Time.timeScale = 0;
			GlobalManager.paused = true;
		}
	}
	
	public void ResumeGame(){
		if(GlobalManager.multiplyerGame && GlobalManager.paused){
			this.gameObject.GetComponent<GameNetworkHandler>().NetworkMessage(GlobalManager.NetworkMessage.ResumeGame);
		}
		
		Destroy(GameObject.Find("Pause Game Popup"));
		
		Time.timeScale = 1.0f;
		GlobalManager.paused = false;
	}
	
	private AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol, float pitch = 1.0f){
		AudioSource newAudio = (AudioSource)gameObject.AddComponent("AudioSource");
		newAudio.clip = clip;
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol;
		newAudio.pitch = pitch;
		
		if(playAwake){
			newAudio.Play();
		}
		
		return newAudio;
	}
	
	public void SetPuzzleStatus(bool tempValue){
		PuzzleStatus = tempValue;
	}
	
	public bool PuzzleStatus{
		set { disablePuzzle = value; }
		get {return disablePuzzle;}
	}
}
