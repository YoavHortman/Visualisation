using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public struct Sizes {
  public float spriteSize;
  public float borderPadding;
  public Sizes(float spriteSize, float borderPadding) {
    this.spriteSize = spriteSize;
    this.borderPadding = borderPadding;
  }

  public float Sum() {
    return spriteSize + borderPadding;
  }
  
  public Sizes Lerp(Sizes sizes, float t) {
    return new Sizes(Mathf.Lerp(spriteSize, sizes.spriteSize, t), Mathf.Lerp(borderPadding, sizes.borderPadding, t));
  }

  public bool IsEqual(Sizes sizes, float tolerance) {
    return Math.Abs(borderPadding - sizes.borderPadding) < tolerance && Math.Abs(spriteSize - sizes.spriteSize) < tolerance;
  }
}
public static class PatternUtils {
  public static IDictionary<string, BasePattern> allPatterns = new Dictionary<string, BasePattern> {
    {"circle", new Circle() },
    { "circles", new Circles() },
    { "default", new Default() },
    { "diagonal", new Diagonal() },
    { "explode", new Explode() },
    { "scatter", new Scatter() },
    { "shake", new Shake() },
    { "snakes", new Snakes() },
    { "steps", new Steps() },
    { "zigzag", new Zigzag() },
    { "fold", new Fold()}
  };

  public static void SetRandomTargetsInRadius(Instance[] instances, float radius) {
    foreach (var instance in instances) {
      instance.targetPos = instance.spriteRenderer.transform.position +
                           new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), 0);
    }
  }

  public static void ResetTargetPosTo(Instance[] instances, Vector2 to) {
    foreach (var i in instances) {
      i.targetPos = to;
    }
  }

  public static void SetRandomTargetsOnGrid(Instance[] instances, Grid grid, Vector2Int colRow) {
    ArrayList allGridPositions = new ArrayList();
    for (var i = 0; i < colRow.x; i++) {
      for (var j = 0; j < colRow.y; j++) {
        allGridPositions.Add(new Vector3Int(i, j, 0));
      }
    }

    foreach (var instance in instances) {
      Vector3Int pos = (Vector3Int) allGridPositions[Random.Range(0, allGridPositions.Count)];
      instance.targetPos = grid.GetCellCenterWorld(pos);
      allGridPositions.Remove(pos);
    }
  }
  
  public static bool DidReach(Transform t, Vector2 target, float minDistance) {
    return Vector2.Distance(t.position, target) < minDistance;
  }

  public static BasePattern GetRandomPattern() {
    // return allPatterns.ElementAt(Random.Range(0, allPatterns.Count)).Value;
    return allPatterns["fold"];
  }
}