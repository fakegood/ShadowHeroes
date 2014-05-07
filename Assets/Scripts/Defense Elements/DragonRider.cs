using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragonRider : Character {

	//public CharacterProperties.CategoryColour category = CharacterProperties.CategoryColour.RED;
	[HideInInspector]
	public List<CharacterProperties> cp = null;
	[HideInInspector]
	public int rank = 1;
	[HideInInspector]
	public int unitLevel = 1;
	[HideInInspector]
	public float size = 1f;
	private List<CharacterProperties> cp2 = new List<CharacterProperties>();
	
	public override void OnAwake()
	{
		base.OnAwake();
	}
	
	// Use this for initialization
	public override void Start () {
		if(base.characterSettings != null)
		{
			cp = base.characterSettings.GetComponent<CharacterSettings>().characterProperties;
			//GetUnitTypeList();
			//if(rank > cp2.Count){ Debug.LogWarning("Invalid value assigned for 'Rank'.");	return;	}

			int tempRank = rank - 1;
			Sprite head = cp[tempRank].head;
			Sprite weapon1 = cp[tempRank].weapon1;
			Sprite weapon2 = cp[tempRank].weapon2;

			Sprite dragonHead = cp[tempRank].dragonHead;
			Sprite dragonTail = cp[tempRank].dragonTail;
			Sprite dragonWing1 = cp[tempRank].dragonWing1;
			Sprite dragonWing2 = cp[tempRank].dragonWing2;
			Sprite dragonRider = cp[tempRank].dragonRider;

			RuntimeAnimatorController animatorController = cp[tempRank].animationController;
			
			base.HitPoint = cp[tempRank].maxHitPoint + ((unitLevel-1) * cp[tempRank].hitPointIncreament);
			base.TotalDamage = cp[tempRank].damage + ((unitLevel-1) * cp[tempRank].damageIncreament);
			base.TotalDefense = cp[tempRank].defense;
			base.MoveSpeed = cp[tempRank].moveSpeed / 10f;
			base.AttackSpeed = cp[tempRank].attackSpeed;
			base.AttackRange = cp[tempRank].attackRange;
			if(!base.DisplayUnit) base.UnitSize = cp[tempRank].unitSize;
			base.Category = cp[tempRank].category;
			base.SkillType = cp[tempRank].skillType;
			base.SkillLevel = (int)cp[tempRank].skillLevel + 1;
			base.PositionType = cp[tempRank].positionType;
			base.AttackType = cp[tempRank].attackType;

			if(dragonHead != null) this.transform.FindChild("Dragon Body/Dragon Head").GetComponent<SpriteRenderer>().sprite = dragonHead;
			if(dragonTail != null) this.transform.FindChild("Dragon Body/Dragon Tail").GetComponent<SpriteRenderer>().sprite = dragonTail;
			if(dragonWing1 != null) this.transform.FindChild("Dragon Body/Left Wing").GetComponent<SpriteRenderer>().sprite = dragonWing1;
			if(dragonWing2 != null) this.transform.FindChild("Dragon Body/Right Wing").GetComponent<SpriteRenderer>().sprite = dragonWing2;
			if(dragonRider != null) this.transform.FindChild("Dragon Body/Rider").GetComponent<SpriteRenderer>().sprite = dragonRider;
			if(weapon1 != null) this.transform.FindChild("Dragon Body/Rider/Weapon").GetComponent<SpriteRenderer>().sprite = weapon1;
			
			if(animatorController != null) this.GetComponent<Animator>().runtimeAnimatorController = animatorController;

			if(cp[tempRank].projectile != null) projectile = cp[tempRank].projectile;

			// move y position
			this.gameObject.transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, this.gameObject.transform.localPosition.y + 100f, this.gameObject.transform.localPosition.z);
		}
		
		base.Start ();
	}

	public void GetUnitTypeList()
	{
		if(characterSettings != null)
		{
			for(int i=0; i<cp.Count; i++)
			{
				if(cp[i].unitType == base.UnitType && cp[i].category == base.Category)
				{
					cp2.Add(cp[i]);
				}
			}
		}
	}
}
