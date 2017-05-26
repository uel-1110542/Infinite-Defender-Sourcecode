using UnityEngine;

public class TL_SineWaveMovement : MonoBehaviour {

    //Variables
    [Header("Speed of Sine Wave")]
    public float HorizontalSpeed;
    public float VerticalSpeed;
    public float Amplitude;
    public bool InvertAxis;
    private float TimeSinceStart;
    private Vector2 InitialPos;
    private Animator Explosion_Animator;


    
    void Start()
    {
        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();

        //Set default position
        InitialPos = new Vector2(transform.position.x, transform.position.y);
    }

	void Update()
    {
        //Function for moving the gameobject in a sine wave motion
        SineWaveMovement();
    }

    void SineWaveMovement()
    {
        //The if statement checks if the explosion animation is not played to
        //prevent the NPC from moving while the explosion animation is on
        if (!Explosion_Animator.enabled)
        {
            //If the axis is not inverted then use the Sin function in the Y axis            
            if (!InvertAxis)
            {
                //Make the position of the NPC move relative with the initial position and with a Sin function to move in a wave motion
                transform.position = new Vector2(transform.position.x + HorizontalSpeed * Time.deltaTime, InitialPos.y + Mathf.Sin(transform.position.x * VerticalSpeed) * Amplitude);
            }
            else        //If the axis is inverted then use the Sin function in the X axis
            {
                transform.position = new Vector2(InitialPos.x + Mathf.Sin(transform.position.y * VerticalSpeed) * Amplitude, transform.position.y + VerticalSpeed * Time.deltaTime);
            }
        }
    }

}
