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
    public ColourGenerator colourGenerator = new ColourGenerator();
    
    [SerializeField, HideInInspector]
    private MeshFilter[] terrainFilters;
    public TerrainFace[] terrainFaces;
    [SerializeField, HideInInspector]
    private MeshFilter[] atmosphereFilters;
    private TerrainFace[] atmosphereFaces;

    [SerializeField, HideInInspector]
    private GameObject terrainMesh;
    private GameObject atmosphereMesh;

    private GameObject terrainMeshes;
    private GameObject atmosphereMeshes;

    private void OnValidate()
    {
        GeneratePlanet();
    }
    
    public void Initialize()
    {
        Debug.Log("Init");
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
                meshTerrain.transform.position = terrainMeshes.transform.position;

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
                meshAtmosphere.transform.position = atmosphereMeshes.transform.position;

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
        if (!(terrainMesh && atmosphereMesh))
            Initialize();

        GenerateTerrain();
        GenerateAtmosphere();
        GenerateColours();
        FusionTerrainMeshes();
        FusionAtmosphereMeshes();
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

    public void FusionTerrainMeshes()
    {
        MeshFilter[] meshFilters = terrainFilters;

        if(terrainMesh == null)
        {
            terrainMesh = new GameObject("TerrainMesh");
            terrainMesh.AddComponent<MeshFilter>();
            terrainMesh.AddComponent<MeshRenderer>();
        }
        terrainMesh.transform.parent = transform;
        terrainMesh.GetComponent<MeshRenderer>().material = colourSettings.planetMaterial;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        terrainMesh.GetComponent<MeshFilter>().mesh = new Mesh();
        terrainMesh.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        terrainMesh.gameObject.SetActive(true);
    }

    public void FusionAtmosphereMeshes()
    {
        MeshFilter[] meshFilters = atmosphereFilters;

        if (atmosphereMesh == null)
        {
            atmosphereMesh = new GameObject("AtmosphereMesh");
            
            atmosphereMesh.AddComponent<MeshFilter>();
            atmosphereMesh.AddComponent<MeshRenderer>();
        }
        atmosphereMesh.transform.parent = transform;
        
        atmosphereMesh.GetComponent<MeshRenderer>().material = colourSettings.atmosphereMaterial;

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        atmosphereMesh.GetComponent<MeshFilter>().mesh = new Mesh();
        atmosphereMesh.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        atmosphereMesh.gameObject.SetActive(true);
    }
}