using MyProject.GamePlay.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject.Core.Controllers
{
    public class PathFinderController
    {
        public List<GridView> FindGridPath(GridView startGrid, GridView endGrid)
        {
            List<GridView> openSet = new List<GridView>();
            HashSet<GridView> closedSet = new HashSet<GridView>();
            openSet.Add(startGrid);

            // Başlangıç ve hedef nokta arasındaki uzaklık
            int distanceBetweenStartAndEnd = GetDistance(startGrid, endGrid);

            // En fazla çizilecek yol uzunluğu (başlangıç ve hedef nokta arasındaki uzaklığın belirlediği oranda)
            int maxPathLength = distanceBetweenStartAndEnd * 2; // Örnek: 2 katı kadar bir oran kullanıyoruz
            if (distanceBetweenStartAndEnd < 3)
            {
                maxPathLength = distanceBetweenStartAndEnd * 4;
            }
            while (openSet.Count > 0)
            {
                GridView currentGrid = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentGrid.fCost || openSet[i].fCost == currentGrid.fCost && openSet[i].hCost < currentGrid.hCost)
                    {
                        currentGrid = openSet[i];
                    }
                }

                openSet.Remove(currentGrid);
                closedSet.Add(currentGrid);

                if (currentGrid == endGrid)
                {
                    return RetracePath(startGrid, endGrid);
                }

                foreach (GridView neighbour in currentGrid.neighborList)
                {
                    if (closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    if (neighbour.IsMountain)
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentGrid.gCost + GetDistance(currentGrid, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, endGrid);
                        neighbour.parent = currentGrid;
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }

                // En fazla çizilecek yol uzunluğunu kontrol etme
                int currentPathLength = GetDistance(startGrid, currentGrid);
                if (currentPathLength + GetDistance(currentGrid, endGrid) > maxPathLength)
                {
                    break;
                }
            }

            return null;
        }

        private List<GridView> RetracePath(GridView startGrid, GridView endGrid)
        {
            List<GridView> path = new List<GridView>();
            GridView currentGrid = endGrid;

            while (currentGrid != startGrid)
            {
                path.Add(currentGrid);
                currentGrid = currentGrid.parent;
            }

            path.Reverse();
            return path;
        }

        private int GetDistance(GridView nodeA, GridView nodeB)
        {
            int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
            int dstZ = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);

            // Köşelerin keskinlik düzeltmesi (Diagonal Cost) için Manhattan mesafesi kullanılıyor
            if (dstX > dstZ)
                return 14 * dstZ + 10 * (dstX - dstZ);
            else
                return 14 * dstX + 10 * (dstZ - dstX);
        }
    }
}

