using UnityEngine;
using System.Collections;

public class ContactEffect : MonoBehaviour
{

  public GameObject effect = null;
  public float playTime = 1.01f;
  public string collisionTag = "";

  // Use this for initialization
  void Start ()
  {
    if (effect == null) {
      effect = GameObject.Find ("ContactEffect");
    }
  }

  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag != "Ground" && (collider.gameObject.tag == collisionTag || collisionTag.Equals(string.Empty))) {
      Vector3 pos = collider.ClosestPointOnBounds( transform.position );
      Quaternion rot = Quaternion.identity;
      if (effect != null) {
        Object lEffect = Instantiate (effect, pos, rot);
        Destroy (lEffect, playTime);
      }
    }
  }

  void OnCollisionEnter (Collision collision)
  {
    if (collision.gameObject.tag != "Ground" && (collider.gameObject.tag == collisionTag || collisionTag.Equals(string.Empty))) {
      ContactPoint contact = collision.contacts [0];
      Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
      Vector3 pos = contact.point;
      if (effect != null) {
        Object lEffect = Instantiate (effect, pos, rot);
        Destroy (lEffect, playTime);
      }
    }
  }
}
