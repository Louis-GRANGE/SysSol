using System;
using System.Collections;
using System.Collections.Generic;
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
    private MeshFilter[] atmosphereFilters;
    private TerrainFace[] atmosphereFaces;

    private void OnValidate()
    {
        GeneratePlanet();
    }

    public void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);
        
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
                meshTerrain.transform.parent = transform;
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
                meshAtmosphere.transform.parent = transform;

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
        colourGenerator.UpdateElevation(shapeGenerator.elevationTerrainMinMax) ;
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

    public void FusionTerrainMeshes()
    {
        MeshFilter[] meshFilters = terrainFilters;
        GameObject TerrainMesh = new GameObject("TerrainMesh");
        TerrainMesh.transform.parent = transform;
        TerrainMesh.AddComponent<MeshFilter>();
        TerrainMesh.AddComponent<MeshRenderer>();
        TerrainMesh.GetComponent<MeshRenderer>().material = colourSettings.planetMaterial;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
        }
        TerrainMesh.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        TerrainMesh.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
    }
}