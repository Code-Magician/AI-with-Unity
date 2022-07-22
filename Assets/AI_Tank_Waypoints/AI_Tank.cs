using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Tank : MonoBehaviour
{
    public List<GameObject> wayPoints;
    public int currentWayPoint = 0;
    public float speed = 20;
    public float rotationSpeed = 3;
    public float maxDistance = 10;

    public GameObject tracker;

    // Start is called before the first frame update
    void Start()
    {
        tracker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        tracker.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = this.transform.position;
        tracker.transform.rotation = this.transform.rotation;
    }

    void ProcessTracker()
    {
        if (Vector3.Distance(tracker.transform.position, this.transform.position) > maxDistance)
            return;

        if (Vector3.Distance(tracker.transform.position, wayPoints[currentWayPoint].transform.position) < 5f)
            currentWayPoint++;

        if (currentWayPoint >= wayPoints.Count)
            currentWayPoint = 0;

        tracker.transform.LookAt(wayPoints[currentWayPoint].transform);
        tracker.transform.Translate(0, 0, (speed + 10) * Time.deltaTime);
    }
    // Update is called once per frame
    void Update()
    {
        ProcessTracker();

        // this.transform.LookAt(wayPoints[currentWayPoint].transform);
        Quaternion lookAtRotation = Quaternion.LookRotation(tracker.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtRotation, rotationSpeed * Time.deltaTime);
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
