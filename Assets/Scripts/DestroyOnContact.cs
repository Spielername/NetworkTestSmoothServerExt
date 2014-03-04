using UnityEngine;
using System.Collections;

public class DestroyOnContact : MonoBehaviour {
  public string CollisionTag = "Target";
  public float delayTime = 2.0f;

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == CollisionTag) {
      DestroyObject(collider.gameObject, delayTime);
    }
  }
}
