using UnityEngine;

public class TL_DirectionalMove : MonoBehaviour {

    //Variables
    public float MoveSpeed;
    public bool ChangeDir;
    private Animator Explosion_Animator;


    void Start()
    {
        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Function for managing movement
        DirectionalMovement();
    }

    void DirectionalMovement()
    {
        //The if statement checks to see if the explosion animation is enabled or not
        //to prevent the explosion animation being played while the NPC still moves
        if (!Explosion_Animator.enabled)
        {
            //If the direction hasn't changed then move it towards the left side of the screen
            if (!ChangeDir)
            {
                transform.position += Vector3.left * Time.deltaTime * MoveSpeed;
            }
            else
            {
                //If the direction has changed then move it towards the bottom of the screen
                transform.position += Vector3.down * Time.deltaTime * MoveSpeed;
            }
        }
    }

}
