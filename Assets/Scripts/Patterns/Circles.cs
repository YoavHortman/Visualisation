using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Circles : BasePattern {
  private float _angle;
  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    var angleInc = movementSpeed.y * Time.deltaTime * 5;
    _angle += angleInc;
  }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.1f, 1f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) { }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / fullSize);
    var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / fullSize);
    return new Vector2Int(nextColCount, nextRowCount);
  }

  public override void PreResizeUpdate(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    instance.targetPos = grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
    t.position = Vector3.MoveTowards(t.position, instance.targetPos, Time.deltaTime * movementSpeed.magnitude / 3);
  }

  public override bool IsReadyForResize(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    foreach (var instance in instances) {
      if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, 0.0001f)) {
        return false;
      }
    }

    return true;
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    var r = fullSize / 4;
    var x = r * Mathf.Cos(_angle + index);
    var y = r * Mathf.Sin(_angle + index);
    var center = grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
    t.position = Vector3.MoveTowards(t.position, center + new Vector3(x, y, 0), Time.deltaTime * movementSpeed.magnitude);
  }

}
