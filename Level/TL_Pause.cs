using UnityEngine;
using UnityEngine.UI;

public class TL_Pause : MonoBehaviour {

    //Variables
    public TL_LevelManager LMScript;
    public Text PauseText;
    public Text NextWaveDisplay;
    public Text RewardDisplay;
    public bool Paused;
    public AudioClip PauseActiveSound;
    public AudioClip PauseInactiveSound;
    public AudioSource PauseActiveSoundSource;
    public AudioSource PauseInactiveSoundSource;
    private string TempWaveDisplay;
    private string TempRewardDisplay;
    private GameObject LevelManager;



    void Start()
    {
        LevelManager = GameObject.Find("LevelManager");
    }

    void FreezeTime()
    {
        //Locate all NPC's in the scene
        GameObject[] NPCs = GameObject.FindGameObjectsWithTag("NPC");

        //Find the PC
        GameObject PC = GameObject.Find("PC");

        //Enable the pause text
        PauseText.enabled = Paused;

        //For each of the NPC's
        foreach (GameObject go in NPCs)
        {
            //If the NPC does have the script attached
            if (go.GetComponent<TL_NPC_Attack>() != null)
            {
                //Obtain the script from the NPC and toggle the script
                TL_NPC_Attack NPCScript = go.GetComponent<TL_NPC_Attack>();
                NPCScript.enabled = !Paused;
            }
            //If the NPC has a boss FSM script attached
            if (go.GetComponent<TL_BossFSM>() != null && go.GetComponent<TL_BossFSM>().enabled == true)
            {
                //Obtain the script from the boss and toggle the script
                TL_BossFSM BossScript = go.GetComponent<TL_BossFSM>();
                BossScript.enabled = !Paused;
            }

            //If the NPC has the spawn manager script attached
            if (go.GetComponent<TL_SpawnManager>() != null)
            {
                //Obtain the script from the boss and toggle the script
                TL_SpawnManager SpawnManagerScript = go.GetComponent<TL_SpawnManager>();
                SpawnManagerScript.enabled = !Paused;
            }
        }
        //If the PC is still alive
        if (PC != null)
        {
            //Obtain the scripts from the PC
            TL_MovePC MovePCScript = PC.GetComponent<TL_MovePC>();
            TL_PCShoot PCShootScript = PC.GetComponent<TL_PCShoot>();

            //Toggle the scripts depending on if the game is paused or not
            MovePCScript.enabled = !Paused;
            PCShootScript.enabled = !Paused;
        }
        //Toggle the level manager script
        LMScript.enabled = !Paused;

        //If the player has paused the game
        if (Paused)
        {
            //Play the sound
            TL_AudioManager.PlaySound(PauseActiveSoundSource, PauseActiveSound);

            //Set the time scale to 0 to freeze the game
            Time.timeScale = 0f;
        }
        else
        {
            //Play the sound
            TL_AudioManager.PlaySound(PauseInactiveSoundSource, PauseInactiveSound);

            //Set the time scale to 1 to unfreeze the game
            Time.timeScale = 1f;
        }
    }

    public void Pause()
    {
        //Toggle the bool to pause and unpause the game
        Paused = !Paused;

        //Toggle the pause to either activate/deactivate the gameobject
        PauseText.gameObject.SetActive(Paused);

        //If the game is paused
        if (Paused)
        {
            TL_AudioManager.PauseMusic(LevelManager.GetComponent<AudioSource>());

            //Store the text in temporary string variables
            TempWaveDisplay = NextWaveDisplay.text;
            TempRewardDisplay = RewardDisplay.text;

            //Set the text to blank
            NextWaveDisplay.text = "";
            RewardDisplay.text = "";
        }
        else
        {
            TL_AudioManager.UnpauseMusic(LevelManager.GetComponent<AudioSource>());

            //Revert the text back
            NextWaveDisplay.text = TempWaveDisplay;
            RewardDisplay.text = TempRewardDisplay;
        }
        //Function for freezing time and disabling scripts
        FreezeTime();
    }

}
