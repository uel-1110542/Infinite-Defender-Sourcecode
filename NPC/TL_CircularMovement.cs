using UnityEngine;

public class TL_CircularMovement : MonoBehaviour {

    //Variables
    [Header("Speed and Size of Circular Movement")]
    public float Angle;
    public float Radius;
    public float MoveSpeed;
    public float RotatorySpeed;
    public float Width;
    public float Height;
    private Animator Explosion_Animator;



    void Start()
    {
        //Set the width and height as the NPC's initial X and Y position
        Width = transform.position.x;
        Height = transform.position.y;

        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();
    }

	void Update()
    {
        //Function for moving the NPC in a circular motion
        CircularMove();
    }

    void CircularMove()
    {
        //The if statement checks to see if the explosion animation is enabled or not
        //to prevent the explosion animation being played while the NPC still moves
        if (!Explosion_Animator.enabled)
        {
            //Calculate the angle of the circle motion with the rotatory speed multiplied with the delta time
            Angle += RotatorySpeed * Time.deltaTime;

            //Use Cos and Sin to calculate the angles
            float x = (Radius * Mathf.Cos(Angle)) + Width;
            float y = (Radius * Mathf.Sin(Angle)) + Height;

            //Set the X and Y as the new positions
            transform.position = new Vector2(x, y);

            //Constanly move the circular motion with the width of the circle by
            //subtracting the move speed multiplied by the delta time
            Width -= MoveSpeed * Time.deltaTime;
        }            
    }

}
