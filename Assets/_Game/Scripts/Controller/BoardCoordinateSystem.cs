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
            SpawnGrid();
        }

        public void SpawnGrid()
        {
            Vector3 offset = CalculateGridOffset();

            for (int row = 0; row < ROWS; row++)
            {
                for (int column = 0; column < COLUMNS; column++)
                {
                    GridView grid = CreateGridView(offset, row, column);
                    // Burada oluşturulan grid'i isterseniz bir listeye veya başka bir yere ekleyebilirsiniz
                }
            }

            SetGridSprites();
            SpawnBoss();
        }

        private Vector3 CalculateGridOffset()
        {
            var halfTileSize = 1 / 2f;
            var gridSizeHalfX = COLUMNS / 2f;
            var gridSizeHalfY = ROWS / 2f;
            var gridOffsetX = -gridSizeHalfX + halfTileSize;
            var gridOffsetY = -gridSizeHalfY;
            return new Vector3(gridOffsetX, gridOffsetY);
        }

        private GridView CreateGridView(Vector3 offset, int row, int column)
        {
            GridView grid = _gridViewfactory.Create(new GridView.Args(_boardView.transform, Vector3.one, offset, row, column));
            grid.name = "grid" + "-" + "(" + row + "," + column + ")";
            CalculateGamePlayArea(row, column, grid);
            return grid;
        }
        
        private void SpawnBoss()
        {
            PlayerBossView playerBoss = _playerBossFactory.Create(_applicationPrefabSettings.PlayerBossPrefab);
            playerBoss.Initialize();

            EnemyBossView enemyBoss = _enemyBossFactory.Create(_applicationPrefabSettings.EnemyBossPrefab);
            enemyBoss.Initialize();
        }
        public void CalculateGamePlayArea(int row, int column, GridView gridView)
        {
            LsAllGridViews.Add(gridView);

            if (!IsGridValid(row - 1, column) ||
                       !IsGridValid(row + 1, column) ||
                       !IsGridValid(row, column - 1)

                      ) // road 
            {
                if (!AreCoordinatesInsideTheBoard(new Vector2Int(row, column)))
                {
                    LsRoadGrids.Add(gridView);
                    gridView.ChangeState(GridState.Filled);
                }
                else
                {
                    LsGameplayGrids.Add(gridView);
                    gridView.ChangeState(GridState.Free);
                  
                }

            }
            else
            {
                LsGameplayGrids.Add(gridView);
                gridView.ChangeState(GridState.Free);
               
            }
        }
        private bool IsGridValid(int targetRow, int targetColumn)
        {

            if (targetRow < 0 || targetRow >= ROWS || targetColumn < 0 || targetColumn >= COLUMNS - 1)
            {

                return false;
            }

            return true;
        }

        private void SetGridSprites()
        {
            SetRoadListIndices();
            GridView previousGrid = null;

            foreach (var grid in LsRoadGrids)
            {
                GridView nextGrid = FindNextGrid(grid);

                if (previousGrid != null && nextGrid != null)
                {
                    SetSpriteBasedOnDirection(grid, previousGrid.transform.position, nextGrid.transform.position);
                }
                else
                {
                    grid.SetSprite(_gridViewSettings.RoadViewData.VerticalSprite);
                }

                previousGrid = grid;
            }
        }

        public GridView GetRandomGridWithNullConnected()
        {

            var nullConnectedGrids = LsGameplayGrids
                 .Where(grid => grid.GridState == GridState.Free).ToList();

            if (nullConnectedGrids.Count > 0)
            {
                return nullConnectedGrids[Random.Range(0, nullConnectedGrids.Count)];
            }
            else
            {
                return null;
            }
        }
        private void SetSpriteBasedOnDirection(GridView grid, Vector3 previousPosition, Vector3 nextPosition)
        {
            Vector2 direction = (previousPosition - nextPosition).normalized;

            if (direction.x == 0 && direction.y != 0) // Vertical
            {
                grid.SetSprite(_gridViewSettings.RoadViewData.VerticalSprite);
            }
            else if (direction.x != 0 && direction.y == 0) // Horizontal
            {
                grid.SetSprite(_gridViewSettings.RoadViewData.HorizontalSprite);
            }
            else if (direction.x < 0 && direction.y > 0) // Corner Left
            {
                grid.SetSprite(_gridViewSettings.RoadViewData.LeftSprite);
            }
            else // Corner Right
            {
                grid.SetSprite(_gridViewSettings.RoadViewData.RightSprite);
            }
        }

        public static bool AreCoordinatesInsideTheBoard(Vector2Int coordinates)
        {
            return coordinates.x == 0 &&
                   coordinates.x < ROWS &&
                   coordinates.y > 0 &&
                   coordinates.y < COLUMNS - 1
                   ;
        }

        private void SetRoadListIndices()
        {
            for (int currentIndex = 0; currentIndex < LsRoadGrids.Count; currentIndex++)
            {
                GridView nextGrid = FindNextGrid(LsRoadGrids[currentIndex]);

                if (nextGrid == null)
                    return;

                float distanceSquared = Vector3.SqrMagnitude(LsRoadGrids[currentIndex].transform.position - nextGrid.transform.position);

                if (distanceSquared > 1)
                {
                    RearrangeGridList(currentIndex);
                }
            }
        }

        private void RearrangeGridList(int checkIndex)
        {
            for (int i = checkIndex + 1; i < LsRoadGrids.Count; i++)
            {
                float distanceSquared = Vector3.SqrMagnitude(LsRoadGrids[checkIndex].transform.position - LsRoadGrids[i].transform.position);

                if (distanceSquared == 1)
                {
                    SwapGrids(checkIndex + 1, i);
                    return;
                }
            }
        }

        private void SwapGrids(int index1, int index2)
        {
            int tempIndex = index2;
            GridView tempGrid = LsRoadGrids[index1];
            LsRoadGrids[index1] = LsRoadGrids[index2];
            LsRoadGrids[tempIndex] = tempGrid;
        }

        private GridView FindNextGrid(GridView currentGrid)
        {
            int currentIndex = LsRoadGrids.IndexOf(currentGrid);

            if (currentIndex < LsRoadGrids.Count - 1)
            {
                return LsRoadGrids[currentIndex + 1];
            }

            return null;
        }
        public void Dispose()
        {
            LsAllGridViews.Clear();
            LsGameplayGrids.Clear();
            LsRoadGrids.Clear();
        }
    }
}

