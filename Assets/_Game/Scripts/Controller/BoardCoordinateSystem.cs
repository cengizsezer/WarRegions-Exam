using MyProject.Core.Const;
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
    public ResourceTypeData ResourceTypeData;
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
        private LevelData _levelData { get; set; }
        private List<ColorRegion> lsColorRegions = new();
        public List<GridView> LsAllGridViews = new();
        public List<GridView> LsPrevMountainViews = new();
        public List<GridView> LsMountainViews = new();
        public List<MilitaryBaseView> LsMilitaryBaseView = new();
        public List<MilitaryBaseView> lsEnemyMilitaryBaseView = new();
        public List<MilitaryBaseView> LsPlayerMilitaryBaseView = new();

        public ColorRegion ColorRegions;

        private int xSize, zSize;

        private GridStartLayout startGroundLayout;
        private GridStartLayout startMountainLayout;
        private GridStartLayout startMilitaryBaseLayout;

        #region Injection
        private readonly GridView.Factory _gridViewfactory;
        private readonly MilitaryBaseView.Factory _millitaryBaseViewFactory;
        private readonly BoardView _boardView;
        private readonly GridViewSettings _gridViewSettings;
        private readonly LevelSettings _levelSettings;
        private readonly ApplicationPrefabSettings _applicationPrefabSettings;
        private readonly LevelController _levelController;
       

        [Inject]
        public BoardCoordinateSystem
        (
            BoardView boardView
            , GridView.Factory gridViewfactory
            , GridViewSettings gridViewSettings
            , ApplicationPrefabSettings applicationPrefabSettings
            ,LevelSettings levelSettings
            , MilitaryBaseView.Factory millitaryBaseViewFactory
            ,LevelController levelController
          
        )
        {
            _boardView = boardView;
            _gridViewfactory = gridViewfactory;
            _gridViewSettings = gridViewSettings;
            _applicationPrefabSettings = applicationPrefabSettings;
            _levelSettings = levelSettings;
            _millitaryBaseViewFactory = millitaryBaseViewFactory;
            _levelController = levelController;
        }

        #endregion

        public void Init()
        {
            _levelData = _levelController.GetLevelData();
            SpawnHexes();
        }
        IBlockEntityTypeDefinition entity;
        public void SpawnHexes()
        {
           
            startGroundLayout = _levelData.LevelGroundTypeSettings.gridStartLayout;
            startMilitaryBaseLayout = _levelData.MilitaryBaseTypeSettings.gridStartLayout;
            startMountainLayout = _levelData.LevelMountainSettings.gridStartLayout;

            int xSize = _levelData.LevelGroundTypeSettings.gridStartLayout.Width;
            int zSize = _levelData.LevelGroundTypeSettings.gridStartLayout.Height;

            float xOffset = _levelData.LevelGroundTypeSettings.xOffset;
            float zOffset = _levelData.LevelGroundTypeSettings.zOffset;

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

        private void AddColorPair(GridView grid)
        {
            ColorType color = grid.ColorType;
            int configureType = grid.ConfigureType;

            // Renk eşleşmesi kontrol edilecek ColorRegion nesnesi
           
            ColorRegion region = lsColorRegions.Find(r => grid.ColorType==r.ResourceTypeData.ColorType
            && grid.ConfigureType==r.ResourceTypeData.ConfigureType);

            
            if (region == null)
            {
                region = new ColorRegion();
                region.ResourceTypeData = grid.ResourceTypeData;
                lsColorRegions.Add(region);
            }

            
            region.RegionsGrids.Add(grid);
        }

      
        private void SetLevelGroundDesign(int xSize, int zSize)
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
                        //Editorde ki ground dizaynı set ediyorum -- LevelSettings,LevelData scripti
                        entity = startGroundLayout.BlockDatas.Blocks[i];
                        GridView view = LsAllGridViews[gridIndex];
                        view.ResourceTypeData = (entity as GroundTypeDefinition).ResourceTypeData;
                        view.TypeDefinition = entity as GroundTypeDefinition;
                       
                        
                        AddColorPair(view);
                        //üzerine mountain gelecek hexagonları ekliyorum.
                        if (startMountainLayout.BlockDatas.Blocks[i] != null)
                        {
                            entity = startMountainLayout.BlockDatas.Blocks[i];
                            LsPrevMountainViews.Add(LsAllGridViews[gridIndex]);


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
                var settings = _levelData.MilitaryBaseTypeSettings;
                view.SetSettingsCountAndValues(settings.DefaultCounts[i]
                    , settings.WaitingTimes[i]
                    , settings.SoldierIncreaseValue
                    , settings.MaxSoldierCounts[i]);
                view.Initialize();

            }
            _boardView.BoardRegion = lsColorRegions;
        }
        private void SetMountains()
        {
            for (int i = 0; i < LsPrevMountainViews.Count; i++)
            {
                CreateMountain(LsPrevMountainViews[i],
                   _levelData.LevelMountainSettings.MountainIndex[i],
                    _levelData.LevelMountainSettings.HexColor);
            }
        }

        private void CreateMountain(GridView grid, int index,string HexColor)
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
                LsMountainViews.Add(mountainGrid);
               
            }
        }

        public List<MilitaryBaseView> GetNötrMilitaryBase()
        {
            var lsNötr = LsMilitaryBaseView.Except(lsEnemyMilitaryBaseView).Except(LsPlayerMilitaryBaseView).ToList();
            return lsNötr;
        }
        private MilitaryBaseView CreateMilitaryBaseDesign(GridView grid, IBlockEntityTypeDefinition entity)
        {
            MilitaryBaseView militaryBase = null;
            Vector3 MillitaryBasePosition = grid.transform.position
                + Vector3.up * 2.5f + Vector3.forward * 1.2f;

          
            var region = new Region();
            region.RegionPairs = lsColorRegions
                                .Where(r => grid.ColorType == r.ResourceTypeData.ColorType
            && grid.ConfigureType == r.ResourceTypeData.ConfigureType)
                                .Select(r=>r.RegionsGrids)
                                .FirstOrDefault();

            militaryBase = _millitaryBaseViewFactory.Create(
                new MilitaryBaseView.Args(_boardView.transform,
                Vector3.one*2f,
                MillitaryBasePosition,
                (entity as MilitaryBaseDefinition).MillitaryBaseType,
                grid,
                region));

            switch (grid.ColorType)
            {
                case ColorType.Blue:
                    militaryBase.UserType = UserType.Player;
                    break;
                case ColorType.Gray:
                    militaryBase.UserType = UserType.Nötr;
                    break;
                default:
                    militaryBase.UserType = UserType.Enemy;
                    break;
            }

            return militaryBase;
        }

        public void AddGamePlayList(MilitaryBaseView view)
        {
            switch (view.UserType)
            {
                case UserType.Player:
                    LsPlayerMilitaryBaseView.Add(view);
                    break;
                case UserType.Enemy:
                    lsEnemyMilitaryBaseView.Add(view);
                    break;
                case UserType.Nötr:
                    break;
                default:
                    break;
            }
        }
        private void AddSea(GridView grid)
        {
            grid.gameObject.SetActive(false);
        }

        public void Dispose()
        {
            foreach (var grid in LsAllGridViews)
            {
                grid.DespawnItem();
            }
            foreach (var Mountain in LsMountainViews)
            {
                if(Mountain.gameObject.activeInHierarchy)
                         Mountain.DespawnItem();
            }

            foreach (var MilitaryBase in LsMilitaryBaseView)
            {
                MilitaryBase.DespawnItem();
            }
            LsAllGridViews.Clear();
            lsColorRegions.Clear();
            LsPrevMountainViews.Clear();
            LsMountainViews.Clear();
            LsMilitaryBaseView.Clear();
        }
    }

   
    
}

