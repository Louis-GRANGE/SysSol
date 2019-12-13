using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class PlanetDistord : MonoBehaviour
{
    [HideInInspector] public Vector3[] Vertices;
    [HideInInspector] public Vector3[] Normals;
    [HideInInspector] public int[] Triangles;
    
    private Mesh _mesh;
    private Dictionary<int, Vertex> _verticesByIndex;
    private Dictionary<int, Vertex[]> _verticesByTriangle;

    private void Awake()
    {
        _verticesByIndex = new Dictionary<int, Vertex>();
        _verticesByTriangle = new Dictionary<int, Vertex[]>();
        
        HydrateDataFromMesh();
        
        for (int i = 0; i < Vertices.Length; i++)
        {
            _verticesByIndex.Add(i, new Vertex(this, i));
        }
        
        DefineAllTrianglesVertices();

        StartCoroutine(SetNearbyByVertex());
    }

    private void DefineAllTrianglesVertices()
    {
        for (int i = 0; i < Triangles.Length; i += 3)
        {
            _verticesByTriangle.Add(i, new[]
            {
                _verticesByIndex[Triangles[i]],
                _verticesByIndex[Triangles[i + 1]],
                _verticesByIndex[Triangles[i + 2]],
            });

            _verticesByIndex[Triangles[i]].TrianglesIndex.Add(i);
            _verticesByIndex[Triangles[i + 1]].TrianglesIndex.Add(i);
            _verticesByIndex[Triangles[i + 2]].TrianglesIndex.Add(i);
        }
    }
    
    public void CrashAsteroid(int indexTriangle, Vector3 impactStrength, float sizeAsteroid)
    {
        Debug.Log("[" + GetType().Name + "] Production de l'impact");
        
        // HydrateDataFromMesh();

        foreach (Vertex vertex in _verticesByTriangle[indexTriangle * 3])
        {
            vertex.MoveWithNeighbor(impactStrength, sizeAsteroid);
        }

        // UpdateMesh();
        
        Debug.Log("[" + GetType().Name + "] Impact produit");
    }

    private void HydrateDataFromMesh()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        Vertices = GetComponent<MeshFilter>().mesh.vertices;
        Normals = GetComponent<MeshFilter>().mesh.normals;
        Triangles = GetComponent<MeshFilter>().mesh.triangles;
    }
    public void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = Vertices;
        _mesh.normals = Normals;
        _mesh.triangles = Triangles;
        _mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = _mesh;
        
        // GetComponentInParent<Planet>().colourGenerator.UpdateColours();
    }
    
    private IEnumerator SetNearbyByVertex()
    {
        Vertex vertex;

        int i = 0;
        foreach (int verticeIndex in _verticesByIndex.Keys)
        {
            vertex = _verticesByIndex[verticeIndex];
            
            // On définit les voisin proches du vertex
            foreach (int triangleIndex in vertex.TrianglesIndex)
            {
                foreach (Vertex triangleVertex in _verticesByTriangle[triangleIndex])
                {
                    if (triangleVertex != vertex)
                    {
                        float dist = Vector3.Distance(Vertices[vertex.PosIndex], Vertices[triangleVertex.PosIndex]);

                        if (!vertex.NearbyVertices.ContainsKey(triangleVertex))
                            vertex.NearbyVertices.Add(triangleVertex, dist);
                    }
                }
            }

            ++i;

            if (i % 300 == 0)
                yield return null;
        }

        Debug.Log("[" + GetType().Name + "] Fin pré-traitement");
    }

}
