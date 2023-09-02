using System.Collections;
using UnityEngine;

/**
 * Static b/c single instance
 * Used to Get a Perlin Noise Map
 * 
 **/
public static class NoiseGeneratorOld {

	/**
	 * Generates and returns a Perlin Noise Map
	 * 
	 * @param mapWidth, mapHeight- size of noise map
	 * @param scale- float value to divide int x, int y by to get a float value (int would produce identical values for perlin map)
	 * @param offset- allows one to scroll through noise
	 * 
	 * @return noiseMap- 2d float [,] that contains perlin noise for noise map
	 **/
	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
	{
		float [,] noiseMap = new float[mapWidth, mapHeight];

		// Start each octave at different location
		Vector2[] octaveOffsets = GetRandV2Array(seed, octaves, offset);

		if (scale == 0) {
			scale = 0.0001f;
		}

		// Lowest possible max
		// Highest possible min
		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		Vector2 mapCenter = new Vector2(mapWidth / 2f, mapHeight / 2f);

		// For area of map
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {

				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				// For octave levels
				// From pebbles to mountainous outline
				// Each octave offset in order to sample from different locations of perlin noise map
				for (int i = 0; i < octaves; i++)
				{
					noiseHeight += CalculateNoiseHeight (x, y, mapCenter, scale, frequency, amplitude, octaveOffsets[i]);

					// Amplitude increases
					// Frequency decreases
					amplitude *= persistance;
					frequency *= lacunarity;
				}

				// Determine range of noiseMap values;
				maxNoiseHeight = GetMax(noiseHeight, maxNoiseHeight);
				minNoiseHeight = GetMin (noiseHeight, minNoiseHeight);

				// Add noise map value
				noiseMap [x, y] = noiseHeight;
			}
		}

		// Normalize noise map values
		for (int y = 0; y < mapHeight; y++) {
			for (int x = 0; x < mapWidth; x++) {
				// Returns between 0 and 1 for each value
				noiseMap [x, y] = Mathf.InverseLerp (minNoiseHeight, maxNoiseHeight, noiseMap [x, y]);
			}
		}

		return noiseMap;
	}

	/**
	 * Calculates and returns a noise height for noiseMap[x, y]
	 * for each octave (each iteration of frequency and amplitude)
	 * 
	 * @param mapCenter- used to keep noise map centered when scaling
	 * @param octaveOffset- Each octave provides a different V2 offset
	 * in order to calculate height from different part of noise map
	 * 
	 **/
	private static float CalculateNoiseHeight(int x, int y, Vector2 mapCenter, float scale, float frequency, float amplitude, Vector2 octaveOffset){
		// Higher frequency values -> sample values farther apart -> noise map changes more rapidly
		float sampleX = (x - mapCenter.x) / scale * frequency + octaveOffset.x;
		float sampleY = (y - mapCenter.y) / scale * frequency + octaveOffset.y;

		// Mathf.Perlin() returns between 0 and 1
		// *2-1 -> to get between -1 and 1
		float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;

		return perlinValue  * amplitude;
	}



	public static float GetMax(float a, float b){
		return a > b ? a : b;
	}

	public static float GetMin(float a, float b){
		return a < b ? a : b;
	}

	/**
	 * Uses int seed to get pseudo-random number
	 * Fills Vector2 array with random values proceeding seed
	 * 
	 **/
	public static Vector2[] GetRandV2Array(int seed, int length, Vector2 offset = new Vector2()){
		System.Random prng = new System.Random (seed);

		Vector2[] array = new Vector2[length];
		for (int i = 0; i < length; i++) {
			float offsetX = prng.Next (-100000, 100000) + offset.x;
			float offsetY = prng.Next (-100000, 100000) + offset.y;

			array [i] = new Vector2 (offsetX, offsetY);
		}

		return array;
	}
}
