using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ResizeListener : MonoBehaviour {
  private Vector2 lastScreenSize;
  public static Vector2 screenSizeInWorldCoords;
  public static UnityEvent<Vector2> onResize = new UnityEvent<Vector2>();
  private Camera cam;

  void Start() {
    lastScreenSize = new Vector2();
    cam = GetComponent<Camera>();
    screenSizeInWorldCoords = Utils.CalculateScreenSizeInWorldCoords(cam);
  }

  void LateUpdate() {
    Vector2 screenSize = new Vector2(cam.pixelWidth, cam.pixelHeight);
    if (lastScreenSize != screenSize) {
      lastScreenSize = screenSize;
      screenSizeInWorldCoords = Utils.CalculateScreenSizeInWorldCoords(cam);
      onResize.Invoke(screenSizeInWorldCoords);
    }
  }
}

