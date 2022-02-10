using UnityEngine;

public class LookAt : MonoBehaviour {
  [SerializeField] public Transform target;

  void Update() {
    Utils.LookAt2D(transform, target);
  }
}