using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public class ChunkGO : MonoBehaviour
{
    public Chunk marchedChunk;
    public static ChunkGO InstantiateChunk(Chunk createdChunk)
    {
        //SEM VARIAVEL CHUNKPREFAB:
        //GameObject chunkGO = new GameObject("Chunk: " + createdChunk.chunkPos.ToString());
        //chunkGO.AddComponent<MeshFilter>();
        //chunkGO.AddComponent<MeshRenderer>().material = Resources.Load<Material>("TestMaterial");
        //chunkGO.transform.position = Chunk.CHUNK_WIDTH * createdChunk.chunkPos;
        //ChunkMarching chunkMarchingScript = chunkGO.AddComponent<ChunkMarching>();
        //------------
        GameObject chunkGO = Instantiate(WorldGeneration.CHUNK_PREFAB, new Vector3(Chunk.CHUNK_WIDTH * createdChunk.chunkPos.x, Chunk.CHUNK_HEIGHT * createdChunk.chunkPos.y, Chunk.CHUNK_WIDTH * createdChunk.chunkPos.z), Quaternion.identity);
        chunkGO.name = "Chunk: " + createdChunk.chunkPos.ToString();
        ChunkGO chunkMarchingScript = chunkGO.GetComponent<ChunkGO>();
        chunkMarchingScript.marchedChunk = createdChunk;
        chunkGO.GetComponent<MeshFilter>().sharedMesh = createdChunk.meshData.BuildMesh();
        chunkMarchingScript.UpdateMeshCollider();
        return chunkMarchingScript;
    }

    protected void UpdateMeshCollider()
    {
        MeshCollider collider = gameObject.GetComponent<MeshCollider>();
        collider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }

    public static ChunkGO InstantiateChunk(Chunk createdChunk, Transform _parent)
    {
        ChunkGO chunkGO = InstantiateChunk(createdChunk);
        chunkGO.transform.parent = _parent;

        return chunkGO;
    }

}
