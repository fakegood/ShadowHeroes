using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Healer : Character {

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
			/*GetUnitTypeList();
			if(rank > cp2.Count){ Debug.LogWarning("Invalid value assigned for 'rank'.");	return;	}*/

			int tempRank = rank - 1;
			Sprite head = cp[tempRank].head;
			Sprite weapon1 = cp[tempRank].weapon1;
			Sprite weapon2 = cp[tempRank].weapon2;
			RuntimeAnimatorController animatorController = cp[tempRank].animationController;
			
			base.HitPoint = cp[tempRank].maxHitPoint + ((unitLevel-1) * cp[tempRank].hitPointIncreament);
			base.TotalDamage = cp[tempRank].damage + ((unitLevel-1) * cp[tempRank].damageIncreament);
			base.TotalDefense = cp[tempRank].defense;
			base.MoveSpeed = cp[tempRank].moveSpeed / 10f;
			base.AttackSpeed = cp[tempRank].attackSpeed;
			base.AttackRange = cp[tempRank].attackRange;
			base.UnitSize = cp[tempRank].unitSize;
			base.Category = cp[tempRank].category;
			base.SkillType = cp[tempRank].skillType;
			base.SkillLevel = (int)cp[tempRank].skillLevel + 1;
			base.PositionType = cp[tempRank].positionType;
			base.AttackType = cp[tempRank].attackType;
			
			if(head != null) this.transform.FindChild("Body/Head").GetComponent<SpriteRenderer>().sprite = head;
			
			if(weapon1 != null)
			{
				this.transform.Find("Body/Left Hand/Weapon 1").GetComponent<SpriteRenderer>().sprite = weapon1;
			}
			else
			{
				this.transform.Find("Body/Left Hand/Weapon 1").gameObject.SetActive(false);
			}
			
			if(weapon2 != null)
			{
				this.transform.Find("Body/Right Hand/Weapon 2").GetComponent<SpriteRenderer>().sprite = weapon2;
			}
			else
			{
				this.transform.Find("Body/Right Hand/Weapon 2").gameObject.SetActive(false);
			}
			
			if(animatorController != null) this.GetComponent<Animator>().runtimeAnimatorController = animatorController;
		}
		
		base.Start ();
	}

	private void Update()
	{
		if(base.EnemyObject != null)
		{
			if(base.EnemyObject.CurrentHitPoint >= base.EnemyObject.HitPoint)
			{
				base.EnemyObject = null;
			}
		}
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
