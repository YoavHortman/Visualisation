using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class Emittable : MonoBehaviour
{
    public float lifeTime;
    [SerializeField] private Rigidbody2D applyInitialForceTo;

    private Vector2 getForceDirection()
    {
        var strength = Random.Range(3, 6);
        if (Random.value > 0.5f)
        {
            return Vector2.up * strength + Vector2.right * strength;
        }

        return Vector2.up * strength + Vector2.left * strength;
    }
    
    public void Emit(float strength)
    {
        applyInitialForceTo.AddForce(getForceDirection() * strength, ForceMode2D.Impulse);
        Destroy(gameObject, lifeTime);
    }
}