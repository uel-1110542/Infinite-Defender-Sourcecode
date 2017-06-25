using UnityEngine;
using System.Collections;

public class TL_BossFSM : MonoBehaviour {

    //Boss Stats
    [Header("Stats")]
    public float MaxHealth;
    public float Health;
    public float MoveSpeed;
    public float AttackCooldown;
    private float TimeToNextAttack;
    public float MoveLimit;

    //Projectile
    [Header("Projectile")]
    public GameObject Projectile;
    public float ProjectileSpeed;

    //State Changes
    [Header("State Changes")]
    public Material CurrentMat;
    public Material WhiteMat;
    public bool StateChange;
    public string CurrentState;
    private TL_LevelManager LevelManagerScript;
    private TL_ScoreManager ScoreManagerScript;

    //Sound
    [Header("Sound")]
    public AudioClip ExplosionSound;
    private AudioSource SoundSource;

    //Animator
    private Animator Explosion_Animator;



    void Start()
    {
        //Find the level manager
        GameObject LevelManager = GameObject.Find("LevelManager");

        //Obtain the audio source component from the gameobject
        SoundSource = GetComponent<AudioSource>();

        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();

        //Find the level manager and obtain the scripts
        LevelManagerScript = LevelManager.GetComponent<TL_LevelManager>();
        ScoreManagerScript = LevelManager.GetComponent<TL_ScoreManager>();
        
        //Add the cooldowns at the start to prevent the NPC from immediate action
        TimeToNextAttack = AttackCooldown + Time.time;

        //Obtain the material from this gameobject
        CurrentMat = GetComponent<Renderer>().material;

        //If the level is beyond 19
        if (LevelManagerScript.Level > 19)
        {
            //Increase the health based on the level multiplied by a value
            MaxHealth += LevelManagerScript.Level * 20;
        }

        //Set initial value for health
        Health = MaxHealth;

        //Set initial state
        CurrentState = "Attack";
    }

	void Update()
    {
        //If the NPC is still alive
        if (Health > 0f)
        {
            //Function for moving the NPC
            MoveNPC();

            //Shooting the projectile when the state is Attack
            ShootProjectile();
        }
    }

    void MoveNPC()
    {
        //Move towards the sides of the screen
        transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(MoveLimit, transform.position.y), MoveSpeed * Time.deltaTime);

        //Convert world space into viewport space
        Vector3 ScreenBoundaries = Camera.main.WorldToViewportPoint(transform.position);

        //If the NPC moves towards the boundaries of the screen then change the move limit
        if (ScreenBoundaries.x <= 0.1f)
        {
            MoveLimit = 8f;
        }
        else if (ScreenBoundaries.x >= 0.9f)
        {
            MoveLimit = -8f;
        }

    }

    void ShootProjectile()
    {
        //If the cooldown is less than the time
        if (CurrentState == "Attack" && TimeToNextAttack < Time.time)
        {
            //Create the projectile
            GameObject ProjectileClone = Instantiate(Projectile, new Vector3(transform.position.x, transform.position.y - 1f, 0f), Quaternion.identity);

            //Obtain the rigidbody 2D from the projectile and make the projectile move towards a direction
            ProjectileClone.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, ProjectileSpeed), ForceMode2D.Impulse);

            //Add the time to next attack
            TimeToNextAttack = AttackCooldown + Time.time;
        }
    }

    IEnumerator HitIndicator()
    {
        //Change the opacity of the sprite back and fourth while waiting for a fraction of a second 
        //to create an indication for the NPC when hit
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.2f);
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.2f);
        yield return new WaitForSeconds(0.05f);
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
    }

    void OnCollisionEnter2D(Collision2D col2D)
    {
        //If the NPC collides into a projectile from the PC
        if (col2D.transform.name == "PC_Projectile(Clone)" && CurrentState != "Shielded")
        {
            //Subtract health
            Health -= 10f;

            //The condition for the NPC's health to be above 0 is to prevent
            //the hit indicator from occuring during the explosion animation
            if (Health > 0f)
            {
                //Show a visual indication of damage
                StartCoroutine(HitIndicator());
            }

            //Destroy the projectile
            Destroy(col2D.gameObject);

            //If the NPC's health drops to 0 or below
            if (Health <= 0f)
            {
                //Play the sound effect
                TL_AudioManager.PlaySound(SoundSource, ExplosionSound);

                //If the animator is not enabled
                if (!Explosion_Animator.enabled)
                {
                    //Enable the animator
                    Explosion_Animator.enabled = true;
                }

                //If the boss' name is Orbiter
                if (transform.name == "Orbiter(Clone)")
                {
                    //Obtain the script and disable it
                    TL_ShieldManager ShieldManagerScript = GetComponent<TL_ShieldManager>();
                    ShieldManagerScript.enabled = false;

                    //Loop through all of the guardians
                    foreach (Transform Child in transform)
                    {
                        //Obtain the script from the child gameobject
                        TL_Orbit OrbitScript = Child.GetComponent<TL_Orbit>();

                        //Disable the script
                        OrbitScript.enabled = false;

                        //Obtain the animator form the child gameobject
                        Animator ExplosionAnimator = Child.GetComponent<Animator>();

                        //If the animator is not enabled
                        if (!ExplosionAnimator.enabled)
                        {
                            //Enable the animator
                            ExplosionAnimator.enabled = true;
                        }
                    }
                }

                //Find all of the boss spawns in the scene
                GameObject[] BossSpawns = GameObject.FindGameObjectsWithTag("BossSpawn");

                //Loop through all of the boss spawns in the scene
                foreach (GameObject go in BossSpawns)
                {
                    //Obtain the animator form the child gameobject
                    Animator ExplosionAnimator = go.GetComponent<Animator>();

                    //If the animator is not enabled
                    if (!ExplosionAnimator.enabled)
                    {
                        //Enable the animator
                        ExplosionAnimator.enabled = true;
                    }
                }

                //Give the player a score depending on the name of the boss
                if (gameObject.name == "Swarm(Clone)")
                {
                    ScoreManagerScript.Score += 500;
                }
                else if (gameObject.name == "Orbiter(Clone)")
                {
                    ScoreManagerScript.Score += 1000;
                }
                else if (gameObject.name == "Sentinel(Clone)")
                {
                    ScoreManagerScript.Score += 1500;
                }
                else if (gameObject.name == "Mothership(Clone)")
                {
                    ScoreManagerScript.Score += 1000;
                }
                //Set the current health to 0
                Health = 0f;

                //Destroy the NPC when the explosion animation ends
                Destroy(gameObject, Explosion_Animator.GetCurrentAnimatorStateInfo(0).length);
            }
        }
    }

}
