using UnityEngine;

public class TL_ArcMovement : MonoBehaviour {

    //Variables
    public float Speed;
    private TL_NPC_Attack NPCScript;
    private Animator Explosion_Animator;
    


    void Start()
    {
        //Obtain the script from the gameobject
        NPCScript = GetComponent<TL_NPC_Attack>();

        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        ArcMovement();
    }

    void ArcMovement()
    {
        //The if statement checks to see if the explosion animation is enabled or not
        //to prevent the explosion animation being played while the NPC still moves
        if (!Explosion_Animator.enabled)
        {
            //Rotate around the Z axis at a fixed speed
            transform.RotateAround(Vector3.zero, Vector3.forward, Speed * Time.deltaTime);

            //If the NPC is not aiming at the PC
            if (!NPCScript.IsNPCAiming)
            {
                //Face the NPC in the downwards direction
                transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.zero);
            }            
        }
    }

}
