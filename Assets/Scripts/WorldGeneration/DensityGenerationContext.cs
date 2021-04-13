using System.Dynamic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public class DensityGenerationContext
{
    Vector3Int chunkPos;
    public const int skyHeight = 250;
    public const int MAX_RELIEF_HEIGHT = skyHeight * 80 / 100;
    public const int MIN_RELIEF_HEIGHT = skyHeight * 10 / 100;
    public const float CUBE_SCALE = 1;//Sempre observar a influencia nos resultados de "Marching Cubes", "noiseScale"'s e "Chunk_WIDTH")
    //
    static FastNoise heightNoise = WorldGeneration.HeightNoise();
    static FastNoise densityNoise = WorldGeneration.DensityNoise();
    public Vector3Int voxelCount;
    public float[,,] sampleNodeMap;
    public bool empty = true;

    public DensityGenerationContext(Vector3Int _chunkPos)
    {
        chunkPos = _chunkPos;
        SetDensity();
    }
    public float GetPointSample(Vector3Int worldPos)
    {
        return sampleNodeMap[worldPos.x, worldPos.y, worldPos.z];
    }

    public bool GetHighestSurfaceOnXZ(Vector3 X0Z, out Vector3 result)
    {
        result = Vector3.zero;
        for (int y = Chunk.CHUNK_HEIGHT - 1; y >= 0; y--)
        {
            X0Z.y = y;
            if(CheckIfIsNotDense(X0Z))
            {
                result = X0Z;
                return true;
            }
        }
        return false;
    }

    public Vector3 LocalToGlobalCoord(Vector3 localCoord)
    {
        return (new Vector3(Chunk.CHUNK_WIDTH * chunkPos.x, Chunk.CHUNK_HEIGHT * chunkPos.y, Chunk.CHUNK_WIDTH * chunkPos.z)) + localCoord;
    }
    public bool CheckIfIsNotDense(Vector3 localCoord)
    {
        Vector3Int convertedCoord = ConvertToSampleMapIndex(localCoord);
        return sampleNodeMap[convertedCoord.x, convertedCoord.y, convertedCoord.z] != 0;
    }

    public Vector3Int ConvertToSampleMapIndex(Vector3 localCoord)
    {
        return new Vector3Int(Mathf.FloorToInt(localCoord.x / CUBE_SCALE), Mathf.FloorToInt(localCoord.y / CUBE_SCALE), Mathf.FloorToInt(localCoord.z / CUBE_SCALE));
    }
    public void SetDensity()
    {
        Vector3 initialCoord = new Vector3((chunkPos.x) * (Chunk.CHUNK_WIDTH), (chunkPos.y) * (Chunk.CHUNK_HEIGHT), (chunkPos.z) * (Chunk.CHUNK_WIDTH));
        int floorWidth = Mathf.FloorToInt((Chunk.CHUNK_WIDTH / CUBE_SCALE));
        int floorHeight = Mathf.FloorToInt((Chunk.CHUNK_HEIGHT / CUBE_SCALE));
        voxelCount = new Vector3Int(floorWidth, floorHeight, floorWidth);
        sampleNodeMap = new float[voxelCount.x + 1, voxelCount.y + 1, voxelCount.z + 1]; // o tamanho da matriz de "nós"(corners) precisa ser 1 unidade maior que o tamanho do terreno.
        //0 define presença de massa.
        //1 define falta de massa.
        for (int x = 0; x < sampleNodeMap.GetLength(0); x++)
        {
            for (int z = 0; z < sampleNodeMap.GetLength(2); z++)
            {
                for (int y = 0; y < sampleNodeMap.GetLength(1); y++)
                {
                    float thisX = (x * CUBE_SCALE) + initialCoord.x;
                    float thisY = (y * CUBE_SCALE) + initialCoord.y;
                    float thisZ = (z * CUBE_SCALE) + initialCoord.z;
                    float heightRule = HeightRule(thisX, thisZ);
                    //float thisHeight = Mathf.Lerp(earthHeight + MIN_RELIEF_HEIGHT, earthHeight + MAX_RELIEF_HEIGHT, heightRule);//outra forma de calcular.
                    float thisHeight = heightRule * MAX_RELIEF_HEIGHT;
;                    if (thisY <= thisHeight)//
                    {
                        float densityRule = DensityRule(thisX, thisY, thisZ);
                        if (thisY > MIN_RELIEF_HEIGHT)//se é maior que o valor minimo de chão, aplica regra de densidade.(em observacao se é min_relief_height ou earth height)
                        {
                            sampleNodeMap[x, y, z] = IsoSurfaceRule(thisHeight, thisY); //*densityRule;
                        }
                        else//se é menor que o valor minimo de chão, não aplica regra de densidade e é sempre denso.
                        {
                            sampleNodeMap[x, y, z] = IsoSurfaceRule(thisHeight, thisY);//usar IsoSurfaceRule(thisHeight, thisY) para resultados mais suavizados. (para resultados mais planificados usar "1").
                            
                        }
                        empty = false;
                    }
                    else//se for maior que altura maxima, não tem terra.
                    {
                        sampleNodeMap[x, y, z] = 0;//o valor 0 define a falta de densidade.
                    }
                }
            }
        }

    }

    public float DensityRule(float x, float y , float z)
    {
        return densityNoise.GetPerlinFractal((float)x, (float)y, (float)z);
    }

    public float IsoSurfaceRule(float thisHeight, float y)
    {       
        return thisHeight - y;//o calculo de isosurface por meio da diferença entre o maior ponto relevo e o pontoy atual, da resultados mais "smooth" de superficies.
    }

    public float HeightRule(float x, float z)
    {
        return heightNoise.GetPerlinFractal((float)(x), (float)(z));
    }
}
