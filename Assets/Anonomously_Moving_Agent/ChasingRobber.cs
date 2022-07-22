using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingRobber : MonoBehaviour
{
    public GameObject cop;
    NavMeshAgent robberAgent;
    bool coolDown = false;
    float canWanderMinDist = 20f;

    void Start()
    {
        robberAgent = this.GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (CanWander())
        {
            Wander();
        }
        else
        {
            if (!coolDown)
            {
                if (CheckHidden() && lookingAtAgent())
                {
                    Hide();
                    coolDown = true;
                    Invoke("CoolingDown", 5);
                }
                else
                    Pursuit();
            }
        }
    }

    private void Seek(Vector3 copLocation)
    {
        robberAgent.SetDestination(copLocation);
    }

    private void Flee(Vector3 copLocation)
    {
        Vector3 fleeDir = this.transform.position - copLocation;
        robberAgent.SetDestination(this.transform.position + fleeDir);
    }

    private void Pursuit()
    {
        Vector3 targetDir = cop.transform.position - this.transform.position;


        float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(cop.transform.forward));
        float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDir));

        if (relativeHeading < 20f && toTarget > 90f || cop.GetComponent<ControlCop>().currentSpeed < 0.01f)
        {
            Seek(cop.transform.position);
            return;
        }

        float lookAhead = targetDir.magnitude / (robberAgent.speed + cop.GetComponent<ControlCop>().currentSpeed);
        Seek(cop.transform.position + cop.transform.forward * lookAhead);
    }

    private void Evade()
    {
        Vector3 targetDir = cop.transform.position - this.transform.position;

        // float relativeHeading = Vector3.Angle(this.transform.forward, this.transform.TransformVector(cop.transform.forward));
        // float toTarget = Vector3.Angle(this.transform.forward, this.transform.TransformVector(targetDir));

        // if (relativeHeading < 20f && toTarget > 90f || cop.GetComponent<ControlCop>().currentSpeed < 0.01f)
        // {
        //     Vector3 x = this.transform.position - cop.transform.position;
        //     x = this.transform.position + x;
        //     Flee(x);
        //     return;
        // }

        float lookAhead = targetDir.magnitude / (robberAgent.speed + cop.GetComponent<ControlCop>().currentSpeed);
        Flee(cop.transform.position + cop.transform.forward * lookAhead);
    }

    Vector3 wanderTarget = Vector3.zero;
    private void Wander()
    {
        float wanderRadius = 50;
        float wanderDistance = 20;
        float wanderJitter = 5;

        wanderTarget += new Vector3(Random.Range(-1f, 1f) * wanderJitter, 0, Random.Range(-1f, -1f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 localTarget = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 worldTarget = this.transform.TransformVector(localTarget);

        Seek(worldTarget);
    }

    private void Hide()
    {
        float dist = Mathf.Infinity;
        Vector3 choosenPos = Vector3.zero;
        Vector3 choosenDir = Vector3.zero;
        GameObject choosenGo = World.Instance.HideObjects()[0];

        for (int i = 0; i < World.Instance.HideObjects().Length; i++)
        {
            Vector3 hideDir = World.Instance.HideObjects()[i].transform.position - cop.transform.position;
            Vector3 hidePos = World.Instance.HideObjects()[i].transform.position + hideDir.normalized * 10;

            if ((this.transform.position - hidePos).magnitude < dist)
            {
                choosenPos = hidePos;
                choosenDir = hideDir;
                choosenGo = World.Instance.HideObjects()[i];
                dist = (this.transform.position - hidePos).magnitude;
            }
        }

        Collider hideCollider = choosenGo.GetComponent<Collider>();
        Ray backRay = new Ray(choosenPos, -1 * choosenDir.normalized);
        RaycastHit info;
        float rayDist = 250f;
        hideCollider.Raycast(backRay, out info, rayDist);

        Seek(info.point + choosenDir.normalized);
    }

    private bool CheckHidden()
    {
        RaycastHit info;
        Vector3 rayToRobber = cop.transform.position - this.transform.position;
        if (Physics.Raycast(this.transform.position, rayToRobber, out info))
        {
            if (info.transform.gameObject.tag == "cop")
            {
                return true;
            }
        }
        return false;
    }

    private bool lookingAtAgent()
    {
        Vector3 toAgent = this.transform.position - cop.transform.position;
        float angle = Vector3.Angle(toAgent, cop.transform.forward);

        if (angle < 60)
        {
            return true;
        }
        return false;
    }

    private void CoolingDown()
    {
        coolDown = false;
    }

    private bool CanWander()
    {
        float dist = Vector3.Distance(this.transform.position, cop.transform.position);
        if (dist > canWanderMinDist)
            return true;
        return false;
    }
}
