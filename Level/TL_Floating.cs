using UnityEngine;

public class TL_Floating : MonoBehaviour {

    


    void Start()
    {
        //Set the Vector2 as a random direction for the powerup to float towards to
        Vector2 RanDir = new Vector2(Random.Range(-1f, 1f), -1f);

        //Normalize the Vector2 variable
        RanDir.Normalize();

        //Obtain the 2D rigidbody
        Rigidbody2D Powerup_RB = GetComponent<Rigidbody2D>();

        //Add the force with the time.deltatime and divide the random direction with 1.25f to slow the speed down
        Powerup_RB.AddForce((RanDir / 1.25f) * Time.deltaTime);
    }

}
