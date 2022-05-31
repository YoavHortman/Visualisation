  using UnityEngine;

  public class Default : BasePattern {
    public override bool GetShouldHandleInstanceBounds() {
      return true;
    }

    public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) { }

    public override Sizes GetSizes() {
      return new Sizes(Random.Range(0.1f, 10f), Random.Range(0f, 0.3f));
    }

    public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) { }

    public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
      return GetColsAndRowsWithBuffer(screenSizeInWorldCoords, fullSize);
    }

    public override void PrePatternChange(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid,
      Vector2Int colRow, Vector2 movementSpeed) {
    }

    public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
      return true;
    }

    public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
      t.position += new Vector3(Time.deltaTime * movementSpeed.x, Time.deltaTime * movementSpeed.y, 0);
    }
  }