using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class PlaneteModif : MonoBehaviour
{
    [HideInInspector] public Vector3[] Vertices;
    [HideInInspector] public Vector3[] Normals;
    [HideInInspector] public int[] Triangles;
    
    private Mesh _mesh;
    private Dictionary<int, Vertice[]> _dictVertices;

    private void Start()
    {
        UpdateData();
    }
    private void Update()
    {
        //Debug.DrawRay(transform.position, transform.TransformDirection(Asteroide.transform.position), Color.red);
    }
    public void CrashAsteroid(int index, Vector3 impactStrength, float sizeAsteroid)
    {
        Debug.Log("Modification du mesh!!!");
        UpdateData();
        InitNeighbor(sizeAsteroid);

        _dictVertices[index * 3][0].MoveWithNeighbor(impactStrength, sizeAsteroid * 2);
        gameObject.GetComponent<MeshFilter>().mesh = _mesh;
        
        UpdateMesh();
    }

    private void UpdateData()
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
    private void InitNeighbor(float radius)
    {
        Debug.Log(radius / 2);
        _dictVertices = new Dictionary<int, Vertice[]>();
        
        List<Vertice> listVerticesObj = new List<Vertice>();
        for (int i = 0; i < Vertices.Length; i++)
        {
         listVerticesObj.Add(new Vertice(this, i));
        }

        List<int> verticeTemp;
        foreach (var verticeitem in listVerticesObj)
        {
            verticeTemp = new List<int>();
            
            for (int i = 0; i < Triangles.Length; i += 3)
            {
                if (Triangles[i] == verticeitem.PosIndex || Triangles[i + 1] == verticeitem.PosIndex || Triangles[i + 2] == verticeitem.PosIndex)
                {
                    verticeTemp.Add(Triangles[i]);
                    verticeTemp.Add(Triangles[i + 1]);
                    verticeTemp.Add(Triangles[i + 2]);

                    if (!_dictVertices.ContainsKey(i))
                    {
                        _dictVertices.Add(i, new Vertice[3]);
                        _dictVertices[i][0] = verticeitem;
                    }
                    else if (_dictVertices[i][1] == null)
                    {
                        _dictVertices[i][1] = verticeitem;
                    }
                    else
                    {
                        _dictVertices[i][2] = verticeitem;
                    }
                }
            }

            
            for (int i = 0; i < Vertices.Length; ++i)
            {
                verticeTemp.AddRange(GetTriangleByVertice(i));
            }

            float dist = 0;
            foreach (var item in verticeTemp)
            {
                dist = Vector3.Distance(Vertices[verticeitem.PosIndex], Vertices[listVerticesObj[item].PosIndex]);
                if (!verticeitem.Neighbor.ContainsKey(listVerticesObj[item]) && dist <= radius / 2)
                    verticeitem.Neighbor.Add(listVerticesObj[item], dist);
            }
        }
        

//        foreach (var item in _dictVertices.Keys)
//        {
//            Debug.Log("Triangle");
//            Debug.Log(Vertices[_dictVertices[item][0].PosIndex]);
//            Debug.Log(Vertices[_dictVertices[item][1].PosIndex]);
//            Debug.Log(Vertices[_dictVertices[item][2].PosIndex]);
//        }
    }


    private List<int> GetTriangleByVertice(int indexVertice)
    {
        List<int> vertices = new List<int>();

        for (int i = 0; i < Triangles.Length; i += 3)
        {
            if (Triangles[i] == indexVertice || Triangles[i + 1] == indexVertice || Triangles[i + 2] == indexVertice)
            {
                vertices.Add(Triangles[i]);
                vertices.Add(Triangles[i + 1]);
                vertices.Add(Triangles[i + 2]);
            }
        }

        return vertices;
    }
}
