using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The central script for our soldier. Handles stats and execures any final decisions the AI makes
/// </summary>
public class Soldier : MonoBehaviour
{
    public int health = 100; // HP
    int morale = 50; // Morale: determines factors such as aggression and liklihood to surrender
    float moveSpeed = 0.5f;
    GameObject target;
    Renderer soldierRenderer;
    MovementHandler mh;
    
    public Team team;

    // Start is called before the first frame update
    void Start()
    {
        soldierRenderer = gameObject.GetComponent<Renderer>();
        mh = GetComponent<MovementHandler>();
        if (team == Team.RED)
        {
            gameObject.layer = LayerMask.NameToLayer("Red");
            soldierRenderer.material.SetColor("_Color", new Color(0.8f, 0.0f, 0.0f, 1.0f));
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Blue");
            soldierRenderer.material.SetColor("_Color", new Color(0.0f, 0.0f, 0.8f, 1.0f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        mh.MoveSoldier(new Vector3(0.5f, 1.0f, 0.3f), 2.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.getComponent<BoxCollider> != null)
        {
            // Subtract health by 5-20 and knock back
            // knockback is optional but it's also funny
        }
    }
}
