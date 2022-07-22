using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public GameObject shellPrefab;
    public GameObject shellSpawnPos;
    public GameObject target;
    public GameObject parent;
    float speed = 20;
    float turnSpeed = 5;
    bool canFire = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = target.transform.position - parent.transform.position;
        Quaternion lookInDirection = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        parent.transform.rotation = Quaternion.Slerp(parent.transform.rotation, lookInDirection, Time.deltaTime * turnSpeed);

        float? angle = RotateTurret();
        if (canFire)
        {
            canFire = false;
            StartCoroutine(FireAI(angle, direction));
        }
    }

    IEnumerator FireAI(float? angle, Vector3 direction)
    {
        if (angle != null)
        {
            yield return new WaitForSeconds(0.2f);
            Fire();
        }
        yield return null;
        canFire = true;
    }

    private void Fire()
    {
        GameObject shell = Instantiate(shellPrefab, shellSpawnPos.transform.position, shellSpawnPos.transform.rotation);
        shell.GetComponent<Rigidbody>().velocity = speed * this.transform.forward;
    }


    float? RotateTurret()
    {

        float? angle = CalculateAngle(true);
        if (angle != null)
        {
            this.transform.localEulerAngles = new Vector3(360 - (float)angle, 0, 0);
        }
        return angle;
    }

    float? CalculateAngle(bool low)
    {
        Vector3 targetDir = target.transform.position - this.transform.position;
        float y = targetDir.y;
        targetDir.y = 0;
        float x = targetDir.magnitude;

        float gravity = 9.8f;

        float sSqr = speed * speed;
        float underSquarRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

        if (underSquarRoot >= 0)
        {
            float root = Mathf.Sqrt(underSquarRoot);
            float highAngle = sSqr + root;
            float lowAngle = sSqr - root;

            if (low)
                return Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg;
            else
                return Mathf.Atan2(highAngle, gravity * x) * Mathf.Rad2Deg;
        }
        else
            return null;
    }
}
