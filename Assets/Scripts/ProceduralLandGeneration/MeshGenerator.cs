using UnityEngine;

public static class MeshGenerator {
    
    public static MeshData GeneratePlaneMesh(float [,] heightMap, float vertexSpacing, float vertexHeight) {
        // 1. Get mesh vertex dimensions
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        // 2. Center mesh with offsets
        float botLeftX = (width - 1) * vertexSpacing / -2f;
        float botLeftZ = (height - 1) * vertexSpacing / -2f;

        // 3. Build MeshData
        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                // 3.1. Add vertex
                float posX = botLeftX + x * vertexSpacing;
                // Must build left -> right and top -> bot to make mesh uvs visible from top, not bot
                float posZ = -1 * (botLeftZ + y * vertexSpacing);
                meshData.vertices[vertexIndex] = new Vector3(posX, heightMap[x, y] * vertexHeight, posZ);
                // meshData.vertices[vertexIndex] = new Vector3(posX, heightMap[x, y] * 1000f * Mathf.Pow(2 * (Mathf.Abs(heightMap[x, y] - 0.5f)), 0.9f), posZ);
                // Debug.Log(heightMap[x, y]);

                // 3.2. Add UVs
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // 3.3. Add triangles, ignoring right-most column (width-1) and bottom-most row (height-1)
                if(x < width - 1 && y < height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    	private static void SpherifyMesh(MeshData meshData, int width){
		float radius = (4 * width) / (2 * Mathf.PI);
		Vector3 origin = new Vector3(0, -1 * radius, 0);

		for (int i = 0; i < meshData.vertices.Length; i++) {

			Vector3 direction = (meshData.vertices[i] - origin).normalized;

			// Keep betweeen -2 and 2
			float height = meshData.vertices [i].y * 4 - 2;
			float scale = 0.04f * width;
			float adjustedHeight = scale * Mathf.Pow (1.25f, height - 2) - (1.5f * width / 100);

			meshData.vertices [i] = direction * (radius + adjustedHeight);

		}
	}

	static public Vector2 rSearchPlane(Mesh mesh, int height, int width, System.Func<Vector3, Vector3> transformVertex, System.Func<Vector3, Vector2> getDisplacement) {
		Vector2 center2d = new Vector2();
		center2d.x = (int) Mathf.Floor(width / 2);
		center2d.y = (int) Mathf.Floor(height / 2);

		return rSearchPlaneHelper(mesh.vertices, height, width, center2d, height, width, transformVertex, getDisplacement);
	}

	static private Vector2 rSearchPlaneHelper(Vector3[] vertices, int totalHeight, int totalWidth, Vector2 center2d, int searchHeight, int searchWidth, System.Func<Vector3, Vector3> transformVertex, System.Func<Vector3, Vector2> getDisplacementVertexToPlayer) {
		int center1d = (int) (center2d.y * totalWidth + center2d.x);
		Debug.Log("center1d: " + center1d);
		Debug.Log("vertices.Length: " + vertices.Length);
		
		Vector3 centerVertex = vertices[center1d];
		centerVertex = transformVertex(centerVertex);

		Vector2 displacementToPlayer = getDisplacementVertexToPlayer(centerVertex);

		int dx = displacementToPlayer.x >= 0 ? (int) (searchWidth / 2) : (int) (-searchWidth / 2);
		center2d.x += dx;

		int dy = displacementToPlayer.y >= 0 ? (int) (searchHeight / 2) : (int) (-searchHeight / 2);
		center2d.y += dy;

		searchHeight = (int) (searchHeight / 2);
		searchWidth = (int) (searchWidth / 2);

		return searchHeight == 1 && searchWidth == 1 ?
			new Vector2(Mathf.Clamp(center2d.x, 0, totalWidth - 1), Mathf.Clamp(center2d.y, 0, totalHeight - 1))
			:
			rSearchPlaneHelper(vertices, totalHeight, totalWidth, center2d, searchHeight, searchWidth, transformVertex, getDisplacementVertexToPlayer);
	}
}

public class MeshData {
		public Vector3[] vertices;
		public int[] triangles;
		public Vector2[] uvs;

		private int triangleIndex;

		public MeshData(int meshWidth, int meshHeight){
			vertices = new Vector3[meshWidth * meshHeight];
			uvs = new Vector2[meshWidth * meshHeight];
			triangles = new int[Mathf.Abs(meshWidth-1)*Mathf.Abs(meshHeight-1) * 6];

			triangleIndex = 0;
		}

		public void AddTriangle(int a, int b, int c){
			triangles [triangleIndex] = a;
			triangles [triangleIndex + 1] = b;
			triangles [triangleIndex + 2] = c;

			triangleIndex += 3;
		}

		public Mesh CreateMesh(){
			Mesh mesh = new Mesh ();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.RecalculateNormals ();

			return mesh;
		}
}