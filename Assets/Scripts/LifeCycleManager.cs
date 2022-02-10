using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class LifeCycleManager : MonoBehaviour {
  private Vector2 _startPosition = Vector2.zero;
  private Rigidbody2D _firstRigidBodyChild;
  private LifeCycleManager _prefab;
  private List<Renderer> _renderers;
  private float timeUntilRespawn = 0.5f;
  private float _invisibleTime = 0;
  private bool _checkIsInScreen = false;
  public UnityAction afterJointBreak;

  public Rigidbody2D bone;

  [SerializeField] [Tooltip("Joint will break at this force")]
  int breakForce = 200;

  [SerializeField] [Tooltip("Marked by a circle, the base position of the anchor")]
  private Vector2 anchorPosition = Vector2.zero;

  public void Init(bool withInserter) {
    _prefab ??= Resources.Load<LifeCycleManager>("Stackables/" + gameObject.name.Replace("(Clone)", ""));
    if (_prefab is null) {
      Debug.LogWarning("The name of the object does not match its prefab: " + gameObject.name);
    }

    _firstRigidBodyChild = GetComponentInChildren<Rigidbody2D>();

    transform.position = bone.position;
    OnInserterDetached();
    _renderers = GetComponentsInChildren<SpriteRenderer>(true).Where(x => x.gameObject.CompareTag("Stackable"))
      .ToList<Renderer>();
    _renderers.AddRange(GetComponentsInChildren<MeshRenderer>(true)
      .Where(x => x.gameObject.CompareTag("Stackable")));
  }

  void Update() {
    if (_checkIsInScreen) {
      handleVisabillity();
    }
  }

  private void OnInserterDetached() {
    transform.parent = null;
  }

  private void OnJointBreak() {
    _checkIsInScreen = true;
    afterJointBreak?.Invoke();
  }

  void handleVisabillity() {
    foreach (Renderer renderer in _renderers) {
      if (renderer.isVisible) {
        _invisibleTime = 0;
        return;
      }
    }

    _invisibleTime += Time.deltaTime;
    if (_invisibleTime > timeUntilRespawn) {
      respawn();
    }
  }

  void respawn() {
    var obj = Instantiate<LifeCycleManager>(_prefab);
    obj._prefab = _prefab;
    obj.bone = bone;
    Destroy(gameObject);
  }
}