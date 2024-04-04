using System;

[Serializable]
public class GridStartLayout
{
    [System.Serializable]
    public struct BlockData
    {
        public BaseBlockEntityTypeDefinition[] Blocks;
    }

    public BlockData BlockDatas;

    public int Width = 4;
    public int Height = 4;

    public GridStartLayout(int width, int height)
    {
        Width = width;
        Height = height;
        BlockDatas = new BlockData { Blocks = new BaseBlockEntityTypeDefinition[Width * Height] };
    }
}

