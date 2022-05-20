using UnityEngine;

public class Circle : BasePattern {
  public override bool GetShouldHandleInstanceBounds() { return false; }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) { }

  public override Sizes GetSizes() {
    var rowCol = new Vector2Int(1, 1);
    var sizes = new Sizes(0, 0);
    while (rowCol.x % 2 != 0 || rowCol.y % 2 != 0) {
      sizes.spriteSize = Random.Range(0.1f, 1f);
      sizes.borderPadding = sizes.spriteSize / 2;
      rowCol = GetNextColAndRow(ResizeListener.screenSizeInWorldCoords, sizes.Sum());
    }

    return sizes;
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) { }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / fullSize);
    var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / fullSize);
    return new Vector2Int(nextColCount, nextRowCount);
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index,
    float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    var cur = grid.WorldToCell(t.position);
    int gridRow = cur.y;
    int gridCol = cur.x;

    int dFromLeft = gridCol;
    int dFromRight = colRow.x - gridCol - 1;
    int dFromTop = colRow.y - gridRow - 1;
    int dFromBottom = gridRow;

    int col = Mathf.Min(dFromLeft, dFromRight);
    int row = Mathf.Min(dFromTop, dFromBottom);
    int n = Mathf.Min(col, row);

    float absSpeed = Mathf.Abs(movementSpeed.y);

    if (instance.leaving) {
      if (n % 2 == 0) {
        if (gridCol == n && gridRow != colRow.y - 1 - n) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }

        if (gridCol == colRow.x - 1 - n && gridRow != n) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }

        if (gridRow == n && gridCol != n) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }

        if (gridRow == colRow.y - 1 - n && gridCol != colRow.x - 1 - n) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }
      }
      else {
        if (gridCol == n && gridRow != n) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }

        if (gridCol == colRow.x - 1 - n && gridRow != colRow.y - 1 - n) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }

        if (gridRow == n && gridCol != colRow.x - n - 1) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }

        if (gridRow == colRow.y - 1 - n && gridCol != n) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
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