using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class CollisionSensor : MonoBehaviour {
    int _overlaps;

    public bool isOverlapping => _overlaps > 0;

    private void OnTriggerEnter2D(Collider2D other) {
        _overlaps++;
    }

    private void OnTriggerExit2D(Collider2D other) {
        _overlaps--;
    }
}