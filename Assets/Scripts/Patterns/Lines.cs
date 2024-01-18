using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lines : BasePattern {
  private float _step = 0;

  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    var stepInc = movementSpeed.y * Time.deltaTime * 5;
    _step += stepInc;
  }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.2f, 0.5f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) { }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetColsAndRows(screenSizeInWorldCoords, fullSize);
  }
  
  public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow,
    Vector2 movementSpeed) {
    throw new System.NotImplementedException();
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize,
    Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    var r = fullSize / 4;
    var center = grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
    if (MainMenuBackground.seed % 4 == 0) {
      var x = r * Mathf.Cos(_step + index);
      t.position = Vector3.MoveTowards(t.position, center + new Vector3(x, x, 0),
        Time.deltaTime * movementSpeed.magnitude);
    } else if (MainMenuBackground.seed % 4 == 1) {
      var y = r * Mathf.Sin(_step + index);
      t.position = Vector3.MoveTowards(t.position, center + new Vector3(y, y, 0),
        Time.deltaTime * movementSpeed.magnitude);
    } else if (MainMenuBackground.seed % 4 == 2) {
      var y = r * Mathf.Sin(_step + index);
      t.position = Vector3.MoveTowards(t.position, center + new Vector3(0, y, 0),
        Time.deltaTime * movementSpeed.magnitude);
    } else {
      var x = r * Mathf.Cos(_step + index);
      t.position = Vector3.MoveTowards(t.position, center + new Vector3(x, 0, 0),
        Time.deltaTime * movementSpeed.magnitude);
    }
  }
}