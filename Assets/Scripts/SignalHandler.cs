using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages signal broadcasting and detection
/// Instead of audio, the soldiers use 'signals', which are sent via SphereColliders attached to each soldier
/// This approximates an audio range using pretty much the same hitbox code I already made for the sword
/// I used this method because it was easy and I had very little time to come up with anything better
/// 
/// This is also the expert on auditory processing, with the capacity to prioritize the most important target at any given time
/// </summary>
public class SignalHandler : MonoBehaviour
{
    public List<GameObject> detectedSignals;

    public Signal currentSignal; // The current signal
    public bool isBroadcasting = false; // Are we broadcasting a signal?

    public GameObject bestSignal; // The 'best' signal as determined by this expert, allows soldier to prioritize and select allies

    void Start()
    {
        detectedSignals = new List<GameObject>();
    }

    /// <summary>
    /// Ends the current broadcast
    /// </summary>
    public void EndBroadcast()
    {
        isBroadcasting = false;
    }

    /// <summary>
    /// Set the current signal and begin the broadcast
    /// </summary>
    /// <param name="signal">The signal we're broadcasting</param>
    public void Broadcast(Signal signal)
    {
        currentSignal = signal;
        isBroadcasting = true;
    }

}
