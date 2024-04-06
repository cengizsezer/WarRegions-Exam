using MyProject.Core.Enums;
using MyProject.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts.BoardConsts;

[System.Serializable]
public class ColorRegion
{
    public BlockType ColorType;
    public List<GridView> RegionsGrids;

    public ColorRegion()
    {
        RegionsGrids = new();
    }
}

namespace MyProject.GamePlay.Controllers
{
    public class BoardCoordinateSystem
    {
        public List<GridView> LsAllGridViews = new();
        public List<GridView> LsMountainViews = new();
        public List<MillitaryBaseView> LsMilitaryBaseView = new();
        public ColorRegion ColorRegions;

        private int xSize, zSize;
        private float xOffset, zOffset;
        public GridView[,] hexs;

        private GridStartLayout startGroundLayout;
        private GridStartLayout startMountainLayout;
        private GridStartLayout startMilitaryBaseLayout;

        #region Injection
        private readonly GridView.Factory _gridViewfactory;
        private readonly MillitaryBaseView.Factory _millitaryBaseViewFactory;
        private readonly BoardView _boardView;
        private readonly GridViewSettings _gridViewSettings;
        private readonly LevelSettings _levelSettings;
        private readonly ApplicationPrefabSettings _applicationPrefabSettings;
       

        [Inject]
        public BoardCoordinateSystem
        (
            BoardView boardView
            , GridView.Factory gridViewfactory
            , GridViewSettings gridViewSettings
            , ApplicationPrefabSettings applicationPrefabSettings
            ,LevelSettings levelSettings
            , MillitaryBaseView.Factory millitaryBaseViewFactory
        )
        {
            _boardView = boardView;
            _gridViewfactory = gridViewfactory;
            _gridViewSettings = gridViewSettings;
            _applicationPrefabSettings = applicationPrefabSettings;
            _levelSettings = levelSettings;
            _millitaryBaseViewFactory = millitaryBaseViewFactory;
        }

        #endregion

        public void Init()
        {
            SpawnHexes(0);
        }
        IBlockEntityTypeDefinition entity;
        public void SpawnHexes(int levelIndex)
        {
           
            startGroundLayout = _levelSettings.LevelData[0].LevelGroundTypeSettings.gridStartLayout;
            startMilitaryBaseLayout = _levelSettings.LevelData[0].MilitaryBaseTypeSettings.gridStartLayout;
            startMountainLayout = _levelSettings.LevelData[0].LevelMountainSettings.gridStartLayout;

            int xSize = _levelSettings.LevelData[0].LevelGroundTypeSettings.gridStartLayout.Width;
            int zSize = _levelSettings.LevelData[0].LevelGroundTypeSettings.gridStartLayout.Height;

            float xOffset = _levelSettings.LevelData[0].LevelGroundTypeSettings.xOffset;
            float zOffset = _levelSettings.LevelData[0].LevelGroundTypeSettings.zOffset;

            if (xSize % 2 == 0)
                xSize++;

            float xStart = (-xOffset / 2f) * (xSize - 1) / 2f;
            float zStart = 0f;

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    float xPos = xStart + (x * xOffset) + (z % 2 == 0 ? 0f : xOffset / 2f);
                    float zPos = zStart + (z * zOffset);

                    // Create hexagon grid
                    GridView grid = _gridViewfactory.Create(
                        new GridView.Args(_boardView.transform,
                        Vector3.one,
                        zSize,
                        xSize,
                        xPos,
                        0f,
                        zPos));

                    grid.name = $"grid--{x}-{z}";

                    LsAllGridViews.Add(grid);
                }
            }
            SetLevelGroundDesign(xSize,zSize);
            SetMountains();
            CalculateNeigbor();



        }

        private void CalculateNeigbor()
        {
            foreach (var view in LsAllGridViews)
            {
                view.FindNeigbor();
            }
        }
        
        

        private List<ColorRegion> lsColorRegions = new();
        void AddColorPair(GridView grid)
        {
            BlockType color = grid.mType;
          

            // Renk eşleşmesi kontrol edilecek ColorRegion nesnesi
            ColorRegion region = null;

            // lsColorRegions listesi boşsa veya renk eşleşmesi yoksa yeni bir ColorRegion oluştur ve listeye ekle
            if (lsColorRegions.Count == 0)
            {
                region = new ColorRegion();
                region.ColorType = color;
                lsColorRegions.Add(region);
            }
            else
            {
                // Renk eşleşmesi kontrol edilecek ColorRegion nesnesini bul
                region = lsColorRegions.Find(r => r.ColorType == color);

                // Eğer renk eşleşmesi yoksa yeni bir ColorRegion oluştur ve listeye ekle
                if (region == null)
                {
                    region = new ColorRegion();
                    region.ColorType = color;
                    lsColorRegions.Add(region);
                }
            }

            // İlgili ColorRegion'a gridi ekle
            region.RegionsGrids.Add(grid);
        }


        void SetLevelGroundDesign(int xSize, int zSize)
        {
            int gridIndex = 0;
            ColorRegions = new();


            for (int z = zSize - 1; z >= 0; z--)
            {
                for (int x = 0; x < xSize; x++)
                {
                    int i = z * xSize + x;

                    if (startGroundLayout.BlockDatas.Blocks[i] != null)
                    {
                        //Editorde ki ground dizaynı set ediyorum -- LevelSettings scripti
                        entity = startGroundLayout.BlockDatas.Blocks[i];
                        GridView view = LsAllGridViews[gridIndex];
                        BlockType type = (entity as GroundTypeDefinition).BlockType;
                        view.TypeDefinition = entity as GroundTypeDefinition;


                        view.mType = type;
                        view.name = $"grid---{entity.DefaultEntitySpriteName}";
                        view.SetColor((entity as GroundTypeDefinition).RGBColor);
                        
                        AddColorPair(view);
                        //üzerine mountain gelecek hexagonları ekliyorum.
                        if (startMountainLayout.BlockDatas.Blocks[i] != null)
                        {
                            entity = startMountainLayout.BlockDatas.Blocks[i];
                            LsMountainViews.Add(LsAllGridViews[gridIndex]);


                        }

                        //üzerine MillitaryBase
                        if (startMilitaryBaseLayout.BlockDatas.Blocks[i] != null)
                        {
                            IBlockEntityTypeDefinition entity = startMilitaryBaseLayout.BlockDatas.Blocks[i];
                            var millitaryBaseView = CreateMilitaryBaseDesign(LsAllGridViews[gridIndex], entity);
                            LsMilitaryBaseView.Add(millitaryBaseView);
                        }
                    }
                    else if (startGroundLayout.BlockDatas.Blocks[i] == null)
                    {
                        AddSea(LsAllGridViews[gridIndex]);
                    }
                    gridIndex++;
                }
            }

            for (int i = 0; i < LsMilitaryBaseView.Count; i++)
            {
                var view = LsMilitaryBaseView[i];
                var settings = _levelSettings.LevelData[0].MilitaryBaseTypeSettings;
                view.SetSettingsCountAndValues(settings.DefaultCounts[i]
                    , settings.WaitingTimes[i]
                    , settings.SoldierIncreaseValue
                    , settings.MaxSoldierCounts[i]);
                view.Initialize();

            }
            _boardView.BoardRegion = lsColorRegions;
        }

        void SetMountains()
        {
            for (int i = 0; i < LsMountainViews.Count; i++)
            {
                CreateMountain(LsMountainViews[i],
                    _levelSettings.LevelData[0].LevelMountainSettings.MountainIndex[i],
                    _levelSettings.LevelData[0].LevelMountainSettings.HexColor);
            }
        }

        void CreateMountain(GridView grid, int index,string HexColor)
        {
            for (int i = 0; i < index; i++)
            {
               
                float mountainHeight = (i + 1) * 5f;
                Vector3 mountainPosition = grid.transform.position
                    + Vector3.up * mountainHeight + Vector3.forward * 1.2f;

                GridView mountainGrid = _gridViewfactory.Create(
                    new GridView.Args(_boardView.transform,
                    Vector3.one,
                    zSize,
                    xSize,
                    mountainPosition.x,
                    mountainPosition.y,
                    mountainPosition.z));

               
                mountainGrid.SetColor(HexColor);
                mountainGrid.name = $"grid--{grid.transform.position.x}-{grid.transform.position.z}";

               
            }
        }

        MillitaryBaseView CreateMilitaryBaseDesign(GridView grid, IBlockEntityTypeDefinition entity)
        {
            MillitaryBaseView militaryBase = null;
            Vector3 MillitaryBasePosition = grid.transform.position
                + Vector3.up * 2.5f + Vector3.forward * 1.2f;

          
            var region = new Region();
            region.RegionPairs = lsColorRegions
                                .Where(n => n.ColorType == grid.mType)
                                .Select(n=>n.RegionsGrids)
                                .FirstOrDefault();

            militaryBase = _millitaryBaseViewFactory.Create(
                new MillitaryBaseView.Args(_boardView.transform, Vector3.one*2f,MillitaryBasePosition,grid, region));

           
            militaryBase.MilitaryBaseType = (entity as MilitaryBaseDefinition).MillitaryBaseType;
            militaryBase.UserType =grid.mType == BlockType.Blue ? militaryBase.UserType = UserType.Player : UserType.Enemy;
           

          



            return militaryBase;
        }


        void AddSea(GridView grid)
        {
            grid.gameObject.SetActive(false);
        }

        public GridView GetHexAt(int x, int z)
        {
            if (x < 0 || x >= xSize) return null;
            if (z < 0 || z >= zSize) return null;

            return hexs[x, z];
        }

        public void Dispose()
        {
            LsAllGridViews.Clear();
            LsMountainViews.Clear();
        }
    }

   
    
}

