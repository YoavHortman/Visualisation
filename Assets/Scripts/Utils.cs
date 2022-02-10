using System.IO;
using UnityEngine;

public static class Utils {
  public static Vector3 GetWorldPointFromScreenPoint(Vector3 screenPoint, float elevation) {
    Ray ray = Camera.main.ScreenPointToRay(screenPoint);
    Plane plane = new Plane(Vector3.back, new Vector3(0, 0, elevation));
    float distance;
    if (plane.Raycast(ray, out distance)) {
      return ray.GetPoint(distance);
    }

    return Vector3.zero;
  }

  public static Vector3 WorldToLocal(Vector3 position, Transform myTransform) {
    Vector3 scaleInvert = myTransform.localScale;
    scaleInvert = new Vector3(1f / scaleInvert.x, 1f / scaleInvert.y, 1f / scaleInvert.z);
    return Vector3.Scale(Quaternion.Inverse(myTransform.rotation) * (position - myTransform.position), scaleInvert);
  }

  public static Vector3 LocalToWorld(Vector3 position, Transform myTransform) {
    return myTransform.position + myTransform.rotation * (Vector3.Scale(position, myTransform.localScale));
  }

  public static void RecursiveSetLayer(Transform transform, string layer) {
    var children = transform.GetComponentsInChildren<Transform>(true);
    for (int i = 0; i < children.Length; i++) {
      children[i].gameObject.layer = LayerMask.NameToLayer(layer);
    }
  }

  public static void RecursiveSetIsKinematic(Transform transform, bool isKinematic) {
    var children = transform.GetComponentsInChildren<Rigidbody2D>(true);
    for (int i = 0; i < children.Length; i++) {
      children[i].isKinematic = isKinematic;
    }
  }

  public static void RecursiveSetIsTrigger(Transform transform, bool isTrigger) {
    var children = transform.GetComponentsInChildren<Collider2D>(true);
    for (int i = 0; i < children.Length; i++) {
      if (!children[i].CompareTag("NeverTrigger")) {
        children[i].isTrigger = isTrigger;
      }
    }
  }

  public static Vector3 SumParentScale(Transform transform) {
    var parent = transform.parent;
    var sum = Vector3.one;
    while (parent != null) {
      sum = Vector3.Scale(sum, parent.transform.localScale);
      parent = parent.parent;
    }

    return sum;
  }

  public static void LookAt2D(Transform toRotate, Transform target) {
    Vector3 relative = toRotate.InverseTransformPoint(target.position);
    toRotate.Rotate(0, 0, -Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg + 90);
  }


  // The following line returns half the screen in world point, regardless of camera: (it will return the cam height + cam pos.y)
  // mainCam.ScreenToWorldPoint(new Vector3(mainCam.pixelWidth, mainCam.pixelHeight, mainCam.nearClipPlane));
  // For this game the camera can be placed anywhere so this implementation is more stable:
  public static Vector2 CalculateScreenSizeInWorldCoords(Camera mainCam) {
    var p1 = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
    var p2 = mainCam.ViewportToWorldPoint(new Vector3(1, 0, mainCam.nearClipPlane));
    var p3 = mainCam.ViewportToWorldPoint(new Vector3(1, 1, mainCam.nearClipPlane));

    var width = (p2 - p1).magnitude;
    var height = (p3 - p2).magnitude;
    return new Vector2(width / 2, height / 2);
  }

  public static Vector2 GetScreenCenter(Camera camera) {
    return camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.transform.position.z));
  }

  public static Vector2 GetScaleToFillScreen(SpriteRenderer renderer, Camera camera) {
    // get the sprite width in world space units
    float worldSpriteWidth = renderer.sprite.bounds.size.x;
    float worldSpriteHeight = renderer.sprite.bounds.size.y;

    // get the screen height & width in world space units
    float worldScreenHeight = camera.orthographicSize * 2;
    float worldScreenWidth = (worldScreenHeight / camera.pixelHeight) * camera.pixelWidth;

    return new Vector2(worldScreenWidth / worldSpriteWidth, worldScreenHeight / worldSpriteHeight);
  }


  public static Texture2D LoadTexture(string FilePath) {
    if (File.Exists(FilePath)) {
      byte[] fileData = File.ReadAllBytes(FilePath);
      Texture2D tex = new Texture2D(2, 2);
      if (tex.LoadImage(fileData)) {
        return tex;
      }
    }

    return null;
  }

  public static bool IsTouchingScreen() {
    return Input.GetMouseButton(0) || Input.touchCount > 0;
  }
}