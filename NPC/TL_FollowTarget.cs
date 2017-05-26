using UnityEngine;

public class TL_FollowTarget : MonoBehaviour {


        

	void Update()
    {
        //Follow the PC
        FollowPC();
    }

    void FollowPC()
    {
        //Find the PC
        GameObject PC_Clone = GameObject.Find("PC(Clone)");

        //If the PC is still alive
        if (PC_Clone != null)
        {
            //Make the projectile move towards the PC slowly
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(PC_Clone.transform.position.x, transform.position.y), 3f * Time.deltaTime);
        }        

    }

}
