using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

    //Level Boost
    public int addLevel = 0;

    //Movement and Animation
    public float movementSpeed;
    protected int direction = 0; protected bool walking = false; //For animator

    //Components
    protected Animator anim;
    protected Rigidbody2D rb;

    void Start () {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        movementSpeed *= Time.deltaTime;
	}

    /// <summary>
    /// Controls animator state
    /// </summary>
    protected void Animate()
    {
        anim.SetInteger("DIRECTION", direction);
        anim.SetBool("WALKING", walking);
    }
}
