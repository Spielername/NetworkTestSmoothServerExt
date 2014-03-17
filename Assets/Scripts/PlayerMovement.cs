using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

  public float speed = 100;
  public float minSpeed = 50;
  public float maxSpeed = 700;
  public float maxVelocity = 50.0f;

  // Use this for initialization
  void Start ()
  {
  
  }

  protected Vector3 fForce = Vector3.zero;

  void FixedUpdate ()
  {
    if ((!Network.isClient && !Network.isServer)) {
      float lH = Input.GetAxis ("Horizontal");
      float lV = Input.GetAxis ("Vertical");
      DoForce (CalcForce (lH, lV));
    } else if (Network.isClient && gameObject.GetComponent<PlayerController> ().player.Equals (Network.player)) {
      float lH = Input.GetAxis ("Horizontal");
      float lV = Input.GetAxis ("Vertical");
      Vector3 lF = CalcForce (lH, lV);
      if (!lF.Equals(Vector3.zero)) {
        networkView.RPC ("AddForce", RPCMode.Server, lF);
      }
    } else if (Network.isServer) {
      if (!fForce.Equals(Vector3.zero)) {
        DoForce (fForce);
        fForce = Vector3.zero;
      }
    }
  }

  public Vector3 CalcForce (float aH, float aV)
  {
    GameObject lCam = GameObject.FindGameObjectWithTag ("MainCamera");
    Vector3 lForward = lCam.transform.forward;
    lForward.y = 0;
    lForward.Normalize ();
    Vector3 lSidewards = new Vector3 (lForward.z, 0, -lForward.x);
    lForward *= aV * speed * Time.deltaTime;
    lSidewards *= aH * speed * Time.deltaTime;
    Vector3 lForce = lForward + lSidewards;
    if (lForce.sqrMagnitude > 0) {
      speed = speed + 20;
      if (speed > maxSpeed) {
        speed = maxSpeed;
      }
    } else {
      speed = speed - 40;
      if (speed < minSpeed) {
        speed = minSpeed;
      }
    }
    return lForce;
  }
 
  public void DoForce (Vector3 aF)
  {
    ///*
    float v = (rigidbody.velocity + aF).magnitude;
    if (v > maxVelocity) {
      aF.Normalize ();
      float c = rigidbody.velocity.magnitude;
      if (c < maxVelocity) {
        aF = aF * (maxVelocity - c);
      } else {
        aF = Vector3.zero;
      }
    }
    //*/
    rigidbody.AddForce (aF);
  }
  
  [RPC]
  public void AddForce (Vector3 aForce)
  {
    fForce = aForce;
  }

}
