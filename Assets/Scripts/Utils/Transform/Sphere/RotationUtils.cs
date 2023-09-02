using System;
using UnityEngine;

public static class RotationUtils {
    
    public static void rotateRelativeToOrigin(Transform transform, Vector3 origin) {
        transform.localEulerAngles = getRotationRelativeToOrigin(transform.position, origin);
    }

    public static Vector3 getRotationRelativeToOrigin(Vector3 position, Vector3 origin) {
        Vector3 displacement = position - origin;
        Debug.Log(displacement);
        

        float zDeg = 180 / Mathf.PI * -Mathf.Sign(displacement.y) * Mathf.Atan2(displacement.x, displacement.y);
        float xDeg = 180 / Mathf.PI * Mathf.Sign(displacement.y) * Mathf.Atan2(displacement.z, displacement.y);

        Debug.Log(xDeg);
        Debug.Log(zDeg);

        return new Vector3(xDeg, 0, zDeg);
    }
}