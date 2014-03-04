using UnityEngine;
using System.Collections;

public class StayOnContact : MonoBehaviour {

  public string collisionTag = "Target";

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == collisionTag) {
      if (collider.rigidbody != null) {
        collider.rigidbody.velocity = Vector3.zero;
      }
    }
  }
}
