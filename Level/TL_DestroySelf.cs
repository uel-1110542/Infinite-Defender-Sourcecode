using UnityEngine;

public class TL_DestroySelf : MonoBehaviour {


	

	void Update()
    {
        //Function for checking position to see if the gameobject has gone outside of boundaries
        CheckPos();
    }

    void CheckPos()
    {
        //If this gameobject has gone outside camera view then destroy itself
        if (transform.position.x < -10f || transform.position.x > 10f || transform.position.y < -6f || transform.position.y > 6f)
        {
            //If the gameobject is either the NPC or a boss spawn
            if (transform.tag == "NPC" || transform.tag == "BossSpawn")
            {
                //Find the level manager and obtain the script
                TL_LevelManager LM_Script = GameObject.Find("LevelManager").GetComponent<TL_LevelManager>();

                //Subtract health from the space station
                LM_Script.Base_HP -= 10f;

                //If the space station's health is equal to 0 or below, set it to 0
                if (LM_Script.Base_HP <= 0f)
                {
                    LM_Script.Base_HP = 0f;
                }
            }
            //Destroy the gameobject
            Destroy(gameObject);
        }
    }

}
