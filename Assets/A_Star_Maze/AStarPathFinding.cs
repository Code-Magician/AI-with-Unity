using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathMarker
{
    public MapLocation location;
    public float H;
    public float G;
    public float F;
    public GameObject model;
    public PathMarker parent;

    public PathMarker(MapLocation l, float f, float g, float h, GameObject m, PathMarker p)
    {
        location = l;
        H = h;
        G = g;
        F = f;
        model = m;
        parent = p;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !this.GetType().Equals(obj.GetType()))
            return false;
        else
        {
            PathMarker temp = (PathMarker)(obj);
            bool eq = location.Equals(temp.location);
            return eq;
        }
    }

    public override int GetHashCode()
    {
        return 0;
    }
}



public class AStarPathFinding : MonoBehaviour
{
    [Header("Properties")]
    public Maze maze;
    public Material openMaterial;
    public Material closedMaterial;
    public GameObject startModel;
    public GameObject pathModel;
    public GameObject goalModel;

    List<PathMarker> open = new List<PathMarker>();
    List<PathMarker> closed = new List<PathMarker>();
    List<PathMarker> finalPath = new List<PathMarker>();
    PathMarker start;
    PathMarker goal;
    PathMarker weAreAt;
    bool done = false;
    bool started = false;


    private void Start()
    {
        maze.Build();
    }

    // public IEnumerator Build()
    // {

    //     BeginSearch();
    //     yield return new WaitForSeconds(2f);
    //     while (!done)
    //         FindPath(weAreAt);

    //     StorePath();
    //     StartCoroutine(MakePath());
    // }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !started)
            BeginSearch();
        if (Input.GetKeyDown(KeyCode.Space) && !started)
        {
            started = true;
            StartCoroutine(FindPath(weAreAt));
        }
        if (Input.GetKeyDown(KeyCode.M) && done)
        {
            StartCoroutine(MakePath());
        }
    }


    public void BeginSearch()
    {
        done = false;
        RemoveAllMarkers();

        List<MapLocation> allCorridors = new List<MapLocation>();
        for (int z = 0; z < maze.depth; z++)
            for (int x = 0; x < maze.width; x++)
                if (maze.map[x, z] != 1)
                    allCorridors.Add(new MapLocation(x, z));

        MapLocation startLocation = allCorridors[Random.Range(0, allCorridors.Count)];
        MapLocation goalLocation = allCorridors[Random.Range(0, allCorridors.Count)];
        while (goalLocation.Equals(start))
            goalLocation = allCorridors[Random.Range(0, allCorridors.Count)];

        start = new PathMarker(startLocation, 0, 0, 0,
                    Instantiate(startModel, new Vector3(startLocation.x * maze.scale, 0, startLocation.z * maze.scale),
                    Quaternion.identity), null);
        goal = new PathMarker(goalLocation, 0, 0, 0,
                    Instantiate(goalModel, new Vector3(goalLocation.x * maze.scale, 0, goalLocation.z * maze.scale),
                    Quaternion.identity), null);

        open.Clear();
        closed.Clear();
        open.Add(start);
        weAreAt = start;
    }


    public IEnumerator FindPath(PathMarker thisNode)
    {
        yield return new WaitForSeconds(0.1f);
        if (thisNode.Equals(goal))
        {
            StorePath();
            done = true;
            yield break;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbourLocation = thisNode.location + dir;

            if (maze.map[neighbourLocation.x, neighbourLocation.z] == 1)
                continue;

            if (neighbourLocation.x <= 0 || neighbourLocation.x >= maze.width ||
                neighbourLocation.z <= 0 || neighbourLocation.z >= maze.depth)
                continue;

            if (IsInClosedList(neighbourLocation))
                continue;

            float G = Vector2.Distance(thisNode.location.ToVector(), neighbourLocation.ToVector()) + thisNode.G;
            float H = Vector2.Distance(neighbourLocation.ToVector(), goal.location.ToVector());
            float F = H + G;

            PathMarker tempMarker = new PathMarker(neighbourLocation, F, G, H,
                    Instantiate(pathModel, new Vector3(neighbourLocation.x * maze.scale, 0, neighbourLocation.z * maze.scale),
                    Quaternion.identity), thisNode);

            TextMesh[] values = tempMarker.model.gameObject.GetComponentsInChildren<TextMesh>();
            values[0].text = "G:" + G.ToString("0.00");
            values[1].text = "H:" + H.ToString("0.00");
            values[2].text = "F:" + F.ToString("0.00");

            if (!UpdateMarker(neighbourLocation, F, G, H, thisNode))
                open.Add(tempMarker);
            else
            {
                Destroy(tempMarker.model);
                tempMarker = null;
            }
        }
        open = open.OrderBy(p => p.F).ToList<PathMarker>();
        PathMarker next = open.ElementAt(0);
        closed.Add(next);
        open.RemoveAt(0);

        next.model.GetComponent<Renderer>().material = closedMaterial;
        weAreAt = next;
        StartCoroutine(FindPath(weAreAt));
    }


    public bool UpdateMarker(MapLocation loc, float f, float g, float h, PathMarker thisNode)
    {
        foreach (PathMarker p in open)
        {
            if (p.location.Equals(loc))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = thisNode;
                return true;
            }
        }
        return false;
    }

    public bool IsInClosedList(MapLocation neighbourLocation)
    {
        foreach (PathMarker p in closed)
        {
            if (p.Equals(neighbourLocation))
                return true;
        }
        return false;
    }


    public void StorePath()
    {
        finalPath.Clear();
        PathMarker curr = weAreAt;
        while (weAreAt != null)
        {
            finalPath.Insert(0, weAreAt);
            weAreAt = weAreAt.parent;
        }
    }

    public IEnumerator MakePath()
    {
        yield return new WaitForSeconds(5f);
        RemoveAllMarkers();
        for (int i = 0; i < finalPath.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Instantiate(pathModel, new Vector3(finalPath.ElementAt(i).location.x * maze.scale, 0, finalPath.ElementAt(i).location.z * maze.scale),
                        Quaternion.identity);
        }
        started = false;
    }


    public void RemoveAllMarkers()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject m in objs)
            Destroy(m);
    }
}
