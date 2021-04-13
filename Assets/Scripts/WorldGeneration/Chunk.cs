using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk
{
    public MarchingCubesData meshData;
    public Vector3Int chunkPos;
    public const int CHUNK_WIDTH = 20;
    public const int CHUNK_HEIGHT = 20;
    public DensityGenerationContext densityGenerationContext;
    public bool spawned = false;
    public Chunk(Vector3Int _chunkPos)
    {
        chunkPos = _chunkPos;
    }


    public bool Equals(Chunk c)
    {
        if (c == null)//observar
        {
            Debug.Log("null");
            return false;
        }
        if (c.chunkPos.x == this.chunkPos.x && c.chunkPos.y == this.chunkPos.y && c.chunkPos.z == this.chunkPos.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool Equals(Vector3Int c)
    {
        if (c.x == this.chunkPos.x && c.y == this.chunkPos.y && c.z == this.chunkPos.z)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GenerateDensityMap()
    {
        densityGenerationContext = new DensityGenerationContext(chunkPos);
    }
    public MeshData GenerateMeshData()
    {
        if (densityGenerationContext == null || meshData != null || densityGenerationContext.empty)
        {
            return meshData;
        }
        
        meshData =  new MarchingCubesData(densityGenerationContext);
        //meshData.vertices = new Vector3[15 * (densityGenerationContext.densityCubes.x) * (densityGenerationContext.densityCubes.z)];//8 são os vertices por cubo.
        //meshData.triangles = new int[meshData.vertices.Length];//15 é o valor de maxTriangles*verticesPerTriangle
        meshData.MarchAllCubes();
        WorldGeneration.instance.playerViewManager.EnqueueChunkToRender(this);
        return meshData;
    }

    public ChunkGO InstantiateGO(Transform t)
    {
        return ChunkGO.InstantiateChunk(this, t);
    }
}
