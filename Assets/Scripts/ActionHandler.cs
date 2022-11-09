using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles non-movement actions for the soldier, which for now is only attacking with the sword
/// Broadcasting is handled by the Signal Handler
/// 
/// This class is the expert on combat- it watches and shares the cooldown timer of the swing, and tracks the last enemy that hit this soldier
/// </summary>
public class ActionHandler : MonoBehaviour
{
    BoxCollider hitbox; // The sword's hitbox
    float hitTimer = 0.1f; // Active frames for the sword swing
    public float hitCD = 0.0f; // Attack cooldown- soldier can only swing once a second and the blackboard knows when it's on cooldown

    public GameObject vendetta; // The last soldier to hit this one
    float vendettaTimer = 0.0f;
    public int imperative = 0; // Stacks for each hit the vendetta deals to this soldier

    // Start is called before the first frame update
    void Start()
    {
        hitbox = GameObject.Find("Hitbox").GetComponent<BoxCollider>();
    }

    public void Attack()
    {
        // Turn the sword's hitbox on
        if (hitCD <= 0.0f && hitbox != null)
            hitbox.enabled = true;
    }

    public void SetVendetta(GameObject target)
    {
        vendetta = target;
        imperative += 1;
        vendettaTimer = 0.5f * imperative; // Default aggro retention increases with imperative
        
    }

    // Update is called once per frame
    void Update()
    {
        // If the hitbox is enabled, count down then disable it
        if (hitbox != null)
        {
            if (hitbox.enabled)
            {
                hitTimer -= Time.deltaTime;
                if (hitTimer <= 0.0f)
                {
                    // Reset the timer, disable the hitbox
                    hitTimer = 0.1f;
                    hitbox.enabled = false;
                    hitCD = 1.0f;
                }
            }
            else if (hitCD > 0.0f)
            {
                hitCD -= Time.deltaTime;
            }
        }

        if (vendettaTimer > 0.0f)
        {
            vendettaTimer -= Time.deltaTime;
        }
        else
        {
            vendetta = null;
            imperative = 0;
        }
    }
}
