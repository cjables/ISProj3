// Patrol.cs
using UnityEngine;
using UnityEngine.AI;                   // for NavMeshAgent
using System.Collections;               // for IEnumerator
using System.Collections.Generic;       // for lists

[RequireComponent(typeof(AIFoV))]
public class Patrol : MonoBehaviour {

    public Transform[] points;
    public float reactionTime = 1;      // how long can we see the player before springing into action.
    public List<Transform> looks = new List<Transform>();
    public Transform eyePivot;
    public AnimationCurve curve;

    private int destPoint = 0;
    private NavMeshAgent agent;

    bool stopped = false;
    bool hunting = false;

    private AIFoV fov;


    void Start () {
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<AIFoV>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        // agent.autoBraking = false;

        GotoNextPoint();
    }


    void GotoNextPoint() {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    private float eyesOnPlayerTimer = 0;

    void Update () {
        if(fov.canSeePlayer == true) {
            eyesOnPlayerTimer += Time.deltaTime;
            // Debug.Log("EyesOnPlayerTimer: " + eyesOnPlayerTimer);
            if(eyesOnPlayerTimer > reactionTime) {
                hunting = true;
                if(stopped) {
                    StopCoroutine(WaitAtPatrolPoint());
                    stopped = false;
                    eyePivot.rotation = looks[0].rotation;
                }
                agent.destination = fov.player.position;
                return;     // don't look at anything else in the Update function.
            }
        }
        else {
            //reset the eyesOnPlayerTimer if we lose sight of the player.
            eyesOnPlayerTimer = 0;
        }

        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            if(hunting) hunting = false;
            if(!stopped)StartCoroutine(WaitAtPatrolPoint());
        }
    }

    IEnumerator WaitAtPatrolPoint() {
        Debug.Log("Starting WaitAtPatrolPoint()");
        stopped = true;
        // play the waiting animation
        // stop
        // Debug.Log("Stopping.");
        // agent.destination = this.transform.position;
        // look left and right
        // lerp -90 degrees and wait - or rotate
        float timer = 0f;
        while(timer < 1) {
            eyePivot.rotation = Quaternion.Lerp(looks[0].rotation, looks[1].rotation, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        // lerp 180 degrees and wait
        timer = 0f;
        while(timer < 1) {
            eyePivot.rotation = Quaternion.Lerp(looks[1].rotation, looks[2].rotation, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        // lerp -90 degrees and go to next point.
        timer = 0f;
        while(timer < 1) {
            eyePivot.rotation = Quaternion.Lerp(looks[2].rotation, looks[0].rotation, curve.Evaluate(timer));
            timer += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("Going to Next Point.");
        stopped = false;
        GotoNextPoint();
    }
}