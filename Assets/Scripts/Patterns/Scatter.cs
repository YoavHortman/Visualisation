using UnityEngine;

public class Scatter : BasePattern {
  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    foreach (var instance in instances) {
      if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime)) {
        return;
      }
    }

    PatternUtils.SetRandomTargetsOnGrid(instances, grid, colRow);
  }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.1f, 1f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) {
    PatternUtils.SetRandomTargetsOnGrid(instances, grid, colRow);
  }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetColsAndRows(screenSizeInWorldCoords, fullSize);
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    t.position = Vector2.Lerp(t.position, instance.targetPos, Mathf.Abs(Time.deltaTime * movementSpeed.y * 3));
  }
}