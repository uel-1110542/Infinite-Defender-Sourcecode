using UnityEngine;

public class TL_RandomMovement : MonoBehaviour {

    [Header("Movement")]
    public float MoveSpeed;
    public float MoveRate;
    private float MoveCooldown;
    private Vector2 TargetPos;
    private int RandomIndex;
    private Animator Explosion_Animator;



    void Start()
    {
        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();

        //Add cooldown to prevent immediate action
        MoveCooldown = MoveRate + Time.time;
    }

	void Update()
    {
        //The if statement checks if the explosion animation is not played to
        //prevent the NPC from moving while the explosion animation is on
        if (!Explosion_Animator.enabled)
        {
            //Randomly move around the scene
            MoveRandomly();

            //Move the NPC
            AnimateMovement();
        }
    }

    void MoveRandomly()
    {
        //If the cooldown is less than time
        if (MoveCooldown < Time.time)
        {
            //Randomly assign a new target position
            TargetPos = new Vector2(Rerandomize(-6, 6), Rerandomize(0, 4));

            //Add the cooldown
            MoveCooldown = MoveRate + Time.time;
        }
    }

    void AnimateMovement()
    {
        if (Vector2.Distance(transform.position, TargetPos) < 0.1f)
        {
            //Set the target position as the new position
            transform.position = TargetPos;
        }
        else
        {
            //Lerp towards the target position
            transform.position = Vector2.Lerp(transform.position, TargetPos, MoveSpeed * Time.deltaTime);
        }
        //Clamp the X and Y values
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -8f, 8f), Mathf.Clamp(transform.position.y, 0f, 4f));
    }

    int Rerandomize(int min_val, int max_val)
    {
        //Randomize the number between the minimum and maximum value
        int Ran_Num = Random.Range(min_val, max_val);

        //While the random number is the same as the previous random number, re-randomize it
        while (Ran_Num == RandomIndex)
        {
            //Randomize the number between the minimum and maximum value again
            Ran_Num = Random.Range(min_val, max_val);
        }
        //Assign the variable to the random number
        RandomIndex = Ran_Num;

        //Return the value
        return RandomIndex;
    }

}
