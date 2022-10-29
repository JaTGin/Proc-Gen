using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    float groundDistance;

    void Start()
    {
        groundDistance = GetComponent<Collider>().bounds.extents.y;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, groundDistance + 0.1f);
    }

    public void MoveSoldier(Vector3 direction, float power)
    {
        if (IsGrounded())
        {
            GetComponent<Rigidbody>().velocity = direction * power;
        }
    }
}
