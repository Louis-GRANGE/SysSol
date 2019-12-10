using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertice
{
    public int PosIndex;
    public Dictionary<Vertice, float> Neighbor;

    private PlaneteModif _planeteModif;
    
    public Vertice(PlaneteModif planeteModif, int posIndex)
    {
        _planeteModif = planeteModif;
        PosIndex = posIndex;
        Neighbor = new Dictionary<Vertice, float>();
    }

    public void MoveByVertice(Vertice reference, Vector3 impactStrength, float radius)
    {
        _planeteModif.Vertices[PosIndex] += (new Vector3(
                                               0,
                                               _planeteModif.Normals[PosIndex].y * impactStrength.y,
                                               0) * (1 + (Neighbor[reference] / radius)));
    }

    public void MoveWithNeighbor(Vector3 impactStrength, float radius)
    {
        _planeteModif.Vertices[PosIndex] += new Vector3(
            0,
            _planeteModif.Normals[PosIndex].y * impactStrength.y,
            0
        );

        foreach (var neighborVertice in Neighbor.Keys)
        {
            neighborVertice.MoveByVertice(this, impactStrength, radius);
        }
    }
}
