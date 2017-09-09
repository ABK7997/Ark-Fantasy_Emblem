using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedProjectile : MonoBehaviour {

    private Entity user;

    private float xSpeed, ySpeed;

    private Vector3 destination;

    private float epsilon = 0.05f;

    private bool hit = true;
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);

        //Has hit target or (if missed) far beyond target. Destroy game object
        if ( (Mathf.Abs(position.x - destination.x) <= epsilon) 
            && (Mathf.Abs(position.y - destination.y) <= epsilon) ) {

            //Spell effect
            if (hit)
            {
                user.SpecialEffect();
            }

            Destroy(gameObject);
        }

        //Animate
        else
        {
            newPosition.x = xSpeed;
            newPosition.y = ySpeed;
        }

        transform.position += newPosition;
    }

    /// <summary>
    /// A method called at this object's instantiation. Gives it the approriate information to animate.
    /// </summary>
    public void StartAnimation(Special spc, Entity u, Entity t, float animationTime, bool accurate)
    {
        Vector3 start = u.transform.position;
        destination = t.transform.position;

        xSpeed = ((destination.x - start.x) * Time.deltaTime) / animationTime;
        ySpeed = ((destination.y - start.y) * Time.deltaTime) / animationTime;

        //Spell will miss - flies past target
        if (!accurate)
        {
            destination *= 2;
        }

        user = u;

        hit = accurate;

        GetComponent<SpriteRenderer>().sprite = spc.GetSprite();
    }
}
