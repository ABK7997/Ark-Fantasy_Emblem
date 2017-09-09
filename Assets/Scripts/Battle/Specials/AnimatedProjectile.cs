using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedProjectile : MonoBehaviour {

    private Entity user;
    private Entity target;

    private float xSpeed, ySpeed;

    private Vector3 destination;

    private float epsilon = 0.05f;

    private bool hit = true;
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Vector3 enemyPosition = target.transform.position;

        if (hit)
        {
            //Has hit target. Perform spell effect and Destroy game object
            if ((Mathf.Abs(position.x - destination.x) <= epsilon)
                && (Mathf.Abs(position.y - destination.y) <= epsilon))
            {
                user.SpecialEffect();
                Destroy(gameObject);
            }
        }

        //If the shot is a miss
        else
        {
            //Has hit target. Perform spell effect and Destroy game object
            if ((Mathf.Abs(position.x - destination.x) <= epsilon)
                && (Mathf.Abs(position.y - destination.y) <= epsilon))
            {
                Destroy(gameObject);
            }

            if ((Mathf.Abs(position.x - enemyPosition.x) <= 0.25f)
                && (Mathf.Abs(position.y - enemyPosition.y) <= 0.25f))
            {
                user.SpecialEffect();
            }
        }

        newPosition.x = xSpeed;
        newPosition.y = ySpeed;

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
        target = t;

        hit = accurate;

        GetComponent<SpriteRenderer>().sprite = spc.GetSprite();
    }
}
