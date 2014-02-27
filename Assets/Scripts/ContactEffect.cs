using UnityEngine;
using System.Collections;

public class ContactEffect : MonoBehaviour
{

  public GameObject effect = null;

  // Use this for initialization
  void Start ()
  {
    if (effect == null) {
      effect = GameObject.Find ("ContactEffect");
    }
  }
  
  void OnCollisionEnter (Collision collision)
  {
    if (collision.gameObject.tag != "Ground") {
      ContactPoint contact = collision.contacts [0];
      Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
      Vector3 pos = contact.point;
      if (effect != null) {
        Object lEffect = Instantiate (effect, pos, rot);
        Destroy (lEffect, 1.01f);
      }
    }
  }
}
