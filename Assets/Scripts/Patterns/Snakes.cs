using UnityEngine;

public class Snakes : BasePattern {

  public override bool GetShouldHandleInstanceBounds() {
    return true;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) { }

  public override Sizes GetSizes() {
    var rowCol = new Vector2Int(1, 1);
    var sizes = new Sizes(0, 0);
    while (rowCol.x % 2 != 0 || rowCol.y % 2 != 0) {
      sizes.spriteSize = Random.Range(0.3f, 5f);
      sizes.borderPadding = sizes.spriteSize / 2;
      rowCol = GetNextColAndRow(ResizeListener.screenSizeInWorldCoords, sizes.Sum());
    }

    return sizes;
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) { }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetColsAndRowsWithBuffer(screenSizeInWorldCoords, fullSize);
  }
  

  public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    foreach (var instance in instances) {
      if (!PatternUtils.DidReach(instance.spriteRenderer.transform, instance.targetPos, 0.0001f)) {
        return false;
      }
    }

    return true;
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    var cur = grid.WorldToCell(t.position);
    int gridRow = Mathf.Abs(cur.y);
    int gridCol = Mathf.Abs(cur.x);
    float absSpeed = Mathf.Abs(movementSpeed.y);

    if (instance.leaving) {
      if (MainMenuBackground.seed % 2 == 0) {
        if (gridCol % 2 == 0 && gridRow % 2 == 0) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }

        if (gridCol % 2 == 1 && gridRow % 2 == 0) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }

        if (gridCol % 2 == 1 && gridRow % 2 == 1) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }

        if (gridCol % 2 == 0 && gridRow % 2 == 1) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }
      }
      else {
        if (gridCol % 2 == 0 && gridRow % 2 == 0) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }

        if (gridCol % 2 == 1 && gridRow % 2 == 0) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }

        if (gridCol % 2 == 1 && gridRow % 2 == 1) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }

        if (gridCol % 2 == 0 && gridRow % 2 == 1) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }
      }

      var newCell = grid.WorldToCell(t.position);
      if (newCell.x != cur.x || newCell.y != cur.y) {
        instance.leaving = false;
      }
    }
    else {
      t.position = Vector2.MoveTowards(t.position, grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed);
      if (PatternUtils.DidReach(t, grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed)) {
        instance.leaving = true;
      }
    }
  }
}