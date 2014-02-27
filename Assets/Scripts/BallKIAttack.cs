using UnityEngine;
using System.Collections;

public class BallKIAttack : MonoBehaviour {

  public float speed = 100.0f;
  public float maxSpeed = 600.0f;
  public string TargetTag = "Player";
  public float pauseTime = 60.0f;
  public float attackTime = 20.0f;
  protected GameObject fTarget = null;
  protected bool fAttack = false;
  protected float fAttackStartTime = 0;
  protected float fPauseStartTime = 0;

  // Use this for initialization
  void Start ()
  {
    fPauseStartTime = Time.time;
  }
  
  // Update is called once per frame
  void FixedUpdate ()
  {
    if (Network.isServer || (!Network.isServer && !Network.isClient)) {
      if (fTarget == null) {
        GameObject[] lTargets = GameObject.FindGameObjectsWithTag (TargetTag);
        if (lTargets.Length > 0) {
          float lminDistance = 10000.0f;
          foreach(GameObject lObj in lTargets) {
            float lDistance = (lObj.transform.position - transform.position).sqrMagnitude;
            if (lDistance < lminDistance) {
              fTarget = lObj;
            }
          }
        }
      }
      if (fTarget != null) {
        Vector3 lDir = fTarget.transform.position - transform.position;
        lDir.Normalize ();
        if (rigidbody.velocity.magnitude < maxSpeed) {
          rigidbody.AddForce (lDir * speed * Time.deltaTime);
        }
      }
      if (!fAttack && Time.time > (fPauseStartTime + pauseTime)) {
        fAttack = true;
        fAttackStartTime = Time.time;
        speed *= 10;
        maxSpeed *= 10;
      } else if (fAttack && Time.time > (fAttackStartTime + attackTime)) {
        speed /= 10;
        maxSpeed /= 10;
        fPauseStartTime = Time.time;
        fAttack = false;
      }
    }
  }
  
  void OnCollisionEnter (Collision collision)
  {
    if (collision.gameObject.tag == TargetTag && !fAttack) {
      fTarget = null;
    }
  }
}
