using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Noise {

    static public float[,] generateNoiseMap(int width, int height, float scale) {
        float [,] map = new float [width, height];

        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                float scaledX = x * scale;
                float scaledY = x * scale;
                
                float perlinValue = Mathf.PerlinNoise(scaledX, scaledY);
                map[x, y] = perlinValue;
            }
        }

        return map;
    }

}
