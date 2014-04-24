using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSettings : MonoBehaviour {
	public int moveSpeedMultiplyer = 10;
	public List<CharacterProperties> characterProperties;
}

[System.Serializable]
public class CharacterProperties {
	public enum Team
	{
		NONE,
		LEFT,
		RIGHT
	}

	public enum CategoryColour
	{
		NONE,
		RED,
		BLUE,
		GREEN,
		PURPLE,
		ORANGE,
		WHITE
	}

	public enum AttackType
	{
		GROUND,
		AIR,
		GROUND_AND_AIR
	}

	public enum PositionType
	{
		GROUND,
		AIR
	}

	public enum UnitType
	{
		ONE_HANDED_WARRIOR,
		TWO_HANDED_WARRIOR,
		SPEAR_WARRIOR,
		ARCHER,
		CANNON,
		DRAGON,
		HEALER,
		BUILDING
	}

	public enum CharacterState
	{
		IDLE,
		WALKING,
		ATTACKING,
		DEAD
	}

	public enum ExternalStatus
	{
		NONE,
		STUN,
		KNOCK_BACK,
		RAGE,
		POISON
	}

	public enum SkillType
	{
		NONE,
		FIRE_KILLER,
		WATER_KILLER,
		GROUND_KILLER,
		NATURE_KILLER,
		DARK_KILLER,
		LIGHT_KILLER,
		FIRE_BOOST,
		WATER_BOOST,
		GROUND_BOOST,
		NATURE_BOOST,
		DARK_BOOST,
		LIGHT_BOOST,
		CASTLE_BREAKER,
		KNOCK_BACK,
		STUN,
		CRITICAL,
		PERFECT_DEFENSE,
		REVIVAL,
		SPLASH_DAMAGE,
		POISON_DAMAGE,
		RAGE_ATTACK,
		HEAL
	}

	public enum SkillLevel
	{
		ONE,
		TWO,
		THREE
	}

	public string characterName = "Character";
	public string cardName = "Default";
	public Sprite head = null;
	public Sprite weapon1 = null;
	public Sprite weapon2 = null;


	public Sprite dragonHead = null;
	public Sprite dragonJaw = null;
	public Sprite dragonTail = null;
	public Sprite dragonWing1 = null;
	public Sprite dragonWing2 = null;
	public Sprite dragonRider = null;

	public RuntimeAnimatorController animationController = null;

	public GameObject projectile = null;

	public CategoryColour category = CategoryColour.RED;
	public AttackType attackType = AttackType.GROUND;
	public PositionType positionType = PositionType.GROUND;
	public UnitType unitType = UnitType.ONE_HANDED_WARRIOR;
	public SkillType skillType = SkillType.NONE;
	public SkillLevel skillLevel = SkillLevel.ONE;
	//public int unitLevel = 1;
	public int rarity = 1;
	public float maxHitPoint = 100;
	public float hitPointIncreament = 0f;
	public float damage = 5;
	public float damageIncreament = 0f;
	public int defense = 1;
	public float moveSpeed = 2f;
	public float attackSpeed = 2f;
	public float attackRange = 50f;
	public float unitSize = 3f;
	public int unitCost = 10;
	public string iconSpriteName = "SpriteIcon";
}

public class CharacterCard
{
	public int rarity = 1;
	public CharacterProperties.UnitType unitType;
	public int unitLevel;
	public CharacterProperties.SkillType skillType;
	public int skillLevel;
	public CharacterProperties.CategoryColour element;
	public int cost;
}