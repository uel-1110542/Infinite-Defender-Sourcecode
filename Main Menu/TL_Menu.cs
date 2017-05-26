using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TL_Menu : MonoBehaviour {

    //Menus
    [Header("Menu")]
    public GameObject MainMenu;
    public GameObject OptionsMenu;
    public GameObject HighScore;
    public Button NextButton;
    public Button PreviousButton;
    public Button YesButton;
    public Button NoButton;
    private bool IsGameLoading;

    [Header("Text Components")]
    public Text GameTitle;
    public Text OrderTable;
    public Text HighScoreTable;
    public Text PlayTimeTable;
    public Text LevelTable;
    public Text WaveTable;
    public Text VolumeText;
    public Text CreditsTitle;
    public Text FirstPage;
    public Text SecondPage;
    public Text LoadingMessage;

    [Header("Sliders")]
    public Slider MusicSlider;
    public Slider SFXSlider;
    


    void Start()
    {
        //Set default listener
        PreviousButton.onClick.AddListener(delegate { Previous(); });
    }

    void Update()
    {
        //Change the transparency of the text when the game loads
        ChangeTransparency();
    }

    public void StartGame()
    {
        //Set the bool to true to indicate that the game is loading
        IsGameLoading = true;

        //Start the coroutine
        StartCoroutine(LoadingScreen());   
    }

    void ChangeTransparency()
    {
        //If the game is loading
        if (IsGameLoading)
        {
            //Set the transparency back and fourth with the ping pong maths function
            LoadingMessage.color = new Color(1f, 1f, 1f, Mathf.PingPong(Time.time, 1f));
        }        
    }

    IEnumerator LoadingScreen()
    {
        //Deactivate the title and the main menu
        GameTitle.gameObject.SetActive(false);
        MainMenu.SetActive(false);

        //Activate the loading message
        LoadingMessage.gameObject.SetActive(true);

        //Wait for 1 second
        yield return new WaitForSeconds(1f);

        //Load the game in the background
        AsyncOperation LoadingAsync = SceneManager.LoadSceneAsync("Main_Level");

        //While the scene is loading in the background, return null to continue waiting
        while (!LoadingAsync.isDone)
        {
            yield return null;
        }
    }

    public void ExitGame()
    {
        //Exit the game
        Application.Quit();
    }

    void ViewHighScores()
    {
        //Reset all of the text for the new updated data
        HighScoreTable.text = "";
        PlayTimeTable.text = "";
        LevelTable.text = "";
        WaveTable.text = "";

        //Display all 5 high scores from the player preferences
        for (int i = 1; i <= 5; i++)
        {
            HighScoreTable.text += PlayerPrefs.GetInt("HighScore" + i).ToString() + "\n\n";
            PlayTimeTable.text += PlayerPrefs.GetFloat("Minutes" + i).ToString("00") + " : " + PlayerPrefs.GetFloat("Seconds" + i).ToString("00") + "\n\n";
            LevelTable.text += PlayerPrefs.GetInt("Level" + i) + "\n\n";
            WaveTable.text += PlayerPrefs.GetInt("Wave" + i) + "\n\n";
        }
    }

    public void PromptMessage()
    {
        //Hide the volume options and show the prompt and the yes and no buttons
        VolumeOptions(false, true, "Are you sure?");
    }

    public void Decline()
    {
        //Show the volume options and hide the prompt and the yes and no buttons
        VolumeOptions(true, false, "Volume");
    }

    public void ClearHighScore()
    {
        //Clear all player preferences
        PlayerPrefs.DeleteAll();

        //Show the volume options
        VolumeOptions(true, false, "Volume");
    }

    public void VolumeOptions(bool hidesliders, bool hidebuttons, string text)
    {
        //Show or hide the sliders
        MusicSlider.gameObject.SetActive(hidesliders);
        SFXSlider.gameObject.SetActive(hidesliders);

        //Change the text
        VolumeText.text = text;

        //Show or hide the buttons
        YesButton.gameObject.SetActive(hidebuttons);
        NoButton.gameObject.SetActive(hidebuttons);
    }

    public void OptionsButton()
    {
        //Deactivate the other menus, buttons, title, and activate the options menu
        GameTitle.gameObject.SetActive(false);
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(true);
        HighScore.SetActive(false);
        CreditsTitle.gameObject.SetActive(false);
        FirstPage.gameObject.SetActive(false);
        SecondPage.gameObject.SetActive(false);
        PreviousButton.gameObject.SetActive(true);        
    }

    public void HighScoreButton()
    {
        //Deactivate the other menus, buttons, title, and activate the high score
        GameTitle.gameObject.SetActive(false);
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        HighScore.SetActive(true);
        CreditsTitle.gameObject.SetActive(false);
        FirstPage.gameObject.SetActive(false);
        SecondPage.gameObject.SetActive(false);
        PreviousButton.gameObject.SetActive(true);

        //View the high scores
        ViewHighScores();
    }

    public void CreditsButton()
    {
        //Deactivate the other menus, buttons, title, and activate the credits
        GameTitle.gameObject.SetActive(false);
        MainMenu.SetActive(false);
        OptionsMenu.SetActive(false);
        HighScore.SetActive(false);
        CreditsTitle.gameObject.SetActive(true);
        FirstPage.gameObject.SetActive(true);
        SecondPage.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        PreviousButton.gameObject.SetActive(true);

        //Add listener to the previous button
        PreviousButton.onClick.AddListener(delegate { Previous(); });
    }

    public void Previous()
    {
        //Show the volume options again
        VolumeOptions(true, false, "Volume");

        //Deactivate the other menus and buttons and activate the main menu and title
        GameTitle.gameObject.SetActive(true);
        MainMenu.SetActive(true);        
        OptionsMenu.SetActive(false);
        HighScore.SetActive(false);
        CreditsTitle.gameObject.SetActive(false);
        FirstPage.gameObject.SetActive(false);
        SecondPage.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);
        PreviousButton.gameObject.SetActive(false);
    }

    public void PreviousCreditsPage()
    {
        //Activate the 1st page of the credits page
        FirstPage.gameObject.SetActive(true);

        //Deactivate the 2nd page of the credits page
        SecondPage.gameObject.SetActive(false);

        //Activate the next button
        NextButton.gameObject.SetActive(true);

        //Remove all listeners from the previous button
        PreviousButton.onClick.RemoveAllListeners();

        //Add the previous function listener to the previous button
        PreviousButton.onClick.AddListener(delegate { Previous(); });
    }

    public void NextCreditsPage()
    {
        //Deactivate the 1st page of the credits page
        FirstPage.gameObject.SetActive(false);

        //Activate the 2nd page of the credits page
        SecondPage.gameObject.SetActive(true);

        //Activate the next button
        NextButton.gameObject.SetActive(false);

        //Remove all listeners from the previous button
        PreviousButton.onClick.RemoveAllListeners();

        //Add the previous credits page listener to the previous button
        PreviousButton.onClick.AddListener(delegate { PreviousCreditsPage(); });
    }

}
