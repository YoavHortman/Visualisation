using UnityEngine;

public class Shake : BasePattern {

  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) { }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.1f, 1f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) {
    PatternUtils.SetRandomTargetsInRadius(instances, 0);
  }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetColsAndRows(screenSizeInWorldCoords, fullSize);
  }

  public override void PreResizeUpdate(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    instance.targetPos = grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
    t.position = Vector2.MoveTowards(t.position, instance.targetPos, Mathf.Abs(Time.deltaTime * movementSpeed.y));
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
    if (PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime * 10)) {
      if (!instance.leaving) {
        instance.targetPos = grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
      }
      else {
        instance.targetPos = instance.spriteRenderer.transform.position + new Vector3(
          Random.Range(-fullSize / 2, fullSize / 2), Random.Range(-fullSize / 2, fullSize / 2), 0);
      }

      instance.leaving = !instance.leaving;
    }

    t.position = Vector2.MoveTowards(t.position, instance.targetPos, Mathf.Abs(Time.deltaTime * movementSpeed.y * 3));
  }
}