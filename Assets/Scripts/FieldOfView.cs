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
    public float viewRadius = 2f;
    public float audioRadius = 10f;
    [Range(0, 360)]
    public float viewAngle = 130f;
    public LayerMask redMask = 8;
    public LayerMask blueMask = 9;
    public Transform closestTarget;
    public float dstToClosest = 999999f;
    public List<Transform> validTargets = new List<Transform>();
    public int deathToll = 0; // Signals broadcast by soldiers when they die

    // Find all enemies in this soldier's field of view
    // Dear god this is going to cook my PC 
    public void FindTargets(LayerMask allyMask, LayerMask enemyMask)
    {
        validTargets.Clear();
        closestTarget = null;
        dstToClosest = 999999f;
        // Get every target within our set view radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, enemyMask);

        bool firstValid = false;
        // For each target in the radius
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            // Get the transform of the target
            Transform target = targetsInViewRadius[i].transform;

            // Get the direction to the target
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            // dirToTarget = Quaternion.AngleAxis(-90, Vector3.up) * dirToTarget;
            
            // If the direction to the target is within the field of view of this soldier
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = FindDistance(target);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, allyMask))
                {
                    if (firstValid)
                    {
                        closestTarget = target;
                        dstToClosest = dstToTarget;
                        firstValid = false;
                    }
                    else if (dstToTarget < dstToClosest)
                    {
                        closestTarget = target;
                        dstToClosest = dstToTarget;
                    }
                    validTargets.Add(target);
                }
            }
        }
    }

    /// <summary>
    /// Finds all detectable signals and stores them as returnable values
    /// </summary>
    /// <param name="allyMask"> Allies to detect </param>
    /// <returns> The best signal to follow </returns>
    public GameObject FindSignals(LayerMask allyMask)
    {
        GameObject bestSignal = null;
        deathToll = 0;

        // Get every ally within our set view radius
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, audioRadius, allyMask);

        // For each ally in the radius
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {

            // Get the transform of the target
            Transform target = targetsInViewRadius[i].transform;

            // Get the direction to the target
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            // dirToTarget = Quaternion.AngleAxis(-90, Vector3.up) * dirToTarget;
            float dstToTarget = FindDistance(target);
            float closestSig = -1f;

            if (target.gameObject.GetComponent<SignalHandler>() != null && target.gameObject.GetComponent<SignalHandler>().isBroadcasting)
            {
                int tgt = (int)target.gameObject.GetComponent<SignalHandler>().currentSignal;
                // Set the best signal if there is none
                if (bestSignal == null)
                {
                    bestSignal = target.gameObject;
                    closestSig = dstToTarget;
                }
                else
                {
                    int best = (int)bestSignal.GetComponent<SignalHandler>().currentSignal;
                    // If the current best signal is lower priority than the current signal, target is the new best
                    if (best > tgt)
                        bestSignal = target.gameObject;
                    // If they're equal, choose the closest
                    else if (best == tgt)
                        if (dstToTarget < closestSig)
                        {
                            bestSignal = target.gameObject;
                            closestSig = dstToTarget;
                        }
                }
                if (tgt == 2) deathToll++;
            }

        }
        return bestSignal;
    }

    // Gets the distance between a target transform and this soldier
    public float FindDistance(Transform target)
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
