using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vertex
{
    public int PosIndex;
    public List<int> TrianglesIndex;
    public Dictionary<Vertex, float> NearbyVertices;

    private readonly PlanetDistord _planetDistord;
    
    public Vertex(PlanetDistord planetDistord, int posIndex)
    {
        this._planetDistord = planetDistord;
        PosIndex = posIndex;
        NearbyVertices = new Dictionary<Vertex, float>();
        TrianglesIndex = new List<int>();
    }

    private void Move(Vector3 impactStrength, float radius, float cumulativeDistance)
    {
        _planetDistord.Vertices[PosIndex] += (new Vector3(
                                                  _planetDistord.Normals[PosIndex].x,
                                                  _planetDistord.Normals[PosIndex].y,
                                                  _planetDistord.Normals[PosIndex].z
                                              ) * - Mathf.Sqrt(impactStrength.magnitude * cumulativeDistance) / radius);
    }

    private Dictionary<Vertex, float> _verticesToCompute;
    public void MoveWithNeighbor(Vector3 impactStrength, float radius)
    {
        _verticesToCompute = new Dictionary<Vertex, float>();

        Debug.Log("[" + GetType().Name + "] Début de la recharge, rayon: " + radius);

        _planetDistord.StartCoroutine(MoveCompute(impactStrength, radius));
    }


    private IEnumerator MoveCompute(Vector3 impactStrength, float radius)
    {
        // Ajout des vertices voisins aux vertices à traiter
        foreach (Vertex nearbyVertex in NearbyVertices.Keys)
            _verticesToCompute.Add(nearbyVertex, NearbyVertices[nearbyVertex]);

        int i, nVerticesToCompute;
        Vertex vertex;
        float cumulativeDistance;
        while (true)
        {
            nVerticesToCompute = _verticesToCompute.Count; ;
            for (i = 0; i < nVerticesToCompute; ++i)
            {
                vertex = _verticesToCompute.Keys.ElementAt(i);
                foreach (Vertex it in vertex.NearbyVertices.Keys)
                {
                    cumulativeDistance = _verticesToCompute[vertex] + vertex.NearbyVertices[it];
                    if (!_verticesToCompute.ContainsKey(it) && (cumulativeDistance < radius))
                        _verticesToCompute.Add(it, cumulativeDistance);
                }
                
                if (i % 1000 == 0)
                    yield return null;
            }
            
            if (_verticesToCompute.Count == nVerticesToCompute)
                break;
        }
        
        Debug.Log("[" + GetType().Name + "] Terminé: N de vertices à traiter: " + _verticesToCompute.Count);

        foreach (Vertex it in _verticesToCompute.Keys)
        {
            it.Move(impactStrength, radius, _verticesToCompute[it]);
        }
        
        _planetDistord.UpdateMesh();
    }
    
}
