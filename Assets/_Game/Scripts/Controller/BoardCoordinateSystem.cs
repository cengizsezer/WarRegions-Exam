using MyProject.Core.Enums;
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
        public List<GridView> LsRoadGrids = new();
        public List<GridView> LsGameplayGrids = new();

        private int xSize, zSize;
        private float xOffset, zOffset;
        public GridView[,] hexs;



        #region Injection
        private readonly GridView.Factory _gridViewfactory;
        private readonly BoardView _boardView;
        private readonly GridViewSettings _gridViewSettings;
        private readonly ApplicationPrefabSettings _applicationPrefabSettings;
        private readonly EnemyBossView.Factory _enemyBossFactory;
        private readonly PlayerBossView.Factory _playerBossFactory;

        [Inject]
        public BoardCoordinateSystem
        (
            BoardView boardView
            , GridView.Factory gridViewfactory
            , GridViewSettings gridViewSettings
            , ApplicationPrefabSettings applicationPrefabSettings
            , PlayerBossView.Factory playerBossFactory
            , EnemyBossView.Factory enemyBossFactory
        )
        {
            _boardView = boardView;
            _gridViewfactory = gridViewfactory;
            _gridViewSettings = gridViewSettings;
            _applicationPrefabSettings = applicationPrefabSettings;
            _playerBossFactory = playerBossFactory;
            _enemyBossFactory = enemyBossFactory;
        }

        #endregion

        public void Init()
        {
            xOffset=_gridViewSettings.GridViewData.xOffset;
            zOffset=_gridViewSettings.GridViewData.zOffset;
            xSize=_gridViewSettings.GridViewData.xSize;
            zSize=_gridViewSettings.GridViewData.zSize;
            SpawnHexes();
        }

        void SpawnHexes()
        {

            if (xSize % 2 == 0)
                xSize++;

            float xStart = (-xOffset / 2f) * (xSize - 1) / 2f;
            float zStart = 0f;

            hexs = new GridView[xSize - 2, zSize - 2];

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    float xPos = xStart + (x * xOffset) + (z % 2 == 0 ? 0f : xOffset / 2f);
                    float zPos = zStart + (z * zOffset);
                    GridView grid = _gridViewfactory.Create
                        (new GridView.Args(_boardView.transform,
                        Vector3.one,
                        zSize,
                        xSize,
                        xPos,
                        zPos));
                   
                    LsAllGridViews.Add(grid);
                }
            }

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
            LsGameplayGrids.Clear();
            LsRoadGrids.Clear();
        }
    }
}

