Shader "Materials/TerrainShader" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MAX_HEIGHT("MAX_HEIGHT", float) = 1
    }

    SubShader {
        // Pass {

            Tags { "RenderType" = "Opaque" }
            LOD 100

            CGPROGRAM
        
            // #pragma vertex vert
            // #pragma fragment frag
            #pragma surface surf Standard fullforwardshadows vertex:vert
            #pragma target 3.5

            #include "UnityCG.cginc"
                    
            // STRUCTS

            struct appdata {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
                float4 terrainData;
                float3 vertPos;
                float3 normal;
                float4 tangent;
            };
            
            // VARIABLES

            sampler2D _MainTex;
            sampler2D _MainTex_ST;

            float4 _Color;
            float _MAX_HEIGHT;

            // FUNCTIONS

            // v2f vert(appdata IN) {
            //     // IN.vertex.x += sin(_Time.y * _Speed + IN.vertex.y * _Amplitude) * _Distance * _Amount;

            //     v2f OUT;
            //     OUT.vertex = UnityObjectToClipPos(IN.vertex);
            //     OUT.normal = IN.normal;
            //     // OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

            //     return OUT;
            // }

            // float4 frag (v2f v) : COLOR {
            //     // Calculate steepness: 0 (flat) - 1 (steep)
            //     // float3 sphereNormal = normalize(IN.vertPos);
            //     // float steepness = 1 - dot(sphereNormal, IN.normal);
            //     float steepness = 1 - dot(v.normal, normalize(v.vertex));
            //     steepness = saturate((steepness-0) / (0.6-0));
            //     float terrainHeight = length(v.vertex);

            //     // Color terrain; Split into 3 sections: Water, Flat, Mountain
            //     float3 flatColor = float3(1, 1, 1);
            //     float3 steepColor = float3(1, 1, 1);

            //     // WATER: 0 - 0.2
            //     if(steepness < 0.2) {
            //         flatColor = float3(25, 23, 226);
            //         steepColor = float3(59, 203, 238);
            //     }

            //     // FLAT: 0.2 - 0.7
            //     else if(steepness < 0.7) {
            //         flatColor = float3(241, 191, 105);
            //         steepColor = float3(36, 123, 53);
            //     }

            //     // MOUNTAIN: 0.7 - 1
            //     else {
            //         flatColor = float3(57, 47, 73);
            //         steepColor = float3(237, 235, 239);
            //     }

            //     float3 color = lerp(flatColor, steepColor, steepness) / 255;

            //     return float4(color, 1);
            // }

            void vert (inout appdata_full v, out Input o) {
			    UNITY_INITIALIZE_OUTPUT (Input, o);
                o.vertPos = v.vertex;
                o.terrainData = v.texcoord;
                o.normal = v.normal;
                o.tangent = v.tangent;
            }
                
            void surf (Input IN, inout SurfaceOutputStandard o) {
                // Calculate steepness: 0 (flat) - 1 (steep)
                // float3 sphereNormal = normalize(IN.vertex);
                // float steepness = 1 - dot(sphereNormal, IN.normal);
                // float3 localUpDir = normalize(float3(1, IN.vertPos.y, 1));
                float steepness = 1 - dot(IN.normal, normalize(float3(0, IN.vertPos.y, 0)));
                float terrainHeight = IN.vertPos.y;
                // float steepness = length(IN.vertPos) / 5;
                // float steepness = IN.vertPos.y;

                float4 deepWater = float4(25, 23, 226, 255) / 255;
                float4 shallowWater = float4(59, 203, 238, 255) / 255;

                float4 lowShoreline = float4(245, 216, 166, 255) / 255;
                float4 highShoreline = float4(150, 75, 0, 255) / 255;

                float4 lowGrassy = float4(168, 222, 138, 255) / 255;
                float4 highGrassy = float4(21, 94, 35, 255) / 255;
                
                float4 lowShrubby = float4(14, 43, 7, 255) / 255;
                float4 highShrubby = float4(34, 107, 16, 255) / 255;

                float4 lowForest = float4(177, 222, 138, 255) / 255;
                float4 highForest = float4(53, 82, 42, 255) / 255;

                float4 lowMountain = float4(139, 143, 139, 255) / 255;
                float4 highMountain = float4(48, 54, 48, 255) / 255;

                float4 lowPeak = float4(64, 66, 64, 255) / 255;
                float4 highPeak = float4(237, 235, 239, 255) / 255;
                
                // Color terrain; Split into 3 sections: Water, Flat, Mountain
                float4 flatColor = deepWater;
                float4 steepColor = highMountain;

                // WATER: 0 - 0.25
                if(terrainHeight < 0.25 * _MAX_HEIGHT) {
                    steepness = saturate((steepness-0) / (0.05 - 0));
                    flatColor = deepWater;
                    steepColor = shallowWater;
                }

                // SHORELINE: 0.25 - 0.4
                else if(terrainHeight < 0.4 * _MAX_HEIGHT) {
                    steepness = saturate((steepness-0) / (0.15 - 0));
                    flatColor = lowShoreline;
                    steepColor = highShoreline;
                }

                // SHRUBBY: 0.4 - 0.5
                // else if(terrainHeight < 0.425 * _MAX_HEIGHT) {
                //     steepness = saturate((steepness-0) / (0.5 - 0));
                //     flatColor = lowShrubby;
                //     steepColor = highShrubby;
                // }

                // GRASSY: 0.5 - 0.6
                else if(terrainHeight < 0.6 * _MAX_HEIGHT) {
                    steepness = saturate((steepness-0) / (0.15 - 0));
                    flatColor = lowGrassy;
                    steepColor = highGrassy;

                    if(terrainHeight < 0.425 * _MAX_HEIGHT && steepness < 0.2) {
                        steepness = saturate((steepness-0) / (0.2 - 0));
                        flatColor = lowShrubby;
                        steepColor = highShrubby;
                    }
                }

                // FOREST: 0.6 - 0.7
                else if(terrainHeight < 0.675 * _MAX_HEIGHT) {
                    steepness = saturate((steepness-0) / (0.15 - 0));
                    flatColor = lowForest;
                    steepColor = highForest;
                }

                // MOUNTAIN: 0.7 - 0.75
                else if(terrainHeight < 0.725 * _MAX_HEIGHT) {
                    steepness = saturate((steepness-0) / (0.15 - 0));
                    flatColor = lowMountain;
                    steepColor = highMountain;
                }

                // PEAK: 0.75 - 1
                else {
                    steepness = saturate((steepness-0) / (0.05 - 0));
                    flatColor = highPeak;
                    steepColor = lowPeak;
                }

                float4 color = lerp(flatColor, steepColor, steepness);

                o.Albedo = color;
            }



            ENDCG
        }
    // }
	FallBack "Diffuse"
}