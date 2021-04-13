using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    //int trianglesIndex;
    //int verticesIndex;

    protected void ClearMeshData() 
    {
        vertices.Clear();
        triangles.Clear();
        //vertices = null;
        //verticesIndex = 0;
        //triangles = null;
        //trianglesIndex = 0;
    }

    public Mesh BuildMesh()
    {
        Mesh createdMesh = new Mesh();
        createdMesh.vertices = vertices.ToArray();
        createdMesh.triangles = triangles.ToArray();
        createdMesh.RecalculateNormals();
        return createdMesh;
    }
    public void CreateTriangle(int a, int b, int c)
    {
        AddSingleTriangleData(a);
        AddSingleTriangleData(b);
        AddSingleTriangleData(c);
    }

    public int AddVertice(Vector3 addedVertice)
    {
        vertices.Add(addedVertice);
        return vertices.Count - 1;
        //vertices[verticesIndex] = addedVertice;
        //return verticesIndex++;
    }

    public void AddSingleTriangleData(int a)
    {
        triangles.Add(a);
        //triangles[trianglesIndex] = a;
        //trianglesIndex++;
    }

    protected bool GetTriangleIndexByVertice(Vector3 vertice, out int triangleIndex)
    {
        
        for (int i = 0; i < vertices.Count; i++)
        {
            if (vertices[i] == vertice)
            {
                triangleIndex = i;
                return true;
            }
        }
        triangleIndex = -1;
        return false;
    }
    public void AddVerticesAndTriangulate(Vector3 a, Vector3 b, Vector3 c)
    {
        CreateTriangle(AddVertice(a), AddVertice(b), AddVertice(c));
    }
}
