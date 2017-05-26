using UnityEngine;
using UnityEngine.SceneManagement;

public class TL_Continue : MonoBehaviour {

    

    public void RestartGame()
    {
        //Obtain the script from the level manager
        TL_LevelManager LevelManagerScript = GetComponent<TL_LevelManager>();

        //Restart the game
        LevelManagerScript.ResetGame();
    }

    public void ReturnToStartScreen()
    {
        //Return to the start screen
        SceneManager.LoadScene("Start_Screen");
    }

}
