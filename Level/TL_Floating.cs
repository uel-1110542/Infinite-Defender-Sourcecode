using UnityEngine;

public class TL_Floating : MonoBehaviour {

    //Variables
    private Vector2 RanDir;
    private Rigidbody2D Powerup_RB;
    
    

    void Start()
    {
        //Set the Vector2 as a random direction for the powerup to float towards to
        RanDir = new Vector2(Random.Range(-1f, 1f), -1f);

        //Normalize the Vector2 variable
        RanDir.Normalize();

        //Obtain the 2D rigidbody
        Powerup_RB = GetComponent<Rigidbody2D>();

        //Add the force with the time.deltatime and divide the random direction with 1.25f to slow the speed down
        Powerup_RB.AddForce((RanDir / 1.25f) * Time.deltaTime);
    }

    void Update()
    {
        //Bounces the powerup off the sides of the screen
        BounceOffBoundaries();
    }

    void BounceOffBoundaries()
    {
        //Converts the transform position from the world point to the viewport point
        Vector3 ViewportPoint = Camera.main.WorldToViewportPoint(transform.position);

        //If the powerup floats towards the left side of the screen
        if (ViewportPoint.x <= 0.025f)
        {
            //Reflect the vector 2 with the velocity and direction
            RanDir = Vector2.Reflect(Powerup_RB.velocity, Vector2.left);

            //Set the new velocity
            Powerup_RB.velocity = RanDir;
        }
        else if (ViewportPoint.x >= 0.975f)    //If the powerup floats towards the right side of the screen
        {
            //Reflect the vector 2 with the velocity and direction
            RanDir = Vector2.Reflect(Powerup_RB.velocity, Vector2.right);

            //Set the new velocity
            Powerup_RB.velocity = RanDir;
        }
        
    }

}
