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
		END_GAME,
		GET_INVENTORY,
		SAVE_DECK,
		ENHANCE,
		POKI,
		SELL_CARD,
		RANKING,
		SHOP_PAYMENT
	}

	public enum CardSortType
	{
		DAMAGE,
		HP,
		LEVEL,
		RARITY,
		NAME,
		COST
	}
	
	public static int gameType = 2;
	public static bool jellyGame = false;
	public static bool bombGame = false;
	public static bool cherryGame = false;
	public static bool bossGame = false;
	public static bool multiplyerGame = false;
	public static bool initCheckDone = false;
	public static bool gameover = false;
	public static bool paused = false;
	public static bool specialCombination = true;
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
	public static CardSortType cardSorting = CardSortType.DAMAGE;
	
	public static int[] totalCandyOnScreen = new int[6];

	//public static int[] skillButtonEditArray = new int[6]{1,2,3,4,5,6};
	//public static string[,] characterDetails;
	public static string[,] skillDetails;
	//public static int[] pebbleCounter;
	public static UILabel[] elementTextArray;

	public static bool ProbabilityCounter(int percentageToCheck)
	{
		bool getLucky = false;
		
		int percentage = Mathf.RoundToInt(Random.Range(0, 100));
		if(percentage >= 0 && percentage <= percentageToCheck)
		{
			getLucky = true;
		}
		
		return getLucky;
	}

	public static void SortInventory()
	{
		if(GlobalManager.cardSorting == GlobalManager.CardSortType.RARITY)
		{
			GlobalManager.UICard.localUserCardInventory = GlobalManager.UICard.localUserCardInventory.OrderByDescending(x=>x.rarity).ToList();
		}
		else if(GlobalManager.cardSorting == GlobalManager.CardSortType.DAMAGE)
		{
			GlobalManager.UICard.localUserCardInventory.Sort(SortByDamage);
		}
		else if(GlobalManager.cardSorting == GlobalManager.CardSortType.COST)
		{
			GlobalManager.UICard.localUserCardInventory.Sort(SortByCost);
		}
		else if(GlobalManager.cardSorting == GlobalManager.CardSortType.HP)
		{
			GlobalManager.UICard.localUserCardInventory.Sort(SortByHitPoint);
		}
		else if(GlobalManager.cardSorting == GlobalManager.CardSortType.LEVEL)
		{
			GlobalManager.UICard.localUserCardInventory.Sort(SortByLevel);
		}
		else if(GlobalManager.cardSorting == GlobalManager.CardSortType.NAME)
		{
			GlobalManager.UICard.localUserCardInventory.Sort(SortByName);
		}
	}
	
	// sorting functions
	public static int SortByDamage(CharacterCard go1, CharacterCard go2)
	{
		int totalDamage1 = (int)GlobalManager.GameSettings.csObj.characterProperties[go1.cardNumber-1].damage + (int)(go1.level * GlobalManager.GameSettings.csObj.characterProperties[go1.cardNumber-1].damageIncreament);
		int totalDamage2 = (int)GlobalManager.GameSettings.csObj.characterProperties[go2.cardNumber-1].damage + (int)(go2.level * GlobalManager.GameSettings.csObj.characterProperties[go2.cardNumber-1].damageIncreament);
		return totalDamage1.CompareTo(totalDamage2);
	}
	
	public static int SortByCost(CharacterCard go1, CharacterCard go2)
	{
		int cost1 = (int)GlobalManager.GameSettings.csObj.characterProperties[go1.cardNumber-1].unitCost;
		int cost2 = (int)GlobalManager.GameSettings.csObj.characterProperties[go2.cardNumber-1].unitCost;
		return cost1.CompareTo(cost2);
	}
	
	public static int SortByHitPoint(CharacterCard go1, CharacterCard go2)
	{
		int totalHitPoint1 = (int)GlobalManager.GameSettings.csObj.characterProperties[go1.cardNumber-1].maxHitPoint + (int)(go1.level * GlobalManager.GameSettings.csObj.characterProperties[go1.cardNumber-1].hitPointIncreament);
		int totalHitPoint2 = (int)GlobalManager.GameSettings.csObj.characterProperties[go2.cardNumber-1].maxHitPoint + (int)(go2.level * GlobalManager.GameSettings.csObj.characterProperties[go2.cardNumber-1].hitPointIncreament);
		return totalHitPoint1.CompareTo(totalHitPoint2);
	}
	
	public static int SortByLevel(CharacterCard go1, CharacterCard go2)
	{
		return go1.level.CompareTo(go2.level);
	}
	
	public static int SortByName(CharacterCard go1, CharacterCard go2)
	{
		string name1 = GlobalManager.GameSettings.csObj.characterProperties[go1.cardNumber-1].characterName;
		string name2 = GlobalManager.GameSettings.csObj.characterProperties[go2.cardNumber-1].characterName;
		return name1.CompareTo(name2);
	}

	public static class LocalUser
	{
		public static int UID = -1;
		public static string nickname = "Player";
		public static int level = 1;
		public static int gold = 0;
		public static int gem = 0;
		public static int battlePoint = 50;
		public static int actionPoint = 20;
		public static int deckCost = 0;
		public static int maxDeckCost = 100;
		public static int experience = 0;
		public static int victoryPoint = 0;
		public static int totalBattle = 0;
		public static int totalWin = 0;

		public static int ComputeNeedLevelupExp(int level)
		{
			if(level <= 0)
			{
				return 0;
			}
			else
			{
				return 500 + (300 * (level-1));
			}
		}

		public static int ComputeActionPoint(int level)
		{
			level = level-1 <= 0 ? 0 : level-1;
			return 20 + (2 * level);
		}

		public static int ComputeLevelTotalExp(int level)
		{
			if(level <= 0)
			{
				return 0;
			}
			else
			{
				return (500*level)+150*((int)Mathf.Pow(level,2f)-level);
			}
		}

		public static int ComputeCardNeedLevelupExp(int level)
		{
			if(level <= 0)
			{
				return 0;
			}
			else
			{
				return 500 + (200 * (level-1));
			}
		}

		public static int ComputeCardLevelTotalExp(int level)
		{
			if(level <= 0)
			{
				return 0;
			}
			else
			{
				return (500*level)+100*((int)Mathf.Pow(level,2f)-level);
			}
		}

		public static int ComputeTotalExpFromCard(CharacterCard card)
		{
			int baseExp = 0;

			if(GameSettings.csObj.characterProperties[card.cardNumber-1].rarity == 1)
			{
				baseExp = 500;
			}
			else if(GameSettings.csObj.characterProperties[card.cardNumber-1].rarity == 2)
			{
				baseExp = 1000;
			}
			else if(GameSettings.csObj.characterProperties[card.cardNumber-1].rarity == 3)
			{
				baseExp = 5000;
			}
			else if(GameSettings.csObj.characterProperties[card.cardNumber-1].rarity == 4)
			{
				baseExp = 10000;
			}
			else if(GameSettings.csObj.characterProperties[card.cardNumber-1].rarity == 5)
			{
				baseExp = 20000;
			}

			int currentExp = card.experience + ComputeCardLevelTotalExp(card.level-1);

			return baseExp + (int)((float)currentExp * 0.1f);
		}
	}

	public static class GameSettings
	{
		public static int chosenArea = -1;
		public static int chosenStage = -1;
		public static CharacterSettings csObj;
	}

	public static class UICard
	{
		public static List<CharacterCard> localUserCardDeck = new List<CharacterCard>(){null, null, null, null, null, null};
		public static List<CharacterCard> localUserCardInventory = new List<CharacterCard>();
		public static bool changed = false;

		public static void SetDeckValue(int index, CharacterCard deckValue)
		{
			localUserCardDeck[index-1] = deckValue;
		}

		public static void SetInventoryValue(int index, CharacterCard deckValue)
		{
			localUserCardInventory[index-1] = deckValue;
		}

		public static void SwapDeckAndInventory(int from, int to)
		{
			if(from == 0)
			{
				localUserCardDeck[to-1] = null;
			}
			else
			{
				localUserCardDeck[to-1] = localUserCardInventory[from-1];
				localUserCardDeck[to-1].order = to;
				localUserCardInventory[from-1].order = -1;
			}

			changed = true;
		}
	}

	public class NetworkSettings
	{
		public static string ServerURL = "http://touchtouch.biz/shadow/";
		public static string Register = "register.jsp";
		public static string Init = "init.jsp";
		public static string EndGame = "endgame.jsp";
		public static string GetInventory = "getinventory.jsp";
		public static string SaveDeck = "savedeck.jsp";
		public static string Enhance = "enhance.jsp";
		public static string Poki = "poki.jsp";
		public static string SellCard = "sellcard.jsp";
		public static string Ranking = "ranking.jsp";
		public static string ShopList = "shopList.jsp";
		public static string ShopPayment = "shopPayment.jsp";
		public static string RefillPoint = "pointRefill.jsp";
		
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
			//case RequestType.START_GAME:
			//	return NetworkSettings.ServerURL + NetworkSettings.EndGame;
			//	break;
			case RequestType.END_GAME:
				return NetworkSettings.ServerURL + NetworkSettings.EndGame;
				break;
			case RequestType.GET_INVENTORY:
				return NetworkSettings.ServerURL + NetworkSettings.GetInventory;
				break;
			case RequestType.SAVE_DECK:
				return NetworkSettings.ServerURL + NetworkSettings.SaveDeck;
				break;
			case RequestType.RANKING:
				return NetworkSettings.ServerURL + NetworkSettings.Ranking;
				break;
			case RequestType.POKI:
				return NetworkSettings.ServerURL + NetworkSettings.Poki;
				break;
			case RequestType.SELL_CARD:
				return NetworkSettings.ServerURL + NetworkSettings.SellCard;
				break;
			case RequestType.SHOP_PAYMENT:
				return NetworkSettings.ServerURL + NetworkSettings.ShopPayment;
				break;
			case RequestType.ENHANCE:
				return NetworkSettings.ServerURL + NetworkSettings.Enhance;
				break;
			default:
				return "";
				break;
			}
		}
	}
}
