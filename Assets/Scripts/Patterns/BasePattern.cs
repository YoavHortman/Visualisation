using System;
using UnityEngine;

public abstract class BasePattern {
  public abstract bool GetShouldHandleInstanceBounds();
  public abstract void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed);
  public abstract Sizes GetSizes();
  public abstract void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid);
  public abstract Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize);

  public abstract void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed);


  protected Vector2Int GetColsAndRows(Vector2 screenSizeInWorldCoords, float fullSize) {
    var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / fullSize);
    var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / fullSize);
    return new Vector2Int(nextColCount, nextRowCount);
  }

  protected Vector2Int GetColsAndRowsWithBuffer(Vector2 screenSizeInWorldCoords, float fullSize) {
    var s = GetColsAndRows(screenSizeInWorldCoords, fullSize);
    return new Vector2Int(++s.x, ++s.y);
  }
}