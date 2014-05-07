using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character : MonoBehaviour {
	[HideInInspector]
	public Sprite head = null;
	[HideInInspector]
	public Sprite weapon1 = null;
	[HideInInspector]
	public Sprite weapon2 = null;
	[HideInInspector]
	public GameObject projectile = null;
	[HideInInspector]
	public RuntimeAnimatorController animatorController = null;
	
	public static List<Character> characterLeft = new List<Character>();
	public static List<Character> characterRight = new List<Character>();
	public CharacterSettings characterSettings = null;
	public SkillSettings skillSettings = null;
	public GameObject indicatorPrefab = null;
	[HideInInspector]
	public GameObject damageEffectPrefab = null;
	public UIProgressBar hitPointProgressBar;

	//private List<CharacterProperties> properties = new List<CharacterProperties>();
	public CharacterProperties.CategoryColour category = CharacterProperties.CategoryColour.RED;
	private CharacterProperties.AttackType attackType = CharacterProperties.AttackType.GROUND;
	private CharacterProperties.PositionType positionType = CharacterProperties.PositionType.GROUND;
	private Animator animator;
	private float maxHitPoint = 100f;
	private float currentHitPoint = 100f;
	private float damage = 5f;
	private int defense = 1;
	private float moveSpeed = 10;
	private float attackSpeed = 2f;
	private float attackRange = 10f;
	private float unitSize = 1f;
	private float direction = 1f;
	private float customGravity = 0f;
	private float originalAir = 0f;
	private int moveSpeedMultiplyer = 1;
	public CharacterProperties.CharacterState movementState = CharacterProperties.CharacterState.IDLE;
	public CharacterProperties.Team team = CharacterProperties.Team.LEFT;
	public CharacterProperties.UnitType unitType = CharacterProperties.UnitType.ONE_HANDED_WARRIOR;
	public CharacterProperties.SkillType skillType = CharacterProperties.SkillType.NONE;
	private int skillLevel = 0;
	private float skillDuration = 0;
	private float skillDamageOnHit = 0;
	private float skillDamagePerSecond = 0;
	private float skillAttackRange = 0;
	private int skillProbability = 0;
	private CharacterProperties.ExternalStatus externalStatus = CharacterProperties.ExternalStatus.NONE;
	public Character enemyObj = null;

	private float rightSpawnPosition = 0f;
	private float leftSpawnPosition = 0f;

	private CharacterProperties.CategoryColour extraDamageTarget = CharacterProperties.CategoryColour.NONE;
	[HideInInspector]
	public int[] bonusDamageDeck;
	private bool showDamageAfterDied = false;

	private DamageClass damageObject = new DamageClass();
	private DamageApplyClass damageApplyObject = new DamageApplyClass();
	private ParticleSystem bombSpark = null;
	private GameObject effectsPoint = null;
	public GameObject[] effectSettings;
	private GameObject persistentEffectHolder;
	private float headSize = 0f;

	private DefenseHandler defObj;
	private bool displayUnit = false;

	public virtual void OnAwake()
	{
		if(characterSettings == null)
		{
			Debug.Log("Character Settings is not assigned, characters will run on default properties.");
			Debug.LogWarning("Character Settings is not assigned, characters will run on default properties.");

			if(head != null) this.transform.Find("Body/Head").GetComponent<SpriteRenderer>().sprite = head;
			
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
	}

	private void Awake()
	{
		//OnAwake ();
		animator = this.GetComponent<Animator>();

		if(GameObject.FindGameObjectWithTag("DefenseHandler"))
		{
			defObj = GameObject.FindGameObjectWithTag("DefenseHandler").GetComponent<DefenseHandler>();
			rightSpawnPosition = GameObject.FindGameObjectWithTag("Spawner Right").transform.localPosition.x;
			leftSpawnPosition = GameObject.FindGameObjectWithTag("Spawner Left").transform.localPosition.x;
		}

		if(this.transform.Find("Healthbar") != null)
		{
			hitPointProgressBar = this.transform.Find("Healthbar").GetComponent<UIProgressBar>();
		}
	}

	// Use this for initialization
	public virtual void Start () {
		SpriteRenderer[] spriteRenderer = this.GetComponentsInChildren<SpriteRenderer>();
		
		foreach(SpriteRenderer sr in spriteRenderer)
		{
			sr.renderer.sortingLayerName = "Weapon Particle System";
		}

		if(!DisplayUnit)
		{
			if(team == CharacterProperties.Team.LEFT)
			{
				characterLeft.Add(this);
				
				for(int j=0; j<characterRight.Count; j++)
				{
					characterRight[j].UpdateTargetEnemy();
				}
			}
			else if(team == CharacterProperties.Team.RIGHT)
			{
				characterRight.Add(this);
				
				for(int i=0; i<characterLeft.Count; i++)
				{
					characterLeft[i].UpdateTargetEnemy();
				}
			}
		}

		if(characterSettings != null)
		{
			moveSpeedMultiplyer = characterSettings.moveSpeedMultiplyer;
		}

		if(PositionType != CharacterProperties.PositionType.AIR)
		{
			//customGravity = Physics2D.gravity.y;
			customGravity = 0;
		}
		else
		{
			originalAir = this.transform.position.y;
		}

		if(UnitType != CharacterProperties.UnitType.BUILDING && !DisplayUnit)
		{
			ApplyConstantBonusDamage();
		}

		// calculate unit highest point - to set HP bar and instantiate damage indicator
		if(UnitType != CharacterProperties.UnitType.DRAGON)
		{
			if(this.transform.Find("Body/Head") != null && this.transform.Find("Body/Highest Point") != null)
			{
				float val = headSize = this.transform.Find("Body/Head").GetComponent<SpriteRenderer>().sprite.rect.height * 2f; // double the head sprite size
				Transform localHead = this.transform.Find("Body/Head").transform; // store head transform;
				Transform highestPoint = this.transform.Find("Body/Highest Point").transform; // store highest point transform

				Vector3 pos = highestPoint.transform.localPosition; // store highest point position
				pos.y = localHead.localPosition.y + ((val / 10f) + 5f); // add Y position according to head's height
				highestPoint.localPosition = pos; // set highest point to the desired location
				highestPoint.parent = this.transform; // reset head's parent to this character
				hitPointProgressBar.gameObject.transform.localPosition = highestPoint.localPosition; // set hp bar to highest point position
			}
		}
		else
		{
			if(this.transform.Find("Dragon Body/Rider") != null && this.transform.Find("Dragon Body/Highest Point") != null)
			{
				float val = headSize = this.transform.Find("Dragon Body/Rider").GetComponent<SpriteRenderer>().sprite.rect.height * 2f; // double the head sprite size
				Transform localHead = this.transform.Find("Dragon Body/Rider").transform; // store head transform
				Transform highestPoint = this.transform.Find("Dragon Body/Highest Point").transform; // store highest point transform

				Vector3 pos = highestPoint.transform.localPosition; // store highest point position
				pos.y = localHead.localPosition.y + (val / 10f); // add Y position according to head's height
				highestPoint.localPosition = pos; // set highest point to the desired location
				highestPoint.parent = this.transform; // reset head's parent to this character
				hitPointProgressBar.gameObject.transform.localPosition = highestPoint.localPosition; // set hp bar to highest point position
			}
		}

		// set fire position for range unit
		if(UnitType == CharacterProperties.UnitType.ARCHER)
		{
			Transform localWeapon = this.transform.Find("Body/Left Hand/Weapon 1").transform; // store weapon transform
			Transform localFirePos = this.transform.Find("Body/Left Hand/Weapon 1/Fire Position").transform; // store fire position transform
			localFirePos.localPosition = new Vector3(localWeapon.localPosition.x + 0.07f, localWeapon.localPosition.y, localFirePos.localPosition.z);
		}
		else if(UnitType == CharacterProperties.UnitType.CANNON)
		{
			Transform localWeapon = this.transform.Find("Body/Left Hand/Weapon 1").transform; // store weapon transform
			Transform localFirePos = this.transform.Find("Body/Left Hand/Weapon 1/Fire Position").transform; // store fire position transform
			localFirePos.localPosition = new Vector3(localWeapon.localPosition.x + 3.8f, localWeapon.localPosition.y + 5.5f, localFirePos.localPosition.z);
		}

		// set sparks emitter
		if(UnitType != CharacterProperties.UnitType.DRAGON && UnitType != CharacterProperties.UnitType.BUILDING)
		{
			Transform sparkObj = this.transform.Find("Body/Left Hand/Weapon 1/Fire Position/Bomb Spark");
			sparkObj.gameObject.SetActive(true);
			bombSpark = sparkObj.GetComponent<ParticleSystem>();
			bombSpark.playOnAwake = false;
		}
		else if(UnitType == CharacterProperties.UnitType.DRAGON)
		{
			Transform sparkObj = this.transform.Find("Dragon Body/Dragon Head/Fire Position/Bomb Spark");
			sparkObj.gameObject.SetActive(true);
			bombSpark = sparkObj.GetComponent<ParticleSystem>();
			bombSpark.playOnAwake = false;
		}

		if(UnitType != CharacterProperties.UnitType.BUILDING && UnitType != CharacterProperties.UnitType.DRAGON)
		{
			effectsPoint = this.transform.Find("Body/Effects Point").gameObject;
			//healEffects.renderer.sortingLayerName = "Particle System";
		}
		else if(UnitType != CharacterProperties.UnitType.BUILDING && UnitType == CharacterProperties.UnitType.DRAGON)
		{
			effectsPoint = this.transform.Find("Dragon Body/Effects Point").gameObject;
			//healEffects.renderer.sortingLayerName = "Particle System";
		}

		if(UnitType != CharacterProperties.UnitType.BUILDING && skillSettings != null)
		{
			SkillDuration = skillSettings.skillProperties[0].duration[SkillLevel-1];
			SkillDamageOnHit = skillSettings.skillProperties[0].damageOnHit[SkillLevel-1];
			SkillDamagePerSecond = skillSettings.skillProperties[0].damagePerSecond[SkillLevel-1];
			SkillAttackRange = skillSettings.skillProperties[0].attackRange[SkillLevel-1];
			SkillProbability = skillSettings.skillProperties[0].posibility[SkillLevel-1];
		}
	}

	private void Update()
	{
		if(!DisplayUnit)
		{
			if(EnemyObject != null)
			{
				if(team == CharacterProperties.Team.LEFT)
				{
					if(EnemyObject.transform.localPosition.x - this.gameObject.transform.localPosition.x < 0)
					{
						EnemyObject = null;
					}
				}
				else
				{
					if(EnemyObject.transform.localPosition.x - this.gameObject.transform.localPosition.x > 0)
					{
						EnemyObject = null;
					}
				}
			}
		}
	}

	private void LateUpdate()
	{
		if(GlobalManager.initCheckDone && !DisplayUnit)
		{
			if(movementState == CharacterProperties.CharacterState.WALKING)
			{
				if(team == CharacterProperties.Team.LEFT)
				{
					if(UnitType == CharacterProperties.UnitType.HEALER)
					{
						if(this.gameObject.transform.localPosition.x <= rightSpawnPosition)
						{
							Vector2 vel = new Vector2(this.MoveSpeed * direction, customGravity);
							this.rigidbody2D.velocity = vel;
						}
						else
						{
							ExternalStatus = CharacterProperties.ExternalStatus.STUN;
							MovementState = CharacterProperties.CharacterState.IDLE;
						}
					}
					else
					{
						Vector2 vel = new Vector2(this.MoveSpeed * direction, customGravity);
						this.rigidbody2D.velocity = vel;
					}
				}
				else
				{
					if(UnitType == CharacterProperties.UnitType.HEALER)
					{
						if(this.gameObject.transform.localPosition.x > leftSpawnPosition)
						{
							Vector2 vel = new Vector2(this.MoveSpeed * -direction, customGravity);
							this.rigidbody2D.velocity = vel;
						}
						else
						{
							ExternalStatus = CharacterProperties.ExternalStatus.STUN;
							MovementState = CharacterProperties.CharacterState.IDLE;
						}
					}
					else
					{
						Vector2 vel = new Vector2(this.MoveSpeed * -direction, customGravity);
						this.rigidbody2D.velocity = vel;
					}
				}
			}

			if(PositionType == CharacterProperties.PositionType.AIR)
			{
				this.transform.position = new Vector3(this.transform.position.x, originalAir, this.transform.position.z);
			}

			if(ExternalStatus == CharacterProperties.ExternalStatus.KNOCK_BACK)
			{
				if(this.Team == CharacterProperties.Team.LEFT)
				{
					this.rigidbody2D.AddForce(-Vector2.right * 100f * 10000f);
				}
				else
				{
					this.rigidbody2D.AddForce(Vector2.right * 100f * 10000f);
				}
			}
		}
	}

	public void ApplyDamageEvent()
	{
		if(enemyObj != null)
		{
			if(UnitType == CharacterProperties.UnitType.ARCHER || UnitType == CharacterProperties.UnitType.CANNON) // IF UNIT IS RANGE ATTACK UNIT
			{
				if(enemyObj.GetComponent<Character>().MovementState != CharacterProperties.CharacterState.DEAD)
				{
					float xpos = 0f;
					if(Team == CharacterProperties.Team.LEFT)
					{
						xpos = this.gameObject.transform.position.x + 10f;
					}
					else
					{
						xpos = this.gameObject.transform.position.x - 10f;
					}
					//Vector3 pos = new Vector3(xpos, this.gameObject.transform.position.y, this.gameObject.transform.position.z);
					Vector3 firePos = this.transform.Find("Body/Left Hand/Weapon 1/Fire Position").transform.position;
					GameObject proj = Instantiate(projectile, firePos, Quaternion.identity) as GameObject;
					proj.transform.parent = this.gameObject.transform.parent;

					if(UnitType == CharacterProperties.UnitType.ARCHER)
					{
						if(Team == CharacterProperties.Team.RIGHT)
						{
							proj.transform.localScale = new Vector3(-2f,2f,2f);
							//proj.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
						}
						else
						{
							proj.transform.localScale = new Vector3(2f,2f,2f);
							//proj.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
						}
					}
					else
					{
						// emit sparks
						bombSpark.Play();

						if(Team == CharacterProperties.Team.RIGHT)
						{
							proj.transform.localScale = new Vector3(-1f,1f,1f);
							proj.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
						}
						else
						{
							proj.transform.localScale = new Vector3(1f,1f,1f);
							proj.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
						}

						//proj.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.8f);
					}

					proj.GetComponent<Projectile>().targetEnemy = EnemyObject.gameObject;
					proj.GetComponent<Projectile>().TotalDamage = TotalDamage;
					proj.GetComponent<Projectile>().parent = this;
				}
			}
			else if(UnitType == CharacterProperties.UnitType.DRAGON) // IF UNIT IS DRAGON
			{
				// emit sparks
				bombSpark.Play();

				Transform firePos = this.transform.Find("Dragon Body/Dragon Head/Fire Position").transform;
				GameObject proj = Instantiate(projectile, firePos.position, Quaternion.identity) as GameObject;
				proj.transform.parent = this.gameObject.transform.parent;
				proj.transform.localScale = Vector3.one;

				float angle = Vector3.Angle(this.transform.position, EnemyObject.transform.position);
				float distance = Mathf.Round(Vector3.Distance(this.transform.localPosition, EnemyObject.transform.localPosition) * 10f) / 10f;
				//Debug.Log("angle: " + angle + " : " + distance);

				if(Team == CharacterProperties.Team.RIGHT)
				{
					//proj.transform.localScale = new Vector3(-0.009f,0.009f,0.009f);
					//proj.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
					proj.transform.rotation = proj.transform.rotation * Quaternion.Euler(-30-angle, 90, 0);
				}
				else
				{
					//proj.transform.localScale = new Vector3(0.009f,0.009f,0.009f);
					//proj.transform.rotation = Quaternion.AngleAxis(-90, Vector3.up);
					proj.transform.rotation = proj.transform.rotation * Quaternion.Euler(-30-angle, -90, 0);
				}

				proj.GetComponent<Projectile>().targetEnemy = enemyObj.gameObject;
				proj.GetComponent<Projectile>().TotalDamage = TotalDamage;
				proj.GetComponent<Projectile>().parent = this;
			}
			else if(UnitType != CharacterProperties.UnitType.HEALER) // IF UNIT IS NOT HEALER
			{
				if(enemyObj.GetComponent<Character>().MovementState != CharacterProperties.CharacterState.DEAD)
				{
					float extraDamage = ApplyEffectDamage();
					float bonusDamage = ApplyBonusDamage();

					damageApplyObject = new DamageApplyClass();
					damageApplyObject.damage = -TotalDamage;
					damageApplyObject.criticalDamage = -extraDamage;
					if(bonusDamage > 0f) damageApplyObject.bonusDamage = -bonusDamage;

					enemyObj.SendMessage("ApplyDamage", damageApplyObject, SendMessageOptions.DontRequireReceiver);

					if(SkillType == CharacterProperties.SkillType.SPLASH_DAMAGE)
					{
						ApplySplashDamage(EnemyObject.transform.localPosition.x);
					}
				}
				else
				{
					EnemyObject = null;
				}
			}
			else if(UnitType == CharacterProperties.UnitType.HEALER) // IF UNIT IS A HEALER
			{
				if(enemyObj.GetComponent<Character>().MovementState != CharacterProperties.CharacterState.DEAD)
				{
					damageApplyObject = new DamageApplyClass();
					damageApplyObject.damage = TotalDamage;

					enemyObj.SendMessage("ApplyDamage", damageApplyObject, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					EnemyObject = null;
				}
			}
		}
	}

	public float ApplyEffectDamage()
	{
		float criticalDamage = 0f;
		
		if(SkillType == CharacterProperties.SkillType.KNOCK_BACK && EnemyObject.UnitType != CharacterProperties.UnitType.BUILDING)
		{
			if(GlobalManager.ProbabilityCounter(SkillProbability))
			{
				damageApplyObject = new DamageApplyClass();
				if(SkillDamageOnHit > 0)
				{
					damageApplyObject.damage = -(TotalDamage * (SkillDamageOnHit / 100f));
				}
				
				DamageApplyClass dpsObj = new DamageApplyClass();
				if(SkillDamagePerSecond > 0)
				{
					dpsObj.damage = -(TotalDamage * (SkillDamagePerSecond / 100f));
				}

				damageObject = new DamageClass();
				damageObject.status = CharacterProperties.ExternalStatus.KNOCK_BACK;
				damageObject.duration = SkillDuration;
				damageObject.damageOnHit = damageApplyObject;
				damageObject.damageOvertime = dpsObj;
				damageObject.damageInterval = 0f;
				EnemyObject.SendMessage("SkillDamage", damageObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if(SkillType == CharacterProperties.SkillType.STUN && EnemyObject.UnitType != CharacterProperties.UnitType.BUILDING)
		{
			if(GlobalManager.ProbabilityCounter(SkillProbability))
			{
				damageApplyObject = new DamageApplyClass();
				if(SkillDamageOnHit > 0)
				{
					damageApplyObject.damage = -(TotalDamage * (SkillDamageOnHit / 100f));
				}

				DamageApplyClass dpsObj = new DamageApplyClass();
				if(SkillDamagePerSecond > 0)
				{
					dpsObj.damage = -(TotalDamage * (SkillDamagePerSecond / 100f));
				}

				damageObject = new DamageClass();
				damageObject.status = CharacterProperties.ExternalStatus.STUN;
				damageObject.duration = SkillDuration;
				damageObject.damageOnHit = damageApplyObject;
				damageObject.damageOvertime = dpsObj;
				damageObject.damageInterval = 1f;
				EnemyObject.SendMessage("SkillDamage", damageObject, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if(SkillType == CharacterProperties.SkillType.CRITICAL)
		{
			if(GlobalManager.ProbabilityCounter(SkillProbability))
			{
				criticalDamage = TotalDamage * (SkillDamageOnHit / 100f);
				return criticalDamage;
			}
		}
		else if(SkillType == CharacterProperties.SkillType.RAGE_ATTACK && ExternalStatus == CharacterProperties.ExternalStatus.NONE)
		{
			if(GlobalManager.ProbabilityCounter(SkillProbability))
			{
				StartCoroutine(SkillBooster(CharacterProperties.ExternalStatus.RAGE, SkillDuration));
			}
		}
		else if(SkillType == CharacterProperties.SkillType.POISON_DAMAGE && EnemyObject.ExternalStatus == CharacterProperties.ExternalStatus.NONE && EnemyObject.UnitType != CharacterProperties.UnitType.BUILDING)
		{
			if(GlobalManager.ProbabilityCounter(SkillProbability))
			{
				damageApplyObject = new DamageApplyClass();
				if(SkillDamageOnHit > 0)
				{
					damageApplyObject.damage = -(TotalDamage * (SkillDamageOnHit / 100f));
				}

				DamageApplyClass dpsObj = new DamageApplyClass();
				if(SkillDamagePerSecond > 0)
				{
					dpsObj.damage = -(TotalDamage * (SkillDamagePerSecond / 100f));
				}

				damageObject = new DamageClass();
				damageObject.status = CharacterProperties.ExternalStatus.POISON;
				damageObject.duration = 5f;
				damageObject.damageOnHit = new DamageApplyClass();
				damageObject.damageOvertime = dpsObj;
				damageObject.damageInterval = 1f;
				EnemyObject.SendMessage("SkillDamage", damageObject, SendMessageOptions.DontRequireReceiver);
			}
		}

		return 0f;
	}

	public float ApplyBonusDamage()
	{
		bool bonus = false;

		if(SkillType == CharacterProperties.SkillType.CASTLE_BREAKER && EnemyObject.UnitType == CharacterProperties.UnitType.BUILDING)
		{
			bonus = true;
		}
		else if(SkillType == CharacterProperties.SkillType.FIRE_KILLER && EnemyObject.Category == CharacterProperties.CategoryColour.RED)
		{
			bonus = true;
		}
		else if(SkillType == CharacterProperties.SkillType.WATER_KILLER && EnemyObject.Category == CharacterProperties.CategoryColour.BLUE)
		{
			bonus = true;
		}
		else if(SkillType == CharacterProperties.SkillType.DARK_KILLER && EnemyObject.Category == CharacterProperties.CategoryColour.PURPLE)
		{
			bonus = true;
		}
		else if(SkillType == CharacterProperties.SkillType.NATURE_KILLER && EnemyObject.Category == CharacterProperties.CategoryColour.GREEN)
		{
			bonus = true;
		}
		else if(SkillType == CharacterProperties.SkillType.GROUND_KILLER && EnemyObject.Category == CharacterProperties.CategoryColour.ORANGE)
		{
			bonus = true;
		}
		else if(SkillType == CharacterProperties.SkillType.LIGHT_KILLER && EnemyObject.Category == CharacterProperties.CategoryColour.WHITE)
		{
			bonus = true;
		}

		if(bonus)
		{
			return TotalDamage / 2f;
		}
		else
		{
			return 0f;
		}
	}

	private void ApplyConstantBonusDamage()
	{
		if(bonusDamageDeck != null)
		{
			if(bonusDamageDeck[0] == 1 && SkillType == CharacterProperties.SkillType.FIRE_BOOST && Category == CharacterProperties.CategoryColour.RED)
			{
				TotalDamage += TotalDamage / 2f;
			}
			else if(bonusDamageDeck[1] == 1 && SkillType == CharacterProperties.SkillType.WATER_BOOST && Category == CharacterProperties.CategoryColour.BLUE)
			{
				TotalDamage += TotalDamage / 2f;
			}
			else if(bonusDamageDeck[2] == 1 && SkillType == CharacterProperties.SkillType.DARK_BOOST && Category == CharacterProperties.CategoryColour.PURPLE)
			{
				TotalDamage += TotalDamage / 2f;
			}
			else if(bonusDamageDeck[3] == 1 && SkillType == CharacterProperties.SkillType.NATURE_BOOST && Category == CharacterProperties.CategoryColour.GREEN)
			{
				TotalDamage += TotalDamage / 2f;
			}
			else if(bonusDamageDeck[4] == 1 && SkillType == CharacterProperties.SkillType.GROUND_BOOST && Category == CharacterProperties.CategoryColour.ORANGE)
			{
				TotalDamage += TotalDamage / 2f;
			}
			else if(bonusDamageDeck[5] == 1 && SkillType == CharacterProperties.SkillType.LIGHT_KILLER && Category == CharacterProperties.CategoryColour.WHITE)
			{
				TotalDamage += TotalDamage / 2f;
			}
		}
	}
	
	public virtual void ApplyDamage(DamageApplyClass obj)
	{
		bool applyDamage = true;

		if(SkillType == CharacterProperties.SkillType.PERFECT_DEFENSE && obj.damage < 0)
		{
			if(GlobalManager.ProbabilityCounter(SkillProbability))
			{
				applyDamage = false;
				SpawnSkillEffects(SkillType);
			}
		}

		if(applyDamage)
		{
			float damageReceived = Mathf.Round(obj.damage + obj.criticalDamage + obj.bonusDamage);
			CurrentHitPoint += damageReceived;

			bool criticalHit = obj.criticalDamage < 0 ? true : false;

			if(CurrentHitPoint <= 0)
			{
				//Debug.Log("DIEEEE!");
				MovementState = CharacterProperties.CharacterState.DEAD;
			}
			else
			{
				GameObject indicator = Instantiate(indicatorPrefab, new Vector3(this.gameObject.transform.position.x, this.transform.Find("Highest Point").transform.position.y, this.gameObject.transform.position.z), Quaternion.identity) as GameObject;
				if(Team == CharacterProperties.Team.RIGHT){
					indicator.GetComponent<DamageIndicator>().lobDistance = 0.06f;
				}

				indicator.GetComponent<DamageIndicator>().criticalHit = criticalHit;
				indicator.transform.parent = this.gameObject.transform.parent;
				indicator.transform.localScale = criticalHit == false ? Vector3.one : Vector3.one * 1.5f;
				indicator.GetComponent<UILabel>().text = damageReceived.ToString();

				float xoffset = 0;
				float yoffset = 0f;
				float rotation = 90f;
				if(UnitType == CharacterProperties.UnitType.BUILDING)
				{
					xoffset = 10f;
					yoffset = 30f;

					if(Team == CharacterProperties.Team.LEFT)
					{
						xoffset *= -1;
						rotation *= -1;
					}
				}
				else if(UnitType != CharacterProperties.UnitType.BUILDING)
				{
					//xoffset = AttackRange * 300f;
					xoffset = 0f;
					yoffset = 10f;
					if(Team == CharacterProperties.Team.LEFT)
					{
						xoffset *= -1;
						rotation *= -1;
					}
				}

				if(UnitType != CharacterProperties.UnitType.BUILDING && damageEffectPrefab != null)
				{
					if(damageReceived <= 0) // if its a valid damage
					{
						GameObject damageEffect = Instantiate(damageEffectPrefab, this.gameObject.transform.position, Quaternion.identity) as GameObject;
						damageEffect.transform.parent = this.gameObject.transform.parent;
						//damageEffect.transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);
						damageEffect.transform.rotation = damageEffect.transform.rotation * Quaternion.Euler(340, rotation, 0);
						damageEffect.transform.localScale = new Vector3(1f,1f,1f);
					}
					else // if its not a damage - heal
					{
						GameObject healEffects = Instantiate(effectSettings[0], Vector3.zero, Quaternion.AngleAxis(-90, Vector3.right)) as GameObject;
						healEffects.transform.parent = effectsPoint.transform;
						healEffects.transform.localPosition = Vector3.zero;
						healEffects.transform.localScale = Vector3.one;
					}
				}
			}
		}
	}
	
	public void FadeOutSelf()
	{
		/*if(SkillType == CharacterProperties.SkillType.REVIVAL)
		{
			int percentage = (int)Random.Range(0, 10);
			if(percentage >= 0 && percentage <= 10)
			{
				//this.gameObject.animation["Archer_Dead"].wrapMode = WrapMode.ClampForever;
				//this.gameObject.animation["Archer_Dead"].speed = -1;
				//this.gameObject.animation["Archer_Dead"].time = this.animation["Archer_Dead"].length;
				//this.gameObject.animation.Play("Archer_Dead");

				//animator.animation.Rewind();
				animator.speed = -1;
				StartCoroutine(ResetAnimator(0.5f));
			}
		}*/

		if(UnitType != CharacterProperties.UnitType.BUILDING)
		{
			iTween.FadeTo(this.gameObject, iTween.Hash("time", 1f, "alpha", 0f, "oncompletetarget", this.gameObject, "oncomplete", "DestroySelf"));
		}
	}

	private IEnumerator ResetAnimator(float duration)
	{
		Debug.Log("hello");
		yield return new WaitForSeconds(duration);
		animator.speed = 1;
		MovementState = CharacterProperties.CharacterState.IDLE;
		ExternalStatus = CharacterProperties.ExternalStatus.NONE;
		HitPoint = 100f;
		EnemyObject = null;
		Debug.Log("hello2");
	}

	public void DestroySelf()
	{
		Destroy(this.gameObject);
	}

	private void OnDestroy()
	{
		if(team == CharacterProperties.Team.LEFT)
		{
			characterLeft.Remove(this);
		}
		else if(team == CharacterProperties.Team.RIGHT)
		{
			characterRight.Remove(this);
		}
	}

	public void SkillDamage(DamageClass damageObj)
	{
		if(damageObj != null)
		{
			StartCoroutine(SkillDamage(damageObj.status, damageObj.duration, damageObj.damageOnHit, damageObj.damageOvertime, damageObj.damageInterval));
		}
	}

	private IEnumerator SkillDamage(CharacterProperties.ExternalStatus status, float duration, DamageApplyClass damageOnHit, DamageApplyClass damageOvertime, float damageInterval = 1f)
	{
		this.ExternalStatus = status;
		float damageOvertimeAmount = 0f;

		if(damageOnHit.damage < 0)	ApplyDamage(damageOnHit);
		if(damageOvertime.damage < 0)
		{
			while(true)
			{
				if(damageOvertimeAmount >= duration) break;
				ApplyDamage(damageOvertime);
				damageOvertimeAmount += damageInterval;
				yield return new WaitForSeconds(damageInterval);
			}
		}
		else
		{
			yield return new WaitForSeconds(duration);
		}

		this.ExternalStatus = CharacterProperties.ExternalStatus.NONE;
	}

	private IEnumerator SkillBooster(CharacterProperties.ExternalStatus status, float duration)
	{
		float originalStatus = 0f;

		// modify character status
		this.ExternalStatus = status;
		if(status == CharacterProperties.ExternalStatus.RAGE)
		{
			originalStatus = AttackSpeed;
			AttackSpeed = AttackSpeed * 2f;
		}

		yield return new WaitForSeconds(duration);

		// reset character status
		if(status == CharacterProperties.ExternalStatus.RAGE)
		{
			AttackSpeed = originalStatus;
		}

		this.ExternalStatus = CharacterProperties.ExternalStatus.NONE;
	}

	public void ApplySplashDamage(float position)
	{
		if(Team == CharacterProperties.Team.LEFT)
		{
			for(int i=0; i< characterRight.Count; i++)
			{
				if(characterRight[i] != null && characterRight[i].MovementState != CharacterProperties.CharacterState.DEAD && characterRight[i] != EnemyObject)
				{
					if(characterRight[i].transform.localPosition.x <= (position + 0.1f) && characterRight[i].transform.localPosition.x >= (position - 0.1f))
					{
						damageApplyObject = new DamageApplyClass();
						damageApplyObject.damage = -(TotalDamage / 2f);

						characterRight[i].ApplyDamage(damageApplyObject);
					}
				}
			}
		}
		else
		{
			for(int j=0; j< characterLeft.Count; j++)
			{
				if(characterLeft[j] != null && characterLeft[j].MovementState != CharacterProperties.CharacterState.DEAD && characterLeft[j] != EnemyObject)
				{
					if(characterLeft[j].transform.localPosition.x <= (position + 0.1f) && characterLeft[j].transform.localPosition.x >= (position - 0.1f))
					{
						damageApplyObject = new DamageApplyClass();
						damageApplyObject.damage = -(TotalDamage / 2f);
						
						characterLeft[j].ApplyDamage(damageApplyObject);
					}
				}
			}
		}
	}

	private void SpawnSkillEffects(CharacterProperties.SkillType skillType)
	{
		switch(skillType)
		{
		case CharacterProperties.SkillType.HEAL:

			break;
		case CharacterProperties.SkillType.PERFECT_DEFENSE:
			if(persistentEffectHolder == null)
			{
				persistentEffectHolder = Instantiate(effectSettings[3], Vector3.zero, Quaternion.identity) as GameObject;
				persistentEffectHolder.transform.parent = effectsPoint.transform;
				persistentEffectHolder.transform.localPosition = new Vector3(3f,8f,0f);
				persistentEffectHolder.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
				StartCoroutine(ClearEffects(0.5f));
			}
			break;
		default:
			Destroy(persistentEffectHolder);
			persistentEffectHolder = null;
			break;
		}
	}

	private IEnumerator ClearEffects(float duration)
	{
		yield return new WaitForSeconds(duration);
		SpawnSkillEffects(CharacterProperties.SkillType.NONE);
	}

	public void UpdateTargetEnemy()
	{
		if(EnemyObject != null)
		{
			if(EnemyObject.GetComponent<Building>() != null)
			{
				//EnemyObject = null;
				EnemyObject = defObj.GetNextEnemy(this.Team, this.transform.localPosition.x, this.AttackType, this.AttackRange, this.UnitType);
			}
		}
	}

	#region GETTER & SETTER FUNCTIONS

	public float HitPoint
	{
		set{ maxHitPoint = CurrentHitPoint = Mathf.Round(value); }
		get{ return maxHitPoint; }
	}

	public float CurrentHitPoint
	{
		set{
			currentHitPoint = value;
			if(currentHitPoint > HitPoint) currentHitPoint = HitPoint;

			if(hitPointProgressBar)
			{
				float hpPercentage = CurrentHitPoint / HitPoint;
				if(hpPercentage <= 0)
				{
					hpPercentage = 0;
					hitPointProgressBar.value = hpPercentage;
					hitPointProgressBar.gameObject.SetActive(false);
				}
				else if(hpPercentage >= 1f)
				{
					hitPointProgressBar.gameObject.SetActive(false);
				}
				else
				{
					hitPointProgressBar.value = hpPercentage;
				}

				if(DisplayUnit) hitPointProgressBar.gameObject.SetActive(false);
			}
		}
		get{ return currentHitPoint; }
	}

	public float TotalDamage
	{
		set{ damage = Mathf.Round(value); }
		get{ return damage; }
	}

	public int TotalDefense
	{
		set{ defense = value; }
		get{ return defense; }
	}

	public float MoveSpeed
	{
		set{ moveSpeed = value; }
		get{ return moveSpeed; }
	}

	public float AttackSpeed
	{
		set{
			attackSpeed = value;
			if(animator != null) animator.speed = attackSpeed;
		}
		get{ return attackSpeed; }
	}

	public float AttackRange
	{
		set{ attackRange = value; }
		get{ return attackRange; }
	}

	public CharacterProperties.AttackType AttackType
	{
		set{ attackType = value; }
		get{ return attackType; }
	}

	public CharacterProperties.PositionType PositionType
	{
		set{ positionType = value; }
		get{ return positionType; }
	}

	public float UnitSize
	{
		set{
			unitSize = value;
			if(team == CharacterProperties.Team.RIGHT){
				this.transform.localScale = new Vector3(-this.UnitSize, this.UnitSize, this.UnitSize);
			}else{
				this.transform.localScale = new Vector3(this.UnitSize, this.UnitSize, this.UnitSize);
			}
		}
		get{ return unitSize; }
	}

	public CharacterProperties.CharacterState MovementState
	{
		set{
			movementState = value;

			if(animator != null)
			{
				animator.SetInteger("MovementState", (int)movementState);
				if(movementState == CharacterProperties.CharacterState.IDLE)
				{
					animator.speed = 1f;
					if(this.rigidbody2D != null) this.rigidbody2D.velocity = Vector2.zero;
				}
				else if(movementState == CharacterProperties.CharacterState.WALKING)
				{
					animator.speed = this.MoveSpeed * 10f;
				}
				else if(movementState == CharacterProperties.CharacterState.ATTACKING)
				{
					animator.speed = this.AttackSpeed;
					if(this.rigidbody2D != null) this.rigidbody2D.velocity = Vector2.zero;
				}
				else if(movementState == CharacterProperties.CharacterState.DEAD)
				{
					animator.speed = 1f;
					if(this.rigidbody2D != null) this.rigidbody2D.velocity = Vector2.zero;
				}
			}
		}
		get{ return movementState; }
	}

	public CharacterProperties.Team Team
	{
		set{ team = value; }
		get{ return team; }
	}

	public CharacterProperties.ExternalStatus ExternalStatus
	{
		set{
			externalStatus = value;
			if(externalStatus != CharacterProperties.ExternalStatus.NONE && externalStatus != CharacterProperties.ExternalStatus.RAGE && externalStatus != CharacterProperties.ExternalStatus.POISON)
			{
				MovementState = CharacterProperties.CharacterState.IDLE;
				EnemyObject = null;
			}

			switch(externalStatus)
			{
			case CharacterProperties.ExternalStatus.POISON:
				persistentEffectHolder = Instantiate(effectSettings[1], this.transform.position, Quaternion.AngleAxis(-90, Vector3.right)) as GameObject;
				persistentEffectHolder.transform.parent = effectsPoint.transform;
				persistentEffectHolder.transform.localPosition = Vector3.zero;
				persistentEffectHolder.transform.localScale = Vector3.one;
				break;
			case CharacterProperties.ExternalStatus.STUN:
				if(persistentEffectHolder == null)
				{
					float xoffset = -5f;
					if(Team == CharacterProperties.Team.RIGHT)
					{
						xoffset = 5f;
					}
					persistentEffectHolder = Instantiate(effectSettings[2], this.transform.position, Quaternion.AngleAxis(74, Vector3.right)) as GameObject;
					persistentEffectHolder.transform.parent = effectsPoint.transform;
					persistentEffectHolder.transform.localPosition = new Vector3(0f, (headSize/10f)+2f, 0f);
					persistentEffectHolder.transform.localScale = new Vector3(0.12f, 0.12f, 0.12f);
				}
				break;
			case CharacterProperties.ExternalStatus.RAGE:
				persistentEffectHolder = Instantiate(effectSettings[4], Vector3.zero, Quaternion.AngleAxis(-90, Vector3.right)) as GameObject;
				persistentEffectHolder.transform.parent = effectsPoint.transform;
				persistentEffectHolder.transform.localPosition = new Vector3(0, 10f, 0f);
				persistentEffectHolder.transform.localScale = Vector3.one;
				break;
			case CharacterProperties.ExternalStatus.NONE:
				Destroy(persistentEffectHolder);
				persistentEffectHolder = null;
				break;
			}
		}
		get{ return externalStatus; }
	}

	public Character EnemyObject
	{
		set{
			enemyObj = value;
			if(ExternalStatus == CharacterProperties.ExternalStatus.NONE)
			{
				if(enemyObj != null)
				{
					MovementState = CharacterProperties.CharacterState.ATTACKING;
				}
				else
				{
					MovementState = CharacterProperties.CharacterState.WALKING;
				}
			}
		}
		get{ return enemyObj; }
	}
	
	public CharacterProperties.UnitType UnitType
	{
		set { unitType = value; }
		get { return unitType; }
	}

	public CharacterProperties.SkillType SkillType
	{
		set { skillType = value; }
		get { return skillType; }
	}

	public int SkillLevel
	{
		set { skillLevel = value; }
		get { return skillLevel; }
	}

	public int SkillProbability
	{
		set { skillProbability = value; }
		get { return skillProbability; }
	}

	public float SkillDuration
	{
		set { skillDuration = value; }
		get { return skillDuration; }
	}

	public float SkillDamageOnHit
	{
		set { skillDamageOnHit = value; }
		get { return skillDamageOnHit; }
	}

	public float SkillDamagePerSecond
	{
		set { skillDamagePerSecond = value; }
		get { return skillDamagePerSecond; }
	}

	public float SkillAttackRange
	{
		set { skillAttackRange = value; }
		get { return skillAttackRange; }
	}

	public CharacterProperties.CategoryColour Category
	{
		set{ category = value; }
		get{ return category; }
	}

	public bool DisplayUnit
	{
		set{ displayUnit = value; }
		get{ return displayUnit; }
	}

	#endregion
}

public static class Physics2DExtensions {
	public static void AddForce (this Rigidbody2D rigidbody2D, Vector2 force, ForceMode mode = ForceMode.Force) {
		switch (mode) {
		case ForceMode.Force:
			rigidbody2D.AddForce (force);
			break;
		case ForceMode.Impulse:
			rigidbody2D.AddForce (force / Time.fixedDeltaTime);
			break;
		case ForceMode.Acceleration:
			rigidbody2D.AddForce (force * rigidbody2D.mass);
			break;
		case ForceMode.VelocityChange:
			rigidbody2D.AddForce (force * rigidbody2D.mass / Time.fixedDeltaTime);
			break;
		}
	}
	
	public static void AddForce (this Rigidbody2D rigidbody2D, float x, float y, ForceMode mode = ForceMode.Force) {
		rigidbody2D.AddForce (new Vector2 (x, y), mode);
	}
}

public class DamageClass
{
	public CharacterProperties.ExternalStatus status;
	public float duration;
	public DamageApplyClass damageOnHit;
	public DamageApplyClass damageOvertime;
	public float damageInterval = 1f;
}

public class DamageApplyClass
{
	public float damage = 0f;
	public float criticalDamage = 0f;
	public float bonusDamage = 0f;
}
