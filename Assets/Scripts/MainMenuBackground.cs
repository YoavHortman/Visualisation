using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

class SpriteWithMetedata {
  public Sprite sprite;
  public Vector2 sizeData;
  public SpriteRenderer spriteRenderer; 
  public SpriteWithMetedata(Sprite sprite, Vector2 sizeData, bool leaving) {
    this.sprite = sprite;
    this.sizeData = sizeData;
  }
}

class Instance {
  public bool leaving;
  public SpriteRenderer spriteRenderer;

  public Instance(SpriteRenderer spriteRenderer, bool leaving) {
    this.spriteRenderer = spriteRenderer;
    this.leaving = leaving;
  }
}

enum Patterns {
  DEFAULT,
  STEPS,
  ZIGZAG,
  DIAGONAL,
  CIRCLE,
}

public class MainMenuBackground : MonoBehaviour {
  [SerializeField] private Sprite[] sprites = new Sprite[0];
  [SerializeField] private float spriteSize = 5f;
  [SerializeField] private float RotationSpeed = 20;
  [SerializeField] private float MovementSpeedX = -2;
  [SerializeField] private float MovementSpeedY = -2;
  [SerializeField] private int orderInLayer = 0;

  private float borderPadding = 0f;
  private SpriteWithMetedata[] _spritesWithMetaData;
  private Instance[] instances;
  private Camera mainCam;
  private int rowCount;
  private int colCount;
  private Sprite[] overrideRandomWith;
  private Quaternion currentRotation;
  private Color color = new Color(1, 1, 1, 1);
  private Color targetColor = new Color(1, 1, 1, 1);
  private float rangeStart = 0;
  private float rangeEnd = 10;
  private Grid _grid;

  private Patterns pattern = Patterns.DEFAULT;


  [Serializable]
  struct Childrens {
    public List<Sprite> childrens;
  }

  [SerializeField] Childrens[] Combos;

  SpriteWithMetedata getRandomSpriteWithMeteData() {
    if (overrideRandomWith != null) {
      return getMetadata(overrideRandomWith[Random.Range(0, overrideRandomWith.Length)]);
    }

    return _spritesWithMetaData[Random.Range(0, _spritesWithMetaData.Length)];
  }

  SpriteWithMetedata getMetadata(Sprite sprite) {
    return _spritesWithMetaData.First(x => x.sprite == sprite);
  }

  float getRandomInRange() {
    return Random.Range(rangeStart, rangeEnd);
  }


  void Start() {
    instances = new Instance[] { };
    mainCam = Camera.main;
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
    _grid.cellGap = new Vector3(borderPadding, borderPadding, 0);
    
  }

  float GetFullSize() {
    return spriteSize + borderPadding;
  }

  [EditorButton]
  void UpdateSize() {
    SetPattern();
    switch (pattern) {
        case Patterns.DEFAULT:
        case Patterns.DIAGONAL:
        case Patterns.ZIGZAG: {
          spriteSize = Random.Range(0.1f, 10f);
          borderPadding = Random.Range(0f, 0.3f);
          break;
        }
        case Patterns.STEPS: {
          var rowCol = new Tuple<int, int>(1,1);
          while (rowCol.Item1 % 2 != 0 || rowCol.Item2 % 2 != 0) {
            spriteSize = Random.Range(0.1f, 1f);
            borderPadding = spriteSize / 2;
            rowCol = GetNextColAndRow(ResizeListener.screenSizeInWorldCoords);
          }
          break;
        }
        case Patterns.CIRCLE: {
          // TODO have it so it can handle odd numbers.
          var rowCol = new Tuple<int, int>(1,1);
          while (rowCol.Item1 % 2 != 0 || rowCol.Item2 % 2 != 0) {
            spriteSize = Random.Range(0.1f, 1f);
            borderPadding = spriteSize / 2;
            rowCol = GetNextColAndRow(ResizeListener.screenSizeInWorldCoords);
          }
          break;
        }
        default:
          throw new Exception("unhandled switch case");
      }
  
    UpdateGrid();
    InitSpritesWithMetaData();
    // Invoke(nameof(UpdateSize), getRandomInRange());
  }

  [EditorButton]
  void UpdateSpeedAndRotation() {
    RotationSpeed = Random.Range(-50f, 50f);
    MovementSpeedX = Random.Range(-2f, 2f);
    MovementSpeedY = Random.Range(-2f, 2f);
    // Invoke(nameof(UpdateSpeedAndRotation), getRandomInRange());
  }


  void UpdateRange() {
    rangeStart = Random.Range(0f, 20f);
    rangeEnd = Random.Range(rangeStart, 21f);
    Invoke(nameof(UpdateRange), 90);
  }


  void UpdateTargetColor() {
    targetColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
    Invoke(nameof(UpdateTargetColor), getRandomInRange());
  }

  void InitSpritesWithMetaData() {
    _spritesWithMetaData = new SpriteWithMetedata[sprites.Length];

    for (var i = 0; i < _spritesWithMetaData.Length; i++) {
      var sprite = sprites[i];
      float ratio = spriteSize / Mathf.Sqrt(sprite.bounds.size.x * sprite.bounds.size.x + sprite.bounds.size.y * sprite.bounds.size.y);
      _spritesWithMetaData[i] = new SpriteWithMetedata(sprite, new Vector2(sprite.bounds.size.x * ratio, sprite.bounds.size.y * ratio), true);
    }

    AfterResize(ResizeListener.screenSizeInWorldCoords);
  }


  void SetMode() {
    if (overrideRandomWith == null) {
      if (Random.value > 0.5f && Combos.Length > 0) {
        overrideRandomWith = Combos[Random.Range(0, Combos.Length)].childrens.ToArray();
      } else {
        overrideRandomWith = new[] {getRandomSpriteWithMeteData().sprite};
      }
    } else {
      overrideRandomWith = null;
    }

    Invoke(nameof(SetMode), getRandomInRange() * 2);
  }

  void SetPattern() {
    var values = Enum.GetValues(typeof(Patterns));
    pattern = (Patterns)values.GetValue(Random.Range(0, values.Length));
  }

  void ChangeColor() {
    color = Color.Lerp(color, targetColor, Time.deltaTime);
    foreach (var i in instances) {
      i.spriteRenderer.color = color;
    }
  }

  Tuple<int, int> GetNextColAndRow(Vector2 screenSizeInWorldCoords) {
    var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / GetFullSize());
    var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / GetFullSize());
    switch (pattern) {
        case Patterns.DEFAULT:
        case Patterns.DIAGONAL:
        case Patterns.ZIGZAG:
        case Patterns.STEPS: {
          nextColCount++;
          nextRowCount++;
          break;
        }
        case Patterns.CIRCLE: {
          break;
        }
        default:
          throw new Exception("unhandled switch case");
      }
    return new Tuple<int, int>(nextColCount, nextRowCount);
  }

  void AfterResize(Vector2 screenSizeInWorldCoords) {
    var nextColRow = GetNextColAndRow(screenSizeInWorldCoords);

    colCount = nextColRow.Item1;
    rowCount = nextColRow.Item2;

    var nextSize = colCount * rowCount;

    if (nextSize < instances.Length) {
      for (var i = nextSize; i < instances.Length; i++) {
        Destroy(instances[i].spriteRenderer.gameObject);
      }
    }
    var newInstances = new Instance[colCount * rowCount];
    for (var i = 0; i < newInstances.Length; i++) {
      if (instances.Length > i) {
        newInstances[i] = instances[i];
        var sr = instances[i].spriteRenderer.GetComponent<SpriteRenderer>();
        sr.size = getMetadata(sr.sprite).sizeData;
      } else {
        newInstances[i] = new Instance(new GameObject("Sprite" + i).AddComponent<SpriteRenderer>(), false);
        newInstances[i].spriteRenderer.transform.parent = transform;
        newInstances[i].spriteRenderer.color = color;
      }
    }


    instances = new Instance[colCount * rowCount];
    instances = newInstances;

    var initialPos = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
    transform.position = initialPos;
    var currCol = 0;
    var currRow = 0;
    foreach (var instance in newInstances) {
      ConfigSpriteRenderer(instance.spriteRenderer);
      instance.spriteRenderer.transform.position = new Vector3(currCol * GetFullSize() + GetFullSize() / 2 + initialPos.x, currRow * GetFullSize() + GetFullSize() / 2 + initialPos.y, 0);
      instance.spriteRenderer.transform.rotation = currentRotation;
      currCol = (currCol + 1) % colCount;
      if (currCol == 0) {
        currRow = (currRow + 1) % rowCount;
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
    foreach (var instance in instances) {
      t = instance.spriteRenderer.transform;

      t.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
      currentRotation = t.rotation;
      switch (pattern) {
        case Patterns.DEFAULT:
          DefaultPattern(t);
          break;
        case Patterns.ZIGZAG:
          ZigZagPattern(t);
          break;
        case Patterns.DIAGONAL:
          DiagonalPattern(t, colCounter, rowCounter);
          break;
        case Patterns.STEPS:
          StepsPattern(t);
          break;
        case Patterns.CIRCLE:
          Circle(t, instance);
          break;
        default:
         throw new Exception("unhandled case");
      }

      colCounter++;
      if (colCount == (colCounter / (rowCounter + 1))) {
        rowCounter++;
      }
      switch (pattern) {
        case Patterns.DEFAULT:
        case Patterns.DIAGONAL:
        case Patterns.ZIGZAG:
        case Patterns.STEPS: {
          handleInstanceBounds(t, instance.spriteRenderer, GetFullSize());
          break;
        }
        case Patterns.CIRCLE: {
          break;
        }
        default:
          throw new Exception("unhandled switch case");
      }
    }

    void DefaultPattern(Transform t) {
      t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
    }

    void DiagonalPattern(Transform t, int colCounter, int rowCounter) {
      if (colCount % 2 == 0) {
        if (colCounter % 2 == 0) {
            t.position += new Vector3(Time.deltaTime * MovementSpeedX, -Time.deltaTime * MovementSpeedY, 0);  
        } else {
            t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
        }
      } else {
        if (rowCounter % 2 == 0) {
            t.position += new Vector3(-Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
        } else {
            t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
        }
      }
    }

    void ZigZagPattern(Transform t) {
      if (colCount % 2 == 0) {
        if (_grid.WorldToCell(t.position).x % 2 == 0) {
          t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);  
        } else {
          t.position += new Vector3(Time.deltaTime * MovementSpeedX, -Time.deltaTime * MovementSpeedY, 0);
        }
      } else {
        if (_grid.WorldToCell(t.position).y % 2 == 0) {
          t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);  
        } else {
          t.position += new Vector3(-Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
        }
      }
    }

  void StepsPattern(Transform t) {
    var cur = _grid.WorldToCell(t.position);
    int curRow = cur.y;
    int curCol = cur.x;
    if (spriteSize % 2 == 0) {
      if ((curRow + curCol) % 2 == 0) {
        t.position += new Vector3(Time.deltaTime * MovementSpeedY, 0, 0);  
      } else {
        t.position += new Vector3(0, Time.deltaTime * MovementSpeedY, 0);  
      }
    } else {
      if ((curRow + curCol) % 2 == 0) {
        t.position -= new Vector3(Time.deltaTime * MovementSpeedY, 0, 0);
      } else {
        t.position -= new Vector3(0, Time.deltaTime * MovementSpeedY, 0);  
      }
    }
  }

  void Circle(Transform t, Instance instance) {
    var cur = _grid.WorldToCell(t.position);
    int curRow = cur.y;
    int curCol = cur.x;

    int dFromLeft = curCol;
    int dFromRight = colCount - curCol - 1;
    int dFromTop = rowCount - curRow - 1;
    int dFromBottom = curRow;


    int middleRow = rowCount / 2;
    
    // if (dFromTop == dFromBottom && dFromRight == dFromLeft) {
    //   return;
    // }§

    int col = Math.Min(dFromLeft, dFromRight);
    int row = Math.Min(dFromTop, dFromBottom);
    int n = Math.Min(col, row);

    float absSpeed = Mathf.Abs(MovementSpeedY);

    if (instance.leaving) {
      if (n % 2 == 0) {
        if (curCol == n && curRow != rowCount - 1 - n) {
          t.position += Vector3.up * Time.deltaTime * absSpeed;
        } 
        if (curCol == colCount - 1 - n && curRow != n) {
          t.position += Vector3.down * Time.deltaTime * absSpeed;
        } 
        if (curRow == n && curCol != n) {
          t.position += Vector3.left * Time.deltaTime * absSpeed;
        } 
        if (curRow == rowCount - 1 - n && curCol != colCount - 1 - n) {
          t.position += Vector3.right * Time.deltaTime * absSpeed;
        }
      } else {
        if (curCol == n && curRow != n) {
          t.position += Vector3.down * Time.deltaTime * absSpeed;
        } 
        if (curCol == colCount - 1 - n && curRow != rowCount - 1 - n) {
          t.position += Vector3.up * Time.deltaTime * absSpeed;
        } 
        if (curRow == n && curCol != colCount - n - 1) {
          t.position += Vector3.right * Time.deltaTime * absSpeed;
        } 
        if (curRow == rowCount - 1 - n && curCol !=  n) {
          t.position += Vector3.left * Time.deltaTime * absSpeed;
        }
      }

      var newCell = _grid.WorldToCell(t.position);
      if (newCell.x != cur.x || newCell.y != cur.y) {
        instance.leaving = false;  
      }
    } else {
      t.position = Vector2.MoveTowards(t.position, _grid.GetCellCenterWorld(cur), Time.deltaTime * absSpeed);
      if (Vector2.Distance(t.position, _grid.GetCellCenterWorld(cur)) < Time.deltaTime * absSpeed) {
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
      if (t.position.x >= fullSize * colCount / 2) {
        t.position -= new Vector3(fullSize * colCount, 0, 0);
        ConfigSpriteRenderer(instance);
      } else if (t.position.y >= fullSize * rowCount / 2) {
        t.position -= new Vector3(0, fullSize * rowCount, 0);
        ConfigSpriteRenderer(instance);
      } else if (t.position.y <= -fullSize * rowCount / 2) {
        t.position += new Vector3(0, fullSize * rowCount, 0);
        ConfigSpriteRenderer(instance);
      } else if (t.position.x <= -fullSize * colCount / 2) {
        t.position += new Vector3(fullSize * colCount, 0, 0);
        ConfigSpriteRenderer(instance);
      }
    }

    ChangeColor();
    if (Input.anyKeyDown) {
      Screen.fullScreen = !Screen.fullScreen;
      AfterResize(ResizeListener.screenSizeInWorldCoords);
    }
  }

  private void OnDisable() {
    ResizeListener.onResize.AddListener(AfterResize);
  }
}