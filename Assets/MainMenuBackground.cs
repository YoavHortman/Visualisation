using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainMenuBackground : MonoBehaviour {
  [SerializeField] private Sprite[] sprites = new Sprite[0];
  [SerializeField] private float spriteSize = 5f;
  [SerializeField] private float borderPadding = 0.5f;
  [SerializeField] private float RotationSpeed = 20;
  [SerializeField] private float MovementSpeedX = -2;
  [SerializeField] private float MovementSpeedY = -2;
  private float fullSize;
  private Tuple<Sprite, Vector2>[] _spritesWithMetaData;
  private SpriteRenderer[] instances;
  private Camera mainCam;
  private int rowCount;
  private int colCount;
  private Sprite[] overrideRandomWith;
  private Quaternion currentRotation;
  private Color color = new Color(1, 1, 1, 1);
  private Color targetColor = new Color(1, 1, 1, 1);
  private float rangeStart = 0;
  private float rangeEnd = 10;


  [Serializable]
  struct Childrens{ public List<Sprite> childrens; }
 
  [SerializeField] Childrens[] Combos;
  // [SerializeField] private Sprite[][] combos;

  Tuple<Sprite, Vector2> getRandomSpriteWithMeteData() {
    if (overrideRandomWith != null) {
      return getMetadata(overrideRandomWith[Random.Range(0, overrideRandomWith.Length)]);
    }
    return _spritesWithMetaData[Random.Range(0, _spritesWithMetaData.Length)];
  }

  Tuple<Sprite, Vector2> getMetadata(Sprite sprite) {
    return _spritesWithMetaData.First(x => x.Item1 == sprite);
  }

  float getRandomInRange() {
    return Random.Range(rangeStart, rangeEnd);
  }
  
  
  void Start() {
    instances = new SpriteRenderer[]{};
    mainCam = Camera.main;
    InitSpritesWithMetaData();
    ResizeListener.onResize.AddListener(AfterResize);
    Invoke(nameof(UpdateTargetColor), getRandomInRange());
    Invoke(nameof(UpdateRange), 15);
    Invoke(nameof(UpdateSize), getRandomInRange());
    Invoke(nameof(UpdateSpeedAndRotation), getRandomInRange());
    Invoke(nameof(SetMode), getRandomInRange());
  }

  void UpdateSize() {
    spriteSize = Random.Range(0.1f, 10f);
    borderPadding = Random.Range(0f, 1f);
    InitSpritesWithMetaData();
    Invoke(nameof(UpdateSize), getRandomInRange());
  }

  void UpdateSpeedAndRotation() {
    RotationSpeed = Random.Range(-50f, 50f);
    MovementSpeedX = Random.Range(-2f, 2f);
    MovementSpeedY = Random.Range(-2f, 2f);
    Invoke(nameof(UpdateSpeedAndRotation), getRandomInRange());
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
    fullSize = spriteSize + borderPadding;
    _spritesWithMetaData = new Tuple<Sprite, Vector2>[sprites.Length];

    for (var i = 0; i < _spritesWithMetaData.Length; i++) {
      var sprite = sprites[i];
      float aspectX = sprite.bounds.size.x / sprite.bounds.size.y;
      float ySize = spriteSize;
      while (aspectX > 1) {
        aspectX /= 1.01f;
        ySize /= 1.01f;
      }

      _spritesWithMetaData[i] = new Tuple<Sprite, Vector2>(sprite, new Vector2(spriteSize * aspectX, ySize));
    }
    AfterResize(ResizeListener.screenSizeInWorldCoords);
  }

  
  void SetMode() {
    if (overrideRandomWith == null) {
      if (Random.value > 0.7f) {
        overrideRandomWith = Combos[Random.Range(0, Combos.Length)].childrens.ToArray();
      } else {
        if (Random.value > 0.7f) {
          overrideRandomWith = new[] {getRandomSpriteWithMeteData().Item1};
        } else {
          overrideRandomWith = new[] {getRandomSpriteWithMeteData().Item1, getRandomSpriteWithMeteData().Item1};
        }
      }
    } else {
      overrideRandomWith = null;
    }
    
    Invoke(nameof(SetMode), getRandomInRange() * 2);
  }
  
  void ChangeColor() {
    color = Color.Lerp(color, targetColor, Time.deltaTime);
    foreach (var i in instances) {
      i.color = color;
    }
  }

  void AfterResize(Vector2 screenSizeInWorldCoords) {
    var nextColCount = Mathf.CeilToInt(screenSizeInWorldCoords.x * 2 / fullSize);
    var nextRowCount = Mathf.CeilToInt(screenSizeInWorldCoords.y * 2 / fullSize);
    nextColCount++;
    nextRowCount++;
    
    if (nextColCount != colCount || nextRowCount != rowCount) {
      colCount = nextColCount;
      rowCount = nextRowCount;
    }

    var nextSize = colCount * rowCount;

    if (nextSize < instances.Length) {
      for (var i = nextSize; i < instances.Length; i++) {
        Destroy(instances[i]);
      }
    }

    var newInstances = new SpriteRenderer[colCount * rowCount];
    for (var i = 0; i < newInstances.Length; i++) {
      if (instances.Length > i) {
        newInstances[i] = instances[i];
        var sr = instances[i].GetComponent<SpriteRenderer>();
        sr.size = getMetadata(sr.sprite).Item2;
      } else {
        newInstances[i] = new GameObject("Sprite" + i).AddComponent<SpriteRenderer>();
        newInstances[i].transform.parent = transform;
        newInstances[i].color = color;
      }
    }
    instances = new SpriteRenderer[colCount * rowCount];
    instances = newInstances;

    var initialPos = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
    transform.position = initialPos;
    var currCol = 0;
    var currRow = 0;
    foreach (var instance in newInstances) {
      ConfigSpriteRenderer(instance);
      instance.transform.position = new Vector3(currCol * fullSize + initialPos.x, currRow * fullSize + initialPos.y, 0);
      instance.transform.rotation = currentRotation;
      currCol = (currCol + 1) % colCount;
      if (currCol == 0) {
        currRow = (currRow + 1) % rowCount;
      }
    }
  }

  void ConfigSpriteRenderer(SpriteRenderer instance) {
    instance.drawMode = SpriteDrawMode.Sliced;
    var spriteWithMeteData = getRandomSpriteWithMeteData();
    instance.sprite = spriteWithMeteData.Item1;
    instance.size = spriteWithMeteData.Item2;
  }

  void Update() {
    Transform t;
    foreach (var instance in instances) {
      t = instance.transform;
      t.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
      currentRotation = t.rotation;
      t.position += new Vector3(Time.deltaTime * MovementSpeedX, Time.deltaTime * MovementSpeedY, 0);
      if (t.position.x > fullSize * colCount / 2) {
        t.position -= new Vector3(fullSize * colCount, 0, 0);
        ConfigSpriteRenderer(instance);
      } else if (t.position.y > fullSize * rowCount / 2) {
        t.position -= new Vector3(spriteSize, fullSize * rowCount, 0);
        ConfigSpriteRenderer(instance);
      } else if (t.position.y < -fullSize * rowCount / 2) {
        t.position += new Vector3(spriteSize, fullSize * rowCount, 0);
        ConfigSpriteRenderer(instance);
      } else if (t.position.x < -fullSize * colCount / 2) {
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