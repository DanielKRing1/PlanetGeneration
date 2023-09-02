using System.Collections;
using UnityEngine;

public static class MeshGeneratorOld {

	public static MeshData GenerateTerrainMesh(float [,] heightMap){
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		// Values to help center mesh
		float topLeftX = (width - 1) / -2f;
		float topLeftZ = (height - 1) / 2f;

		MeshData meshData = new MeshData (width, height);
		int vertexIndex = 0;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {

				// Add vertex
				meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, heightMap [x, y], topLeftZ - y);
				// Add UVs
				meshData.uvs [vertexIndex] = new Vector2 (x / (float)width, y / (float)height);
				// Add triangles, ignoring right most column (width-1) and bottom most row (height-1)
				if (x < width - 1 && y < height - 1) {
					meshData.AddTriangle (vertexIndex, vertexIndex + width + 1, vertexIndex + width);
					meshData.AddTriangle (vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}

		SpherifyMesh (meshData, width);

		return meshData;
	}


	/**
	 * Curves mesh so each vertex is 'radius' units from a defined 'origin'
	 * by multiplying the direction unit vector (from 'origin' to each vertex) by 'radius'
	 * 
	 * Then applies an exponential function
	 * 		(0.04*width) * 1.25^(height-2)
	 * to smooth out lower heights and emphasize higher points
	 **/
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
}


public class MeshDataOld {
		public Vector3[] vertices;
		public int[] triangles;
		public Vector2[] uvs;

		private int triangleIndex;

		public MeshDataOld(int meshWidth, int meshHeight){
			vertices = new Vector3[meshWidth * meshHeight];
			uvs = new Vector2[meshWidth * meshHeight];
			triangles = new int[(meshWidth-1)*(meshHeight-1) * 6];

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
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uvs;
			mesh.RecalculateNormals ();

			return mesh;
		}
}