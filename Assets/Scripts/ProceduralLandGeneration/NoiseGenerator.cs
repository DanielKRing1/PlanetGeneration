using UnityEngine;

public static class NoiseGenerator {

    public static float[,] GenerateNoiseMap(int width, int height, int seed, float vertexSpacing, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float [,] noiseMap = new float[(int) (width / vertexSpacing), (int) (height / vertexSpacing)];

		// Start each octave at a different location
		Vector2[] octaveOffsets = GetRandV2Array(seed, octaves, offset);

        scale = Mathf.Max(scale, 0.0001f);

		// Init max/min values at polar opposite
        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        Vector2 mapCenter = new Vector2(width / 2f, height / 2f);

        float BASE_AMPLITUDE = 1;
        float BASE_FREQUENCY = 1;
        float BASE_NOISE_HEIGHT = 0;

        // For area of map
        for(int y = 0; y < (int) (height / vertexSpacing); y++) {
            for(int x = 0; x < (int) (width / vertexSpacing); x++) {

                float amplitude = BASE_AMPLITUDE;
                float frequency = BASE_FREQUENCY;
                float noiseHeight = BASE_NOISE_HEIGHT;

                // For octave levels
				// From pebbles to mountainous outline
				// Each octave offset is different in order to sample from different locations of perlin noise map
                for(int i = 0; i < octaves; i++) {
                    noiseHeight += CalculateNoiseHeight(x * vertexSpacing, y * vertexSpacing, mapCenter, scale, frequency, amplitude, octaveOffsets[i]);

                    // Amplitude increases
					// Frequency decreases
					amplitude *= persistance;
					frequency *= lacunarity;
                }

                // Determine range of noiseMap values;
				maxNoiseHeight = Mathf.Max(noiseHeight, maxNoiseHeight);
				minNoiseHeight = Mathf.Min(noiseHeight, minNoiseHeight);
                
				// Add noise map value
				noiseMap [x, y] = noiseHeight;
            }
		}

		maxNoiseHeight = 0;
		minNoiseHeight = 0;

		for(int i = 0; i < octaves; i++) {
			maxNoiseHeight += 1 * Mathf.Pow(persistance, i);
 			minNoiseHeight -= 1 * Mathf.Pow(persistance, i);
		}

		// Normalize noise map values
		for(int y = 0; y < noiseMap.GetLength(1); y++) {
			for(int x = 0; x < noiseMap.GetLength(0); x++) {
				// Returns between 0 and 1 for each value
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
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
	private static float CalculateNoiseHeight(float x, float y, Vector2 mapCenter, float scale, float frequency, float amplitude, Vector2 octaveOffset){
		// Higher frequency values -> sample values farther apart -> noise map changes more rapidly
		float sampleX = (x - mapCenter.x) * scale * frequency + octaveOffset.x;
		float sampleY = (y - mapCenter.y) * scale * frequency + octaveOffset.y;

		// Mathf.Perlin() returns between 0 and 1
		// *2-1 -> to get between -1 and 1
		float perlinValue = Mathf.PerlinNoise (sampleX, sampleY) * 2 - 1;

		return perlinValue * amplitude;
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