using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

internal class SpriteWithMetadata
{
  public readonly Sprite sprite;
  public Vector2 sizeData;
  public SpriteRenderer spriteRenderer;

  public SpriteWithMetadata(Sprite sprite, Vector2 sizeData) {
    this.sprite = sprite;
    this.sizeData = sizeData;
  }
}

class Instance
{
  // Indicates if currently leaving or entering a cell
  public bool leaving;
  public readonly SpriteRenderer spriteRenderer;

  public Vector2 targetPos;

  public Instance(SpriteRenderer spriteRenderer) {
    this.spriteRenderer = spriteRenderer;
    leaving = true;
  }
}

internal enum Patterns
{
  Default,
  Steps,
  Zigzag,
  Diagonal,
  Circle,
  Scatter,
  Snakes,
  Explode,
  Shake,
  Circles,
  Spiral,
}

public class MainMenuBackground : MonoBehaviour
{
  [SerializeField] private Sprite[] sprites = Array.Empty<Sprite>();
  [SerializeField] private float spriteSize = 5f;
  [SerializeField] private float RotationSpeed = 20;
  [SerializeField] private float MovementSpeedX = -2;
  [SerializeField] private float MovementSpeedY = -2;
  [SerializeField] private int orderInLayer = 0;

  private float _borderPadding = 0f;
  private SpriteWithMetadata[] _spritesWithMetaData;
  private Instance[] _instances;
  private Camera _mainCam;
  private int _rowCount;
  private int _colCount;
  private Sprite[] _overrideRandomWith;
  private Quaternion _currentRotation;
  private Color _color = new Color(1, 1, 1, 1);
  private Color _targetColor = new Color(1, 1, 1, 1);
  private float _rangeStart = 0;
  private float _rangeEnd = 10;
  private Grid _grid;

  private int _seed = 0;

  private Patterns pattern = Patterns.Default;


  [Serializable]
  struct Childrens
  {
    public List<Sprite> childrens;
  }

  [SerializeField] Childrens[] Combos;

  SpriteWithMetadata getRandomSpriteWithMeteData() {
    if (_overrideRandomWith != null) {
      return getMetadata(_overrideRandomWith[Random.Range(0, _overrideRandomWith.Length)]);
    }

    return _spritesWithMetaData[Random.Range(0, _spritesWithMetaData.Length)];
  }

  SpriteWithMetadata getMetadata(Sprite sprite) {
    return _spritesWithMetaData.First(x => x.sprite == sprite);
  }

  float getRandomInRange() {
    return Random.Range(_rangeStart, _rangeEnd);
  }


  void Start() {
    _instances = new Instance[] { };
    _mainCam = Camera.main;
    _grid = GetComponent<Grid>();
    UpdateGrid();
    InitSpritesWithMetaData();

    ResizeListener.onResize.AddListener(AfterResize);
    Invoke(nameof(UpdateTargetColor), getRandomInRange());
    Invoke(nameof(UpdateRange), 15);
    Invoke(nameof(UpdateSize), getRandomInRange());
    Invoke(nameof(UpdateSpeedAndRotation), getRandomInRange());
    Invoke(nameof(SetMode), getRandomInRange());
  }

  void UpdateGrid() {
    _grid.cellSize = new Vector3(spriteSize, spriteSize, 0);
    _grid.cellGap = new Vector3(_borderPadding, _borderPadding, 0);
  }

  float GetFullSize() {
    return spriteSize + _borderPadding;
  }

  [EditorButton]
  void UpdateSize() {
    SetPattern();
    switch (pattern) {
      case Patterns.Default:
      case Patterns.Zigzag: {
        spriteSize = Random.Range(0.1f, 10f);
        _borderPadding = Random.Range(0f, 0.3f);
        break;
      }
      case Patterns.Spiral:
      case Patterns.Circles:
      case Patterns.Shake:
      case Patterns.Explode:
      case Patterns.Scatter: {
        spriteSize = Random.Range(0.1f, 1f);
        _borderPadding = Random.Range(0f, 0.3f);
        break;
      }
      // TODO have it so CIRCLE can handle odd numbers.
      case Patterns.Circle:
      case Patterns.Snakes:
      case Patterns.Diagonal:
      case Patterns.Steps: {
        var rowCol = new Tuple<int, int>(1, 1);
        while (rowCol.Item1 % 2 != 0 || rowCol.Item2 % 2 != 0) {
          spriteSize = Random.Range(0.1f, 1f);
          _borderPadding = spriteSize / 2;
          rowCol = GetNextColAndRow(ResizeListener.screenSizeInWorldCoords);
        }

        break;
      }
      default:
        throw new Exception("unhandled switch case");
    }

    UpdateGrid();
    InitSpritesWithMetaData();

    switch (pattern) {
      case Patterns.Circles:
      case Patterns.Default:
      case Patterns.Zigzag:
      case Patterns.Circle:
      case Patterns.Snakes:
      case Patterns.Diagonal:
      case Patterns.Steps: {
        break;
      }
      case Patterns.Spiral: {
        ResetTargetPosTo(_instances, Vector2.zero);
        break;
      }
      case Patterns.Shake: {
        SetRandomTargetsInRadius(_instances, 0);
        break;
      }
      case Patterns.Explode: {
        ResetTargetPosTo(_instances, Vector2.zero);
        break;
      }
      case Patterns.Scatter: {
        SetRandomTargetsOnGrid(_instances);
        break;
      }
      default:
        throw new Exception("unhandled switch case");
    }

    Invoke(nameof(UpdateSize), getRandomInRange());
  }

  [EditorButton]
  void UpdateSpeedAndRotation() {
    RotationSpeed = Random.Range(-50f, 50f);
    MovementSpeedX = Random.Range(-2f, 2f);
    MovementSpeedY = Random.Range(-2f, 2f);
    Invoke(nameof(UpdateSpeedAndRotation), getRandomInRange());
  }


  void UpdateRange() {
    _rangeStart = Random.Range(0f, 20f);
    _rangeEnd = Random.Range(_rangeStart, 21f);
    Invoke(nameof(UpdateRange), 90);
  }


  void UpdateTargetColor() {
    _targetColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    Invoke(nameof(UpdateTargetColor), getRandomInRange());
  }

  void InitSpritesWithMetaData() {
    _spritesWithMetaData = new SpriteWithMetadata[sprites.Length];

    for (var i = 0; i < _spritesWithMetaData.Length; i++) {
      var sprite = sprites[i];
      float ratio = spriteSize /
                    Mathf.Sqrt(
                      sprite.bounds.size.x * sprite.bounds.size.x + sprite.bounds.size.y * sprite.bounds.size.y);
      _spritesWithMetaData[i] =
        new SpriteWithMetadata(sprite, new Vector2(sprite.bounds.size.x * ratio, sprite.bounds.size.y * ratio));
    }

    AfterResize(ResizeListener.screenSizeInWorldCoords);
  }


  void SetMode() {
    if (_overrideRandomWith == null) {
      if (Random.value > 0.5f && Combos.Length > 0) {
        _overrideRandomWith = Combos[Random.Range(0, Combos.Length)].childrens.ToArray();
      }
      else {
        _overrideRandomWith = new[] {getRandomSpriteWithMeteData().sprite};
      }
    }
    else {
      _overrideRandomWith = null;
    }

    Invoke(nameof(SetMode), getRandomInRange() * 2);
  }

  void SetPattern() {
    var values = Enum.GetValues(typeof(Patterns));
    pattern = (Patterns) values.GetValue(Random.Range(0, values.Length));
  }

  void ChangeColor() {
    _color = Color.Lerp(_color, _targetColor, Time.deltaTime);
    foreach (var i in _instances) {
      i.spriteRenderer.color = _color;
    }
  }

  Tuple<int, int> GetNextColAndRow(Vector2 screenSizeInWorldCoords) {
    var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / GetFullSize());
    var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / GetFullSize());
    switch (pattern) {
      case Patterns.Default:
      case Patterns.Diagonal:
      case Patterns.Zigzag:
      case Patterns.Snakes:
      case Patterns.Steps: {
        nextColCount++;
        nextRowCount++;
        break;
      }
      case Patterns.Spiral:
      case Patterns.Circles:
      case Patterns.Shake:
      case Patterns.Explode:
      case Patterns.Scatter:
      case Patterns.Circle: {
        break;
      }
      default:
        throw new Exception("unhandled switch case");
    }

    return new Tuple<int, int>(nextColCount, nextRowCount);
  }

  void AfterResize(Vector2 screenSizeInWorldCoords) {
    var nextColRow = GetNextColAndRow(screenSizeInWorldCoords);
    _seed = Random.Range(0, 10);
    _colCount = nextColRow.Item1;
    _rowCount = nextColRow.Item2;

    var nextSize = _colCount * _rowCount;

    if (nextSize < _instances.Length) {
      for (var i = nextSize; i < _instances.Length; i++) {
        Destroy(_instances[i].spriteRenderer.gameObject);
      }
    }

    var newInstances = new Instance[_colCount * _rowCount];
    for (var i = 0; i < newInstances.Length; i++) {
      if (_instances.Length > i) {
        newInstances[i] = _instances[i];
        var sr = _instances[i].spriteRenderer.GetComponent<SpriteRenderer>();
        sr.size = getMetadata(sr.sprite).sizeData;
      }
      else {
        newInstances[i] = new Instance(new GameObject("Sprite" + i).AddComponent<SpriteRenderer>());
        newInstances[i].spriteRenderer.transform.parent = transform;
        newInstances[i].spriteRenderer.color = _color;
      }
    }


    _instances = new Instance[_colCount * _rowCount];
    _instances = newInstances;

    var initialPos = _mainCam.ViewportToWorldPoint(new Vector3(0, 0, _mainCam.nearClipPlane));
    initialPos.z = 0;
    transform.position = initialPos;
    var currCol = 0;
    var currRow = 0;
    foreach (var instance in newInstances) {
      ConfigSpriteRenderer(instance.spriteRenderer);
      instance.spriteRenderer.transform.position = new Vector3(
        currCol * GetFullSize() + GetFullSize() / 2 + initialPos.x,
        currRow * GetFullSize() + GetFullSize() / 2 + initialPos.y, 0);
      instance.spriteRenderer.transform.rotation = _currentRotation;
      currCol = (currCol + 1) % _colCount;
      if (currCol == 0) {
        currRow = (currRow + 1) % _rowCount;
      }
    }
  }

  void ConfigSpriteRenderer(SpriteRenderer instance) {
    instance.drawMode = SpriteDrawMode.Sliced;
    var spriteWithMeteData = getRandomSpriteWithMeteData();
    instance.sprite = spriteWithMeteData.sprite;
    instance.size = spriteWithMeteData.sizeData;
    instance.sortingOrder = orderInLayer;
  }

  void Update() {
    Transform t;
    var colCounter = 0;
    var rowCounter = 0;
    var index = 0;
    foreach (var instance in _instances) {
      t = instance.spriteRenderer.transform;

      t.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
      _currentRotation = t.rotation;
      switch (pattern) {
        case Patterns.Default:
          DefaultPattern(t);
          break;
        case Patterns.Zigzag:
          ZigZagPattern(t);
          break;
        case Patterns.Diagonal:
          DiagonalPattern(t, colCounter, rowCounter);
          break;
        case Patterns.Steps:
          StepsPattern(t);
          break;
        case Patterns.Circle:
          Circle(t, instance);
          break;
        case Patterns.Explode:
          ExplodeMove(t, instance, rowCounter, colCounter);
          break;
        case Patterns.Scatter:
          ScatterMove(t, instance);
          break;
        case Patterns.Snakes:
          SnakesPattern(t, instance);
          break;
        case Patterns.Shake:
          ShakeMove(t, instance, rowCounter, colCounter);
          break;
        case Patterns.Circles:
          CirclesMove(t, colCounter, rowCounter, index);
          break;
        case Patterns.Spiral:
          SpiralMove(t, instance, colCounter, rowCounter, index);
          break;
        default:
          throw new Exception("unhandled case");
      }

      colCounter++;
      index++;
      if (colCounter == _colCount) {
        rowCounter++;
        colCounter = 0;
      }

      switch (pattern) {
        case Patterns.Default:
        case Patterns.Diagonal:
        case Patterns.Zigzag:
        case Patterns.Snakes:
        case Patterns.Steps: {
          handleInstanceBounds(t, instance.spriteRenderer, GetFullSize());
          break;
        }
        case Patterns.Circles:
        case Patterns.Shake:
        case Patterns.Explode:
        case Patterns.Scatter:
        case Patterns.Spiral:
        case Patterns.Circle: {
          break;
        }
        default:
          throw new Exception("unhandled switch case");
      }
    }


    switch (pattern) {
      case Patterns.Default:
      case Patterns.Zigzag:
      case Patterns.Diagonal:
      case Patterns.Steps:
      case Patterns.Circle:
      case Patterns.Snakes:
      case Patterns.Shake:
      case Patterns.Spiral:
        break;
      case Patterns.Scatter:
        ScatterEnd(_instances);
        break;
      case Patterns.Explode:
        ExplodeEnd(_instances);
        break;
      case Patterns.Circles:
        CirclesEnd();
        break;
      default:
        throw new Exception("unhandled case");
    }

    ChangeColor();
    if (Input.anyKeyDown) {
      Screen.fullScreen = !Screen.fullScreen;
      AfterResize(ResizeListener.screenSizeInWorldCoords);
    }
  }

  void DefaultPattern(Transform t) {
    t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
  }

  void DiagonalPattern(Transform t, int colCounter, int rowCounter) {
    if (_seed % 2 == 0) {
      if (colCounter % 2 == 0) {
        t.position += new Vector3(Time.deltaTime * MovementSpeedX, -Time.deltaTime * MovementSpeedY, 0);
      }
      else {
        t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      }
    }
    else {
      if (rowCounter % 2 == 0) {
        t.position += new Vector3(-Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      }
      else {
        t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      }
    }
  }

  void ZigZagPattern(Transform t) {
    if (_seed % 2 == 0) {
      if (_grid.WorldToCell(t.position).x % 2 == 0) {
        t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      }
      else {
        t.position += new Vector3(Time.deltaTime * MovementSpeedX, -Time.deltaTime * MovementSpeedY, 0);
      }
    }
    else {
      if (_grid.WorldToCell(t.position).y % 2 == 0) {
        t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      }
      else {
        t.position += new Vector3(-Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      }
    }
  }

  void StepsPattern(Transform t) {
    var cur = _grid.WorldToCell(t.position);
    int curRow = cur.y;
    int curCol = cur.x;
    if (_seed % 2 == 0) {
      if ((curRow + curCol) % 2 == 0) {
        t.position += new Vector3(Time.deltaTime * MovementSpeedY, 0, 0);
      }
      else {
        t.position += new Vector3(0, Time.deltaTime * MovementSpeedY, 0);
      }
    }
    else {
      if ((curRow + curCol) % 2 == 0) {
        t.position -= new Vector3(Time.deltaTime * MovementSpeedY, 0, 0);
      }
      else {
        t.position -= new Vector3(0, Time.deltaTime * MovementSpeedY, 0);
      }
    }
  }

  void Circle(Transform t, Instance instance) {
    var cur = _grid.WorldToCell(t.position);
    int curRow = cur.y;
    int curCol = cur.x;

    int dFromLeft = curCol;
    int dFromRight = _colCount - curCol - 1;
    int dFromTop = _rowCount - curRow - 1;
    int dFromBottom = curRow;

    int col = Math.Min(dFromLeft, dFromRight);
    int row = Math.Min(dFromTop, dFromBottom);
    int n = Math.Min(col, row);

    float absSpeed = Mathf.Abs(MovementSpeedY);

    if (instance.leaving) {
      if (n % 2 == 0) {
        if (curCol == n && curRow != _rowCount - 1 - n) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }

        if (curCol == _colCount - 1 - n && curRow != n) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }

        if (curRow == n && curCol != n) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }

        if (curRow == _rowCount - 1 - n && curCol != _colCount - 1 - n) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }
      }
      else {
        if (curCol == n && curRow != n) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }

        if (curCol == _colCount - 1 - n && curRow != _rowCount - 1 - n) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }

        if (curRow == n && curCol != _colCount - n - 1) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }

        if (curRow == _rowCount - 1 - n && curCol != n) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }
      }

      var newCell = _grid.WorldToCell(t.position);
      if (newCell.x != cur.x || newCell.y != cur.y) {
        instance.leaving = false;
      }
    }
    else {
      t.position = Vector2.MoveTowards(t.position, _grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed);
      if (DidReach(t, _grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed)) {
        instance.leaving = true;
      }
    }
  }

  void SetRandomTargetsOnGrid(Instance[] instances) {
    ArrayList allGridPositions = new ArrayList();
    for (var i = 0; i < _colCount; i++) {
      for (var j = 0; j < _rowCount; j++) {
        allGridPositions.Add(new Vector3Int(i, j, 0));
      }
    }

    foreach (var instance in instances) {
      Vector3Int pos = (Vector3Int) allGridPositions[Random.Range(0, allGridPositions.Count)];
      instance.targetPos = _grid.GetCellCenterWorld(pos);
      allGridPositions.Remove(pos);
    }
  }

  void SetRandomTargetsInRadius(Instance[] instances, float radius) {
    foreach (var instance in instances) {
      instance.targetPos = instance.spriteRenderer.transform.position +
                           new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0);
    }
  }

  void ResetTargetPosTo(Instance[] instances, Vector2 to) {
    foreach (var i in instances) {
      i.targetPos = to;
    }
  }

  void ScatterMove(Transform t, Instance instance) {
    t.position = Vector2.Lerp(t.position, instance.targetPos, Math.Abs(Time.deltaTime * MovementSpeedY * 3));
  }

  void ExplodeMove(Transform t, Instance instance, int rowCounter, int colCounter) {
    t.position = Vector2.MoveTowards(t.position, instance.targetPos, Math.Abs(Time.deltaTime * MovementSpeedY * 3));
  }

  void ShakeMove(Transform t, Instance instance, int curRow, int curCol) {
    if (DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime * 10)) {
      if (!instance.leaving) {
        instance.targetPos = _grid.GetCellCenterWorld(new Vector3Int(curCol, curRow, 0));
      }
      else {
        instance.targetPos = instance.spriteRenderer.transform.position + new Vector3(
          Random.Range(-GetFullSize() / 2, GetFullSize() / 2), Random.Range(-GetFullSize() / 2, GetFullSize() / 2), 0);
      }

      instance.leaving = !instance.leaving;
    }

    t.position = Vector2.MoveTowards(t.position, instance.targetPos, Mathf.Abs(Time.deltaTime * MovementSpeedY * 3));
  }

  void CirclesMove(Transform t, int colCounter, int rowCounter, int index) {
    var r = GetFullSize() / 4;
    var x = r * Mathf.Cos(_angle + index);
    var y = r * Mathf.Sin(_angle + index);
    var center = _grid.GetCellCenterWorld(new Vector3Int(colCounter, rowCounter, 0));
    t.position = center + new Vector3(x, y, 0);
  }

  void SpiralMove(Transform t, Instance instance, int colCounter, int rowCounter, int index) {
    var r = colCounter + rowCounter;
    var x = r * Mathf.Cos(_angle + index);
    var y = r * Mathf.Sin(_angle + index);
    var center = _grid.GetCellCenterWorld(new Vector3Int(0, 0, 0));
    t.position = center + new Vector3(x, y, 0);
  }

  float _angle = 0;

  void CirclesEnd() {
    var angleInc = MovementSpeedY * Time.deltaTime * 5;
    _angle += angleInc;
  }

  void ScatterEnd(Instance[] instances) {
    foreach (var instance in instances) {
      if (!DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime * 10)) {
        return;
      }
    }

    SetRandomTargetsOnGrid(instances);
  }

  bool explosionStarting = false;

  void ExplodeEnd(Instance[] instances) {
    foreach (var instance in instances) {
      if (!DidReach(instance.spriteRenderer.transform, instance.targetPos, Time.deltaTime * 10)) {
        return;
      }
    }

    if (!explosionStarting) {
      SetRandomTargetsOnGrid(instances);
    }
    else {
      ResetTargetPosTo(instances, instances[Random.Range(0, instances.Length)].spriteRenderer.transform.position);
    }

    explosionStarting = !explosionStarting;
  }


  void SnakesPattern(Transform t, Instance instance) {
    var cur = _grid.WorldToCell(t.position);
    int curRow = Math.Abs(cur.y);
    int curCol = Math.Abs(cur.x);
    float absSpeed = Mathf.Abs(MovementSpeedY);

    if (instance.leaving) {
      if (_seed % 2 == 0) {
        if (curCol % 2 == 0 && curRow % 2 == 0) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }

        if (curCol % 2 == 1 && curRow % 2 == 0) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }

        if (curCol % 2 == 1 && curRow % 2 == 1) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }

        if (curCol % 2 == 0 && curRow % 2 == 1) {
          t.position += Vector3.up * (Time.deltaTime * absSpeed);
        }
      }
      else {
        if (curCol % 2 == 0 && curRow % 2 == 0) {
          t.position += Vector3.left * (Time.deltaTime * absSpeed);
        }

        if (curCol % 2 == 1 && curRow % 2 == 0) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }

        if (curCol % 2 == 1 && curRow % 2 == 1) {
          t.position += Vector3.right * (Time.deltaTime * absSpeed);
        }

        if (curCol % 2 == 0 && curRow % 2 == 1) {
          t.position += Vector3.down * (Time.deltaTime * absSpeed);
        }
      }

      var newCell = _grid.WorldToCell(t.position);
      if (newCell.x != cur.x || newCell.y != cur.y) {
        instance.leaving = false;
      }
    }
    else {
      t.position = Vector2.MoveTowards(t.position, _grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed);
      if (DidReach(t, _grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed)) {
        instance.leaving = true;
      }
    }
  }

  void handleInstanceBounds(Transform t, SpriteRenderer instance, float fullSize) {
    // var cur = _grid.WorldToCell(t.position);
    // if (cur.x >= colCount) {
    //   t.position = _grid.CellToWorld(new Vector3Int(-1, cur.y, 0));
    // } else if (cur.x < -1) {
    //   t.position = _grid.CellToWorld(new Vector3Int(colCount, cur.y, 0));
    // }

    // cur = _grid.WorldToCell(t.position);
    // if (cur.y >= rowCount) {
    //   t.position = _grid.CellToWorld(new Vector3Int(cur.x, -1, 0));
    // } else if (cur.y < -1) {
    //   t.position = _grid.CellToWorld(new Vector3Int(cur.x, rowCount, 0));
    // }
    if (t.position.x >= fullSize * _colCount / 2) {
      t.position -= new Vector3(fullSize * _colCount, 0, 0);
      ConfigSpriteRenderer(instance);
    }
    else if (t.position.y >= fullSize * _rowCount / 2) {
      t.position -= new Vector3(0, fullSize * _rowCount, 0);
      ConfigSpriteRenderer(instance);
    }
    else if (t.position.y <= -fullSize * _rowCount / 2) {
      t.position += new Vector3(0, fullSize * _rowCount, 0);
      ConfigSpriteRenderer(instance);
    }
    else if (t.position.x <= -fullSize * _colCount / 2) {
      t.position += new Vector3(fullSize * _colCount, 0, 0);
      ConfigSpriteRenderer(instance);
    }
  }

  private void OnDisable() {
    ResizeListener.onResize.AddListener(AfterResize);
  }

  bool DidReach(Transform t, Vector2 target, float minDistance) {
    return Vector2.Distance(t.position, target) < minDistance;
  }
}