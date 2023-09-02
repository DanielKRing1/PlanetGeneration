using System.Collections;
using UnityEngine;
public class ProceduralLandGeneration : MonoBehaviour, Updatable<Transform, Object>
{

    // Best configs:
    // VertexHeight 800, Dim (100, 100), Vertex Spacing 20, Scale 0.001, Octaves 2, Persistance 0.075, Lacunarity 8
    // VertexHeight 500/800/1000 (Beach/Normal/Mountain) Dim(1000, 1000), Vertex Spacing 20, Scale 0.001, Octaves 3, Persistance 0.5, Lacunarity 2

    public Vector2 dim;

    // height vetexSpacing 0.25: 4, vetexSpacing 1: 4, vetexSpacing 0.005: 0.1, vetexSpacing 0.25: 8, vetexSpacing 0.25: 4

    public float vertexSpacing = 0.05f;
    public float scale; // vetexSpacing 0.25: 0.001, 0.05; vertexSpacing 1: 0.05, 0.05, vertexSpacing 0.005: 0.015, vetexSpacing 0.25: 0.015, vetexSpacing 0.25: 0.0001
    public int octaves; // vetexSpacing 0.25: 5, 2; vertexSpacing 1: 3, 3, vertexSpacing 0.005: 2, vetexSpacing 0.25: 2, vetexSpacing 0.25: 4
    public float persistance; // vetexSpacing 0.25: 0.9, 0.25; vertexSpacing 1: 0.2, 0.15, vertexSpacing 0.005: 0.05, vetexSpacing 0.25: 0.05, vetexSpacing 0.25: 0.2
    public float lacunarity; // vetexSpacing 0.25: 100, 4; vertexSpacing 1: 5, 8, vertexSpacing 0.005: 10, vetexSpacing 0.25: 8, vetexSpacing 0.25: 400

    public float vertexHeight;

    public Vector2 offset;

    public Material material;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    void Awake()
    {
        this.meshRenderer = this.GetComponent<MeshRenderer>();
        this.meshRenderer.material = this.material;
        this.meshRenderer.material.SetFloat("_MAX_HEIGHT", vertexHeight);
        this.meshFilter = this.GetComponent<MeshFilter>();

        onUpdate(this.transform);

        StartCoroutine(this.updateOffset());
    }

    // void Update() {
    //     this.offset.x += this.scale * 5f;
    //     this.offset.y += this.scale * 5f;
    
    //     this.onUpdate(this.transform);
    // }

    private IEnumerator updateOffset() {
        while(true) {
            this.offset.x += this.scale * 5f;
            this.offset.y += this.scale * 5f;
        
            this.onUpdate(this.transform);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public Object onUpdate(Transform transform) {
        float[,] heightMap = NoiseGenerator.GenerateNoiseMap((int) dim.x, (int) dim.y, 10, this.vertexSpacing, scale, octaves, persistance, lacunarity, new Vector2(offset.x, offset.y));
        MeshData meshData = MeshGenerator.GeneratePlaneMesh(heightMap, this.vertexSpacing, this.vertexHeight);
		this.meshFilter.mesh = meshData.CreateMesh();
        this.meshFilter.mesh.MarkModified();

        // this.material.EnableKeyword("_NORMALMAP");
        // this.material.EnableKeyword("_DETAIL_MULX2");

        // foreach(Vector3 v in this.meshFilter.mesh.vertices) {
        //     print(v);
        // }

        return null;
    }
}
