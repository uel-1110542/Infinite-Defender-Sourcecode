using UnityEngine;
using UnityEngine.UI;

public class TL_LevelManager : MonoBehaviour {

    //Enemy spawns
    [Header("Enemies")]
    public GameObject Snake;
    public GameObject Sweeper;
    public GameObject Scout;
    public GameObject Looper;
    public GameObject Dummy;

    //Boss spawns
    [Header("Bosses")]
    public GameObject Swarm;
    public GameObject Orbiter;
    public GameObject Sentinel;
    public GameObject Mothership;
    public bool HasBossSpawned;
    private string BossName;

    //Environment
    [Header("Environment")]
    public GameObject Star;
    public float StarSpawnCooldown;
    private float StarSpawnTime;

    //Gameobjects and variables for the intro sequence
    [Header("Intro Sequence")]
    public GameObject InteriorBase;
    public Text Instructions;
    public Text Countdown;
    public bool StartCountdown;

    //Displaying text, sprites and values
    [Header("Text and Icon displays")]
    public Text BaseHP_Display;
    public Text HP_Display;
    public Text TimerDisplay;
    public Text WaveDisplay;
    public Text NextWaveDisplay;
    public Text RewardDisplay;
    public Text DurationDisplay;    
    public Text ContinueDisplay;
    public Image AtkSpdUpIcon;
    public Image MoveSpdUpIcon;
    public Image DefUpIcon;

    //Buttons
    [Header("Buttons")]
    public Button TimerButton;
    public Button NextButton;
    public Button YesButton;
    public Button NoButton;

    //Health
    [Header("Health")]
    public float Max_BaseHP;
    public float Base_HP;
    public float Prev_BaseHP;
    public Image BaseHealthBar;
    public Image ShieldBar;

    //Wave
    [Header("Wave")]
    public int Level = 0;
    public int Wave = 1;
    public int BossWave = -1;
    public int PrevLevel;
    public bool IsPCAwarded;

    //Timers
    [Header("Timers")]
    public float Timer;
    public float Minutes;
    public float Seconds;
    public float SpawnCooldown;
    public float AdvanceLvlCooldown;
    private float SpawnTime;
    private float AdvanceLvlTime;
    public float Pause = 2f;
    public float DisplayDuration = 2f;
    public float CountdownTimer = 4f;

    //Sound and background music
    [Header("Sound and BGM")]
    public AudioClip BackgroundMusic;
    public AudioClip WhooshSound;
    public AudioClip BossShipWhooshSound;
    public AudioSource WhooshSoundSource;
    public AudioSource BackgroundSource;
    public AudioSource BossShipWhooshSoundSource;

    //Misc
    [Header("Misc")]    
    public GameObject PC;
    public int Increment = 0;
    public bool HasSpawningStopped;
    public bool GameOver;
    public bool IntroSequenceOn;
    private int RandomIndex;
    private int RandomNumber;
    private TL_MovePC PCScript;
    private TL_PCShoot PCShootScript;
    private TL_NPC_Attack NPCScript;
    private TL_ScoreManager ScoreManagerScript;
    private GameObject PC_Clone;



    void Start()
    {
        //Set default health
        Base_HP = Max_BaseHP;
        Prev_BaseHP = Base_HP;

        //Set a random number for spawning enemies
        RandomNumber = Rerandomize(12);

        //Obtain the script from the score manager
        ScoreManagerScript = GetComponent<TL_ScoreManager>();

        //Show UI
        ToggleUIVisibility(false);
    }
    
	void Update()
    {
        //If the intro sequence is not on
        if (!IntroSequenceOn)
        {
            //Displays the current wave
            DisplayWave();

            //Updates the display of the space station's health
            UpdateBaseHealth();

            //Function to check if game is over
            CheckGameOver();

            //If the game isn't over
            if (!GameOver)
            {
                //Keeps track of the playing time
                PlayingDuration();

                //Spawns the enemies in a pattern based on a random number
                SpawnPatterns(RandomNumber);

                //Checks if there are no more enemies left and if there aren't any, spawn the enemies
                CheckEnemies();
            }
        }
        else
        {
            //Play the intro sequence
            IntroSequence();

            //If the countdown is on
            if (StartCountdown)
            {
                //Start the game
                StartGame();
            }
        }
        //Generate stars
        GenerateStars();
    }

    public Vector3 ScreenPosConverter(Vector3 Pos)
    {
        //Convert the world space into viewport space with the vector3 parameter
        Vector3 ViewportPoint = Camera.main.WorldToViewportPoint(Pos);

        //Clamp the X position
        ViewportPoint.x = Mathf.Clamp(ViewportPoint.x, 0.05f, 0.95f);

        //Convert the vector3 variable from viewport space into world space and return the vector3 variable
        return Camera.main.ViewportToWorldPoint(ViewportPoint);
    }

    void IntroSequence()
    {
        //If the PC hasn't spawned yet
        if (GameObject.Find("PC(Clone)") == null)
        {
            //Play the sound
            TL_AudioManager.PlaySound(WhooshSoundSource, WhooshSound);

            //Create the PC
            PC_Clone = Instantiate(PC, new Vector2(0f, -5.5f), Quaternion.identity);
        }
        else
        {
            //If the base for the intro is still present in the scene
            if (InteriorBase != null)
            {
                //If the PC is not in the starting position
                if (PC_Clone.transform.position != new Vector3(0f, -3f, 0f))
                {
                    //Move the PC and the ship loader to the position
                    PC_Clone.transform.position = Vector3.MoveTowards(PC_Clone.transform.position, new Vector3(0f, -3f, 0f), 4f * Time.deltaTime);
                }
                else
                {
                    //Move the base towards the bottom of the screen
                    InteriorBase.transform.position = Vector3.Lerp(InteriorBase.transform.position, new Vector3(InteriorBase.transform.position.x, -13f, 1f), 1f * Time.deltaTime);

                    //Once the base is out of camera view
                    Vector3 BaseScreenPoint = Camera.main.WorldToViewportPoint(InteriorBase.transform.position);

                    //If the base has reached at the bottom of the screen
                    if (BaseScreenPoint.y < -0.6f)
                    {
                        //Destroy the base
                        Destroy(InteriorBase);
                    }
                }
            }
            else
            {
                //Once the base is moved out of the screen, reveal the instructions
                Instructions.gameObject.SetActive(true);

                //Obtain the scripts from the spawned PC
                PCShootScript = PC_Clone.GetComponent<TL_PCShoot>();

                //Enable the PC scripts
                PCScript = PC_Clone.GetComponent<TL_MovePC>();
                PCScript.enabled = true;
                PCShootScript.enabled = true;

                //Show the instructions
                Instructions.gameObject.SetActive(true);

                //Set the value to 0 for spawning the dummy
                SpawnPatterns(0);

                //If the tutorial NPC is destroyed
                if (GameObject.Find("TutorialNPC(Clone)") == null)
                {
                    //Display the instructions text
                    Instructions.text = "- Defend the base by destroying enemies before they traverse off-screen. \n\n - The game is lost when either the ship or the base gets destroyed. \n\n - When the level starts, you can pause the game by pressing the Time button located at the top of the screen when it appears.";

                    //Show the next button
                    NextButton.gameObject.SetActive(true);

                    //Change the listener
                    NextButton.onClick.AddListener(delegate { TurnOnCountdown(); });
                }
            }
        }
    }

    void TurnOnCountdown()
    {
        //Set bool to true to start the countdown
        StartCountdown = true;
    }

    void StartGame()
    {
        //Hide instructions and next button
        Instructions.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(false);

        //Show countdown message
        Countdown.gameObject.SetActive(true);

        //If the BGM is not playing
        if (!BackgroundSource.isPlaying)
        {
            //Play the BGM from the audio manager
            TL_AudioManager.PlayMusic(BackgroundSource, BackgroundMusic);
        }

        //Reduce the countdown
        CountdownTimer -= Time.deltaTime;

        //Display the countdown with the timer rounding off to an integer
        Countdown.text = Mathf.Floor(CountdownTimer).ToString("F0");

        //When the countdown timer reaches less than 1
        if (CountdownTimer < 1f)
        {
            //Turn off countdown
            StartCountdown = false;

            //Turn off bool to indicate that the intro sequence is finished
            IntroSequenceOn = false;

            //Change text to indicate the start of the game
            Countdown.text = "Mission Start";

            //Obtain the script from the PC
            PCScript = PC_Clone.GetComponent<TL_MovePC>();

            //Enable the health bar
            PCScript.HealthBar.gameObject.SetActive(true);

            //Set countdown timer to 0
            CountdownTimer = 0f;

            //Set pause to 2
            Pause = 2f;

            //Show UI
            ToggleUIVisibility(true);
        }

    }

    void GenerateStars()
    {
        //If the spawn time is less than the time
        if (StarSpawnTime < Time.time)
        {
            //Spawn the star
            GameObject StarClone = Instantiate(Star, new Vector3(Random.Range(-8f, 8f), 6f, 1f), Quaternion.identity);

            //Obtain the 2D Rigidbody component from the spawned gameobject
            Rigidbody2D StarMovement = StarClone.GetComponent<Rigidbody2D>();

            //Add force to the spawned gameobject
            StarMovement.AddForce(Vector2.down * Random.Range(0.1f, 1f) * Time.deltaTime);

            //Add the spawn time
            StarSpawnTime = StarSpawnCooldown + Time.time;
        }
    }

    public void ResetGame()
    {
        //If the BGM is not playing
        if (!BackgroundSource.isPlaying)
        {
            //Play the BGM from the audio manager
            TL_AudioManager.PlayMusic(BackgroundSource, BackgroundMusic);
        }

        //Reset all values to default
        Base_HP = Max_BaseHP;
        Prev_BaseHP = Base_HP;
        Level = 1;
        Wave = 1;
        BossWave = -1;
        PrevLevel = 0;
        SpawnCooldown = 1f;
        RandomNumber = Rerandomize(12);

        //Reset timers
        Timer = 0;
        Minutes = 0;
        Seconds = 0;
        Increment = 0;
        SpawnTime = 0;
        StarSpawnTime = 0;
        AdvanceLvlTime = 0;
        Pause = 2f;
        DisplayDuration = 2f;

        //Switch all booleans to false
        IsPCAwarded = false;
        HasSpawningStopped = false;
        GameOver = false;
        HasBossSpawned = false;
        ScoreManagerScript.ScoreDisplayed = false;
        ScoreManagerScript.UpdateScore = false;

        //Reset score to 0
        ScoreManagerScript.Score = 0;

        //Show UI
        ToggleUIVisibility(true);

        //Hide the continue prompt
        ContinueDisplay.gameObject.SetActive(false);
        YesButton.gameObject.SetActive(false);
        NoButton.gameObject.SetActive(false);

        //Spawn the PC
        PC_Clone = Instantiate(PC, new Vector2(0f, -3f), Quaternion.identity);

        //Enable the PC scripts
        PCScript = PC_Clone.GetComponent<TL_MovePC>();
        PCScript.enabled = true;
        PCShootScript = PC_Clone.GetComponent<TL_PCShoot>();
        PCShootScript.enabled = true;
    }

    void ToggleUIVisibility(bool toggle)
    {
        //Show/Hide the rest of the UI
        DurationDisplay.gameObject.SetActive(toggle);
        ScoreManagerScript.ScoreDisplay.gameObject.SetActive(toggle);
        BaseHP_Display.gameObject.SetActive(toggle);
        BaseHealthBar.gameObject.SetActive(toggle);
        ShieldBar.gameObject.SetActive(toggle);
        HP_Display.gameObject.SetActive(toggle);
        TimerButton.gameObject.SetActive(toggle);
        TimerDisplay.gameObject.SetActive(toggle);
        WaveDisplay.gameObject.SetActive(toggle);
        AtkSpdUpIcon.gameObject.SetActive(toggle);
        MoveSpdUpIcon.gameObject.SetActive(toggle);
        DefUpIcon.gameObject.SetActive(toggle);
    }

    void CheckGameOver()
    {
        //If the game isn't over yet
        if (!GameOver)
        {
            //If the PC or the base is destroyed
            if (GameObject.Find("PC(Clone)") == null || Base_HP <= 0f)
            {
                //If the BGM is still playing
                if (BackgroundSource.isPlaying)
                {
                    //Stop the BGM
                    BackgroundSource.Stop();
                }
                //Show the touch screen to continue message
                NextButton.gameObject.SetActive(true);

                //Display the game over text
                NextWaveDisplay.text = "Game Over";

                //If the base is destroyed
                if (Base_HP <= 0f)
                {
                    //Set the text to indicate the base is destroyed
                    NextWaveDisplay.text += "\nYour base has been destroyed!";
                }

                //Remove the previous listeners and add the view high score function
                NextButton.onClick.RemoveAllListeners();
                NextButton.onClick.AddListener(delegate { ViewHighScore(); });

                //Set the bool to true to prevent any waves from advancing
                GameOver = true;
            }
        }        
    }

    public void ViewHighScore()
    {
        //When it's game over
        if (GameOver)
        {
            //Set the text to blank
            RewardDisplay.text = "";
            NextWaveDisplay.text = "";

            //Find all NPCs, projectiles and powerups to destroy them
            GameObject[] FindAllNPCs = GameObject.FindGameObjectsWithTag("NPC");
            GameObject[] FindAllBossSpawns = GameObject.FindGameObjectsWithTag("BossSpawn");
            GameObject[] NPC_Projectiles = GameObject.FindGameObjectsWithTag("NPC_Projectile");
            GameObject[] PC_Projectiles = GameObject.FindGameObjectsWithTag("PC_Projectile");
            GameObject[] PowerUps = GameObject.FindGameObjectsWithTag("Powerup");
            foreach (GameObject go in FindAllNPCs)
            {
                Destroy(go);
            }

            foreach (GameObject go in FindAllBossSpawns)
            {
                Destroy(go);
            }

            foreach (GameObject go in NPC_Projectiles)
            {
                Destroy(go);
            }

            foreach (GameObject go in PC_Projectiles)
            {
                Destroy(go);
            }

            foreach (GameObject go in PowerUps)
            {
                Destroy(go);
            }
            //Find the PC and destroy it if the PC is still alive
            Destroy(GameObject.FindGameObjectWithTag("PC"));

            //Hide the rest of the UI
            ToggleUIVisibility(false);

            //Displays the high score table
            ScoreManagerScript.DisplayHighScore();

            //Remove the previous listener and add the view high score function
            NextButton.onClick.RemoveListener(delegate { ViewHighScore(); });
            NextButton.onClick.AddListener(delegate { PromptContinue(); });
        }
    }

    void PromptContinue()
    {
        //Hide the next button
        NextButton.gameObject.SetActive(false);

        //Hide the high score
        ScoreManagerScript.ToggleHighScore(false);

        //Enable the continue message and the yes and no buttons
        ContinueDisplay.gameObject.SetActive(true);
        YesButton.gameObject.SetActive(true);
        NoButton.gameObject.SetActive(true);
    }

    void PlayingDuration()
    {
        //Add deltatime to the timer variable
        Timer += Time.deltaTime;

        //Calculate the seconds with the timer divided by 60 with the modulus to return a reminder
        //and returns the largest integer
        Seconds = Mathf.Floor(Timer % 60f);

        //Calculate the minutes with the timer divided by 60 and returns the largest integer
        Minutes = Mathf.Floor(Timer / 60f);

        //Displays the duration of the playing time in a certain format
        TimerDisplay.text = "Time\n" + string.Format("{00:00}:{1:00}", Minutes, Seconds);
    }

    void DisplayWave()
    {
        //Displays the text for the current level and wave
        WaveDisplay.text = "Level: " + Level + "   Wave: " + Wave;

        //When the current level has advanced onto the next level
        if (PrevLevel < Level)
        {
            //Subtract time with the duration of the pause
            Pause -= Time.deltaTime;

            //Display the message for the next level
            NextWaveDisplay.text = "Level " + Level;

            //If the level is a boss level, add in the warning message
            if (Level % 4 == 0)
            {
                //Displays the text for the current level
                NextWaveDisplay.text += "\n Warning! The " + BossName + " emerges!";
            }

            //If the pause has reached 0 or below
            if (Pause <= 0f)
            {
                //Set the previous base health as the current base health
                Prev_BaseHP = Base_HP;

                //Set the text and string to blank
                NextWaveDisplay.text = "";

                //If the countdown is off then make the text blank and hide it
                Countdown.text = "";
                Countdown.gameObject.SetActive(false);

                //Turn bool off for the next wave
                IsPCAwarded = false;

                //Update the previous level with the current level value
                PrevLevel = Level;

                //Reset the pause duration
                Pause = 2f;
            }
        }
    }

    GameObject SpawnEnemy(GameObject go, float x, float y)
    {
        //Set the gameobject variable to the spawned gameobject
        GameObject Clone = Instantiate(go, new Vector2(x, y), Quaternion.identity);

        //Return the spawned gameobject
        return Clone;
    }

    void UpdateBaseHealth()
    {
        //Displays the space station health
        BaseHP_Display.text = "Base:";

        //Calculate the bar width by converting the value of current and max base health
        //and multiplying it by a default size
        float BaseBarWidth = Base_HP / Max_BaseHP * 125f;

        //Adjust the size of the health bar with the bar width
        BaseHealthBar.rectTransform.sizeDelta = new Vector2(BaseBarWidth, 17f);

        //Change the color of the health bar depending on the width of the health bar
        if (BaseBarWidth <= 30f)
        {
            BaseHealthBar.color = Color.red;
        }
        else if (BaseBarWidth <= 75f)
        {
            BaseHealthBar.color = Color.yellow;
        }
        else if (BaseBarWidth > 75f)
        {
            BaseHealthBar.color = Color.green;
        }

        //If the current health falls below 0
        if (Base_HP <= 0f)
        {
            //Set it to 0
            Base_HP = 0f;
        }
        else if (Base_HP > Max_BaseHP)
        {
            //If the current base health is higher than maximum then set it to maximum value
            Base_HP = Max_BaseHP;
        }

    }

    void CheckEnemies()
    {
        //If the spawning has stopped
        if (HasSpawningStopped)
        {
            //If the advance level time is less than the time and there are no more enemies left
            if (AdvanceLvlTime < Time.time && GameObject.FindGameObjectsWithTag("NPC").GetLength(0) == 0)
            {
                //When the boss hasn't spawned
                if (!HasBossSpawned)
                {
                    //Increment the amount of waves spawned
                    Wave++;

                    //If the amount of waves spawned are more than 6
                    if (Wave > 6)
                    {
                        //If the current level is 2 or more
                        if (Level >= 2)
                        {
                            //Decrease the spawn cooldown
                            SpawnCooldown -= 0.05f;

                            //If the spawn cooldown is less than or equal to 0.5
                            if (SpawnCooldown <= 0.5f)
                            {
                                //Set the value to 0.5
                                SpawnCooldown = 0.5f;
                            }
                        }
                        //Increase the current Level
                        Level++;

                        //Reset the value back to 1
                        Wave = 1;
                    }
                }
                else
                {
                    //Find all of the boss spawns
                    GameObject[] BossSpawns = GameObject.FindGameObjectsWithTag("BossSpawn");

                    //Destroy them all
                    foreach (GameObject go in BossSpawns)
                    {
                        Destroy(go);
                    }

                    //Decrease the value to spawn the next boss when it occurs
                    BossWave -= 1;

                    //If the boss wave value reaches -4 then set it back to -1
                    if (BossWave < -4)
                    {
                        BossWave = -1;
                    }

                    //Increase the current level
                    Level++;

                    //Reset the value back to 1
                    Wave = 1;

                    //Set the bool to false for the next boss wave
                    HasBossSpawned = false;
                }

                //When the value of the level is a multiple of 4
                if (Level % 4 == 0)
                {
                    //Set the value as the boss wave value
                    RandomNumber = BossWave;

                    //Set bool to true to indicate a boss has spawned
                    HasBossSpawned = true;
                }
                else
                {
                    //Re-randomize a number
                    RandomNumber = Rerandomize(12);
                }
                //Resume spawning
                HasSpawningStopped = false;

                //Add the advance level time with the cooldown and the time
                AdvanceLvlTime = AdvanceLvlCooldown + Time.time;
            }
        }
    }

    void SpawnSnakeEnemy(GameObject go, float x, float y, float x_speed, float y_speed, float diff, float maxvalue, float amplitude, bool invertvalues, bool spawnabove, bool aiming)
    {
        //If the spawning hasn't stopped
        if (!HasSpawningStopped)
        {
            //If the spawn time is less than the time
            if (SpawnTime < Time.time)
            {
                //Spawn the enemies on opposite sides of the screen
                GameObject Clone = SpawnEnemy(go, x, y);

                //Obtain the script from the spawned enemy
                TL_SineWaveMovement SineWaveScript = Clone.GetComponent<TL_SineWaveMovement>();
                TL_NPC_Attack NPCScript = Clone.GetComponent<TL_NPC_Attack>();

                //When the level is after the boss wave
                if (Level > 4)
                {
                    //Set the boolean if the NPC is aiming
                    NPCScript.IsNPCAiming = aiming;
                }

                //Set the amplitude, horizontal and vertical speed
                SineWaveScript.Amplitude = amplitude;
                SineWaveScript.HorizontalSpeed = x_speed;
                SineWaveScript.VerticalSpeed = y_speed;

                //Set the boolean if the axis will be inverted or not
                SineWaveScript.InvertAxis = spawnabove;

                //Change the vertical speed of the enemy if the enemy spawns from above the screen
                if (SineWaveScript.InvertAxis)
                {
                    //If the current level is 2 or more, increase the difficulty by increasing the enemy stats
                    if (Level >= 2f)
                    {
                        //Increase the attack rate by subtracting the value of the current level and multiply it by 0.1
                        NPCScript.NPC_AttackCooldown -= Level * 0.1f;

                        //If the attack rate is less than or equal to 1
                        if (NPCScript.NPC_AttackCooldown <= 1f)
                        {
                            //Set the variable to 1 to prevent it from getting any faster
                            NPCScript.NPC_AttackCooldown = 1f;
                        }

                        //Increase the vertical speed by subtracting the value of the current level and multiply it by 0.1
                        SineWaveScript.VerticalSpeed -= Level * 0.1f;

                        //If the vertical speed is less than -2f
                        if (SineWaveScript.VerticalSpeed <= -2f)
                        {
                            //Set it to -2f to prevent it from getting any faster
                            SineWaveScript.VerticalSpeed = -2f;
                        }
                    }
                }

                //If the current level is 2 or more, increase the movement speed
                if (Level >= 2f)
                {
                    //Increase the horizontal speed by adding the current value of the level and multiply it with the difference
                    SineWaveScript.HorizontalSpeed += Level * diff;
                }

                //If the values are not inverted then check if it is less than maximum value
                //Otherwise, check if it is more than equal to maximum value
                if (!invertvalues)
                {
                    //If the horizontal speed is more than or equal to the maximum value
                    if (SineWaveScript.HorizontalSpeed >= maxvalue)
                    {
                        //Set the horizontal speed to the maximum value to prevent it from getting any faster
                        SineWaveScript.HorizontalSpeed = maxvalue;
                    }
                }
                else
                {
                    //If the horizontal speed is less than or equal to the maximum value
                    if (SineWaveScript.HorizontalSpeed <= maxvalue)
                    {
                        //Set the horizontal speed to the maximum value to prevent it from getting any faster
                        SineWaveScript.HorizontalSpeed = maxvalue;
                    }
                }                                
                //Increase the counter
                Increment++;

                //If the increment reaches to 5
                if (Increment == 5)
                {
                    //Reset the increment
                    Increment = 0;

                    //Stop spawning
                    HasSpawningStopped = true;
                }
                //Add the spawn time
                SpawnTime = SpawnCooldown + Time.time;
            }
        }
    }

    void SpawnLooperEnemy(GameObject go, float x, float y, float movespd, float rotspd, float diff, float maxvalue, bool inverted, bool aiming)
    {
        //If the spawning hasn't stopped
        if (!HasSpawningStopped)
        {
            //If the spawn time is less than the time
            if (SpawnTime < Time.time)
            {
                //Spawn the enemies on opposite sides of the screen
                GameObject Clone = SpawnEnemy(go, x, y);

                //Obtain the script from the spawned gameobject
                TL_CircularMovement CircularMovementScript = Clone.GetComponent<TL_CircularMovement>();
                TL_NPC_Attack NPCScript = Clone.GetComponent<TL_NPC_Attack>();

                //Set the default values for the speed
                CircularMovementScript.MoveSpeed = movespd;
                CircularMovementScript.RotatorySpeed = rotspd;

                //If the loopers are spawning on the right side of the screen
                if (x > 0f)
                {
                    //Change the angle of the loop movement to 3 to prevent the enemy from going off-screen in the wrong direction
                    CircularMovementScript.Angle = 3f;
                }

                //When the level is after the boss wave
                if (Level > 4)
                {
                    //Set the boolean if the NPC is aiming
                    NPCScript.IsNPCAiming = aiming;
                }

                //If the current level is 2 or more, increase the difficulty by increasing the enemy stats
                if (Level >= 2)
                {
                    //Increase the attack rate by subtracting the value of the current level and multiply it by 0.1
                    NPCScript.NPC_AttackCooldown -= Level * 0.1f;

                    //If the attack rate is less than or equal to 1
                    if (NPCScript.NPC_AttackCooldown <= 1f)
                    {
                        //Set the variable to 1 to prevent it from getting any faster
                        NPCScript.NPC_AttackCooldown = 1f;
                    }

                    //Increase the movement speed by the value of the current level multiplied by the difference
                    CircularMovementScript.MoveSpeed += Level * diff;

                    //Increase the rotatory speed by the value of the current level multiplied by the difference
                    CircularMovementScript.RotatorySpeed += Level * diff;

                    //If the NPC is moving from the left side of the screen to the right
                    if (!inverted)
                    {
                        //If the movement speed is more than or equal to the maximum value
                        if (CircularMovementScript.MoveSpeed >= maxvalue)
                        {
                            //Set the movement speed to the maximum value to prevent it from getting any faster
                            CircularMovementScript.MoveSpeed = maxvalue;
                        }

                        //If the rotatory speed is more than or equal to the maximum value
                        if (CircularMovementScript.RotatorySpeed >= maxvalue)
                        {
                            //Set the rotatory speed to the maximum value to prevent it from getting any faster
                            CircularMovementScript.RotatorySpeed = maxvalue;
                        }
                    }
                    else    //If the NPC is moving from the right side of the screen to the left
                    {
                        //If the movement speed is more than or equal to the maximum value
                        if (CircularMovementScript.MoveSpeed <= maxvalue)
                        {
                            //Set the movement speed to the maximum value to prevent it from getting any faster
                            CircularMovementScript.MoveSpeed = maxvalue;
                        }

                        //If the rotatory speed is more than or equal to the maximum value
                        if (CircularMovementScript.RotatorySpeed <= maxvalue)
                        {
                            //Set the rotatory speed to the maximum value to prevent it from getting any faster
                            CircularMovementScript.RotatorySpeed = maxvalue;
                        }
                    }                    
                }

                //Increase the counter
                Increment++;

                //If the increment reaches to 5
                if (Increment == 5)
                {
                    //Reset the increment
                    Increment = 0;

                    //Stop spawning
                    HasSpawningStopped = true;
                }

                //Add the spawn time
                SpawnTime = SpawnCooldown + Time.time;
            }
        }
    }

    void SpawnScoutEnemy(GameObject go, float x, float y, float speed, float diff, float maxvalue, bool inverted, bool spawnfromabove, bool aiming)
    {
        //If the spawning hasn't stopped
        if (!HasSpawningStopped)
        {
            //If the spawn time is less than the time
            if (SpawnTime < Time.time)
            {
                //Spawn the enemy
                GameObject Clone = SpawnEnemy(go, x, y);

                //Obtain the script from the spawned enemy
                TL_DirectionalMove DirectionalMoveScript = Clone.GetComponent<TL_DirectionalMove>();
                TL_NPC_Attack NPCScript = Clone.GetComponent<TL_NPC_Attack>();

                //When the level is after the boss wave
                if (Level > 4)
                {
                    //Set the boolean if the NPC is aiming at the PC
                    NPCScript.IsNPCAiming = aiming;
                }

                //Set the movement speed
                DirectionalMoveScript.MoveSpeed = speed;

                //If the current level is 2 or more, increase the difficulty by increasing the enemy stats
                if (Level >= 2f)
                {
                    //Increase the attack rate by subtracting the value of the current level and multiply it by 0.1
                    NPCScript.NPC_AttackCooldown -= Level * 0.1f;

                    //If the attack rate is less than or equal to 1.2f
                    if (NPCScript.NPC_AttackCooldown <= 1.2f)
                    {
                        //Set the variable to 1.2f to prevent it from getting any faster
                        NPCScript.NPC_AttackCooldown = 1.2f;
                    }

                    //Increase the movement speed by the value of the current level multiplied by the difference
                    DirectionalMoveScript.MoveSpeed += Level * diff;
                }

                //If the enemy is being spawned from the top of the screen
                if (spawnfromabove)
                {
                    //Set the bool to indicate that it is spawning from above
                    DirectionalMoveScript.ChangeDir = spawnfromabove;

                    //If the movement speed is less than or equal to the maximum value
                    if (DirectionalMoveScript.MoveSpeed <= maxvalue)
                    {
                        //Set the movement speed to the maximum value
                        DirectionalMoveScript.MoveSpeed = maxvalue;
                    }
                }

                //If the axis is not inverted
                if (!inverted)
                {
                    //If the movement speed is more than or equal to the maximum value
                    if (DirectionalMoveScript.MoveSpeed >= maxvalue)
                    {
                        //Set the movement speed to the maximum value to prevent it ffrom getting any faster
                        DirectionalMoveScript.MoveSpeed = maxvalue;
                    }
                }
                else
                {
                    //If the movement speed is less than or equal to the maximum value
                    if (DirectionalMoveScript.MoveSpeed <= maxvalue)
                    {
                        //Set the movement speed to the maximum value to prevent it ffrom getting any faster
                        DirectionalMoveScript.MoveSpeed = maxvalue;
                    }
                }

                //Increase the counter
                Increment++;

                //If the increment reaches to 5
                if (Increment == 5)
                {
                    //Reset the increment
                    Increment = 0;

                    //Stop spawning
                    HasSpawningStopped = true;
                }

                //Add the spawn time
                SpawnTime = SpawnCooldown + Time.time;
            }
        }                
    }

    void SpawnSweeperEnemy(GameObject go, float x, float y, float speed, float diff, float maxvalue, bool inverted, bool aiming)
    {
        //If the spawning hasn't stopped
        if (!HasSpawningStopped)
        {
            //If the spawn time is less than the time
            if (SpawnTime < Time.time)
            {
                //Spawn the enemies on opposite sides of the screen
                GameObject Clone = SpawnEnemy(go, x, y);

                //Obtain the script from the spawned enemy
                TL_DirectionalMove HorizontalMoveScript = Clone.GetComponent<TL_DirectionalMove>();
                TL_ArcMovement ArcMoveScript = Clone.GetComponent<TL_ArcMovement>();
                TL_NPC_Attack NPCScript = Clone.GetComponent<TL_NPC_Attack>();

                //When the level is after the boss wave
                if (Level > 4)
                {
                    //Set the boolean if the NPC is aiming
                    NPCScript.IsNPCAiming = aiming;
                }

                //Set the movement speed with the parameter
                HorizontalMoveScript.MoveSpeed = speed;
                ArcMoveScript.Speed = speed;

                //If the axis is inverted
                if (inverted)
                {
                    //If the horizontal speed is less than or equal to the maximum value
                    if (HorizontalMoveScript.MoveSpeed <= maxvalue)
                    {
                        //Set the movement speed to the maximum value to prevent it from getting any faster
                        HorizontalMoveScript.MoveSpeed = maxvalue;
                    }

                    //If the speed is less than or equal to the maximum value
                    if (ArcMoveScript.Speed <= maxvalue)
                    {
                        //Set the speed to the maximum value to prevent it from getting any faster
                        ArcMoveScript.Speed = maxvalue;
                    }
                }
                else        //If the axis is not inverted
                {
                    //If the horizontal speed is more than or equal to the maximum value
                    if (HorizontalMoveScript.MoveSpeed >= maxvalue)
                    {
                        //Set the movement speed to the maximum value to prevent it from getting any faster
                        HorizontalMoveScript.MoveSpeed = maxvalue;
                    }

                    //If the speed is more than or equal to the maximum value
                    if (ArcMoveScript.Speed >= maxvalue)
                    {
                        //Set the speed to the maximum value to prevent it from getting any faster
                        ArcMoveScript.Speed = maxvalue;
                    }
                }
                //If the current level is 2 or more, increase the difficulty by increasing the enemy stats
                if (Level >= 2f)
                {
                    //Increase the attack rate by subtracting the value of the current level and multiply it by 0.1
                    NPCScript.NPC_AttackCooldown -= Level * 0.1f;

                    //If the attack rate is less than or equal to 1
                    if (NPCScript.NPC_AttackCooldown <= 1f)
                    {
                        //Set the variable to 1 to prevent it from getting any faster
                        NPCScript.NPC_AttackCooldown = 1f;
                    }

                    //Increase the movement speed by the value of the current wave multiplied by the difference
                    HorizontalMoveScript.MoveSpeed += Level * diff;
                    ArcMoveScript.Speed += Level * diff;
                }

                //Increase the counter
                Increment++;

                //If the increment reaches to 5
                if (Increment == 5)
                {
                    //Reset the increment
                    Increment = 0;

                    //Stop spawning
                    HasSpawningStopped = true;
                }

                //Add the spawn time
                SpawnTime = SpawnCooldown + Time.time;
            }
        }
    }

    void SpawnBoss(GameObject go, float x, float y)
    {
        //If the boss hasn't been spawned yet
        if (!HasSpawningStopped)
        {
            //Play the sound indicating the arrival of the boss ship
            TL_AudioManager.PlaySound(BossShipWhooshSoundSource, BossShipWhooshSound);

            //Spawn the boss
            Instantiate(go, new Vector2(x, y), Quaternion.identity);

            //Obtain the boss name from the spawned gameobject
            BossName = go.name;

            //Set the bool to true to prevent any more bosses from spawning
            HasSpawningStopped = true;
        }
        //Locate the Boss
        GameObject BossClone = GameObject.FindGameObjectWithTag("NPC");

        //If the boss is still alive
        if (BossClone != null)
        {
            //If the boss has descended to the screen
            if (BossClone.transform.position.y == 3.5f)
            {
                //Obtain the boss' 2D box collider and enable it
                BoxCollider2D BossCollider = BossClone.GetComponent<BoxCollider2D>();                
                BossCollider.enabled = true;

                //Obtain the scripts from the boss
                TL_BossFSM BossScript = BossClone.GetComponent<TL_BossFSM>();
                TL_SpawnManager SpawnManagerScript = BossClone.GetComponent<TL_SpawnManager>();

                //Enable the scripts
                BossScript.enabled = true;                
                SpawnManagerScript.enabled = true;

                //If the boss' name is called the Orbiter
                if (BossName == "Orbiter")
                {
                    //Obtain the script attached to the orbiter and enable it
                    TL_ShieldManager ShieldManagerScript = BossClone.GetComponent<TL_ShieldManager>();
                    ShieldManagerScript.enabled = true;
                }
            }
            else
            {
                //If the boss has not descended towards the camera view then move towards the top of the screen
                BossClone.transform.position = Vector2.MoveTowards(new Vector2(BossClone.transform.position.x, BossClone.transform.position.y), new Vector2(BossClone.transform.position.x, 3.5f), 3f * Time.deltaTime);
            }
        }        
    }

    void SpawnDummy(GameObject go, float x, float y)
    {
        //If the spawning hasn't stopped
        if (!HasSpawningStopped)
        {
            //Spawn the dummy
            SpawnEnemy(go, x, y);

            //Stop spawning
            HasSpawningStopped = true;
        }
    }

    void SpawnPatterns(int value)
    {
        switch (value)
        {
            //Spawns the Mothership
            case -4:
                SpawnBoss(Mothership, 0f, 6f);
                break;

            //Spawns the Sentinel
            case -3:
                SpawnBoss(Sentinel, 0f, 6f);
                break;

            //Spawns the Orbiter
            case -2:
                SpawnBoss(Orbiter, 0f, 6f);
                break;

            //Spawns the Swarm
            case -1:
                SpawnBoss(Swarm, 0f, 6f);
                break;

            //Spawn the dummy for the tutorial
            case 0:
                SpawnDummy(Dummy, -10f, 3f);
                break;

            //Spawns the enemies in a sine wave motion pattern from the left side of the screen
            case 1:
                //Spawn the snake enemy
                SpawnSnakeEnemy(Snake, -10f, 2f, 2f, 0.75f, 0.15f, 4f, 1f, false, false, true);
                break;

            //Spawns the enemies in a sine wave motion pattern from the right side of the screen
            case 2:
                //Spawn the snake enemy
                SpawnSnakeEnemy(Snake, 10f, 2f, -2f, 0.75f, -0.15f, -4f, 1f, true, false, true);
                break;

            //Spawns the enemies in a sine wave motion pattern from both the left and right side of the screen
            case 3:
                //If the value of the variable is a multiple of 2
                if (Increment % 2 == 0)
                {
                    //Spawn the snake enemy
                    SpawnSnakeEnemy(Snake, -10f, 1f, 2f, 0.75f, 0.15f, 4f, 1f, false, false, true);
                }
                else
                {
                    //Spawn the snake enemy
                    SpawnSnakeEnemy(Snake, 10f, 2f, -2f, 0.75f, -0.15f, -4f, 1f, true, false, true);
                }
                break;

            //Spawns the enemies in an sine wave motion pattern from the top of the screen
            case 4:
                SpawnSnakeEnemy(Snake, 0f, 5.25f, 0f, -1.3f, -0.1f, -2f, 1f, false, true, false);
                break;

            //Spawns the enemies in an arc motion pattern from the right side of the screen
            case 5:
                SpawnSweeperEnemy(Sweeper, 10f, 1.5f, 2f, 0.15f, 4f, false, true);
                break;

            //Spawns the enemies in an arc motion pattern from the left side of the screen
            case 6:
                SpawnSweeperEnemy(Sweeper, -10f, 1.5f, -2f, -0.15f, -4f, true, true);
                break;

            //Spawns the enemies in a loop the loop movement pattern from the right side of the screen
            case 7:
                SpawnLooperEnemy(Looper, 10f, 1f, 2f, 4f, 0.1f, 6f, false, true);
                break;

            //Spawns the enemies in a loop the loop movement pattern from the left side of the screen
            case 8:
                SpawnLooperEnemy(Looper, -10f, 1f, -2f, -4f, -0.1f, -6f, true, true);
                break;

            //Spawn the enemies in a random position from the left side of the screen
            case 9:
                SpawnScoutEnemy(Scout, -10f, Random.Range(1f, 3f), -2f, -0.15f, 7f, false, false, true);
                break;

            //Spawn the enemies in a random position from the right side of the screen
            case 10:
                SpawnScoutEnemy(Scout, 10f, Random.Range(1f, 3f), 2f, 0.15f, -7f, true, false, true);
                break;

            //Spawn the enemies in a random position from both the left and right sides of the screen
            case 11:
                //If the value of the variable is a multiple of 2
                if (Increment % 2 == 0)
                {
                    //Spawn enemies from the left side of the screen
                    SpawnScoutEnemy(Scout, -10f, Random.Range(1f, 3f), -2f, -0.15f, 7f, false, false, true);
                }
                else   //If the value of the variable is not a multiple of 2
                {
                    //Spawn enemies from the right side of the screen
                    SpawnScoutEnemy(Scout, 10f, Random.Range(1f, 3f), 2f, 0.15f, -7f, true, false, true);
                }
                break;

            //Spawn the enemies from a random position from the top of the screen
            case 12:
                SpawnScoutEnemy(Scout, Mathf.Round(Random.Range(-3f, 3f)), 5f, 2f, 0.15f, -7f, true, true, false);
                break;
        }
    }

    int Rerandomize(int max_val)
    {
        //Randomize the number between the minimum and maximum value
        int Ran_Num = Random.Range(1, max_val);

        //While the random number is the same as the previous random number, re-randomize it
        while (Ran_Num == RandomIndex)
        {
            //Randomize the number between the minimum and maximum value again
            Ran_Num = Random.Range(1, max_val);
        }
        //Assign the variable to the random number
        RandomIndex = Ran_Num;

        //Return the value
        return RandomIndex;
    }

}
