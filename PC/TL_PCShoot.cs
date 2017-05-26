using UnityEngine;

public class TL_PCShoot : MonoBehaviour {

    //Variables
    public GameObject Projectile;
    public float ProjectileSpeed;
    public float AttackCooldown;
    private float TimeToShootNext;
    public bool SpreadShot;
    public AudioClip LaserSound;
    private AudioSource SoundSource;
    


    void Start()
    {
        //Obtain the audio source component from the gameobject
        SoundSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //Function for shooting projectiles
        ShootProjectile();
    }
    
    void ShootProjectile()
    {
        //If the player touches the screen
        if (Input.GetMouseButton(0))
        {
            //If the cooldown is lower than the time
            if (TimeToShootNext < Time.time)
            {
                //Play the sound
                TL_AudioManager.PlaySound(SoundSource, LaserSound);

                //If the spread shot has been obtained
                if (SpreadShot)
                {
                    //Initialize Vector2 variables for both the left and right diagonal directions
                    Vector2 DiagonalLeftDir = new Vector2(-1f, 1.5f);
                    Vector2 DiagonalRightDir = new Vector2(1f, 1.5f);

                    //Spawn the projectile in front of the PC while rotated at an angle
                    GameObject Projectile01Clone = Instantiate(Projectile, new Vector3(transform.position.x - 0.1f, transform.position.y + 0.75f, 0f), Quaternion.LookRotation(Vector3.forward, DiagonalLeftDir));

                    //Obtain the rigidbody 2D from the projectile and make the projectile move towards a normalized direction
                    Projectile01Clone.GetComponent<Rigidbody2D>().AddForce(DiagonalLeftDir.normalized * ProjectileSpeed, ForceMode2D.Impulse);

                    //Spawn the projectile in front of the PC while rotated at an angle
                    GameObject Projectile02Clone = Instantiate(Projectile, new Vector3(transform.position.x + 0.1f, transform.position.y + 0.75f, 0f), Quaternion.LookRotation(Vector3.forward, DiagonalRightDir));

                    //Obtain the rigidbody 2D from the projectile and make the projectile move towards a normalized direction
                    Projectile02Clone.GetComponent<Rigidbody2D>().AddForce(DiagonalRightDir.normalized * ProjectileSpeed, ForceMode2D.Impulse);
                }
                else
                {
                    //Spawn the projectile in front of the PC
                    GameObject ProjectileClone = Instantiate(Projectile, new Vector3(transform.position.x, transform.position.y + 0.75f, 0f), Quaternion.identity);

                    //Obtain the rigidbody 2D from the projectile and make the projectile move towards a direction
                    ProjectileClone.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, ProjectileSpeed), ForceMode2D.Impulse);
                }

                //Add the time to shoot next
                TimeToShootNext = AttackCooldown + Time.time;
            }

        }
        
    }

}
