using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { RED, BLUE }; // The team colors

public enum Signal { RALLY = 0, CHARGE = 1, DEAD = 2 }; // The types of signals a soldier can broadcast to allies

public class GameManager : MonoBehaviour
{
    public GameObject soldier;
    public int simSize = 20;
    List<GameObject> soldiersRed, soldiersBlue;

    /// <summary>
    /// Flip a coin and assign a soldier to a side 
    /// </summary>
    void Start()
    {
        soldiersRed = new List<GameObject>();
        soldiersBlue = new List<GameObject>();
        for (int i = 0; i < simSize; i++)
        {
            GameObject soldierTemp = Instantiate(soldier, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            if (soldierTemp.GetComponent<Soldier>().team == Team.RED) soldiersRed.Add(soldierTemp);
            else soldiersBlue.Add(soldierTemp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
