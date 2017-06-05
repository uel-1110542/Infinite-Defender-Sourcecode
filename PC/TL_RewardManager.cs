using UnityEngine;
using UnityEngine.UI;

public class TL_RewardManager : MonoBehaviour {

    //Buff Durations
    [Header("Durations")]
    public float AttackSpeedDuration;
    public float MoveSpeedDuration;
    public float DefenseDuration;
    public float DisplayDuration;
    public float OverheatDuration;
    public float GradualTime = 30f;
    private float LerpDuration = 0;

    [Header("Powerup Icons")]
    public Image AttackSpeedUp;
    public Image DefenseUp;
    public Image MoveSpeedUp;

    [Header("Sound Effects")]
    public AudioClip AttackSpeedUpSound;
    public AudioClip DefenseUpSound;
    public AudioClip MoveSpeedUpSound;
    public AudioClip ShieldUpSound;
    public AudioClip BaseHealthUpSound;
    public AudioSource AtkSpdUpSoundSource;
    public AudioSource DefUpSoundSource;
    public AudioSource MoveSpdUpSoundSource;
    public AudioSource ShieldUpSoundSource;
    public AudioSource BaseHealthUpSoundSource;

    //Weapons
    [Header("Weapons")]
    public GameObject Gun;

    //Displays
    [Header("Displays")]
    public Text DurationDisplay;
    public Text RewardDisplay;
    public bool ToggleTimer;
    public bool UpgradedWeapons;
    private string OverheatText;
    private string RewardDesc;
    private TL_LevelManager LMScript;
    private TL_MovePC MovementScript;



    void Start()
    {
        //Find the level manager and obtain the script
        LMScript = GameObject.Find("LevelManager").GetComponent<TL_LevelManager>();

        //Find the text for displaying the duration and obtain the text component
        DurationDisplay = GameObject.Find("Camera/Canvas/Duration_Display").GetComponent<Text>();

        //Find the text for displaying the reward message and obtain the text component
        RewardDisplay = GameObject.Find("Camera/Canvas/Reward_Display").GetComponent<Text>();

        //Obtain the script from the PC   
        MovementScript = GetComponent<TL_MovePC>();

        //Obtain the image components from the gameobjects
        AttackSpeedUp = GameObject.Find("Camera/Canvas/AttackSpeedUp").GetComponent<Image>();
        DefenseUp = GameObject.Find("Camera/Canvas/DefenseUp").GetComponent<Image>();
        MoveSpeedUp = GameObject.Find("Camera/Canvas/MoveSpeedUp").GetComponent<Image>();
    }

    void Update()
    {
        //Function for the duration of the boosts the player obtains
        BoostManager();

        //Manages the rewards the player obtains
        RewardManager();

        //Manages the cooldown for the weapons
        WeaponCooldown();
    }

    void WeaponCooldown()
    {
        //If the side weapons exists and it has been upgraded
        if (transform.Find("Side_Gun(Clone)") != null && UpgradedWeapons)
        {
            //If the toggle is true
            if (ToggleTimer)
            {
                //If the PC obtains the attack speed pickup
                if (AttackSpeedDuration > 0f)
                {
                    //Halve the gradual time
                    GradualTime = 15f;

                    //Double the value of deltatime and subtract it from the countdown
                    OverheatDuration -= (Time.deltaTime * 2f);
                }
                else    //If the attack speed duration is finished
                {
                    //Subtract from deltatime to count down
                    OverheatDuration -= Time.deltaTime;
                }
                //Set the text to display the overheat duration
                OverheatText = "Overheat: " + Mathf.Floor(OverheatDuration).ToString("F0");
                
                //Add the duration for the deltatime divided by 30 seconds
                LerpDuration += Time.deltaTime / GradualTime;
            }
            else
            {
                //Set default gradual time
                GradualTime = 30f;

                //Subtract from deltatime to count down
                OverheatDuration -= Time.deltaTime;

                //Set the text to display the durability
                OverheatText = "Cooldown: " + Mathf.Floor(OverheatDuration).ToString("F0");

                //Subtract the duration for the deltatime divided by 30 seconds
                LerpDuration -= Time.deltaTime / GradualTime;
            }

            //If the overheat reaches to 0 then set it back to 30 and use it for the cooldown
            if (OverheatDuration <= 0f)
            {
                //Toggle the boolean
                ToggleTimer = !ToggleTimer;

                //Set the duration to 30
                OverheatDuration = 30f;
            }

            //Obtain the script from the PC and activate the spread shot
            GetComponent<TL_PCShoot>().SpreadShot = ToggleTimer;

            //Find the child gameobjects in the PC
            foreach (Transform Child in transform)
            {
                //If the child gameobject has the PC shoot script attached
                if (Child.GetComponent<TL_PCShoot>() != null)
                {
                    //Slowly change the color from default to red and vice-versa
                    Child.GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.red, LerpDuration);

                    //Obtain the script from the child and activate the spread shot
                    Child.GetComponent<TL_PCShoot>().SpreadShot = ToggleTimer;
                }
            }
            
        }
    }

    void BoostManager()
    {
        //Obtain the script from the PC        
        TL_PCShoot ShootingScript = GetComponent<TL_PCShoot>();

        //Obtain the text components from child gameobjects
        Text AtkSpdDuration = AttackSpeedUp.GetComponentInChildren<Text>();
        Text MoveSpdDuration = MoveSpeedUp.GetComponentInChildren<Text>();
        Text DefenseUpDuration = DefenseUp.GetComponentInChildren<Text>();
        
        //If the duration reaches 0
        if (DisplayDuration <= 0f)
        {
            //Set the duration to 0
            DisplayDuration = 0f;

            //Set the text to blank
            RewardDisplay.text = "";
        }
        else
        {
            //Subtract the deltatime with the variable
            DisplayDuration -= Time.deltaTime;
        }
        //Subtract the deltatime with the variable
        AttackSpeedDuration -= Time.deltaTime;

        //If the duration reaches 0
        if (AttackSpeedDuration < 1f)
        {
            //Hide the powerup icon
            AttackSpeedUp.enabled = false;

            //Set the text to blank
            AtkSpdDuration.text = "";

            //Set the attack rate back to default
            ShootingScript.AttackCooldown = 0.25f;

            //For each of the child transforms from the PC
            foreach (Transform Child in transform)
            {
                //If the child gameobject is not null and it has the PC shoot script
                if (Child != null && Child.GetComponent<TL_PCShoot>() != null)
                {
                    //Obtain the script from the child and set the attack rate back to default
                    ShootingScript = Child.GetComponent<TL_PCShoot>();
                    ShootingScript.AttackCooldown = 0.25f;
                }
            }
            //Set the duration to 0
            AttackSpeedDuration = 0f;
        }
        else
        {
            //Show the powerup icon
            AttackSpeedUp.enabled = true;

            //Set the new attack rate
            ShootingScript.AttackCooldown = 0.175f;

            //For each of the child transforms from the PC
            foreach (Transform Child in transform)
            {
                //If the child gameobject is not null and it has the PC shoot script
                if (Child != null && Child.GetComponent<TL_PCShoot>() != null)
                {
                    //Obtain the script from the child and set the new attack rate
                    ShootingScript = Child.GetComponent<TL_PCShoot>();
                    ShootingScript.AttackCooldown = 0.175f;
                }
            }
            //Set the text for displaying the duration
            AtkSpdDuration.text = Mathf.Floor(AttackSpeedDuration).ToString("F0");
        }

        //Subtract the duration with the variable
        MoveSpeedDuration -= Time.deltaTime;

        //If the duration reaches 0
        if (MoveSpeedDuration < 1f)
        {
            //Hide the powerup icon
            MoveSpeedUp.enabled = false;

            //Set the text to blank
            MoveSpdDuration.text = "";

            //Set the movement speed back to default
            MovementScript.MoveSpeed = 12f;

            //Set the duration to 0
            MoveSpeedDuration = 0f;
        }
        else
        {
            //Show the powerup icon
            MoveSpeedUp.enabled = true;

            //Set the new movement speed
            MovementScript.MoveSpeed = 16f;

            //Set the text for displaying the duration
            MoveSpdDuration.text = Mathf.Floor(MoveSpeedDuration).ToString("F0");
        }
        //Subtract the duration with the variable
        DefenseDuration -= Time.deltaTime;

        //If the duration reaches 0
        if (DefenseDuration < 1f)
        {
            //Hide the powerup icon
            DefenseUp.enabled = false;

            //Set the text to blank
            DefenseUpDuration.text = "";

            //Set the duration to 0
            DefenseDuration = 0f;
        }
        else
        {
            //Show the powerup icon
            DefenseUp.enabled = true;

            //Set the text for displaying the duration
            DefenseUpDuration.text = Mathf.Floor(DefenseDuration).ToString("F0");
        }
        
        //If all of the durations reached 0
        if (AttackSpeedDuration <= 0f && MoveSpeedDuration <= 0f && DefenseDuration <= 0f && OverheatDuration <= 0f)
        {
            //Display the text as blank
            DurationDisplay.text = "";
        }
        else
        {
            //Set the text for displaying the duration
            DurationDisplay.text = "Duration\n" + OverheatText;
        }
    }

    void RewardManager()
    {
        //When the game progresses to the next level and the HP of the base has not been reduced since the previous level
        if (LMScript.PrevLevel != LMScript.Level && LMScript.Prev_BaseHP == LMScript.Base_HP && LMScript.Level >= 2)
        {
            //If a level value is an odd number then reward the player with a weapon
            if (LMScript.Level % 2 == 1)
            {
                //If the PC hasn't been rewarded yet
                if (!LMScript.IsPCAwarded)
                {
                    //If the PC does not have any weapons yet
                    if (transform.Find("Side_Gun(Clone)") == null)
                    {
                        //Set the name of the weapon into the string variable
                        RewardDesc = "Additional Lasers!";

                        //Spawn the weapons
                        GameObject SideGun01Clone = Instantiate(Gun, Vector2.zero, Quaternion.identity);
                        GameObject SideGun02Clone = Instantiate(Gun, Vector2.zero, Quaternion.identity);

                        //Child the spawned weapons onto the PC
                        SideGun01Clone.transform.SetParent(transform);
                        SideGun02Clone.transform.SetParent(transform);

                        //Set their initial local positions
                        SideGun01Clone.transform.localPosition = new Vector3(-0.5f, 0.5f, 0f);
                        SideGun02Clone.transform.localPosition = new Vector3(0.5f, 0.5f, 0f);

                        //Display the reward message
                        RewardDisplay.text = "For completing the wave with the base unharmed, you get \n" + RewardDesc;

                        //Set the display duration to 2
                        DisplayDuration = 2f;
                    }
                    else if(transform.Find("Side_Gun(Clone)") != null && !UpgradedWeapons)
                    {
                        //If the PC has the additional lasers and not the spread shot yet
                        //Set the name of the weapon into the string variable
                        RewardDesc = "Spread Shot!";

                        //Obtain the script from the PC and activate the spread shot
                        GetComponent<TL_PCShoot>().SpreadShot = true;

                        //Find the child gameobjects in the PC
                        foreach (Transform Child in transform)
                        {
                            //If the child gameobject has the PC shoot script attached
                            if (Child.GetComponent<TL_PCShoot>() != null)
                            {
                                //Obtain the script from the child and activate the spread shot
                                Child.GetComponent<TL_PCShoot>().SpreadShot = true;
                            }
                        }
                        //Display the reward message
                        RewardDisplay.text = "For completing the wave with the base unharmed, you get \n" + RewardDesc;

                        //Set the display duration to 2
                        DisplayDuration = 2f;

                        //Set bool to true to indicate upgraded weapons
                        UpgradedWeapons = true;
                    }
                    else
                    {
                        //Make the text blank
                        RewardDisplay.text = "";
                        DisplayDuration = 0f;
                    }
                    //Turn on bool to prevent it from occuring again
                    LMScript.IsPCAwarded = true;
                }
            }
        }
    }

    void DropPickup(GameObject Pickup)
    {
        //Use the name of the gameobject to determine which pickup it is
        switch (Pickup.name)
        {
            case "AtkSpdUp(Clone)":
                //If the attack speed duration is 0 to prevent refreshing the duration
                //after picking up another attack speed powerup
                if (AttackSpeedDuration == 0)
                {
                    //Play the sound effect from the powerup
                    TL_AudioManager.PlaySound(AtkSpdUpSoundSource, AttackSpeedUpSound);

                    //Display the message
                    RewardDisplay.text = "Attack speed boost obtained!";

                    //Set the display and powerup duration
                    DisplayDuration = 2f;
                    AttackSpeedDuration = 16f;
                }
                break;

            case "MoveSpdUp(Clone)":
                //If the movement speed duration is 0 to prevent refreshing the duration
                //after picking up another movement speed powerup
                if (MoveSpeedDuration == 0)
                {
                    //Play the sound effect from the powerup
                    TL_AudioManager.PlaySound(MoveSpdUpSoundSource, MoveSpeedUpSound);

                    //Display the message
                    RewardDisplay.text = "Movement speed boost obtained!";

                    //Set the display and powerup duration
                    DisplayDuration = 2f;
                    MoveSpeedDuration = 16f;
                }
                break;

            case "DefUp(Clone)":
                //If the defense duration is 0 to prevent refreshing the duration
                //after picking up another defense up powerup
                if (DefenseDuration == 0)
                {
                    //Play the sound effect from the powerup
                    TL_AudioManager.PlaySound(DefUpSoundSource, DefenseUpSound);

                    //Display the text
                    RewardDisplay.text = "Defense boost obtained!";

                    //Set the display and powerup duration
                    DisplayDuration = 2f;
                    DefenseDuration = 16f;
                }
                break;

            case "ShieldUp(Clone)":
                //If the current shields is less than its' maximum value
                if (MovementScript.Shields < MovementScript.MaxShields)
                {
                    //Play the sound effect from the powerup
                    TL_AudioManager.PlaySound(ShieldUpSoundSource, ShieldUpSound);

                    //Increase the current shields by 10
                    MovementScript.Shields += 10f;

                    //Display the text
                    RewardDisplay.text = "Shield Up obtained!";

                    //Set the duration to 2
                    DisplayDuration = 2f;
                }
                break;

            case "BaseHealthUp(Clone)":
                //If the current base health is less than the maximum base health
                if (LMScript.Base_HP < LMScript.Max_BaseHP)
                {
                    //Play the sound effect from the powerup
                    TL_AudioManager.PlaySound(BaseHealthUpSoundSource, BaseHealthUpSound);

                    //Increase the current base health by 10
                    LMScript.Base_HP += 10f;

                    //Display the text
                    RewardDisplay.text = "Base Health Up obtained!";

                    //Set the duration to 2
                    DisplayDuration = 2f;
                }
                break;
        }
        //Disable the sprite renderer
        Pickup.GetComponent<SpriteRenderer>().enabled = false;

        //Disable the 2D box collider
        Pickup.GetComponent<BoxCollider2D>().enabled = false;

        //Destroy the gameobject
        Destroy(Pickup);
    }

    void OnCollisionEnter2D(Collision2D col2D)
    {
        //Obtain the rigidbody 2D from the PC and set velocity to 0 during collision
        //to prevent the PC from being moved by NPC's or their projectiles
        Rigidbody2D PCPhysics = GetComponent<Rigidbody2D>();
        PCPhysics.velocity = Vector2.zero;

        //If the PC does not collide into an enemy or anything related to an enemy like a projectile
        if (col2D.transform.tag != "NPC" && col2D.transform.tag != "BossSpawn" && col2D.transform.tag != "NPC_Projectile")
        {
            //If the PC touches any powerup then determine which one it is from this function
            DropPickup(col2D.gameObject);
        }

    }

}
