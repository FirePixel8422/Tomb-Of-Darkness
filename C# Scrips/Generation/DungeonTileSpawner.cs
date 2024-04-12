using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


public class DungeonTileSpawner : MonoBehaviour
{
    public static DungeonTileSpawner Instance;
    private void Awake()
    {
        Instance = this;
    }

    [Range(0,100)]
    public float spikeChance;

    public GameObject hallway;
    public GameObject corner;
    public GameObject tSplit;
    public GameObject xSplit;
    public GameObject stair;
    public GameObject spikes;
    public GameObject entranceFiller;


    public IEnumerator SpawnTiles(List<List<Node>> tiles)
    {
        GameObject tileHolder = new GameObject("_SPAWNED_ TileHolder");

        for (int i = 0; i < tiles.Count; i++)
        {
            for (int i2 = 0; i2 < tiles[i].Count; i2++)
            {
                yield return null;

                Node node = tiles[i][i2];
                if (node.placedInWorld)
                {
                    continue;
                }
                node.placedInWorld = true;




                Vector3 worldPos = node.worldPos - Vector3.up * DungeonGrid.Instance.tileSize / 2;
                Quaternion rot = Quaternion.identity;
                GameObject spawnedObj = null;

                List<int2> dir = new List<int2>(node.worldTileEntrances.Count);
                for (int i3 = 0; i3 < node.worldTileEntrances.Count; i3++)
                {
                    dir.Add(node.worldTileEntrances[i3]);
                }

                if (node.partOfStair == 1)
                {
                    int2[] stairDirections = new int2[]
                    {
                        new int2(3, 0),
                        new int2(-3, 0),
                        new int2(0, 3),
                        new int2(0, -3),
                    };
                    int[] stairRotation = new int[]
                    {
                        -90,
                        90,
                        180,
                        0,
                    };
                    for (int i3 = 0; i3 < stairDirections.Length; i3++)
                    {
                        if (dir.Contains(stairDirections[i3]))
                        {
                            int2 stairDir = INT_Logic.Clamp(stairDirections[i3], -1, 1);
                            rot = Quaternion.Euler(0, stairRotation[i3], 0);

                            spawnedObj = Instantiate(stair, worldPos + new Vector3(stairDir.x, 0, stairDir.y) * DungeonGrid.Instance.tileSize, rot);

                            if (node.stairDir.y == -1)
                            {
                                spawnedObj.transform.rotation *= Quaternion.Euler(0, 180, 0);
                                spawnedObj.transform.position += spawnedObj.transform.forward * DungeonGrid.Instance.tileSize;
                                spawnedObj.transform.transform.position += Vector3.up * DungeonGrid.Instance.tileSize;
                            }
                            break;
                        }
                    }
                }


                int amount = node.worldTileEntrances.Count;
                dir.Clear();
                for (int i3 = 0; i3 < amount; i3++)
                {
                    dir.Add(INT_Logic.Clamp(node.worldTileEntrances[i3], -1, 1));
                }




                if (dir.Count == 2)
                {
                    if ((dir[0] + dir[1]).Equals(int2.zero))
                    {
                        //hallway

                        if (dir[0].x == 0)
                        {
                            spawnedObj = Instantiate(hallway, worldPos, Quaternion.identity);
                        }
                        else
                        {
                            rot = Quaternion.Euler(0, 90, 0);
                            spawnedObj = Instantiate(hallway, worldPos, rot);
                        }
                    }
                    else
                    {
                        //corner

                        int2[] cornerVariations = new int2[]
                        {
                            new int2(1, 1),
                            new int2(-1, 1),
                            new int2(1, -1),
                            new int2(-1, -1),
                        };
                        int[] cornerRotation = new int[]
                        {
                            90,
                            0,
                            180,
                            -90,
                        };

                        int2 totalDir = dir[0] + dir[1];
                        for (int i3 = 0; i3 < cornerVariations.Length; i3++)
                        {
                            if (totalDir.Equals(cornerVariations[i3]))
                            {
                                rot = Quaternion.Euler(0, cornerRotation[i3], 0);
                                break;
                            }
                        }

                        spawnedObj = Instantiate(corner, worldPos, rot);
                    }
                }

                else if (dir.Count == 3)
                {
                    //T-Split
                    List<int2> allPosibleDir = new List<int2>(4)
                    {
                        new int2 (-1, 0),
                        new int2 (1, 0),
                        new int2 (0, 1),
                        new int2 (0, -1),
                    };

                    int[] tSplitRotation = new int[]
                    {
                        90,
                        -90,
                        180,
                        0,
                    };

                    for (int i3 = 0; i3 < allPosibleDir.Count; i3++)
                    {
                        if (dir.Contains(allPosibleDir[i3]) == false)
                        {
                            rot = Quaternion.Euler(0, tSplitRotation[i3], 0);
                            spawnedObj = Instantiate(tSplit, worldPos, rot);
                            break;
                        }
                    }
                }

                else if (dir.Count == 4)
                {
                    spawnedObj = Instantiate(xSplit, worldPos, Quaternion.identity);
                }

                spawnedObj.transform.SetParent(tileHolder.transform, true);

                #region spikeTrap spawn logic

                int3 downNodeGridPos = node.gridPos + new int3(0, -1, 0);
                if (UnityEngine.Random.Range(0, 100f) > (100 - spikeChance) &&
                    DungeonGrid.Instance.IsInsideGrid(downNodeGridPos) && DungeonGrid.Instance.GetNodeFromGridPos(downNodeGridPos).isOpen == false
                    && DungeonGrid.Instance.GetNodeFromGridPos(downNodeGridPos).walkable == true)
                {
                    Destroy(spawnedObj.transform.GetChild(0).gameObject);
                    GameObject spikeTrap = Instantiate(spikes, worldPos, Quaternion.identity);
                    spikeTrap.transform.SetParent(tileHolder.transform, true);
                }
                else
                {
                    Quaternion floorRot = Quaternion.Euler(0, Mathf.RoundToInt(-rot.y / 90) * 90, 0);
                    spawnedObj.transform.GetChild(0).rotation = floorRot;
                }
                #endregion
            }
        }

        foreach(Building b in FindObjectsOfType<Building>())
        {
            for (int i = 0; i < b.entrancesComplete.Length; i++)
            {
                if (b.entrancesComplete[i] == false)
                {
                    float rotY = Mathf.Atan2(b.entranceDirs[i].x, b.entranceDirs[i].y) * -Mathf.Rad2Deg;
                    float tileSize = DungeonGrid.Instance.tileSize;

                    GameObject spawnedObj = Instantiate(entranceFiller, 
                        DungeonGrid.Instance.NodeFromWorldPoint(b.entrances[i].position).worldPos
                        - Vector3.up * tileSize / 2
                        - new Vector3(b.entranceDirs[i].x, 0, b.entranceDirs[i].y) * tileSize
                        , Quaternion.Euler(0, rotY, 0));;

                    spawnedObj.transform.SetParent(tileHolder.transform, true);
                }
            }
        }
    }
}