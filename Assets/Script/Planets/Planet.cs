using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)] 
    public int resolution = 10;
    public bool autoUpdate = true;

    public enum FaceRenderMask
    {
        All,
        Top,
        Bottom,
        Left,
        Right,
        Front,
        Back
    };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldOut;

    [HideInInspector] 
    public bool colourSettingsFoldOut;
    
    private ShapeGenerator shapeGenerator = new ShapeGenerator();
    private ColourGenerator colourGenerator = new ColourGenerator();
    
    [SerializeField, HideInInspector]
    private MeshFilter[] terrainFilters;
    private TerrainFace[] terrainFaces;
    [SerializeField, HideInInspector]
    private MeshFilter[] atmosphereFilters;
    private TerrainFace[] atmosphereFaces;

    [SerializeField, HideInInspector]
    private GameObject terrainMesh;

    private GameObject terrainMeshes;
    private GameObject atmosphereMeshes;

    private void OnValidate()
    {
        GeneratePlanet();
    }
    
    public void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);
        
        if (terrainMeshes == null)
            terrainMeshes = new GameObject("TerrainFaces");
        terrainMeshes.transform.parent = transform;
        terrainMeshes.transform.localPosition = Vector3.zero;
        
        if (atmosphereMeshes == null)
            atmosphereMeshes = new GameObject("AtmosphereFaces");
        atmosphereMeshes.transform.parent = transform;
        atmosphereMeshes.transform.localPosition = Vector3.zero;

        if (terrainFilters == null || terrainFilters.Length == 0)
        {
            terrainFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];
        
        if (atmosphereFilters == null || atmosphereFilters.Length == 0)
        {
            atmosphereFilters = new MeshFilter[6];
        }
        atmosphereFaces = new TerrainFace[6];
        
        Vector3[] directions = {Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back,};

        for (int i = 0; i < 6; i++)
        {
            if (terrainFilters[i] == null)
            {
                GameObject meshTerrain = new GameObject("Terrain");
                meshTerrain.transform.parent = terrainMeshes.transform;
                meshTerrain.transform.localPosition = Vector3.zero;

                meshTerrain.AddComponent<MeshRenderer>();
                terrainFilters[i] = meshTerrain.AddComponent<MeshFilter>();
                terrainFilters[i].sharedMesh = new Mesh();
            }
            terrainFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, terrainFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
            terrainFilters[i].gameObject.SetActive(renderFace);
            
            if (atmosphereFilters[i] == null)
            {
                GameObject meshAtmosphere = new GameObject("Atmosphere");
                meshAtmosphere.transform.parent = atmosphereMeshes.transform;

                meshAtmosphere.AddComponent<MeshRenderer>();
                atmosphereFilters[i] = meshAtmosphere.AddComponent<MeshFilter>();
                atmosphereFilters[i].sharedMesh = new Mesh();
            }
            atmosphereFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.atmosphereMaterial;

            atmosphereFaces[i] = new TerrainFace(shapeGenerator, atmosphereFilters[i].sharedMesh, resolution, directions[i]);
            atmosphereFilters[i].gameObject.SetActive(true);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateTerrain();
        GenerateAtmosphere();
        GenerateColours();
        //FusionTerrainMeshes();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateTerrain();
            GenerateAtmosphere();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    public void GenerateTerrain()
    {
        for (int i = 0; i < 6; i++)
        {
            if (terrainFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructTerrain();
            }
        }
        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax) ;
    }
    
    public void GenerateAtmosphere()
    {
        for (int i = 0; i < 6; i++)
        {
            if (atmosphereFilters[i].gameObject.activeSelf)
            {
                atmosphereFaces[i].ConstructAtmosphere();
            }
        } 
    }
    
    public void GenerateColours()
    {
        colourGenerator.UpdateColours();
        for (int i = 0; i < 6; i++)
        {
            if (terrainFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }
    }

    private void FusionTerrainMeshes()
    {
        Debug.Log("fusion ah!");
        if (terrainMesh == null)
            terrainMesh = new GameObject("TerrainMesh");
        terrainMesh.transform.parent = transform;
        terrainMesh.transform.localPosition = Vector3.zero;

        //terrainMesh.AddComponent<MeshFilter>();
        //terrainMesh.AddComponent<MeshRenderer>();

        /*List<Mesh> meshes = new List<Mesh>();
        
        foreach (var meshFilter in terrainFilters)
        {
            meshes.Add(meshFilter.sharedMesh);
            Debug.Log(meshes.Last());
        }
        
        terrainMesh.GetComponent<MeshFilter>().sharedMesh = CombineMeshes(meshes);*/

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[resolution * resolution * 6];
        Debug.Log(resolution * resolution * 6);
        int[] triangles = new int[((resolution - 1) * (resolution - 1) * 6) * 6];

        int triIndex = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                 vertices[j] = terrainFilters[i].mesh.vertices[i + j];
            }
        }

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < (resolution - 1) * (resolution - 1) * 6; j++)
            {
                triangles[i * resolution + j] = terrainFilters[i].mesh.triangles[j];
            }
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        terrainMesh.AddComponent<MeshFilter>().mesh = mesh;
        terrainMesh.AddComponent<MeshRenderer>();
        
        terrainMesh.GetComponent<MeshRenderer>().material = colourSettings.planetMaterial;
        Debug.Log("Fin");
    }
    
    private Mesh CombineMeshes(List<Mesh> meshes)
    {
        CombineInstance[] combine = new CombineInstance[meshes.Count];
        for (int i = 0; i < meshes.Count; i++)
        {
            combine[i].mesh = meshes[i];
            combine[i].transform = transform.localToWorldMatrix;
        }

        var mesh = new Mesh();
        mesh.CombineMeshes(combine);
        return mesh;
    }
}