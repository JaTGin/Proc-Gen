using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    float groundDistance; // Distance to the ground
    Transform currentTarget; // The current target

    void Start()
    {
        groundDistance = GetComponent<Collider>().bounds.extents.y;
    }

    /// <summary>
    /// Clamp the height the soldier can reach and ground it if it goes too high
    /// Prevents gravity from breaking
    /// </summary>
    void Update()
    {
        if (transform.position.y > 1f)
        {
            transform.position = new Vector3(transform.position.x, 0.1f, transform.position.z);
        }
    }

    /// <summary>
    /// Checks if the soldier is on the ground
    /// </summary>
    /// <returns> True if grounded </returns>
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundDistance + 0.1f);
    }

    /// <summary>
    /// Allows the soldier to move itself
    /// </summary>
    /// <param name="direction"> The direction of the soldier </param>
    /// <param name="power"> The power fo teh movement- move speed </param>
    public void MoveSoldier(Vector3 direction, float power)
    {
        if (IsGrounded())
        {
            GetComponent<Rigidbody>().velocity = direction * power;
        }
    }

    /// <summary>
    /// THrows the soldier when acted upon by an external force
    /// </summary>
    /// <param name="direction"> dir </param>
    /// <param name="power"> power </param>
    public void LaunchSoldier (Vector3 direction, float power) { GetComponent<Rigidbody>().velocity = direction * power; }
}
