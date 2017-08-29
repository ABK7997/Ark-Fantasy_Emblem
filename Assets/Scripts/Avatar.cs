using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

    //Movement and Animation
    public float movementSpeed;
    private Animator anim;
    private int direction = 0; private bool walking = false; //For animator

	void Start () {
        anim = GetComponent<Animator>();

        movementSpeed *= Time.deltaTime;
	}
	
	/// <summary>
    /// Picks up keystrokes for freeroam movement
    /// </summary>
	void Update () {
        Vector3 newPos = new Vector3();
        walking = false;

        //Right and Left
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            newPos.x += movementSpeed;
            transform.localScale = new Vector3(1, 1, 1);
            walking = true;
            direction = 0;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            newPos.x -= movementSpeed;
            transform.localScale = new Vector3(-1, 1, 1);
            walking = true;
            direction = 0;
        }

        //Up and Down
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            newPos.y += movementSpeed;
            walking = true;
            direction = 2;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            newPos.y -= movementSpeed;
            walking = true;
            direction = 1;
        }

        transform.position += newPos;
        animate();
	}

    /// <summary>
    /// Controls animator state
    /// </summary>
    private void animate()
    {
        anim.SetInteger("DIRECTION", direction);
        anim.SetBool("WALKING", walking);
    }
}
