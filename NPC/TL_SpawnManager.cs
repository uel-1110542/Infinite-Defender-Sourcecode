using UnityEngine;

public class TL_SpawnManager : MonoBehaviour {

    [Header("Spawned Enemy")]
    public GameObject Enemy;
    public float SpawnCooldown;
    public float MaxSpawnAmount;
    private float SpawnTime;    
    private TL_BossFSM BossScript;

    [Header("State Change")]
    public float StateChangeCooldown;
    private float StateChangeTime;
    

    void Start()
    {
        //Obtain the script from this gameobject
        BossScript = GetComponent<TL_BossFSM>();
        
        //Add the time with the cooldown at the start to prevent the NPC from immediate action
        StateChangeTime = StateChangeCooldown + Time.time;
        SpawnTime = SpawnCooldown + Time.time;
    }

    void Update()
    {
        //For changing states
        ChangeState();

        //Function for spawning enemies
        SpawnEnemy();
    }

    void ChangeState()
    {
        //If the current state is not Shielded and the state change time is less than the time
        if (BossScript.CurrentState != "Shielded" && StateChangeTime < Time.time)
        {
            //Toggle the bool to change the state of the boss
            BossScript.StateChange = !BossScript.StateChange;

            //If the state has not changed then set it to Attack otherwise change it to Spawn
            if (!BossScript.StateChange)
            {
                BossScript.CurrentState = "Attack";
            }
            else
            {
                BossScript.CurrentState = "Spawn";                
            }

            //Add the spawn time
            StateChangeTime = StateChangeCooldown + Time.time;

        }

    }

    void SpawnEnemy()
    {
        //Locate all of the boss spawns
        GameObject[] BossSpawns = GameObject.FindGameObjectsWithTag("BossSpawn");

        //If the state of the boss is Spawn and the spawn does not equal to the max spawn amount and the spawn time is less than the time
        if (BossSpawns.GetLength(0) != MaxSpawnAmount)
        {
            if (BossScript.CurrentState == "Spawn" && SpawnTime < Time.time)
            {
                //Create the enemy
                GameObject EnemyClone = Instantiate(Enemy, new Vector3(transform.position.x, transform.position.y - 1.3f, 0f), Quaternion.identity);

                //If the enemy spawned is a guardian
                if (EnemyClone != null)
                {
                    //Increase the count by the amount of boss spawns
                    float SpawnCount = BossSpawns.GetLength(0);

                    if (EnemyClone.name == "Guardian(Clone)")
                    {
                        //If the boss hasn't been shielded
                        if (BossScript.CurrentState != "Shielded")
                        {
                            //Find the orbiter
                            GameObject Orbiter = GameObject.Find("Orbiter(Clone)");

                            //Set the spawned guardian to the parent of the orbiter
                            EnemyClone.transform.SetParent(Orbiter.transform);
                            
                            //When the counter reaches up to the max spawn amount, change the state to Attack and reset the counter to 0
                            if (SpawnCount == MaxSpawnAmount)
                            {
                                BossScript.CurrentState = "Shielded";
                            }
                        }
                    }
                }
                //Add the spawn time
                SpawnTime = SpawnCooldown + Time.time;
            }
        }
        else
        {
            //Switch the state back to Attack if the boss spawns aren't dead
            BossScript.StateChange = true;
        }
    }
}
