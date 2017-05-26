using UnityEngine;
using System.Collections;

public class TL_NPC_Attack : MonoBehaviour {

    
    public GameObject NPC_Projectile;
    
    [Header("NPC Stats")]
    public float NPC_MaxHealth;
    public float NPC_Health;
    public float NPC_ProjectileSpeed;
    public float NPC_AttackCooldown;
    private float NPC_TimeToNextAttack;
    public bool IsNPCAiming;

    [Header("NPC State")]
    public string NPC_State;

    [Header("Hit Indicator")]
    public Material CurrentMat;
    public Material WhiteMat;

    [Header("Powerups")]
    public GameObject AtkSpdPowerup;
    public GameObject MoveSpdPowerup;
    public GameObject DefPowerup;
    public GameObject Shieldup;
    public GameObject BaseHealthup;

    [Header("Sound")]
    public AudioClip ExplosionSound;
    private AudioSource SoundSource;
    private TL_ScoreManager ScoreManagerScript;

    //Animator
    private Animator Explosion_Animator;



    void Start()
    {
        //Find the level manager and obtain the script
        ScoreManagerScript = GameObject.Find("LevelManager").GetComponent<TL_ScoreManager>();

        //Obtain the audio source component from the gameobject
        SoundSource = GetComponent<AudioSource>();

        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();

        //Set default value for current health as maximum health
        NPC_Health = NPC_MaxHealth;

        //Obtain the material from this gameobject
        CurrentMat = GetComponent<Renderer>().material;

        //Add initial time to shoot to prevent immediate attack
        NPC_TimeToNextAttack = NPC_AttackCooldown + Time.time;
    }

    void Update()
    {
        //The function for shooting a projectile
        ShootProjectile();
    }
    
    void ShootProjectile()
    {
        //This if statement prevents the NPC from shooting a projectile during the explosion animation
        if (NPC_Health > 0f)
        {
            //If the NPC is aiming at the PC
            if (IsNPCAiming)
            {
                //Find the PC
                GameObject PC = GameObject.Find("PC(Clone)");

                //If the PC is still alive
                if (PC != null)
                {
                    //Calcuate the distance between the NPC and the PC
                    Vector3 TargetDir = transform.position - PC.transform.position;

                    //Make the NPC look towards the target
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, new Vector2(TargetDir.x, TargetDir.y));
                }
            }

            //If the time to next attack is lower than the time
            if (NPC_TimeToNextAttack < Time.time)
            {
                //Spawn the projectile in front of the NPC and make it face the target
                GameObject ProjectileClone = Instantiate(NPC_Projectile, new Vector3(transform.position.x, transform.position.y - 0.5f, 0f), transform.rotation);

                //Obtain the rigidbody 2D from the projectile
                Rigidbody2D ProjectilePhysics = ProjectileClone.GetComponent<Rigidbody2D>();

                //Force the projectile to shoot downwards
                ProjectilePhysics.AddRelativeForce((new Vector2(0f, 1f) * NPC_ProjectileSpeed), ForceMode2D.Impulse);

                //Add the cooldown
                NPC_TimeToNextAttack = NPC_AttackCooldown + Time.time;
            }
        }        
    }

    IEnumerator HitIndicator()
    {
        //Change the opacity of the sprite and wait for a fraction of a second then
        //change the opacity back to normal then repeat to create an indication for the NPC when hit
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.2f);
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.2f);
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
    }

    void CheckDeathState()
    {
        //If the NPC's current health falls to zero or below, destroy itself
        if (NPC_Health <= 0f)
        {
            //Destroy the box collider to prevent projectiles colliding into the explosion animation
            Destroy(GetComponent<BoxCollider2D>());

            //Play the sound effect
            TL_AudioManager.PlaySound(SoundSource, ExplosionSound);

            //If the animator is not enabled
            if (!Explosion_Animator.enabled)
            {
                //Enable the animator
                Explosion_Animator.enabled = true;
            }

            //Give the player a score depending on the name of the enemy
            if (gameObject.name == "Scout(Clone)" || gameObject.name == "Spawnling(Clone)" || gameObject.name == "Guardian(Clone)")
            {
                ScoreManagerScript.Score += 100;
            }
            else if (gameObject.name == "Sweeper(Clone)" || gameObject.name == "Snake(Clone)")
            {
                ScoreManagerScript.Score += 150;
            }
            else if (gameObject.name == "Looper(Clone)" || gameObject.name == "Sentry(Clone)" || gameObject.name == "Blitzer(Clone)")
            {
                ScoreManagerScript.Score += 200;
            }

            //Random chance of either the attack speed, movement speed or defense powerup to drop
            int Ran_Spawn = Random.Range(0, 5);

            //A 1 in 20 chance of dropping a powerup
            int RandomChance = Random.Range(0, 21);

            //If random chance is equal to 10
            if (RandomChance == 10)
            {
                //Depending on the value it raondomizes, it will spawn either an attack speed, movement speed or a defense powerup
                switch (Ran_Spawn)
                {
                    case 0:
                        Instantiate(AtkSpdPowerup, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                        break;

                    case 1:
                        Instantiate(MoveSpdPowerup, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                        break;

                    case 2:
                        Instantiate(DefPowerup, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                        break;

                    case 3:
                        Instantiate(Shieldup, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                        break;

                    case 4:
                        Instantiate(BaseHealthup, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                        break;
                }
            }
            //Destroy the NPC when the explosion animation ends
            Destroy(gameObject, Explosion_Animator.GetCurrentAnimatorStateInfo(0).length);
        }        
    }

    void OnCollisionEnter2D(Collision2D col2D)
    {
        //If the NPC collides with the PC itself
        if (col2D.transform.tag == "PC")
        {
            //Subtract current health
            NPC_Health -= 30f;

            //Check if the NPC is still alive or not
            CheckDeathState();
        }

        //If the PC collides with the projectile from the NPC
        if (col2D.transform.name == "PC_Projectile(Clone)")
        {
            //Destroy the projectile
            Destroy(col2D.gameObject);

            //Subtract current health
            NPC_Health -= 10f;

            //If the NPC's health is above 0
            if (NPC_Health > 0f)
            {
                //Display the NPC being hit by a projectile
                StartCoroutine(HitIndicator());
            }

            //Check if the NPC is still alive or not
            CheckDeathState();
        }

        

    }

}
