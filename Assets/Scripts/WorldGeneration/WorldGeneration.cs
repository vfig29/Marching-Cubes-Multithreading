using System.Collections;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public static GameObject CHUNK_PREFAB;
    public static WorldGeneration instance;
    public PlayerViewChunkManager playerViewManager;
    public static bool onSpawning = false;
    public static int worldSeed = GerarSeed();
    private void Awake()
    {
        CHUNK_PREFAB = Resources.Load<GameObject>("ChunkPREFAB");
        instance = this;
        playerViewManager = GetComponent<PlayerViewChunkManager>();
    }

    private void Start()
    {
        StartCoroutine(PlayerSpawn());
    }
    /*
    void TestCoordFuncs()
    {
        
        for (int i = -20; i <= 20; i++)
        {
            print("---------------------------");
            print(i);
            print(CoordinateSystem.GetChunkPosByCoord(new Vector3(i, 0, 0)));
            print(CoordinateSystem.GetLocalCoord(new Vector3(i, 0, 0)));
        }
        
    }
    */
    private void Update()
    {
        
    }
    public static int GerarSeed()
    {
        return System.Guid.NewGuid().GetHashCode();
    }

    IEnumerator PlayerSpawn()
    {
        
        //playerViewManager.playerTransform.parent = transform;
        Vector3 offset = new Vector3(0, 1.5f, 0);
        if (!CheckHighestGlobalSurface(new Vector3(0, 0, 0), out Vector3 playerSpawnCoord, out Vector3Int playerSpawnChunkPos))
        {
            Debug.LogError("Não há superficie para spawn.");
        }
        playerViewManager.playerTransform.position = playerSpawnCoord + offset;
        //bool canSpawn = false;
        /*
        while (!canSpawn)
        {
            if (playerViewManager.spawnedChunks.Exists(i => i.marchedChunk.Equals(playerSpawnChunkPos)))
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        */
        //playerViewManager.playerTransform.gameObject.GetComponent<Rigidbody>().useGravity = true;
        yield return null;
    }

    bool CheckHighestGlobalSurface(Vector3 globalCoordXZ, out Vector3 _surfaceCoordResult, out Vector3Int _surfaceChunkPosResult)
    {
        _surfaceCoordResult = Vector3.zero;
        _surfaceChunkPosResult = Vector3Int.zero;
        Vector3Int maxChunkPos = CoordinateSystem.GetChunkPosByCoord(new Vector3(globalCoordXZ.x, DensityGenerationContext.skyHeight, globalCoordXZ.z));
        for (int y = maxChunkPos.y; y >= -DensityGenerationContext.MAX_RELIEF_HEIGHT; y--)
        {
            Vector3Int chunkPosXZ = new Vector3Int(maxChunkPos.x, y, maxChunkPos.z);
            Chunk chunk = playerViewManager.DiscoverChunk(chunkPosXZ);
            chunk.GenerateDensityMap();
            if (chunk.densityGenerationContext.GetHighestSurfaceOnXZ(CoordinateSystem.GetLocalCoord(globalCoordXZ), out Vector3 result))
            {
                _surfaceCoordResult = chunk.densityGenerationContext.LocalToGlobalCoord(result);
                _surfaceChunkPosResult = chunkPosXZ;
                return true;
            }
        }
        return false;
    }

    public static FastNoise HeightNoise()
    {
        FastNoise noise = new FastNoise(worldSeed);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        noise.SetFrequency(0.002f);
        noise.SetFractalOctaves(6);
        noise.SetFractalLacunarity(3.2f);
        noise.SetFractalGain(0.3f);

        return noise;
    }

    public static FastNoise DensityNoise()
    {
        FastNoise noise = new FastNoise(worldSeed);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        noise.SetFrequency(0.002f);
        noise.SetFractalOctaves(6);
        noise.SetFractalLacunarity(3.2f);
        noise.SetFractalGain(0.3f);


        return noise;
    }
    public static FastNoise MountainNoise()
    {
        FastNoise noise = new FastNoise(worldSeed);
        noise.SetFractalType(FastNoise.FractalType.FBM);
        noise.SetFrequency(0.002f);
        noise.SetFractalOctaves(6);
        noise.SetFractalLacunarity(3.2f);
        noise.SetFractalGain(0.3f);
        
        
        return noise;
    }
}
