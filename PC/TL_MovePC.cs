using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TL_MovePC : MonoBehaviour {

        
    [Header("Indicators")]
    public Text HealthText;
    public Image HealthBar;
    public GameObject ShieldIndicator;

    [Header("PC Stats")]
    public float MaxShields;
    public float Shields;
    public float MoveSpeed;
    private bool FreezePC;
    private TL_RewardManager RewardScript;
    
    [Header("Sound")]    
    public AudioClip ExplosionSound;
    public AudioClip ShieldHitSound;
    public AudioSource HitSoundSource;
    public AudioSource ExplosionSoundSource;

    //Animator
    private Animator Explosion_Animator;

    //Sprite
    private SpriteRenderer PC_Sprite;



    void Awake()
    {
        //Set default value for current health as maximum health
        Shields = MaxShields;

        //Find the display for the health display and obtain the text component
        HealthText = GameObject.Find("Camera/Canvas/HP_Display").GetComponent<Text>();

        //Find the image for the health bar and obtain the image component
        HealthBar = GameObject.Find("Camera/Canvas/ShieldHealthBar").GetComponent<Image>();

        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();

        //Obtain the sprite renderer from the gameobject
        PC_Sprite = GetComponent<SpriteRenderer>();
    }

	void Update()
    {
        //Obtain the script from the PC
        RewardScript = GetComponent<TL_RewardManager>();

        //Function for moving
        MovePC();

        //Displays PC health
        UpdateHPDisplay();
    }

    void UpdateHPDisplay()
    {
        //Updates the health display
        HealthText.text = "Shields:";

        //Calculate the bar width by converting the value of current and max shields
        //and multiplying it by a default size
        float BarWidth = Shields / MaxShields * 125f;

        //Adjust the size of the health bar with the bar width
        HealthBar.rectTransform.sizeDelta = new Vector2(BarWidth, 17f);

        //Change the color of the health bar depending on the width of the health bar
        if (BarWidth <= 30f)
        {
            HealthBar.color = Color.red;
        }
        else if(BarWidth <= 75f)
        {
            HealthBar.color = Color.yellow;
        }
        else if (BarWidth > 75f)
        {
            HealthBar.color = Color.green;
        }

        //If the current health falls below 0 then set it to 0
        if (Shields <= 0f)
        {
            Shields = 0f;
        }
    }

    void MovePC()
    {
        //If the PC is still alive in the scene
        if (Shields > 0f)
        {
            //If the player has touched the screen
            if (Input.GetMouseButton(0) && !FreezePC)
            {
                //Convert the mouse position from the screen to world point position
                Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //Use MoveTowards to move the PC towards the converted mouse position
                transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(MousePos.x, transform.position.y), MoveSpeed * Time.deltaTime);

                //Clamp the X position of the PC to prevent it from moving outside the screen
                transform.position = new Vector2(Mathf.Clamp(transform.position.x, -8f, 8f), transform.position.y);
            }

            //If the touch screen device detects more than 1 touch on the screen          
            if (Input.touchCount > 1)
            {
                //If the touches are detected from either the left or right side of the screen      
                if (Input.GetTouch(0).position.x < Screen.width / 2 && Input.GetTouch(1).position.x > Screen.width / 2 ||
                Input.GetTouch(0).position.x > Screen.width / 2 && Input.GetTouch(1).position.x < Screen.width / 2)
                {
                    //Freeze the movement of the PC
                    FreezePC = true;
                }
            }
            else
            {
                //If the touch screen device does not detect more than 1 touch on the screen then unfreeze the movement of the PC
                FreezePC = false;
            }

        }
        
    }

    IEnumerator ShieldHitIndicator()
    {
        //Activate the shield and wait for a fraction of a second to deactivate the shield to create a hit indicator
        //and set the transparency of the sprite in-between
        ShieldIndicator.SetActive(true);
        PC_Sprite.color = new Color(1f, 1f, 1f, 0.5f);
        yield return new WaitForSeconds(0.05f);
        ShieldIndicator.SetActive(false);
        PC_Sprite.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.05f);
        ShieldIndicator.SetActive(true);
        PC_Sprite.color = new Color(1f, 1f, 1f, 0.5f);
        yield return new WaitForSeconds(0.05f);
        ShieldIndicator.SetActive(false);
        PC_Sprite.color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.05f);
        ShieldIndicator.SetActive(true);
        PC_Sprite.color = new Color(1f, 1f, 1f, 0.5f);
        yield return new WaitForSeconds(0.05f);
        ShieldIndicator.SetActive(false);
        PC_Sprite.color = new Color(1f, 1f, 1f, 1f);
    }

    void OnCollisionEnter2D(Collision2D col2D)
    {
        //If the PC collides with the NPC or a boss spawn itself
        if (col2D.transform.tag == "NPC" || col2D.transform.tag == "BossSpawn")
        {
            //Play the shield hit sound
            TL_AudioManager.PlaySound(ExplosionSoundSource, ShieldHitSound);

            //When the defense duration has not run out
            if (RewardScript.DefenseDuration > 0f)
            {
                //Subtract current health
                Shields -= 15f;
            }
            else
            {
                //Subtract current health
                Shields -= 30f;
            }
            //The condition for the PC's health to be above 0 is to prevent
            //the hit indicator from occuring during the explosion animation
            if (Shields > 0f)
            {
                //Start the coroutine function to display the PC getting damaged
                StartCoroutine(ShieldHitIndicator());
            }
        }

        //If the PC collides with the projectile from the NPC
        if (col2D.transform.tag == "NPC_Projectile")
        {
            //Play the shield hit sound
            TL_AudioManager.PlaySound(HitSoundSource, ShieldHitSound);

            //When the defense duration has not run out
            if (RewardScript.DefenseDuration > 0f)
            {
                //Subtract current health
                Shields -= 5f;
            }
            else
            {
                //Subtract current health
                Shields -= 10f;
            }
            //The condition for the PC's health to be above 0 is to prevent
            //the hit indicator from occuring during the explosion animation
            if (Shields > 0f)
            {
                //Start the coroutine function to display the PC getting damaged
                StartCoroutine(ShieldHitIndicator());
            }
            //Destroy the NPC projectile
            Destroy(col2D.gameObject);
        }

        //If the PC's current health falls to zero or below, destroy itself
        if (Shields <= 0f)
        {
            //Play the sound effect
            TL_AudioManager.PlaySound(ExplosionSoundSource, ExplosionSound);

            //If the animator is not enabled
            if (!Explosion_Animator.enabled)
            {
                //Enable the animator
                Explosion_Animator.enabled = true;
            }

            //Find the child gameobjects in the PC
            foreach (Transform Child in transform)
            {
                //If the child gameobject has the PC shoot script attached
                if (Child.GetComponent<TL_PCShoot>() != null)
                {
                    //Obtain the animator from the child gameobject
                    Animator ChildComponent = Child.GetComponent<Animator>();

                    //If the animator is not enabled
                    if (!ChildComponent.enabled)
                    {
                        //Enable the animator
                        ChildComponent.enabled = true;
                    }
                }
            }

            //Destroy the gameobject when the explosion animation ends
            Destroy(gameObject, Explosion_Animator.GetCurrentAnimatorStateInfo(0).length);
        }

    }

}
