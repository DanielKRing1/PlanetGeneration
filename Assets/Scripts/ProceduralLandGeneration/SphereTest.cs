using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereTest : MonoBehaviour
{
    static private Vector3 ORIGIN = new Vector3(0, 0, 0);
    static private float RADIUS = 1;

    // Update is called once per frame
    void Update()
    {
        float radius = Vector3.Distance(this.transform.position, SphereTest.ORIGIN);
        this.updateRotation(SphereTest.ORIGIN, radius);
        this.updatePosition(SphereTest.ORIGIN);
    }

    private void updateRotation(Vector3 origin, float radius) {
        // float xDeg = Mathf.Atan2(displacement.x, displacement.y);
        // float zDeg = Mathf.Atan2(displacement.z, displacement.y);

        // print("Z: " + displacement.x / radius);
        // print("X: " + displacement.z / radius);

        // print("Z Acos: " + Mathf.Acos(displacement.x / radius));
        // print("X Acos: " + Mathf.Acos(displacement.z / radius));

        // print("Z deg: " + zDeg);
        // print("X deg: " + xDeg);


        this.transform.localEulerAngles = RotationUtils.getRotationRelativeToOrigin(this.transform.position, origin);
        // print(Quaternion.Euler(90, 0, 90));
    }

    private void updatePosition(Vector3 origin) {
        this.transform.position = (this.transform.position - origin).normalized * RADIUS;
    }
}
