﻿using MyProject.Core.Enums;
using MyProject.Core.Settings;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static MyProject.Core.Const.GlobalConsts.BoardConsts;

namespace MyProject.GamePlay.Controllers
{
    public class BoardCoordinateSystem
    {
        public List<GridView> LsAllGridViews = new();
        public List<GridView> LsMountainViews = new();

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
           
            startGroundLayout = _levelSettings.LevelGroundTypeSettings.gridStartLayout;
            startMilitaryBaseLayout = _levelSettings.MilitaryBaseTypeSettings.gridStartLayout;
            startMountainLayout = _levelSettings.LevelMountainSettings.gridStartLayout;

            int xSize = _levelSettings.LevelGroundTypeSettings.gridStartLayout.Width;
            int zSize = _levelSettings.LevelGroundTypeSettings.gridStartLayout.Height;

            float xOffset = _levelSettings.LevelData[levelIndex].gridViewData.xOffset;
            float zOffset = _levelSettings.LevelData[levelIndex].gridViewData.zOffset;

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
        
        Dictionary<BlockType, List<GridView>> colorToGrids = new Dictionary<BlockType, List<GridView>>();

        void AddColorPair(GridView grid)
        {
            BlockType color = grid.mType;

            if (!colorToGrids.ContainsKey(color))
            {
                colorToGrids[color] = new List<GridView>();
            }
            colorToGrids[color].Add(grid);
        }
                void SetLevelGroundDesign(int xSize, int zSize)
        {
            int gridIndex = 0;
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
                      
                       

                        view.mType = type;
                        view.name = $"grid---{entity.DefaultEntitySpriteName}";
                        view.SetColor((entity as GroundTypeDefinition).RGBColor);
                        AddColorPair(view);
                        //üzerine mountain gelecek hexagonları ekliyorum.
                        if (startMountainLayout.BlockDatas.Blocks[i]!=null)
                        {
                            entity = startMountainLayout.BlockDatas.Blocks[i];
                            LsMountainViews.Add(LsAllGridViews[gridIndex]);

                            
                        }

                        //üzerine MillitaryBase
                        if (startMilitaryBaseLayout.BlockDatas.Blocks[i] != null)
                        {
                            IBlockEntityTypeDefinition entity = startMilitaryBaseLayout.BlockDatas.Blocks[i];
                            var millitaryBaseView=CreateMilitaryBaseDesign(LsAllGridViews[gridIndex],entity);
                           
                        }
                    }
                    else if (startGroundLayout.BlockDatas.Blocks[i] == null)
                    {
                        AddSea(LsAllGridViews[gridIndex]);
                    }
                    gridIndex++;
                }
            }

         
        }

        void SetMountains()
        {
            for (int i = 0; i < LsMountainViews.Count; i++)
            {
                CreateMountain(LsMountainViews[i], _levelSettings.LevelMountainSettings.MountainIndex[i]);
            }
        }

        void CreateMountain(GridView grid, int index)
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

               
                mountainGrid.SetColor(grid.GetColor());
                mountainGrid.name = $"grid--{grid.transform.position.x}-{grid.transform.position.z}";

               
            }
        }

        MillitaryBaseView CreateMilitaryBaseDesign(GridView grid, IBlockEntityTypeDefinition entity)
        {
            MillitaryBaseView militaryBase = null;
            Vector3 MillitaryBasePosition = grid.transform.position
                + Vector3.up * 2.5f + Vector3.forward * 1.2f;
            var region = new Region();
            region.RegionPairs = colorToGrids
                                .Where(pair => pair.Key == grid.mType) 
                                .Select(pair => pair.Value) 
                                .FirstOrDefault();

            militaryBase = _millitaryBaseViewFactory.Create(
                new MillitaryBaseView.Args(_boardView.transform, Vector3.one*2f,MillitaryBasePosition,grid, region));

            militaryBase.MilitaryBaseType = (entity as MilitaryBaseDefinition).MillitaryBaseType;
            militaryBase.UserType =grid.mType == BlockType.Blue ? militaryBase.UserType = UserType.Player : UserType.Enemy;
            militaryBase.Initialize();



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

