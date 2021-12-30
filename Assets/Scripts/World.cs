using UnityEngine;
using UnityEngine.Tilemaps;

public class World : MonoBehaviour
{
    public GameObject[,] chunks;

    public GameObject chunkFab;

    public int viewDistanceInChunks;

    public static World instance;

    public Block[] blockTypes;

    public int noiseHeightModifier;

    public int seed;

    public int worldSizeInChunks;

    public int chunkWidth;
    public float scale, frequency, amplitude;
    public float rockScale;
    public float treeScale;
    public int rockOctaves;
    public int octaves;

    public int seaLevel;
    public int grassLevel;
    public int rockLevel;
    public int forestLevel;

    public float mainOffset;
    public float treeOffset;
    public float rockOffset;

    public float radialGradientScale;

    Transform player;

    Vector2Int lastChunk;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        chunks = new GameObject[worldSizeInChunks, worldSizeInChunks];

        player = GameObject.Find("Player").transform;

        if (seed == 0)
            seed = Random.Range(0, int.MaxValue);

        Random.InitState(seed);

        mainOffset = Random.value;
        rockOffset = Random.value;
        treeOffset = Random.value;

        transform.position = new Vector3(worldSizeInChunks * chunkWidth / 2, worldSizeInChunks * chunkWidth / 2, 0);

        lastChunk = GetChunkFromVector2(new Vector2(player.position.x, player.position.y));

        GenerateStartArea();
    }

    void Update()
    {
        if (lastChunk != GetChunkFromVector2(new Vector2(player.position.x, player.position.y)))
            UpdateChunks();
    }

    void UpdateChunks()
    {
        Vector2Int newChunkPos;

        Vector2Int pos = GetChunkFromVector2(player.position);

        for (int x = pos.x - viewDistanceInChunks; x <= pos.x + viewDistanceInChunks; x++)
        {
            for (int y = pos.y - viewDistanceInChunks; y <= pos.y + viewDistanceInChunks; y++)
            {
                newChunkPos = new Vector2Int(x, y);
                if (ChunkInWorld(newChunkPos))
                {
                    if (chunks[x, y] == null)
                    {
                        CreateChunk(newChunkPos);
                    }
                }
            }
        }
    }

    void GenerateStartArea()
    {
        Vector2Int newChunk;

        for (int x = worldSizeInChunks / 2 - viewDistanceInChunks; x <= worldSizeInChunks / 2 + viewDistanceInChunks; x++)
        {
            for (int y = worldSizeInChunks / 2 - viewDistanceInChunks; y <= worldSizeInChunks / 2 + viewDistanceInChunks; y++)
            {
                newChunk = new Vector2Int(x, y);

                CreateChunk(newChunk);
            }
        }
    }

    void CreateChunk(Vector2Int pos)
    {
        GameObject chunk = Instantiate(chunkFab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform);
        chunks[pos.x, pos.y] = chunk;
    }

    public Vector2Int GetChunkFromVector2(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x / chunkWidth);
        int y = Mathf.FloorToInt(pos.y / chunkWidth);

        return new Vector2Int(x, y);
    }

    public byte GetBlockFromVector2(Vector2 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);

        int xChunk = xCheck / chunkWidth;
        int yChunk = yCheck / chunkWidth;

        xCheck -= (xChunk * chunkWidth);
        yCheck -= (yChunk * chunkWidth);

        return chunks[xChunk, yChunk].GetComponent<Chunk>().blockMap[xCheck, yCheck];
    }

    public Vector2Int GetBlockCoordFromVector2(Vector2 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);

        int xChunk = xCheck / chunkWidth;
        int yChunk = yCheck / chunkWidth;

        xCheck -= (xChunk * chunkWidth);
        yCheck -= (yChunk * chunkWidth);

        return new Vector2Int(xCheck, yCheck);
    }

    bool ChunkInWorld(Vector2Int pos)
    {
        return !(pos.x < 0 || pos.x >= worldSizeInChunks * chunkWidth || pos.y < 0 || pos.y >= worldSizeInChunks * chunkWidth);
    }
}

[System.Serializable]
public class Block
{
    public string name;

    public TileBase tile;

    public byte value;
    public bool isSolid;

    public float strength;

    public byte blockBelowValue;

    public string walkAudio, hitAudio, breakAudio;
}
