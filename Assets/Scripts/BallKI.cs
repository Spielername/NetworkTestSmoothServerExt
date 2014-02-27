using UnityEngine;
using System.Collections;

public class BallKI : MonoBehaviour
{
  public float speed = 100.0f;
  public float maxSpeed = 600.0f;
  public string TargetTag = "Target";
  protected GameObject fTarget = null;

  // Use this for initialization
  void Start ()
  {
  }
  
  // Update is called once per frame
  void FixedUpdate ()
  {
    if (Network.isServer || (!Network.isServer && !Network.isClient)) {
      if (fTarget == null) {
        GameObject[] lTargets = GameObject.FindGameObjectsWithTag (TargetTag);
        fTarget = lTargets [Mathf.RoundToInt (Random.Range (0, lTargets.Length))];
      }
      Vector3 lDir = fTarget.transform.position - transform.position;
      lDir.Normalize ();
      if (rigidbody.velocity.magnitude < maxSpeed) {
        rigidbody.AddForce (lDir * speed * Time.deltaTime);
      }
    }
  }

  void OnCollisionEnter (Collision collision)
  {
    if (collision.gameObject.tag == TargetTag) {
      fTarget = null;
    }
  }

}
