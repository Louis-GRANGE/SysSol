using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public int PosIndex;
    public List<int> TrianglesIndex;
    public Dictionary<Vertex, float> NearbyVertices;

    private readonly PlanetDistord planetDistord;
    
    public Vertex(PlanetDistord planetDistord, int posIndex)
    {
        this.planetDistord = planetDistord;
        PosIndex = posIndex;
        NearbyVertices = new Dictionary<Vertex, float>();
        TrianglesIndex = new List<int>();
    }

    public void MoveByVertice(Vertex reference, Vector3 impactStrength, float radius)
    {
        planetDistord.Vertices[PosIndex] += (new Vector3(
                                                 0,
                                                 planetDistord.Normals[PosIndex].y * impactStrength.y,
                                                 0) * (1 + (NearbyVertices[reference] / radius)));
    }

    public void MoveWithNeighbor(Vector3 impactStrength, float radius)
    {
        planetDistord.Vertices[PosIndex] += new Vector3(
            0,
            planetDistord.Normals[PosIndex].y * impactStrength.y,
            0
        );

        foreach (var neighborVertice in NearbyVertices.Keys)
        {
            neighborVertice.MoveByVertice(this, impactStrength, radius);
            
        }
    }
}
