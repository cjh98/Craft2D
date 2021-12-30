using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Chunk : MonoBehaviour
{
    [System.NonSerialized]
    public byte[,] blockMap;

    public Tilemap tilemap;

    void Start()
    {
        transform.position = new Vector3(transform.position.x * World.instance.chunkWidth, transform.position.y * World.instance.chunkWidth, 0);

        transform.name = "Chunk at (" + transform.position.x / World.instance.chunkWidth + ", " + transform.position.y / World.instance.chunkWidth + ")";

        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        StartCoroutine(GenerateChunk());
        yield return 0;
    }

    IEnumerator GenerateChunk()
    {
        Vector3 center = new Vector3(World.instance.worldSizeInChunks * World.instance.chunkWidth / 2, World.instance.worldSizeInChunks * World.instance.chunkWidth / 2, 0);

        blockMap = new byte[World.instance.chunkWidth, World.instance.chunkWidth];

        float heightValue, rockValue, treeValue;
        int forestBuffer = 5;

        float dist;
        float valToSubtract;

        // generate water, sand, and grass
        for (int x = 0; x < World.instance.chunkWidth; x++)
        {
            for (int y = 0; y < World.instance.chunkWidth; y++)
            {
                heightValue = World.instance.noiseHeightModifier * NoiseGen.GenerateNoise(x + gameObject.transform.position.x, y + gameObject.transform.position.y,
                    World.instance.scale, World.instance.mainOffset, World.instance.octaves, World.instance.frequency, World.instance.amplitude);

                heightValue = 100;

                // this creates a radial gradient, land gets lower when it gets farther from the center.
                dist = Vector3.SqrMagnitude(center - new Vector3(transform.position.x, transform.position.y, 0));
                valToSubtract = dist * World.instance.radialGradientScale;

                heightValue -= valToSubtract;

                if (heightValue <= World.instance.seaLevel)
                {
                    blockMap[x, y] = 0;
                }
                else if (heightValue > World.instance.seaLevel && heightValue < World.instance.grassLevel)
                {
                    blockMap[x, y] = 1;
                }
                else
                {
                    // only generate rocks and trees above grass
                    // noise functions are called again because trees and rocks use different offsets and scales than the landd

                    treeValue = World.instance.noiseHeightModifier * NoiseGen.GenerateNoise(x + gameObject.transform.position.x,
                        y + gameObject.transform.position.y, World.instance.treeScale, World.instance.treeOffset, World.instance.rockOctaves, World.instance.frequency, World.instance.amplitude);

                    rockValue = World.instance.noiseHeightModifier * NoiseGen.GenerateNoise(x + gameObject.transform.position.x,
                        y + gameObject.transform.position.y, World.instance.rockScale, World.instance.rockOffset, World.instance.rockOctaves, World.instance.frequency, World.instance.amplitude);

                    treeValue -= valToSubtract;
                    rockValue -= valToSubtract;

                    if (treeValue >= World.instance.forestLevel && heightValue > World.instance.grassLevel + forestBuffer && rockValue < World.instance.rockLevel)
                    {
                        blockMap[x, y] = 3;
                    }
                    else if (rockValue >= World.instance.rockLevel && heightValue > World.instance.grassLevel + forestBuffer)
                    {
                        blockMap[x, y] = 4;
                    }
                    else
                    {
                        blockMap[x, y] = 2;
                    }
                }
            }
        }
        StartCoroutine(CreateTileMap());

        yield return 0;
    }

    IEnumerator CreateTileMap()
    {
        byte block;
        Vector3Int pos = new Vector3Int();

        for (int x = 0; x < blockMap.GetLength(0); x++)
        {
            for (int y = 0; y < blockMap.GetLength(1); y++)
            {
                block = blockMap[x, y];
                pos.x = x;
                pos.y = y;
                pos.z = 0;

                switch (block)
                {
                    case 0:
                        tilemap.SetTile(pos, World.instance.blockTypes[0].tile);
                        break;
                    case 1:
                        tilemap.SetTile(pos, World.instance.blockTypes[1].tile);
                        break;
                    case 2:
                        tilemap.SetTile(pos, World.instance.blockTypes[2].tile);
                        break;
                    case 3:
                        tilemap.SetTile(pos, World.instance.blockTypes[3].tile);
                        break;
                    case 4:
                        tilemap.SetTile(pos, World.instance.blockTypes[4].tile);
                        break;
                    default:
                        Debug.LogError("ERROR: Invalid block value at " + x + ", " + y);
                        break;
                }
            }
        }

        tilemap.RefreshAllTiles();
        yield return 0;
    }

    public void SetTile(Vector3Int pos, TileBase newTile)
    {
        tilemap.SetTile(pos, newTile);
    }
}