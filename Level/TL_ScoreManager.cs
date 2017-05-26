using UnityEngine;
using UnityEngine.UI;

public class TL_ScoreManager : MonoBehaviour {

    //Score
    [Header("Score")]
    public Text ScoreDisplay;
    public GameObject HeaderDisplay;
    public Text OrderTable;
    public Text HighScoreTable;
    public Text PlayTimeTable;
    public Text LevelTable;
    public Text WaveTable;
    public int Score;
    public bool ScoreDisplayed;
    public bool UpdateScore;    
    private TL_LevelManager LevelManagerScript;



    void Start()
    {
        //Obtain the script from the gameobject
        LevelManagerScript = GetComponent<TL_LevelManager>();
    }

	void Update()
    {
        //Displays current score
        DisplayScore();
    }
    
    void DisplayScore()
    {
        //Set the text to display the score
        ScoreDisplay.text = "Score: " + Score.ToString("0000000");
    }

    public void ToggleHighScore(bool toggle)
    {
        //Show/Hide the high score, time, level, and wave table
        HeaderDisplay.SetActive(toggle);
        OrderTable.gameObject.SetActive(toggle);
        HighScoreTable.gameObject.SetActive(toggle);
        PlayTimeTable.gameObject.SetActive(toggle);
        LevelTable.gameObject.SetActive(toggle);
        WaveTable.gameObject.SetActive(toggle);
    }

    public void DisplayHighScore()
    {
        //If it's game over and the score hasn't been displayed yet
        if (LevelManagerScript.GameOver)
        {
            //Enable the high score, playing time and wave table
            ToggleHighScore(true);
            
            //Loop through the scores
            for (int i = 1; i <= 5; i++)
            {
                //If the top 5 scores is less than the current score
                if (PlayerPrefs.GetInt("HighScore" + i) < Score && !UpdateScore)
                {                    
                    //Store the high score, time, level, and wave in a temporary variable
                    int TempScore = PlayerPrefs.GetInt("HighScore" + i);
                    float TempMinutes = PlayerPrefs.GetFloat("Minutes" + i);
                    float TempSeconds = PlayerPrefs.GetFloat("Seconds" + i);
                    int TempLevel = PlayerPrefs.GetInt("Level" + i);
                    int TempWave = PlayerPrefs.GetInt("Wave" + i);

                    //Save the current score, time and wave
                    PlayerPrefs.SetInt("HighScore" + i, Score);
                    PlayerPrefs.SetFloat("Minutes" + i, LevelManagerScript.Minutes);
                    PlayerPrefs.SetFloat("Seconds" + i, LevelManagerScript.Seconds);
                    PlayerPrefs.SetInt("Level" + i, LevelManagerScript.Level);
                    PlayerPrefs.SetInt("Wave" + i, LevelManagerScript.Wave);

                    //If the current iteration is less than 5
                    if (i <= 5)
                    {
                        //Shift the position of the score towards the bottom and store it in a temporary variable
                        int k = i + 1;

                        //Set the score, time, level, and wave in the new shifted position
                        PlayerPrefs.SetInt("HighScore" + k, TempScore);
                        PlayerPrefs.SetFloat("Minutes" + k, TempMinutes);
                        PlayerPrefs.SetFloat("Seconds" + k, TempSeconds);
                        PlayerPrefs.SetInt("Level" + k, TempLevel);
                        PlayerPrefs.SetInt("Wave" + k, TempWave);
                    }
                    //Set bool to true to prevent the scores from updating again
                    UpdateScore = true;
                }
            }

            if (!ScoreDisplayed)
            {
                //Reset all of the text for the new updated data
                HighScoreTable.text = "";
                PlayTimeTable.text = "";
                LevelTable.text = "";
                WaveTable.text = "";

                for (int i = 1; i <= 5; i++)
                {
                    //Display all 5 high scores
                    HighScoreTable.text += PlayerPrefs.GetInt("HighScore" + i).ToString() + "\n\n";
                    PlayTimeTable.text += PlayerPrefs.GetFloat("Minutes" + i).ToString("00") + " : " + PlayerPrefs.GetFloat("Seconds" + i).ToString("00") + "\n\n";
                    LevelTable.text += PlayerPrefs.GetInt("Level" + i).ToString() + "\n\n";
                    WaveTable.text += PlayerPrefs.GetInt("Wave" + i).ToString() + "\n\n";

                    //At the end of the iteration, set the bool to true to prevent the score from showing repeatedly
                    if (i == 5)
                    {
                        ScoreDisplayed = true;
                    }
                }
            }

        }

    }

}
