using UnityEngine;

public class TL_MoveOffscreen : MonoBehaviour {

    
    public Vector2 RandomTargetPos;
    public float MoveSpeed;
    private Animator Explosion_Animator;



    void Start()
    {
        //Set an initial random target position
        RandomTargetPos = new Vector2(Random.Range(-8f, 8f), -6f);

        //Obtain the animator from the gameobject
        Explosion_Animator = GetComponent<Animator>();
    }

	void Update()
    {
        //The if statement checks if the explosion animation is not played to
        //prevent the NPC from moving while the explosion animation is on
        if (!Explosion_Animator.enabled)
        {
            //Set the target position randomly
            SetTargetPos();

            //Animate the movement
            AnimateMovement();
        }
    }

    void AnimateMovement()
    {
        if (Vector2.Distance(transform.position, RandomTargetPos) < 0.1f)
        {
            //Set the random target position as the new position
            transform.position = RandomTargetPos;

            //Randomize a new target position
            RandomTargetPos = new Vector2(Random.Range(-8f, 8f), -6f);
        }
        else
        {
            //Move towards the target position
            transform.position = Vector2.MoveTowards(transform.position, RandomTargetPos, MoveSpeed * Time.deltaTime);
        }
    }

    void SetTargetPos()
    {
        //If the NPC moves off the camera view
        if (transform.position.x <= -10f || transform.position.y <= -6f)
        {
            //Teleport the NPC back to the top of the screen with a randomized X and Y position
            transform.position = new Vector2(Random.Range(-8f, 8f), 6f);
        }
    }

}
