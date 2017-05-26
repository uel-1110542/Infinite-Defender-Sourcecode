using UnityEngine;

public class TL_DummyBehaviour : MonoBehaviour
{
    //Variables
    public AudioClip ExplosionSound;
    private AudioSource SoundSource;
    private Animator Explosion_Animator;



    void Start()
    {
        //Obtain the audio source component from the gameobject
        SoundSource = GetComponent<AudioSource>();

        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Function for bringing the dummy back to the starting position
        Teleport();
    }

    void Teleport()
    {
        //The if statement checks to see if the explosion animation is enabled or not
        //to prevent the explosion animation being played while the NPC still moves
        if (!Explosion_Animator.enabled)
        {
            //If the dummy moves offscreen then teleport the dummy back to the starting position
            if (transform.position.x > 10f)
            {
                transform.position = new Vector2(-9f, 3f);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col2D)
    {
        //If the PC collides with the projectile from the NPC
        if (col2D.transform.name == "PC_Projectile(Clone)")
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

            //Destroy the projectile
            Destroy(col2D.gameObject);

            //Destroy itself when the explosion animation ends
            Destroy(gameObject, Explosion_Animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

}
