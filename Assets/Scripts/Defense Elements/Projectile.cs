using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	[HideInInspector]
	public GameObject targetEnemy;
	[HideInInspector]
	public Character parent;
	public ParticleSystem destroyedExplosion;

	private float damage = 0f;
	private DamageClass damageObj = null;
	private DamageApplyClass damageApplyObject = null;
	private ParticleSystem[] thisParticleSystem;

	// Use this for initialization
	void Start () {
		if(targetEnemy != null)
		{
			thisParticleSystem = this.GetComponentsInChildren<ParticleSystem>();

			foreach(ParticleSystem ps in thisParticleSystem)
			{
				ps.renderer.sortingLayerName = "Weapon Particle System";
			}

			if(targetEnemy.GetComponent<Character>().PositionType == CharacterProperties.PositionType.AIR)
			{
				/*float angle = Vector2.Angle(targetEnemy.transform.localPosition, this.transform.localPosition);
				float distance = Vector2.Distance(targetEnemy.transform.localPosition, this.transform.localPosition);
				if(parent.Team == CharacterProperties.Team.LEFT)
				{
					this.transform.rotation = Quaternion.AngleAxis(angle + (100f * (0.6f-distance)), Vector3.forward);
				}
				else
				{
					this.transform.rotation = Quaternion.AngleAxis(angle - (100f * (0.6f-distance)), Vector3.forward);
				}*/
			}
			float offset = 0f;
			if(parent.UnitType == CharacterProperties.UnitType.CANNON)
			{
				offset = 0.05f;
			}
			Vector3 pos = new Vector3(targetEnemy.transform.localPosition.x, targetEnemy.transform.localPosition.y + offset, targetEnemy.transform.localPosition.z);
			iTween.MoveTo(this.gameObject, iTween.Hash("position", pos, "time", 1.5f, "islocal", true, "easetype", iTween.EaseType.easeOutExpo));
		}
	}

	private void Update()
	{
		if(targetEnemy == null) DestroySelf();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<Character>() != null && other.gameObject == targetEnemy)
		{
			float extraDamage = parent.ApplyEffectDamage();
			float bonusDamage = parent.ApplyBonusDamage();

			damageApplyObject = new DamageApplyClass();
			damageApplyObject.damage = -damage;
			damageApplyObject.criticalDamage = -extraDamage;
			if(bonusDamage > 0f) damageApplyObject.bonusDamage = -bonusDamage;

			other.GetComponent<Character>().ApplyDamage(damageApplyObject);

			if(parent.SkillType == CharacterProperties.SkillType.SPLASH_DAMAGE)
			{
				parent.ApplySplashDamage(targetEnemy.transform.localPosition.x);
			}

			DestroySelf();
		}
	}

	private void ApplyDamage()
	{
		if(targetEnemy != null)
		{

		}

		DestroySelf();
	}

	private void DestroySelf()
	{
		if(parent != null && (parent.UnitType == CharacterProperties.UnitType.CANNON || parent.UnitType == CharacterProperties.UnitType.DRAGON) && destroyedExplosion != null)
		{
			Instantiate(destroyedExplosion, this.transform.position, Quaternion.identity);
		}

		Destroy(this.gameObject);
	}

	public float TotalDamage
	{
		set{ damage = value; }
		get{ return damage; }
	}

	public DamageClass Effect
	{
		set{ damageObj = value; }
		get{ return damageObj; }
	}
}
