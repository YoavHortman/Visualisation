using UnityEngine;


// TODO BROKEN PATTERN
public class Spiral : BasePattern {
  private float _angle = 0;
  float lastAlpha = 0f;
  float lastR = 5;

  public override bool GetShouldHandleInstanceBounds() {
    return false;
  }

  public override void AfterUpdate(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    _angle += Time.deltaTime * movementSpeed.y;
  }

  public override Sizes GetSizes() {
    return new Sizes(Random.Range(0.1f, 1f), Random.Range(0f, 0.3f));
  }

  public override void AfterSizeUpdate(Instance[] instances, Vector2Int colRow, Grid grid) { }

  public override Vector2Int GetNextColAndRow(Vector2 screenSizeInWorldCoords, float fullSize) {
    return GetNextColAndRow(screenSizeInWorldCoords, fullSize);
  }

  public override void PrePatternChange(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    throw new System.NotImplementedException();
  }

  public override bool IsReadyForPatternChange(Instance[] instances, Grid grid, Vector2Int colRow, Vector2 movementSpeed) {
    throw new System.NotImplementedException();
  }

  public override void Update(Transform t, Instance instance, int curCol, int curRow, int index, float fullSize, Grid grid,
    Vector2Int colRow, Vector2 movementSpeed) {
    var center = Vector3.zero;
    var alphaInc = 0.01f;
    var rInc = 1;
    float currAlpha = lastAlpha + alphaInc;
    int counter = 0;
    float currR = currAlpha + lastR;
    var RESULT = currR * currR + lastR * lastR - 2 * currR * lastR * Mathf.Cos(currAlpha);

    while (RESULT <= fullSize * fullSize) {
      Debug.Log(index);
      currAlpha += alphaInc;
      currR = currAlpha + lastR;
      RESULT = currR * currR + lastR * lastR - 2 * currR * lastR * Mathf.Cos(currAlpha);
    }

    lastR = currR;
    lastAlpha = currAlpha;
    // float r1 = index / 10f;

    // var angle = Vector3.Angle(new Vector2(center.x + r1, center.y + r1), center);
    // float angle = Mathf.Asin(Mathf.Clamp((r0 * r0 + r1 * r1 - GetFullSize() * GetFullSize()) / (2 * r1* r0), -1, 1));
    // Debug.Log(index + " " + (r0 * r0 + r1 * r1 - GetFullSize() * GetFullSize()) / Mathf.Max((2 * r1* r0), Mathf.Epsilon));
    // var r = Vector3.Distance(center, t.position);
    // +curRow;
    // var sqrt = Mathf.Sqrt(colRow.x * colRow.y);
    // var n = (index % 4) * (Mathf.PI / 2);
    var n = 0;
    // Scale R to increase intensity
    // _angle = 0;
    var x = currR * Mathf.Cos(currAlpha);
    var y = currR * Mathf.Sin(currAlpha);

    t.position = new Vector3(x, y, 0);
  }


  // void SpiralMove(Transform t, Instance instance, int curCol
  //   , int curRow, int index) {
  //   var center = Vector3.zero;
  //   float r0 = Mathf.Max((index - 1) / 5f, Mathf.Epsilon);
  //   float r1 = index / 5f;
  //   float angle = Mathf.Acos(Mathf.Clamp((r1 * r1 + r1 * r1 - GetFullSize() * GetFullSize()) / (2 * r1* r1), -1, 1));
  //   // Debug.Log(index + " " + (r0 * r0 + r1 * r1 - GetFullSize() * GetFullSize()) / Mathf.Max((2 * r1* r0), Mathf.Epsilon));
  //   // var r = Vector3.Distance(center, t.position);
  //   // +curRow;
  //   // var sqrt = Mathf.Sqrt(colRow.x * colRow.y);
  //   // var n = (index % 4) * (Mathf.PI / 2);
  //   var n = 0;
  //   // Scale R to increase intensity
  //   _angle = 0;
  //   var x = r1 * Mathf.Cos(_angle + angle + r1 + n);
  //   var y = r1 * Mathf.Sin(_angle + angle + r1 + n);
  //   
  //   t.position = new Vector3(x, y, 0);
  // }
}
