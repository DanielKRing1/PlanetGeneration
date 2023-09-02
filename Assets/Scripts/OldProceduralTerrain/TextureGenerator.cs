using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Generates Texture2D from Color Map or Height Map
 **/
public static class TextureGeneratorOld {

	public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height){
		Texture2D texture = new Texture2D(width, height);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colorMap);
		texture.Apply ();
		return texture;
	}

	public static Texture2D TextureFromHeightMap(float[,] heightMap){
		int width = heightMap.GetLength (0);
		int height = heightMap.GetLength (1);

		// Colors for Texture2D
		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int index = y * width + x;
				// Color based on Noise Map (values are between 0 and 1)
				colorMap [index] = Color.Lerp (Color.blue, Color.white, heightMap [x, y]);
			}
		}

		return TextureFromColorMap (colorMap, width, height);
	}
	
}
