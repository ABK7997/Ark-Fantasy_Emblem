using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedProjectile : MonoBehaviour {

    private Entity user;
    private Entity target;

    private float xSpeed, ySpeed;

    private Vector3 destination;
    private Vector3 enemyPosition;

    private float epsilon = 0.25f;

    private bool hit = true;
    private bool dodgePlayed = false;
    private bool targetIsUser = false;
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);

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
            //Miss
            if ((Mathf.Abs(position.x - enemyPosition.x) <= 5f)
                && (Mathf.Abs(position.y - enemyPosition.y) <= 5f)
                && !dodgePlayed
                && !targetIsUser)
            {
                Miss();
                dodgePlayed = true;

            }

            //Destroy
            if ((Mathf.Abs(position.x - destination.x) <= 0.25f)
                && (Mathf.Abs(position.y - destination.y) <= 0.25f))
            {
                user.SpecialEffect();
                Destroy(gameObject);
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
            //destination = new Vector3(destination.x * 1.5f, destination.y * 1.5f);
        }

        //Target is same as user
        if (Vector3.Distance(u.transform.position, t.transform.position) == 0 || spc.hitAll) {
            targetIsUser = true;
            destination = new Vector3(start.x + 2.5f, start.y, 0);

            xSpeed = ((destination.x - start.x) * Time.deltaTime) / animationTime;
            ySpeed = ((destination.y - start.y) * Time.deltaTime) / animationTime;

            //If it's a miss, projectile will fly in opposite direction
            if (!accurate)
            {
                destination = new Vector3(start.x - 5f, start.y, 0);
                xSpeed *= -2;
            }
        }

        user = u;
        target = t;

        hit = accurate;

        GetComponent<SpriteRenderer>().sprite = spc.GetSprite();
        enemyPosition = target.transform.position;

        SetRotation();
    }

    //Angle towards target
    private void SetRotation()
    {
        Vector3 pos = transform.position;
        Vector3 tPos = target.transform.position;

        Vector3 scale = transform.localScale;

        //Left or Right
        if (tPos.x < pos.x) scale.x *= -1;

        //Above or Below
        if (tPos.y > pos.y) scale.y *= -1;

        transform.localScale = scale;
    }

    //Miss animation
    private void Miss()
    {
        if (user.pc.IsRightOf(target)) target.pc.SetPosition(2f, 0f);
        else target.pc.SetPosition(-2f, 0f);
    }
}
