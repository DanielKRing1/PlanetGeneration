using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEntityAi : MovingEntity {
	
    static public readonly float MAX_DIST_SQR_FROM_PLAYER = 100000f;
    static private readonly float Y_ROTATION_DURATION = 0.25f;
    
    private Transform player;
    private float roamTimeout = 0;

    public void Start() {
        this.player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    override protected Vector3 getRoamNormal() {
        if(this.roamExpired()) {
            this.setRoamTimeout();
            StartCoroutine(this.rotateYAxis());
        }
            

		return this.transform.forward;
	}

// ROAM TIMEOUT UTILS

    private void setRoamTimeout() {
        this.roamTimeout = Time.time + Random.Range(3, 8);
    }

    private bool roamExpired() {
        return Time.time > roamTimeout;
    }

    private IEnumerator rotateYAxis() {
        Quaternion startingRotation = this.transform.rotation;
        Quaternion newRotation = this.tooFarFromPlayer() ?
                                    this.getYQuaternionToPlayer()
                                    :
                                    this.getYQuaternionRand();

        float elapsedTime = 0;
        float t = 0;
        while(t <= 1) {
            elapsedTime += Time.deltaTime;
            t = (elapsedTime/Y_ROTATION_DURATION);

            transform.rotation = Quaternion.Slerp (startingRotation, newRotation, t);

            yield return null;
        }
    }

    private bool tooFarFromPlayer() {
        float distSqr = this.getDistSqrFromPlayer();

        // print("Too far: " + (distSqr > MovingEntityAi.MAX_DIST_SQR_FROM_PLAYER) + " " + Time.deltaTime);
        // print("Distance Squared: " + distSqr);

        return distSqr > MovingEntityAi.MAX_DIST_SQR_FROM_PLAYER;
    }
    private float getDistSqrFromPlayer() {
        float distSqr = Mathf.Pow(this.player.position.x - this.transform.position.x, 2) + Mathf.Pow(this.player.position.z - this.transform.position.z, 2);

        return distSqr;
    }

    private Quaternion getYQuaternionRand() {
        return Quaternion.Euler (0, Random.Range (-360, 360), 0);
    }

    private Quaternion getYQuaternionToPlayer() {
        float degToPlayer = (180 / Mathf.PI) * Mathf.Atan2(this.player.position.x - this.transform.position.x, this.player.position.z - this.transform.position.z);
        
        // print("Too far: "+ degToPlayer);

        return Quaternion.Euler (0, degToPlayer, 0);
    }

// // RANDOM VECTOR3 UTILS

//     private Vector3 getRandVector2() {
//         float x = Random.Range(-1f, 1f);
//         float z = Random.Range(-1f, 1f);

//         return new Vector3(x, 0, z).normalized;
//     }

//     private Vector3 getRandVector3() {
//         // 1. Get random Vector2
//         Vector3 v2 = this.getRandVector2();

//         // 2. Add y component to Vector2
//         float y = Random.Range(-1f, 1f);
//         v2.y = y;

//         return v2;
//     }

}
