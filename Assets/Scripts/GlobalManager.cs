using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GlobalManager : MonoBehaviour {
	
	public enum UnitType {
		Army,
		Archer,
		StrongMan,
		Hero,
		Enemy
	};
	
	public enum AttackStat {
		Idle,
		Walking,
		Attacking
	};
	
	public enum AttackType {
		Air,
		Ground,
		Both
	}
	
	public enum SkillType{
		Heal,
		Meteor,
		Lightning,
		Bomb,
		Revive,
		EarthQuake
	};
	
	public enum CombinationType{
		Normal,
		Stripe,
		Bomb,
		Chocolate,
		StripeAndNormal,
		StripeAndStripe,
		StripeAndBomb,
		BombAndNormal,
		BombAndBomb,
		ChocolateAndNormal,
		ChocolateAndStripe,
		ChocolateAndBomb,
		ChocolateAndChocolate
	};
	
	public enum RaceType {
		Zerg,
		Protoss,
		Terran
	}
	
	public enum AIType {
		Aggressive,
		Defensive
	}
	
	public enum Player{
		One,
		Two
	}
	
	public enum AtackType{
		Air,
		Ground,
		Both
	}
	
	public enum NetworkMessage{
		SpawnUnit,
		SpawnSkill,
		PauseGame,
		ResumeGame,
		QuitGame
	}

	public enum RequestType
	{
		INIT,
		REGISTER,
		START_GAME,
		END_GAME
	}
	
	public static int gameType = 2;
	public static RaceType playerRaceType = RaceType.Zerg;
	public static bool jellyGame = false;
	public static bool bombGame = false;
	public static bool cherryGame = false;
	public static bool bossGame = false;
	public static bool multiplyerGame = false;
	public static bool initCheckDone = false;
	public static bool gameover = false;
	public static bool paused = false;
	public static bool specialCombination = false;
	public static bool ignoreCost = false;
	public static bool createTrail = true;
	public static Player playerNumber = Player.One;
	
	public static float ratioMultiplierX = Screen.width / 480.0f;
	public static float ratioMultiplierY = Screen.height / 800.0f;
	
	public static int row = 8;
	public static int col = 8;
	public static int gap = 12;
	public static float size = 75f;
	public static float sizeForCalc = 68f;
	public static float offsetX = 0;
	public static float offsetY = 0;
	
	public static float switchSpeed = 0.1f;
	public static float dropSpeed = 0.000001f;
	public static float shrinkSpeed = 0.000001f;
	public static float oriDropSpeed = 0.4f;
	public static float oriShrinkSpeed = 0.2f;
	
	public static iTween.EaseType dropTweenEaseType = iTween.EaseType.easeOutBounce;
	
	public static int[] totalCandyOnScreen = new int[6];

	//public static int[] skillButtonEditArray = new int[6]{1,2,3,4,5,6};
	//public static string[,] characterDetails;
	public static string[,] skillDetails;
	//public static int[] pebbleCounter;
	
	public static UILabel[] elementTextArray;
	//public static tk2dSpriteCollectionData mainSpriteCollectionData;
	//public static tk2dSpriteAnimation mainSpriteAnimation;

	public static class LocalUser
	{
		public static int UID = -1;
		public static string nickname = "Player";
		public static int level = 1;
		public static int gold = 0;
		public static int gem = 0;
		public static int battlePoint = 0;
		public static int actionPoint = 0;
		public static int deckCost = 0;
		public static int maxDeckCost = 20;
		public static int experience = 0;
		public static int victoryPoint = 0;
		public static int totalBattle = 0;
		public static int totalWin = 0;
	}

	public static class GameSettings
	{
		public static int chosenArea = -1;
		public static int chosenStage = -1;
	}

	public static class UICard
	{
		public static int[] localUserCardDeck = new int[6]{1,2,4,-1,-1,-1};
		public static int[] localUserCardDeckLevel = new int[6]{1,1,1,1,1,1};
		public static List<int> localUserCardInventory = new List<int>(){1,2,3,4,5,6,7,8,9,10,11};
		public static List<int> localUserCardInventoryLevel = new List<int>(){1,1,1,1,1,1,1,1,1,1,1};

		public static void SetDeckValue(int index, int deckValue, int deckLevel)
		{
			localUserCardDeck[index-1] = deckValue;
			localUserCardDeckLevel[index-1] = deckLevel;
		}

		public static void SetInventoryValue(int index, int deckValue, int deckLevel)
		{
			localUserCardInventory[index-1] = deckValue;
			localUserCardInventoryLevel[index-1] = deckLevel;
		}

		public static void SwapDeckAndInventory(int from, int to)
		{
			int tempDeckValue = -1;
			int tempDeckLevel = -1;

			if(from > 0)
			{
				tempDeckValue = localUserCardInventory[from-1];
				tempDeckLevel = localUserCardInventoryLevel[from-1];
			}

			if(localUserCardDeck[to-1] != -1 && from == 0)
			{
				localUserCardInventory.Add(localUserCardDeck[to-1]);
				localUserCardInventoryLevel.Add(localUserCardDeckLevel[to-1]);
			}
			else if(localUserCardDeck[to-1] == -1 && from == 0)
			{
				// "None" chosen for "None" deck slot -
				// do nothing
			}
			else if(localUserCardDeck[to-1] != -1)
			{
				localUserCardInventory[from-1] = localUserCardDeck[to-1];
				localUserCardInventoryLevel[from-1] = localUserCardDeckLevel[to-1];
			}
			else
			{
				localUserCardInventory.RemoveAt(from-1);
				localUserCardInventoryLevel.RemoveAt(from-1);
			}

			localUserCardDeck[to-1] = tempDeckValue;
			localUserCardDeckLevel[to-1] = tempDeckLevel;
		}
	}

	public class NetworkSettings
	{
		public static string ServerURL = "http://touchtouch.biz/shadow/";
		public static string Register = "register.jsp";
		public static string Init = "init.jsp";
		public static string EndGame = "endgame.jsp";
		
		public static string GetFullURL(GlobalManager.RequestType requestType)
		{
			switch(requestType)
			{
			case RequestType.INIT:
				return NetworkSettings.ServerURL + NetworkSettings.Init;
				break;
			case RequestType.REGISTER:
				return NetworkSettings.ServerURL + NetworkSettings.Register;
				break;
			case RequestType.START_GAME:
				return NetworkSettings.ServerURL + NetworkSettings.EndGame;
				break;
			case RequestType.END_GAME:
				return NetworkSettings.ServerURL + NetworkSettings.EndGame;
				break;
			default:
				return "";
				break;
			}
		}
	}
}
