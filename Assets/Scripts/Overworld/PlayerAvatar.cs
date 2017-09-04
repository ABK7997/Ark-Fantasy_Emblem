using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The avatar of the player, who wanders around the overworld but is unsuable in any other game state
/// </summary>
public class PlayerAvatar : Avatar {

    /// <summary>
    /// Picks up keystrokes for freeroam movement
    /// </summary>
    void Update()
    {
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
        Animate();
    }

    void OnTriggerEnter2D(Collider2D collide)
    {
        if (collide.gameObject.tag == "Enemy")
        {
            SceneManager.LoadScene("battle");
        }
    }
}
