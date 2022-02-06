using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ExtendToScreen : MonoBehaviour {
  public enum Orientation {
    Width,
    Height,
    Fill
  };

  [SerializeField] private Orientation extendTo = Orientation.Width;
  // [SerializeField]

  // TODO
  // [Tooltip("NOT YET IMPLEMENTED")]
  // private bool TODO_maintainAspectRatio = false;
  private Orientation lastOrientation;
  private SpriteRenderer _renderer;

  private Camera _camera;

  void Start() {
    _renderer = GetComponent<SpriteRenderer>();
    _camera = Camera.main;
    var fixScale = transform.localScale;
    fixScale.z = 1;
    transform.localScale = fixScale;
    Reposition(ResizeListener.screenSizeInWorldCoords);
    ResizeListener.onResize.AddListener(Reposition);
  }

  void Update() {
    if (lastOrientation != extendTo) {
      Reposition(ResizeListener.screenSizeInWorldCoords);
    }
  }


  private void Reposition(Vector2 _) {
    lastOrientation = extendTo;
    var dimensions = getDimensions(extendTo, transform, _renderer, Camera.main);

    transform.localScale = dimensions.Item1;
    transform.position = dimensions.Item2;
  }

  public static System.Tuple<Vector3, Vector2> getDimensions(Orientation orientation, Transform t,
    SpriteRenderer renderer, Camera camera) {
    Vector2 fullScreenScale = Utils.GetScaleToFillScreen(renderer, camera);
    Vector2 center = Utils.GetScreenCenter(camera);

    Vector3 newScale = t.localScale;
    Vector2 newPos = t.position;
    switch (orientation) {
      case Orientation.Width: {
        newPos.x = center.x;
        newScale.x = fullScreenScale.x;
        break;
      }
      case Orientation.Height: {
        newPos.y = center.y;
        newScale.y = fullScreenScale.y;
        break;
      }
      case Orientation.Fill: {
        newPos.x = center.x;
        newPos.y = center.y;
        newScale.x = fullScreenScale.x;
        newScale.y = fullScreenScale.y;
        break;
      }
    }

    return new System.Tuple<Vector3, Vector2>(newScale, newPos);
  }

  private void OnDisable() {
    ResizeListener.onResize.RemoveListener(Reposition);
  }
}