using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Linq;

[RequireComponent(typeof(SpriteSkin))]
public class SoftBody : MonoBehaviour {
  [SerializeField] Rigidbody2D center;

  [SerializeField] float totalMass = 1;

  [SerializeField] float frequency = 5;

  [SerializeField] float dampingRatio = 1;

  void Awake() {
    Utils.RecursiveSetLayer(transform, "Water");
    var allBones = GetComponent<SpriteSkin>().boneTransforms;
    var softBones = allBones.Where(bone => !bone.CompareTag("NotSoft")).ToArray();
    var massPerItem = totalMass / (softBones.Length + 1);
    center.mass = massPerItem;
    for (int i = 0; i < softBones.Length; i++) {
      var bone = softBones[i];
      var lookAt = bone.gameObject.AddComponent<LookAt>();
      lookAt.target = center.transform;
      SetConnection(bone, softBones[(i + 1) % softBones.Length].GetComponent<Rigidbody2D>());
      SetConnection(bone, center);
      bone.GetComponent<Rigidbody2D>().mass = massPerItem;
    }
  }

  void SetConnection(Transform from, Rigidbody2D to) {
    var spring = from.gameObject.AddComponent<SpringJoint2D>();
    spring.connectedBody = to;
    spring.autoConfigureConnectedAnchor = true;
    spring.autoConfigureDistance = false;
    spring.distance = 0;
    spring.frequency = frequency;
    spring.dampingRatio = dampingRatio;


    spring.autoConfigureConnectedAnchor = false;
  }
}