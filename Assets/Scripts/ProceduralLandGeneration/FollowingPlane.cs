using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingPlane : MonoBehaviour
{
    static private float RATE = 1;
    static readonly public float RADIUS = 10;

    // Recenter this plane once the player passes any of these markers
    private Vector3 topMarker = new Vector3();
    private Vector3 botMarker = new Vector3();
    private Vector3 rightMarker = new Vector3();
    private Vector3 leftMarker = new Vector3();

    // Using arbitrary apiori knowledge
    private Vector3 gridCell = new Vector3(1, 0, 1);

    private Transform player;
    private bool shouldFollowPlayer;

    private Updatable<Transform, Object>[] updatableComponents;

    private Mesh mesh;

    protected void Awake() {
        this.trackPlayer();
        this.trackUpdatableComponents();
    }

    protected void Start() {
        StartCoroutine(this.followPlayer());
    }

// INIT

    /**
    * When the player reaches 1/4 of the plane x/y length, recenter the plane at the 1/4 mark
    *
    * Call when start following Player and each time Player passes a marker

     _________________________________________
    |                                         |
    |                                         |
    |                                         |
    |           ___________________           |
    |          |                   |          |
    |          |                   |          |
    |          |                   |          |
    |          |         x         |          |
    |          |                   |          |
    |          |                   |          |
    |          |                   |          |
    |           ___________________           |
    |                                         |
    |                                         |
    |                                         |
     _________________________________________
    */
    private void setMarkers() {
        float xBound = this.mesh.bounds.size.x / 4 * this.transform.localScale.x;
        float zBound = this.mesh.bounds.size.z / 4 * this.transform.localScale.z;

        this.topMarker.x = this.transform.position.x;
        this.topMarker.y = this.transform.position.y;
        this.topMarker.z = this.transform.position.z + zBound;

        this.botMarker.x = this.transform.position.x;
        this.botMarker.y = this.transform.position.y;
        this.botMarker.z = this.transform.position.z - zBound;

        this.rightMarker.x = this.transform.position.x + xBound;
        this.rightMarker.y = this.transform.position.y;
        this.rightMarker.z = this.transform.position.z;

        this.leftMarker.x = this.transform.position.x - xBound;
        this.leftMarker.y = this.transform.position.y;
        this.leftMarker.z = this.transform.position.z;
    }

    private void trackPlayer() {
        this.player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update() {
        // this.transform.position = this.mapDisplacementOnSphere(this.transform.position, new Vector2(50, 50));
        // print("Sphere point: " + this.transform.position);
        // this.transform.eulerAngles = 180 / Mathf.PI * this.getRadians(this.transform.position);
    }

// CENTER ON PLAYER

    private IEnumerator followPlayer() {
        this.mesh = this.GetComponent<MeshFilter>().mesh;
        this.setMarkers();
        this.shouldFollowPlayer = true;

        while(this.shouldFollowPlayer) {
            if(this.playerPassedMarker())
                this.recenterOnPlayer();

            yield return new WaitForSeconds(FollowingPlane.RATE);
        }
    }
    
    // DISTANCE UTILS

    virtual protected bool playerPassedMarker() {
        return this.player.position.z > this.topMarker.z || // Top
        this.player.position.z < this.botMarker.z || // Bot
        this.player.position.x > this.rightMarker.x || // Right
        this.player.position.x < this.leftMarker.x; // Left
    }

    private void recenterOnPlayer() {
        this.mesh = this.GetComponent<MeshFilter>().mesh;

        // 1. Recenter near player
        this.transform.position = this.getNewPosition();

        // 2. Update rotation based on new position
        this.transform.localEulerAngles = this.getNewRotation(this.transform.position);

        // 3. Reset markers
        this.setMarkers();

        // 4. Do something after recentering, eg Compute new terrain
        this.onRecenterOnPlayer();
    }

    virtual protected Vector3 getNewPosition() {
        float offsetX = this.player.position.x % this.gridCell.x - this.transform.position.x % this.gridCell.x;
        float offsetZ = this.player.position.z % this.gridCell.z - this.transform.position.z % this.gridCell.z;

        return new Vector3(offsetX, 0, offsetZ);
    }

    virtual protected Vector3 getNewRotation(Vector3 newPosition) {
        return this.transform.localEulerAngles;
    }

    private void trackUpdatableComponents() {
        this.updatableComponents = this.GetComponents<Updatable<Transform, Object>>();
    }

    virtual protected void onRecenterOnPlayer() {
        foreach(Updatable<Transform, Object> updatableComponent in this.updatableComponents) {
            updatableComponent.onUpdate(this.transform);
        }
    }

    protected Mesh getMesh() { return this.mesh; }

    protected Transform getPlayer() { return this.player; }

// RADIAN UTILS

// 1. Given point on sphere
// 2. Calc x, y, z radians
// 3.   z = cos(z) * r + sin(x) * r
//      x = cos(x) * r + sin(z) * r
// 4. Add some  amount to x, z
// 5. Covert back to sphere
// 6. Calc long/ lat
//      lon = z / (2PI * r)
//      lat = x / (2PI * r)
// 7.   x = -r * cos(lat)sin(lon)
//      z = r * cos(lat)cos(lon)
//      y = r * sin(lat)
// 8. 
// 9. 
// 10. 

    private Vector3 mapDisplacementOnSphere(Vector3 spherePoint, Vector2 displacement) {
        // 1. Map sphere point to cartesian
        Vector2 cartesianPoint = this.flattenSpherePoint(spherePoint);
        print("Cartesian point: " + cartesianPoint);

        // 2. Add displacement to cartesian point
        cartesianPoint += displacement;
        print("Cartesian point + displacement: " + cartesianPoint);

        // 3. Spherify cartesian point
        return this.spherifyCartesianPoint(cartesianPoint);
    }

    private Vector2 flattenSpherePoint(Vector3 spherePoint) {
        // 1. Get radians
        Vector3 radians = this.getRadians(spherePoint);
        print("radians: " + radians);

        float cartesianX = Mathf.Cos(radians.x) * RADIUS + Mathf.Sin(radians.z) * RADIUS;
        float cartesianZ = Mathf.Cos(radians.z) * RADIUS + Mathf.Sin(radians.x) * RADIUS;

        // 2. Get distance around sphere in x and z directions
        // (radians / 2PI) * (2PI * r)

        return new Vector2(cartesianX, cartesianZ);
    }
    private Vector3 spherifyCartesianPoint(Vector2 cartesianPoint) {
        float lat = cartesianPoint.x / (2 * Mathf.PI * RADIUS);
        float lon = cartesianPoint.y / (2 * Mathf.PI * RADIUS);

        float x = -RADIUS * Mathf.Cos(lat) * Mathf.Sin(lon);
        float z = RADIUS * Mathf.Cos(lat) * Mathf.Cos(lon);
        float y = RADIUS * Mathf.Sin(lon);

        return new Vector3(x, y, z);
    }

    private Vector3 getRadians(Vector3 spherePoint) {
        // float xRadians = Mathf.Acos(spherePoint.z / RADIUS);
        // float zRadians = Mathf.Acos(spherePoint.x / RADIUS);

        float xRadians = Mathf.Acos(Mathf.Clamp(spherePoint.x, -RADIUS, RADIUS) / RADIUS);
        // print("x/radius: " + spherePoint.x / RADIUS);
        // print("X radians: " + xRadians);
        float zRadians = Mathf.Acos(Mathf.Clamp(spherePoint.z, -RADIUS, RADIUS) / RADIUS);
        // print("z/radius: " + spherePoint.z / RADIUS);
        // print("Z radians: " + zRadians);
        float yRadians = Mathf.Acos(Mathf.Clamp(spherePoint.y, -RADIUS, RADIUS) / RADIUS);
        // print("y/radius: " + spherePoint.y / RADIUS);
        // print("Y radians: " + yRadians);

        return new Vector3(xRadians, yRadians, zRadians);
    }
}