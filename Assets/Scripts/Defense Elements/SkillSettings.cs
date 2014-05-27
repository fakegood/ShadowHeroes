using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillSettings : MonoBehaviour {
	public List<SkillProperties> skillProperties;
}

[System.Serializable]
public class SkillProperties {
	public string skillName = "Skill";
	public CharacterProperties.SkillType skillType = CharacterProperties.SkillType.NONE;
	public string skillDescription = "None";
	//public RuntimeAnimatorController animationController = null;
	
	public float[] duration;
	public float[] damageOnHit;
	public float[] damagePerSecond;
	public float[] attackRange;
	public int[] posibility;
	public float[] damageBoost;
}