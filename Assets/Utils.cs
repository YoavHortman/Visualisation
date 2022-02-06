using UnityEngine;

public static class Utils {
  public static Vector2 CalculateScreenSizeInWorldCoords(Camera mainCam) {
    var p1 = mainCam.ViewportToWorldPoint(new Vector3(0, 0, mainCam.nearClipPlane));
    var p2 = mainCam.ViewportToWorldPoint(new Vector3(1, 0, mainCam.nearClipPlane));
    var p3 = mainCam.ViewportToWorldPoint(new Vector3(1, 1, mainCam.nearClipPlane));

    var width = (p2 - p1).magnitude;
    var height = (p3 - p2).magnitude;
    return new Vector2(width / 2, height / 2);
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
  public static Vector2 GetScreenCenter(Camera camera) {
    return camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, camera.transform.position.z));
  }
}
