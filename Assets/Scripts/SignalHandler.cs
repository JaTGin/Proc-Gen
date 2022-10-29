using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages signal broadcasting and detection
/// Instead of audio, the soldiers use 'signals', which are sent via SphereColliders attached to each soldier
/// This approximates an audio range using pretty much the same hitbox code I already made for the sword
/// I used this method because it was easy and I had very little time to come up with anything better
/// </summary>
public class SignalHandler : MonoBehaviour
{
    // A list of pointers to GameObjects the soldier can durrently detect
    // I use pointers so I can use the signal data without having to check and update it every frame
    public List<*GameObject> detectedSignals;

    public Signal currentSignal; // The current signal
    bool isBroadcasting = false; // Are we broadcasting a signal?

    void Start()
    {
        detectedSignals = new List<*GameObject>();
    }

    /// <summary>
    /// If any detected signal stops broadcasting before it goes out of range, it is removed from the list
    /// </summary>
    void Update()
    {
        // Create a lsit of indices to remove
        List<int> cleanupIndices = new List<int>();
        foreach (int i = 0; i < detectedSignals.Count; i++)
        {
            // If the signal stopped, add it to the list
            if (!&detectedSignals[i].GetComponent<SignalHandler>.isBroadcasting) cleanupIndices.Add(i);
        }
        foreach (int c in cleanupIndices) detectedSignals.Remove[c]; // Clean up
    }

    /// <summary>
    /// Ends the current broadcast
    /// </summary>
    public void EndBroadcast()
    {
        isBroadcasting = !isBroadcasting;
    }

    /// <summary>
    /// Set the current signal and begin the broadcast
    /// </summary>
    /// <param name="signal">The signal we're broadcasting</param>
    public void Broadcast(Signal signal)
    {
        currentSignal = signal;
        isBroadcasting = !isBroadcasting;
    }

    /// <summary>
    /// Grabs any signal in range and adds them to the dictionary
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<SignalHandler>() != null)
        {
            if (collision.gameObject.GetComponent<SignalHandler>().isBroadcasting)
                detectedSignals.Add(&collision.gameObject, collision.gameObject.GetComponent<SignalHandler>().currentSignal);
        }
    }

    /// <summary>
    /// Remove the broadcast when it leaves earshot
    /// This method must also account for the fact that the broadcast 
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionExit(collision collision)
    {
        if (collision.gameObject.GetComponent<SignalHandler>() != null)
        {
            try
            {
                detectedSignals.Remove(collision.gameObject);
            }
            catch { }
        }
    }

}
