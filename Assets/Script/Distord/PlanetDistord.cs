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

        foreach (var vertices in _verticesByTriangle[indexTriangle * 3])
        {
            vertices.MoveWithNeighbor(impactStrength, sizeAsteroid);
        }

        UpdateMesh();
        
        Debug.Log("[" + GetType().Name + "] Impact produit");
    }

    private void HydrateDataFromMesh()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        Vertices = GetComponent<MeshFilter>().mesh.vertices;
        Normals = GetComponent<MeshFilter>().mesh.normals;
        Triangles = GetComponent<MeshFilter>().mesh.triangles;
    }
    private void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = Vertices;
        _mesh.normals = Normals;
        _mesh.triangles = Triangles;
        _mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = _mesh;
    }
    
    private IEnumerator SetNearbyByVertex()
    {
        Vertex vertex;
        int i = 0;
        foreach (var verticeIndex in _verticesByIndex.Keys)
        {
            vertex = _verticesByIndex[verticeIndex];
            
            i++;
            
            if ((i % 10) == 9) 
                yield return null;
                
            // On définit les voisin proches du vertex
            foreach (var triangleIndex in vertex.TrianglesIndex)
            {
               
                foreach (var triangleVertex in _verticesByTriangle[triangleIndex])
                {
                    if (triangleVertex != vertex)
                    {
                        float dist = Vector3.Distance(Vertices[vertex.PosIndex], Vertices[triangleVertex.PosIndex]);

                        if (!vertex.NearbyVertices.ContainsKey(triangleVertex))
                            vertex.NearbyVertices.Add(triangleVertex, dist);
                    }
                }
            }

            // Vertex[] primaryVertices = vertex.NearbyVertices.Keys.ToArray();

            /*foreach (var primaryVertice in primaryVertices)
            {
                DefineNearby(vertex, primaryVertice);
            }*/
        }

        Debug.Log("[" + GetType().Name + "] Fin pré-traitement");

    }

    private void DefineNearby(Vertex vertexOrigin, Vertex vertex)
    {
        bool lastElementDetector = false;
        for (int i = 0; i < Triangles.Length; i += 3)
        {
            if (_verticesByTriangle[i].Contains(vertex))
            {
                foreach (var vertexTriangle in _verticesByTriangle[i])
                {
                    if (vertex != vertexTriangle)
                    {
                        if (vertexOrigin.NearbyVertices.ContainsKey(vertexTriangle))
                        {
                            lastElementDetector = true;
                            continue;
                        }
                        
                        float distance = Vector3.Distance(Vertices[vertex.PosIndex], Vertices[vertexTriangle.PosIndex]) + vertexOrigin.NearbyVertices[vertex];
                        vertexOrigin.NearbyVertices.Add(vertexTriangle, distance);
                        lastElementDetector = false;
                    }
                }
            }
        }
    }
    
}
