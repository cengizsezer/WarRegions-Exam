using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridItem
{
    void BondWithGrid(GridView gridView);
    void OnSelected(GridView gridView);
    void OnMerged(GridView gridView);
    void OnDragged(Vector3 dragPosition);
    void MoveToGrid(GridView gridView, float speed);
}
