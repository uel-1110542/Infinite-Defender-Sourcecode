using UnityEngine;

public class TL_Orbit : MonoBehaviour {


    public float OrbitSpeed;
    private Animator Explosion_Animator;



    void Start()
    {
        //Obtain the animator component from the gameobject
        Explosion_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Function for orbiting around the parent gameobject
        OrbitAround();
    }

    void OrbitAround()
    {
        //The if statement checks to see if the explosion animation is enabled or not
        //to prevent the explosion animation being played while the NPC still moves
        if (!Explosion_Animator.enabled)
        {
            //Orbit around the parent
            transform.RotateAround(transform.parent.position, Vector3.forward, OrbitSpeed * Time.deltaTime);
        }            
    }

}
