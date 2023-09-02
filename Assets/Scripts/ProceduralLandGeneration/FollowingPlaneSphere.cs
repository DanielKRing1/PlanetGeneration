using UnityEngine;

class FollowingPlaneSphere : FollowingPlane {
    
    protected void Awake() {
        base.Awake();
    }

    protected void Start() {
        base.Start();
    }

    override protected bool playerPassedMarker() {
        return Vector3.Distance(this.transform.position, this.getPlayer().transform.position) > (RADIUS / 10);
    }

    override protected Vector3 getNewPosition() {
        // 1. Get closest vertex coordinates
        int height = (int) Mathf.Sqrt(this.getMesh().vertices.Length);
        int width = (int) Mathf.Sqrt(this.getMesh().vertices.Length);
        Vector2 closestVertexCoord = MeshGenerator.rSearchPlane(this.getMesh(), height, width, (Vector3 vertex) => vertex, (Vector3 vertex) => this.transform.InverseTransformPoint(this.getPlayer().position) - vertex);

        // // 2. Convert 2d coordinates to 1d, then to the vertex value
        // int closestVertexIndex = (int) (width * closestVertexCoord.y + closestVertexCoord.x);
		// Debug.Log("closestVertexIndex: " + closestVertexIndex);
        // Debug.Log(width);
        // Debug.Log(closestVertexCoord);
        // Debug.Log("mesh.vertices.Length: " + this.getMesh().vertices.Length);

        
        // // 3. Convert vertex to world space; Replace height with radius
        // Vector3 closestVertex = this.getMesh().vertices[closestVertexIndex];
        // Vector3 worldPos = this.transform.TransformPoint(closestVertex);
        // Debug.Log("Position: " + closestVertex + ", " + worldPos);
        
        // return worldPos.normalized * RADIUS;

        return this.transform.position;
    }

    override protected Vector3 getNewRotation(Vector3 newPosition) {
        return RotationUtils.getRotationRelativeToOrigin(newPosition, Vector3.zero);
    }

}