using System.Collections;
using UnityEngine;

public static class CoordinateSystem
{
    static public int StandardChunkRound(float a)
    {
        return Mathf.FloorToInt(a);
    }

    public static Vector3Int GetChunkPosByCoord(Vector3 coord)
    {
        int chunkWidth = Chunk.CHUNK_WIDTH;
        int chunkHeight = Chunk.CHUNK_HEIGHT;
        int xAxis = StandardChunkRound((coord.x / chunkWidth));
        int yAxis = StandardChunkRound((coord.y / chunkHeight));
        int zAxis = StandardChunkRound((coord.z / chunkWidth));
        Vector3Int chunkPos = new Vector3Int(xAxis, yAxis, zAxis);


        return chunkPos;
    }

    public static Vector2Int GetTotalViewChunkPosXZ(Vector2 coord)
    {
        int chunkWidth = Chunk.CHUNK_WIDTH;
        
        int minValue = StandardChunkRound((coord.x / chunkWidth));
        int maxValue = StandardChunkRound((coord.y / chunkWidth));
        Vector2Int chunkPos = new Vector2Int(minValue, maxValue);
        return chunkPos;
    }

    public static Vector2Int GetTotalViewChunkPosY(Vector2 coord)
    {
        int chunkHeight = Chunk.CHUNK_HEIGHT;

        int minValue = StandardChunkRound((coord.x / chunkHeight));
        int maxValue = StandardChunkRound((coord.y / chunkHeight));
        Vector2Int chunkPos = new Vector2Int(minValue, maxValue);
        return chunkPos;
    }

    public static Vector3 GetLocalCoord(Vector3 coord)
    {
        Vector3 c = GetChunkPosByCoord(coord);
        c.x = c.x * Chunk.CHUNK_WIDTH;
        c.y = c.y * Chunk.CHUNK_HEIGHT;
        c.z = c.z * Chunk.CHUNK_WIDTH;

        return coord - (c);
    }
}
