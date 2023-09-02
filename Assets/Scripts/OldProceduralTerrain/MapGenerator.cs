using System.Collections;
using UnityEngine;

/**
 * Used to Generate and Modify a Noise Map
 * Adds Color to map
 * 
 * Calls MapDisplay.DrawMap()
 * 
 **/
public class MapGenerator : MonoBehaviour {

	public enum DrawMode { NoiseMap, ColorMap, Mesh };
	public DrawMode drawMode;

	public int mapWidth;
	public int mapHeight;
	public float noiseScale;

	public int octaves;
	[Range (0, 1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	// Possible land/color types
	public Region[] regions;

	public bool autoUpdate;

	void Update(){
		// offset.x += Time.deltaTime;

		GenerateMap ();
	}

	public void GenerateMap(){
		float[,] noiseMap = NoiseGeneratorOld.GenerateNoiseMap (mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

		Color[] colorMap = CreateColorMap (mapWidth, mapHeight, noiseMap);

		// Draw Map
		DrawMap(noiseMap, colorMap);
	}

	private Color[] CreateColorMap(int width, int height, float[,] noiseMap){
		Color[] colorMap = new Color[width * height];

		for(int y = 0; y < height; y++){
			for (int x = 0; x < width; x++) {
				// Assign color to colorMap
				int index = y * mapWidth + x;
				colorMap[index] = DetermineColor (noiseMap[x, y]);
			}
		}

		return colorMap;
	}
	private Color DetermineColor(float mapHeight){
		// Check each region
		for(int r = 0; r < regions.Length; r++){
			if(mapHeight <= regions[r].maxHeight){
				return regions[r].color;
			}
		}
		return Color.black;
	}

	private void DrawMap(float[,] noiseMap, Color[] colorMap){
		MapDisplay display = FindObjectOfType<MapDisplay> ();

		if (drawMode == DrawMode.NoiseMap) {
			display.DrawTexture (TextureGeneratorOld.TextureFromHeightMap (noiseMap));
		} else if (drawMode == DrawMode.ColorMap) {
			display.DrawTexture (TextureGeneratorOld.TextureFromColorMap (colorMap, mapWidth, mapHeight));
		} else if (drawMode == DrawMode.Mesh) {
			display.DrawMesh (MeshGeneratorOld.GenerateTerrainMesh (noiseMap), TextureGeneratorOld.TextureFromColorMap (colorMap, mapWidth, mapHeight));
		}
	}

	// Called whenver variable is changed in inspector
	void OnValidate(){
		if (mapWidth < 1) {
			mapWidth = 1;
		}
		if (mapHeight < 1) {
			mapHeight = 1;
		}

		if (lacunarity < 1) {
			lacunarity = 1;
		}
		if (octaves < 0) {
			octaves = 0;
		}

	}

	[System.Serializable]
	public struct Region {
		public string type;
		public float maxHeight;
		public Color color;
	}

}
