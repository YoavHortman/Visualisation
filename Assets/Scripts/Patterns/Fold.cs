using UnityEngine;

public class Fold : BasePattern {
  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    foreach (var instance in instances) {
      if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime)) {
        return;
      }
    }

    SetFoldTargets(instances, grid, colRow);
  }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.1f, 1f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) {
    SetFoldTargets(instances, grid, colRow);
  }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetColsAndRows(screenSizeInWorldCoords, fullSize);
  }

  public override void PrePatternChange(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    Update(t, instance, curCol, curRow, index, fullSize, grid, colRow, movementSpeed);
  }

  public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    foreach (var instance in instances) {
      if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, 0.0001f)) {
        return false;
      }
    }

    return true;
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize,
    Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    t.position = Vector2.MoveTowards(t.position, instance.targetPos, Mathf.Abs(Time.deltaTime * movementSpeed.y * 3));
  }

  private void SetFoldTargets(Instance[] instances, Grid grid, Vector2Int colRow) {
    var counter = 0;
    for (var i = 0; i < colRow.x; i++) {
      for (var j = 0; j < colRow.y; j++) {
        var cell = grid.WorldToCell(instances[counter].spriteRenderer.transform.position);
        instances[counter].targetPos =
          grid.GetCellCenterWorld(new Vector3Int(colRow.x - cell.x - 1, colRow.y - cell.y - 1, 0));
        counter++;
      }
    }
  }
}