using UnityEngine;

public class TL_ShieldManager : MonoBehaviour {

    //Variables
    public GameObject Shield;
    public bool HasShieldSpawned;
    private TL_BossFSM BossScript;
    private TL_SpawnManager SpawnManagerScript;


    void Start()
    {
        //Obtain the script from this gameobject
        BossScript = GetComponent<TL_BossFSM>();
        SpawnManagerScript = GetComponent<TL_SpawnManager>();
    }

    void Update()
    {
        //Checking the current guardians in the scene
        CheckGuardians();
    }

    void CheckGuardians()
    {
        //Find all of the guardians on the scene
        GameObject[] Guardians = GameObject.FindGameObjectsWithTag("BossSpawn");

        //If the guardians are still alive in the scene
        if (!HasShieldSpawned && Guardians.GetLength(0) == SpawnManagerScript.MaxSpawnAmount)
        {
            //Spawn the shield
            GameObject ShieldClone = Instantiate(Shield, transform.position, Quaternion.identity);

            //Enable all of the NPC scripts and colliders on the guardians
            foreach (GameObject Child in Guardians)
            {
                TL_NPC_Attack NPCScript = Child.GetComponent<TL_NPC_Attack>();
                NPCScript.enabled = true;

                BoxCollider2D GuardianCollider = Child.GetComponent<BoxCollider2D>();
                GuardianCollider.enabled = true;
            }

            //Change the boss' state to Shielded
            BossScript.CurrentState = "Shielded";

            //Set the shield as the child gameobject to the boss
            ShieldClone.transform.SetParent(transform);

            //Obtain the component from the spawned shield and enable it
            CircleCollider2D ShieldCollider = ShieldClone.GetComponent<CircleCollider2D>();
            ShieldCollider.enabled = true;

            //Centre the child gameobject's position
            ShieldClone.transform.localPosition = new Vector3(0f, 0f, -1f);

            //Turn the bool on to prevent any more from spawning
            HasShieldSpawned = true;
        }

        //If all of the guardians are destroyed and the state is Shielded
        if(Guardians.GetLength(0) == 0 && BossScript.CurrentState == "Shielded")
        {
            //Turn the bool off to spawn another shield when the boss' state changes
            HasShieldSpawned = false;

            //Change the boss' state to Attack
            BossScript.CurrentState = "Attack";

            //Destroy the shield
            Destroy(transform.Find("Shield(Clone)").gameObject);
        }
    }

}
