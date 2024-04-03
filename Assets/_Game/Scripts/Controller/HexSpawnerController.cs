using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSpawnerController : MonoBehaviour
{
    enum HexBorderIDs
    {
        topRight = 0,
        right = 1,
        botRight = 2,
        botLeft = 3,
        left = 4,
        topLeft = 5

    }

    public int xSize, zSize;
    [SerializeField] GridView hexPrefab;

    [SerializeField] List<HexInfos> lsHexInfos;

    public GridView[,] hexs;

    private void Start()
    {
        SpawnHexes();
        //SetPlayers();
        //SetHexInfos();
    }

    [SerializeField] float xOffset, zOffset;

    void SpawnHexes()
    {
        if (xSize % 2 == 0)
            xSize++;

        //LevelHandler.I.GetLevel().xSize = xSize;
        //LevelHandler.I.GetLevel().zSize = zSize;

        xSize += 2;
        zSize += 2;

        float xStart = (-xOffset / 2f) * (xSize - 1) / 2f;
        float zStart = 0f;

        hexs = new GridView[xSize - 2, zSize - 2];

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                GridView h = Instantiate(hexPrefab);
                //hexs[x, z] 
                //hexs[x, z].gameObject.name = x + "," + z;
                float xPos = xStart + (x * xOffset) + (z % 2 == 0 ? 0f : xOffset / 2f);
                float zPos = zStart + (z * zOffset);

                h.transform.position = new Vector3(xPos, -2.5f, zPos);

                if (x == 0 || x == xSize - 1 || z == 0 || z == zSize - 1)
                {
                    h.SetOutsideHex();
                }
                else
                {
                    hexs[x - 1, z - 1] = h;

                    hexs[x - 1, z - 1].x = x - 1;
                    hexs[x - 1, z - 1].z = z - 1;

                    if (x == 1)
                    {
                        hexs[x - 1, z - 1].EnableBorder(4);

                        if (z % 2 == 0)
                        {
                            hexs[x - 1, z - 1].EnableBorder(3);
                            hexs[x - 1, z - 1].EnableBorder(5);
                        }
                    }
                    else if (x == (xSize - 2))
                    {
                        hexs[x - 1, z - 1].EnableBorder(1);

                        if (z % 2 == 1)
                        {
                            hexs[x - 1, z - 1].EnableBorder(0);
                            hexs[x - 1, z - 1].EnableBorder(2);
                        }
                    }
                    if (z == 1)
                    {
                        hexs[x - 1, z - 1].EnableBorder(2);
                        hexs[x - 1, z - 1].EnableBorder(3);
                    }
                    else if (z == (zSize - 2))
                    {
                        hexs[x - 1, z - 1].EnableBorder(0);
                        hexs[x - 1, z - 1].EnableBorder(5);
                    }
                }
            }
        }

        xSize -= 2;
        zSize -= 2;
    }

    //void SetPlayers()
    //{
    //    hexs[0, 0].Init(true, InitialMobType.Player);
    //}

    //void SetHexInfos()
    //{
    //    foreach (var hexInfo in lsHexInfos)
    //    {
    //        if (hexInfo.x >= xSize && hexInfo.z >= zSize) continue;

    //        if (hexInfo.mobType != InitialMobType.None)
    //        {
    //            hexs[hexInfo.x, hexInfo.z].Init(true, hexInfo.mobType, null);
    //        }
    //        else if (hexInfo.hexType != HexTypes.None)
    //        {
    //            hexs[hexInfo.x, hexInfo.z].value = hexInfo.hexVal;
    //            hexs[hexInfo.x, hexInfo.z].mType = hexInfo.hexType;
    //            hexs[hexInfo.x, hexInfo.z].Init(false);
    //        }
    //    }
    //}

    public GridView GetHexAt(int x, int z)
    {
        if (x < 0 || x >= xSize) return null;
        if (z < 0 || z >= zSize) return null;

        return hexs[x, z];
    }

    [System.Serializable]
    public class HexInfos
    {
        public int x, z;

        public HexTypes hexType;
        public int hexVal;

        public InitialMobType mobType;
        
    }
}
