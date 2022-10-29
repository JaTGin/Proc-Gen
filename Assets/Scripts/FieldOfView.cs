using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script handles FOV calculations and basic vision processing for the soldier
/// Code heavily based on an excellent tutorial by Sebastian Lague on YouTube: https://www.youtube.com/watch?v=rQG9aUWarwE
/// My modifications are fairly minor, I primarily added code to keep the closest enemy target as a variable and replace the obstacle layer with allies
/// </summary>
public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask allyMask;
    public LayerMask enemyMask;

    public List<Transform> validTargets = new List<Transform>();
    public Transform closestTarget;

    void Start()
    {
        // Kick off the vision detection coroutine
        StartCoroutine("TargetFinderCoroutine", 0.2f);
    }

    // Every 0.2 seconds, look for enemies
    IEnumerator TargetFinderCoroutine(float delay)
    {
        while(true) {
            yield return new WaitForSeconds(delay);
            FindTargets();
        }
    }

    // Find all enemies in this soldier's field of view
    // Dear god this is going to cook my PC 
    void FindTargets()
    {
        validTargets.Clear();
        // Get every target within our set view radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, enemyMask);

        // For each target in the radius
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            // Get the transform of the target
            Transform target = targetsInViewRadius[i].transform;

            // Get the direction to the target
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            dirToTarget = Quaternion.AngleAxis(-90, Vector3.up) * dirToTarget;

            // If the direction to the target is within the field of view of this soldier
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = FindDistance(target);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, allyMask))
                {
                    validTargets.Add(target);
                    Debug.Log("ENEMY SPOTTED");
                }
            }
        }

        // Only execute if there are 2 or more valid targets
        if (validTargets.Count > 1)
        {
            // Set the closest target as the first valid target and get its distance to this soldier
            closestTarget = validTargets[0];
            float closestDistance = FindDistance(closestTarget);

            // For each valid target
            for (int i = 1; i < validTargets.Count; i++)
            {
                // Get this target's distance
                float currentDistance = FindDistance(validTargets[i]);

                // If this target is closer than the current closest it becomes the closest
                if (currentDistance < closestDistance)
                {
                    currentDistance = closestDistance;
                    closestTarget = validTargets[i];
                }
            }
        }
    }

    // Gets the distance between a target transform and this soldier
    float FindDistance(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    // Gets a direction from angle
    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
