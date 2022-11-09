using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The central script for our soldier.
/// This is essentially our 'blackboard'- the experts are as follows:
///     - ActionHandler determines the most pertinent enemy threat and whether or not an attack can be made
///     - FieldOfView determines the closest enemy target that can be seen
///         - It also acts as a repository for detection methods that are used in the TargetFinder coroutine
///     - FieldOfView and SignalHandler work together to detect and broadcast signals sent between ally soldiers
///         - This is because if I put the FindSignals method in SignalHandler it broke for some reason
///     - MoraleHandler calculates the level of morale the soldier has, determining how cautiously it will act
///         - It also determines selfishness, e.g. whether or not the soldier will surrender
/// From these sensory and 'emotional' elements, the blackboard can theoretically build a complete plan of action for the soldier's most efficient move.
/// </summary>
public class Soldier : MonoBehaviour
{
    

    public int health = 100; // HP
    public Team team = Team.RED; // The team the soldier is on
    int lastCycleHealth = 100; // Health the last time the coroutine was called
    public GameObject target; // The target the soldier is trying to move to
    float moveSpeed = 1.0f; // The amount of force applied when the soldier is trying to move in a direction
    Vector3 patrolNode; // A semi-random patrol point to be used if the soldier can't find anything better to do
    float deathBroadcastTimer = 0.3f; // Stalls the destruction of the soldier postmortem so allies can know it died
    bool surrendered = false; // Did this unit change teams?

    // Handler references
    // These are essentially our blackboard, they store the vast majority of essential data and solution logic for the soldier
    Renderer soldierRenderer;
    MovementHandler mh;
    ActionHandler ah;
    FieldOfView fov;
    SignalHandler sh;
    MoraleHandler mo;

    // Start is called before the first frame update
    void Start()
    {
        
        // Establish references to scripts
        mh = GetComponent<MovementHandler>();
        sh = GetComponent<SignalHandler>();
        ah = GetComponent<ActionHandler>();
        fov = GetComponent<FieldOfView>();
        mo = GetComponent<MoraleHandler>();
        soldierRenderer = gameObject.GetComponent<Renderer>();

        // Randomly choose a team to be assigned to, then get placed depending on the team chosen
        int teamPicker = Random.Range(0, 2);
        if (teamPicker == 0)
        {
            transform.position = new Vector3(
                    Random.Range(3.0f, 4.5f),
                    0.2f,
                    Random.Range(-4.7f, 4.8f));
            SetTeam(Team.BLUE);
        }
        else
        {
            transform.position = new Vector3(
                    Random.Range(-4.5f, -3.0f),
                    0.2f,
                    Random.Range(-4.7f, 4.8f));
            SetTeam(Team.RED);
        }
        
        // Set up the first patrol node
        patrolNode = new Vector3(0f, transform.position.y, transform.position.z);
        transform.LookAt(patrolNode);

        // Start the experts' data-gathering coroutine
        StartCoroutine("TargetCoroutine", 0.2f);
    }

    /// <summary>
    /// This coroutine detects enemies in a vision cone and ally signals in an auditory radius, then sets those scripts' respective reccommendations
    /// By letting the sensing code do some heavy lifting over multiple frames instead of one, we can alleviate some compute time issues
    /// </summary>
    /// <param name="delay"> The delay at which the coroutine executes </param>
    /// <returns></returns>
    IEnumerator TargetCoroutine(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            if (team == Team.RED)
            {
                fov.FindTargets(fov.redMask, fov.blueMask);
                if (!sh.isBroadcasting) sh.bestSignal = fov.FindSignals(fov.redMask);
            }
            else
            {
                fov.FindTargets(fov.blueMask, fov.redMask);
                if (!sh.isBroadcasting) sh.bestSignal = fov.FindSignals(fov.blueMask);
            }

            // Perform morale calculations every cycle
            if (lastCycleHealth != health)
            {
                mo.CalculateMorale(fov.deathToll, sh.bestSignal, true);
                lastCycleHealth = health;
            }
            else mo.CalculateMorale(fov.deathToll, sh.bestSignal);

        }
    }

    /// <summary>
    /// This is where the system's control shell is executed. Using data pulled from its experts, the soldier answers the following questions:
    ///     - What target should I move to?
    ///     - What actions can/should I engage in?
    ///     - Should I act in self-preservation or come to the aid of others?
    /// </summary>
    void Update()
    {
        sh.EndBroadcast(); // Reset the broadcast
        

        // Process death
        if (health <= 0)
        {
            if (deathBroadcastTimer <= 0.0f)
            {
                foreach(Transform c in gameObject.transform)
                    Destroy(c.gameObject);
                Destroy(gameObject);
                return;
            }
            else
            {
                sh.Broadcast(Signal.DEAD);
                GameObject.Find("Hitbox").GetComponent<BoxCollider>().enabled = false;
                deathBroadcastTimer -= Time.deltaTime;
                return;
            }
        }

        if (mo.morale < 25.0f && surrendered == false) Surrender();

        // If the soldier can find a target
        if (Target() != null)
        {
            // Look at the target, move towards it, and attack if it is an enemy
            transform.LookAt(target.transform);
            mh.MoveSoldier(new Vector3(transform.forward.x, 1.0f, transform.forward.z), moveSpeed);
            if (target.layer != gameObject.layer)
                if (fov.FindDistance(target.transform) <= 0.2f && ah.hitCD <= 0.0f)
                    ah.Attack();
        }
        else
        {
            // Otherwise, patrol
            if (Vector3.Distance(transform.position, patrolNode) <= 0.5f) RecalculatePatrolNode();
            transform.LookAt(patrolNode);
            mh.MoveSoldier(new Vector3(transform.forward.x, 1.0f, transform.forward.z), moveSpeed);
        }

        // If the soldier isn't echoing any orders, it is allowed to make its own
        if (sh.isBroadcasting == false)
        {
            // If it needs help, it broadcasts rally
            if (health < 50 && mo.morale < 40.0f)
            {
                sh.Broadcast(Signal.RALLY);
            }
            // If it has an enemy target, it broadcasts a charge
            else if (target != null && mo.morale > 55.0f && target.layer != gameObject.layer)
            {
                sh.Broadcast(Signal.CHARGE);
            }
        }
    }

    GameObject Target()
    {
        target = null;
        // Choosing a signal to follow takes highest priority- soldiers should focus teamwork
        if (sh.bestSignal != null)
        {
            switch ((int)sh.bestSignal.GetComponent<SignalHandler>().currentSignal)
            {
                case 0:
                    // If the soldier has low imperative and detects a rally, they will help and echo the call to arms
                    if (ah.imperative < 4)
                    {
                        target = sh.bestSignal;
                        sh.Broadcast(Signal.RALLY);
                        return target;
                    }
                    break;
                case 1:
                    // If the soldier has high morale and low imperative, they will join a charge
                    if (mo.morale >= 50.0f && ah.imperative < 2)
                    {
                        target = sh.bestSignal.GetComponent<Soldier>().target;
                        sh.Broadcast(Signal.CHARGE);
                        return target;
                    }

                    break;
            }
        }

        // Process what to do if nothing comes up in broadcasts
        if (target == null)
        {
            // Target any vendettas first, then try closest target in vision
            if (ah.vendetta != null)
            {
                target = ah.vendetta;
                return target;
            }
            else if (fov.closestTarget != null)
            {
                target = fov.closestTarget.gameObject;
                return target;
            }
        }

        return target;
    }

    /// <summary>
    /// Patrol to a different position along the center of the map if no enemy is spotted
    /// </summary>
    void RecalculatePatrolNode()
    {
        patrolNode = new Vector3(
                    Random.Range(1.0f, 1.1f),
                    transform.position.y,
                    Random.Range(-4.7f, 4.8f));
    }

    /// <summary>
    /// Switch teams in the name of self-preservation
    /// </summary>
    void Surrender()
    {
        surrendered = true;
        if (team == Team.RED)
        {
            SetTeam(Team.BLUE);
        }
        else
        {
            SetTeam(Team.RED);
        }
    }

    /// <summary>
    /// Set the team of the soldier. Used for instantiation and when the soldier surrenders.
    /// </summary>
    /// <param name="team">The team the soldier is now assigned to</param>
    public void SetTeam(Team team)
    {
        this.team = team;
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
}
