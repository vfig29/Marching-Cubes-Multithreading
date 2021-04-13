using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using System;
using System.Collections;

public class PlayerViewChunkManager : MonoBehaviour
{
    public Transform playerTransform;
    int viewRangeXZ = 160;// total range é o dobro, pois engloba 2 sentidos a partir do player. //por algum motivo só funciona com o Chunk_width e Chunk_height iguais. Devo investigar.
    int viewRangeY = 40; // total é a view + chunksize
    public static bool onRoutine = false;
    Dictionary<Vector3Int, Chunk> discoveredChunks;
    Stack<Chunk> renderLine;
    public List<ChunkGO> spawnedChunks;
    const int maxChunkSpawnsPerFrame = 10;
    void Awake()
    {
        discoveredChunks = new Dictionary<Vector3Int, Chunk>();
        renderLine = new Stack<Chunk>();
        spawnedChunks = new List<ChunkGO>();
    }

    void Update()
    {
        if (!onRoutine)
        {
            
            StartCoroutine(CalculateProximityChunks());
        }
        RenderChunksOnLine();
        // O problema identificado consiste em dar pop enquanto da push.
    }

    void RenderChunksOnLine()
    {
        Chunk chunk = null;
        for (int i = 0; renderLine.Count > 0 && i < maxChunkSpawnsPerFrame; i++)
        {
            chunk = renderLine.Pop();
            SpawnChunk(chunk);
        }
        System.GC.Collect();
    }

    public void SpawnChunk(Chunk c)
    {
        if (c != null)
        {
            spawnedChunks.Add(c.InstantiateGO(this.transform));
            c.spawned = true;
        }
    }


    IEnumerator CalculateProximityChunks()
    {
        onRoutine = true;
        //Vector3Int playerPos = CoordinateSystem.GetChunkPosByCoord(playerTransform.position);
        Vector2 totalViewX = GetTotalViewX(); // onde x é o minimo e y é o maximo
        Vector2 totalViewY = GetTotalViewY(); // onde x é o minimo e y é o maximo
        Vector2 totalViewZ = GetTotalViewZ(); // onde x é o minimo e y é o maximo
        //print(totalViewX);
        //print(totalViewY);
        //print(totalViewZ);
        Vector2Int totalViewChunkPosX = CoordinateSystem.GetTotalViewChunkPosXZ(totalViewX); // converte os valores pra coordenada de chunk.
        Vector2Int totalViewChunkPosY = CoordinateSystem.GetTotalViewChunkPosY(totalViewY); // converte os valores pra coordenada de chunk.
        Vector2Int totalViewChunkPosZ = CoordinateSystem.GetTotalViewChunkPosXZ(totalViewZ); // converte os valores pra coordenada de chunk.
        //print(totalViewChunkPosX);
        //print(totalViewChunkPosY);
        //print(totalViewChunkPosZ);
        //int deltaChunksX = Mathf.Abs(totalViewChunkPosX.x - totalViewChunkPosX.y); //quantidade de chunks entre o menor chunk renderizado em X e o maior chunk renderizado em X;
        //int deltaChunksY = Mathf.Abs(totalViewChunkPosY.x - totalViewChunkPosY.y); // mesma coisa para y
        //int deltaChunksZ = Mathf.Abs(totalViewChunkPosZ.x - totalViewChunkPosZ.y); // mesma coisa para z;
        int count = 0;
        for (int x = totalViewChunkPosX.x; x < totalViewChunkPosX.y; x++)
        {
            for (int z = totalViewChunkPosZ.x; z < totalViewChunkPosZ.y; z++)
            {
                for (int y = totalViewChunkPosY.x; y < totalViewChunkPosY.y; y++, count++)
                {
                    DiscoverChunk(new Vector3Int(x, y, z));
                    if (count > 50)
                    {
                        count = 0;
                        yield return null;
                    }
                }
            }
        }
        onRoutine = false;
    }


    public Chunk DiscoverChunk(Vector3Int coord)
    {
        //Chunk chunk = discoveredChunks.Find(i => i.Equals(coord));
        discoveredChunks.TryGetValue(coord, out Chunk chunk);
        
        if (chunk != null)
        {
            if (chunk.spawned)
            {
                return chunk;
            }
            Task.Run(() => chunk.GenerateMeshData());
            return chunk;
        }
        else
        {
            chunk = new Chunk(coord);
            Task.Run(()=> chunk.GenerateDensityMap());
            discoveredChunks.Add(coord, chunk);

        }
        return chunk;
    }

    public void EnqueueChunkToRender(Chunk c)
    {
        renderLine.Push(c);
    }
    public int GetDeltaPlayerPos(Vector3Int chunkPos)
    {
        Vector3Int playerChunkPos = CoordinateSystem.GetChunkPosByCoord(playerTransform.position);
        return Mathf.Abs((chunkPos.x - playerChunkPos.x)) + Mathf.Abs((chunkPos.z - playerChunkPos.z)) + Mathf.Abs((chunkPos.y - playerChunkPos.y));
    }

    Vector2 GetTotalViewX()
    {
        Vector3 playerPos = playerTransform.position;
        float min = playerPos.x - viewRangeXZ;
        float max = playerPos.x + viewRangeXZ;

        return new Vector2(min, max); ;
    }

    Vector2 GetTotalViewZ()
    {
        Vector3 playerPos = playerTransform.position;
        float min = playerPos.z - viewRangeXZ;
        float max = playerPos.z + viewRangeXZ;

        return new Vector2(min, max); ;
    }

    Vector2 GetTotalViewY()
    {
        Vector3 playerPos = playerTransform.position;
        float min = playerPos.y - Chunk.CHUNK_HEIGHT;
        float max = playerPos.y + viewRangeY;

        return new Vector2(min, max); ;
    }

}
