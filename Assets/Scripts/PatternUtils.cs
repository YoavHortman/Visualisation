using System;
using System.Collections;
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
}
public static class PatternUtils {
  public static BasePattern[] allPatterns = {
    new Circle(),
    new Circles(),
    new Default(),
    new Diagonal(),
    new Explode(),
    new Scatter(),
    new Shake(),
    new Snakes(),
    new Steps(),
    new Zigzag(),
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
    return allPatterns[Random.Range(0, allPatterns.Length)];
  }
}