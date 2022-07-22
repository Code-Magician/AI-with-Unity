using UnityEngine;
using UnityEngine.AI;

public class FollowPath : MonoBehaviour
{

    // // Tank targer
    // Transform goal;
    // // Tank speed
    // float speed = 5.0f;
    // // Final distance from target
    // float accuracy = 1.0f;
    // // Tank rotation speed
    // float rotSpeed = 2.0f;
    // Access to the WPManager script
    public GameObject wpManager;
    // Array of waypoints
    GameObject[] wps;
    // Current waypoint
    // GameObject currentNode;
    // // Starting waypoint index
    // int currentWP = 0;
    // // Access to the Graph script
    // Graph g;

    NavMeshAgent agent;

    // Use this for initialization
    void Start()
    {

        // Get hold of wpManager and Graph scripts
        wps = wpManager.GetComponent<WPManager>().waypoints;

        // g = wpManager.GetComponent<WPManager>().graph;
        // // Set the current Node
        // currentNode = wps[7];
        agent = this.GetComponent<NavMeshAgent>();
    }

    public void GoToHeli()
    {
        agent.SetDestination(wps[4].transform.position);
        // Use the AStar method passing it currentNode and distination
        // g.AStar(currentNode, wps[4]);
        // // Reset index
        // currentWP = 0;
    }

    public void GoToRuin()
    {
        agent.SetDestination(wps[0].transform.position);
        // Use the AStar method passing it currentNode and distination
        // g.AStar(currentNode, wps[0]);
        // // Reset index
        // currentWP = 0;
    }

    public void GoBehindHeli()
    {
        agent.SetDestination(wps[11].transform.position);
        // Use the AStar method passing it currentNode and distination
        // g.AStar(currentNode, wps[11]);
        // // Reset index
        // currentWP = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        // // If we've nowhere to go then just return
        // if (g.getPathLength() == 0 || currentWP == g.getPathLength())
        //     return;

        // //the node we are closest to at this moment
        // currentNode = g.getPathPoint(currentWP);

        // //if we are close enough to the current waypoint move to next
        // if (Vector3.Distance(
        //     g.getPathPoint(currentWP).transform.position,
        //     transform.position) < accuracy)
        // {
        //     currentWP++;
        // }

        // //if we are not at the end of the path
        // if (currentWP < g.getPathLength())
        // {
        //     goal = g.getPathPoint(currentWP).transform;
        //     Vector3 lookAtGoal = new Vector3(goal.position.x,
        //                                     this.transform.position.y,
        //                                     goal.position.z);
        //     Vector3 direction = lookAtGoal - this.transform.position;

        //     // Rotate towards the heading
        //     this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
        //                                             Quaternion.LookRotation(direction),
        //                                             Time.deltaTime * rotSpeed);

        //     // Move the tank
        //     this.transform.Translate(0, 0, speed * Time.deltaTime);
        // }

    }
}