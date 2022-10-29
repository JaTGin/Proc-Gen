using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles non-movement actions for the soldier, which for now is only attacking with the sword
/// Broadcasting is handled by the Signal Handler
/// </summary>
public class ActionHandler : MonoBehaviour
{
    GameObject sword; // The sword we're rotating for a super chunky animation
    BoxCollider hitbox; // The sword's hitbox
    float hitTimer = 0.2f; // Active frames for the sword swing

    // Start is called before the first frame update
    void Start()
    {
        sword = GameObject.Find("Sword");
        hitbox = GameObject.Find("Hitbox").GetComponent<BoxCollider>();
    }

    public void Attack()
    {
        // Turn the sword's hitbox on and animate the sword object
        hitbox.enabled = true;
        sword.transform.Rotate(0f, 0f, 90.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // If the hitbox is enabled, count down then disable it
        if (hitbox.enabled)
        {
            hitTimer -= Time.deltaTime;
            if (hitTimer <= 0.0f)
            {
                // Reset the timer, disable the hitbox, reset the sword rotation
                hitTimer = 0.2f;
                hitbox.enabled = false;
                sword.transform.Rotate(0f, 0f, -90f);
            }
        }
    }
}
