using UnityEngine;

[System.Serializable]
public struct Link {
    // Our two possible choices of single or bi direction
    public enum direction { UNI, BI };
    // Nodes
    public GameObject node1;
    public GameObject node2;
    // Direction UNI or BI
    public direction dir;
}

public class WPManager : MonoBehaviour {

    // An array of GameObjects to store all the waypoints;
    public GameObject[] waypoints;
    // An array of possible links between nodes
    public Link[] links;
    public Graph graph = new Graph();

    // Use this for initialization
    void Start() {

        // Check we have some waypoints to work with
        if (waypoints.Length > 0) {
            // Loop through all the waypoints and add them to the graph
            foreach (GameObject wp in waypoints) {
                graph.AddNode(wp);
            }

            // Loop through all the possible links and add them to the graph
            foreach (Link l in links) {
                graph.AddEdge(l.node1, l.node2);
                if (l.dir == Link.direction.BI)
                    graph.AddEdge(l.node2, l.node1);
            }
        }
    }

    // Update is called once per frame
    void Update() {

        // Call the graph debugDraw code
        graph.debugDraw();
    }
}
