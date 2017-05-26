using UnityEngine;

public class TL_BlockProjectiles : MonoBehaviour {


    

    void OnCollisionEnter2D(Collision2D col2D)
    {
        //If the projectile from the PC enters the shield, destroy it
        if (col2D.transform.name == "PC_Projectile(Clone)")
        {
            Destroy(col2D.gameObject);
        }

    }


}
