using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SpriteWithMetadata {
  public readonly Sprite sprite;
  public Vector2 sizeData;

  public SpriteWithMetadata(Sprite sprite, Vector2 sizeData) {
    this.sprite = sprite;
    this.sizeData = sizeData;
  }
}

public class Instance {
  // Indicates if currently leaving or entering a cell
  public bool leaving;
  public readonly SpriteRenderer spriteRenderer;
  public Vector2 targetPos;
  public SpriteWithMetadata spriteWithMetadata;

  public Instance(SpriteRenderer spriteRenderer, SpriteWithMetadata spw, int orderInLayer) {
    this.spriteRenderer = spriteRenderer;
    ConfigSpriteWithMetadata(spw, orderInLayer);
    leaving = true;
  }

  public void ConfigSpriteWithMetadata(SpriteWithMetadata spw, int orderInLayer) {
    spriteWithMetadata = spw;
    spriteRenderer.drawMode = SpriteDrawMode.Sliced;
    spriteRenderer.sortingOrder = orderInLayer;
    spriteRenderer.sprite = spriteWithMetadata.sprite;
    spriteRenderer.size = spriteWithMetadata.sizeData;
  }
}

class MainMenuBackground : MonoBehaviour {
  [SerializeField] protected Sprite[] sprites = Array.Empty<Sprite>();
  [SerializeField] protected float rotationSpeed = 20;
  [SerializeField] protected Vector2 movementSpeed = new(-2, -2);
  [SerializeField] protected int orderInLayer = 0;

  private Sizes _sizes;
  private Sizes _targetSizes;
  private SpriteWithMetadata[] _spritesWithMetaData;
  private Instance[] _instances;
  private Camera _mainCam;

  private Vector2Int _colRow;

  private Sprite[] _overrideRandomWith;
  private Quaternion _currentRotation;
  private Color _color = new(1, 1, 1, 1);
  private Color _targetColor = new(1, 1, 1, 1);
  private float _rangeStart = 0;
  private float _rangeEnd = 10;
  private Grid _grid;

  public static int seed;

  private BasePattern pattern;

  [Serializable]
  struct Childrens {
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
    pattern = new Default();
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
    _grid.cellSize = new Vector3(_sizes.spriteSize, _sizes.spriteSize, 0);
    _grid.cellGap = new Vector3(_sizes.borderPadding, _sizes.borderPadding, 0);
  }

  float GetFullSize() {
    return _sizes.Sum();
  }

  [EditorButton]
  void UpdateSizeEditor() {
    SetPattern();
    _sizes = pattern.GetSizes();

    UpdateGrid();
    UpdateSwm();

    pattern.AfterSizeUpdate(_instances, _colRow, _grid);
  }

  void UpdateSize() {
    SetPattern();
    _sizes = pattern.GetSizes();

    UpdateGrid();
    UpdateSwm();

    pattern.AfterSizeUpdate(_instances, _colRow, _grid);
    Invoke(nameof(UpdateSize), getRandomInRange());
  }

  [EditorButton]
  void UpdateSpeedAndRotation() {
    rotationSpeed = Random.Range(-50f, 50f);
    movementSpeed = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
    Invoke(nameof(UpdateSpeedAndRotation), getRandomInRange());
    // rotationSpeed = 0;
    // movementSpeed = Vector2.zero;
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
      float ratio = _sizes.spriteSize /
                    Mathf.Sqrt(
                      sprite.bounds.size.x * sprite.bounds.size.x + sprite.bounds.size.y * sprite.bounds.size.y);
      _spritesWithMetaData[i] =
        new SpriteWithMetadata(sprite, new Vector2(sprite.bounds.size.x * ratio, sprite.bounds.size.y * ratio));
    }

    AfterResize(ResizeListener.screenSizeInWorldCoords);
  }

  private void UpdateSwm() {
    for (var i = 0; i < _spritesWithMetaData.Length; i++) {
      var sprite = sprites[i];
      var ratio = _sizes.spriteSize /
                  Mathf.Sqrt(
                    sprite.bounds.size.x * sprite.bounds.size.x + sprite.bounds.size.y * sprite.bounds.size.y);
      _spritesWithMetaData[i].sizeData = new Vector2(sprite.bounds.size.x * ratio, sprite.bounds.size.y * ratio);
    }
  }


  void SetMode() {
    if (_overrideRandomWith == null) {
      if (Random.value > 0.5f && Combos.Length > 0) {
        _overrideRandomWith = Combos[Random.Range(0, Combos.Length)].childrens.ToArray();
      }
      else {
        _overrideRandomWith = new[] { getRandomSpriteWithMeteData().sprite };
      }
    }
    else {
      _overrideRandomWith = null;
    }

    Invoke(nameof(SetMode), getRandomInRange() * 2);
  }

  void SetPattern() {
    pattern = PatternUtils.GetRandomPattern();
  }

  void ChangeColor() {
    _color = Color.Lerp(_color, _targetColor, Time.deltaTime);
    foreach (var i in _instances) {
      i.spriteRenderer.color = _color;
    }
  }
  // TODO is this needed / rename if so
  private bool x = true;
  void AfterResize(Vector2 screenSizeInWorldCoords) {
    
    // TODO extract this, only needs to happen when SCREEN resizes
    var initialPos = _mainCam.ViewportToWorldPoint(new Vector3(0, 0, _mainCam.nearClipPlane));
    initialPos.z = 0;
    transform.position = initialPos;
    
    _colRow = pattern.GetNextColAndRow(screenSizeInWorldCoords, GetFullSize());
    seed = Random.Range(0, 10);

    var nextSize = _colRow.x * _colRow.y;
    Vector3 zero = Vector3.zero;
    if (x) {
      Array.Sort(_instances, (instance1, instance2) => {
        var pos1 = instance1.spriteRenderer.transform.position;
        var pos2 = instance2.spriteRenderer.transform.position;
        return pos1.x + pos1.y > pos2.x + pos2.y ? 1 : -1;
      });
    }
    if (_instances.Length > 0) {
      _instances[0].spriteRenderer.transform.position = Vector3.Lerp(_instances[0].spriteRenderer.transform.position,
        initialPos, Time.deltaTime);
      zero = _grid.GetCellCenterWorld(Vector3Int.zero) - _instances[0].spriteRenderer.transform.position;
    }

    if (_instances.Length > nextSize) {
      for (var i = nextSize; i < _instances.Length; i++) {
        Destroy(_instances[i].spriteRenderer.gameObject);
      }
    }

    var currCol = 0;
    var currRow = 0;

    var newInstances = new Instance[nextSize];
    for (var i = 0; i < newInstances.Length; i++) {
      if (_instances.Length > i) {
        _instances[i].spriteRenderer.size = _instances[i].spriteWithMetadata.sizeData;
        _instances[i].spriteRenderer.transform.position =
          _grid.GetCellCenterWorld(new Vector3Int(currCol, currRow, 0)) - zero;
        newInstances[i] = _instances[i];
      }
      else {
        newInstances[i] = new Instance(new GameObject("Sprite" + i).AddComponent<SpriteRenderer>(),
          getRandomSpriteWithMeteData(), orderInLayer);
        newInstances[i].spriteRenderer.transform.parent = transform;
        newInstances[i].spriteRenderer.color = _color;
        newInstances[i].spriteRenderer.transform.position =
          _grid.GetCellCenterWorld(new Vector3Int(currCol, currRow, 0)) - zero;
        newInstances[i].spriteRenderer.transform.rotation = _currentRotation;
      }

      currCol++;
      if (currCol == _colRow.x) {
        currRow++;
        currCol = 0;
      }
    }

    _instances = newInstances;
  }

  void Update() {
    if (!_sizes.IsEqual(_targetSizes, 0.1f)) {
      var nextSizes = _sizes.Lerp(_targetSizes, Time.deltaTime);
      _sizes = nextSizes;
      UpdateSwm();
      UpdateGrid();
      AfterResize(ResizeListener.screenSizeInWorldCoords);
      x = false;
      return;
    }

    x = true;

    Transform t;
    var curCol = 0;
    var curRow = 0;
    var index = 0;
    foreach (var instance in _instances) {
      t = instance.spriteRenderer.transform;

      t.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
      _currentRotation = t.rotation;
      pattern.Update(t, instance, curCol, curRow, index, GetFullSize(), _grid, _colRow, movementSpeed);
      if (pattern.GetShouldHandleInstanceBounds()) {
        handleInstanceBounds(t, instance, GetFullSize());
      }

      curCol++;
      index++;
      if (curCol == _colRow.x) {
        curRow++;
        curCol = 0;
      }
    }
    pattern.AfterUpdate(_instances, _grid, _colRow, movementSpeed);


    ChangeColor();
    if (Input.anyKeyDown) {
      Screen.fullScreen = !Screen.fullScreen;
      AfterResize(ResizeListener.screenSizeInWorldCoords);
    }
  }

  public void handleInstanceBounds(Transform t, Instance instance, float fullSize) {
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
    if (t.position.x >= fullSize * _colRow.x / 2) {
      t.position -= new Vector3(fullSize * _colRow.x, 0, 0);
      instance.ConfigSpriteWithMetadata(getRandomSpriteWithMeteData(), orderInLayer);
    }
    else if (t.position.y >= fullSize * _colRow.y / 2) {
      t.position -= new Vector3(0, fullSize * _colRow.y, 0);
      instance.ConfigSpriteWithMetadata(getRandomSpriteWithMeteData(), orderInLayer);
    }
    else if (t.position.y <= -fullSize * _colRow.y / 2) {
      t.position += new Vector3(0, fullSize * _colRow.y, 0);
      instance.ConfigSpriteWithMetadata(getRandomSpriteWithMeteData(), orderInLayer);
    }
    else if (t.position.x <= -fullSize * _colRow.x / 2) {
      t.position += new Vector3(fullSize * _colRow.x, 0, 0);
      instance.ConfigSpriteWithMetadata(getRandomSpriteWithMeteData(), orderInLayer);
    }
  }

  private void OnDisable() {
    ResizeListener.onResize.AddListener(AfterResize);
  }
}