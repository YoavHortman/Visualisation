using UnityEngine;

public class Explode : BasePattern {
  private bool _explosionStarting = false;

  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    foreach (var instance in instances) {
      if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime * 3)) {
        return;
      }
    }

    if (!_explosionStarting) {
      PatternUtils.SetRandomTargetsOnGrid(instances, grid, colRow);
    } else {
      PatternUtils.ResetTargetPosTo(instances,
        instances[Random.Range(0, instances.Length)].spriteRenderer.transform.position);
    }

    _explosionStarting = !_explosionStarting;
  }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.1f, 1f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) {
    PatternUtils.ResetTargetPosTo(instances, Vector2.zero);
    _explosionStarting = false;
  }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetColsAndRows(screenSizeInWorldCoords, fullSize);
  }

  public override void PreResizeUpdate(Transform t, Instance instance, int curCol, int curRow, int index,
    float fullSize, Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    Update(t, instance, curCol, curRow, index, fullSize, grid, colRow, movementSpeed);
  }

  public override bool IsReadyForResize(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    if (_explosionStarting) {
      foreach (var instance in instances) {
        if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime * 3)) {
          return false;
        }
      }

      return true;
    }
    
    
    AfterUpdate(instances, grid, colRow, movementSpeed);
    return false;
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize,
    Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    t.position = Vector2.MoveTowards(t.position, instance.targetPos, Mathf.Abs(Time.deltaTime * movementSpeed.y * 3));
  }
}