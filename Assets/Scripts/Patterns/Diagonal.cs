using UnityEngine;
using Random = UnityEngine.Random;

public class Diagonal : BasePattern {
  public override bool GetShouldHandleInstanceBounds() {
    return true;
  }

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


  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index,
    float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    if (MainMenuBackground.seed % 2 == 0) {
      if (curCol % 2 == 0) {
        t.position += new Vector3(Time.deltaTime * movementSpeed.x, -Time.deltaTime * movementSpeed.y, 0);
      } else {
        t.position += new Vector3(Time.deltaTime * movementSpeed.x, Time.deltaTime * movementSpeed.y, 0);
      }
    } else {
      if (curRow % 2 == 0) {
        t.position += new Vector3(-Time.deltaTime * movementSpeed.x, Time.deltaTime * movementSpeed.y, 0);
      } else {
        t.position += new Vector3(Time.deltaTime * movementSpeed.x, Time.deltaTime * movementSpeed.y, 0);
      }
    }
  }
}