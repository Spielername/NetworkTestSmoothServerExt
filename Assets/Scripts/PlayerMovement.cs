using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

  public float speed = 100;

  // Use this for initialization
  void Start ()
  {
  
  }

  protected float fHorizontal = 0.0f;
  protected float fVertical = 0.0f;
  protected Vector3 fForce = Vector3.zero;

  void FixedUpdate ()
  {
    if ((!Network.isClient && !Network.isServer)) {
      float lH = Input.GetAxis ("Horizontal");
      float lV = Input.GetAxis ("Vertical");
      DoInput (lH, lV);
    } else if (Network.isClient && gameObject.GetComponent<PlayerController>().player.Equals(Network.player)) {
      float lH = Input.GetAxis ("Horizontal");
      float lV = Input.GetAxis ("Vertical");
      Vector3 lF = CalcForce(lH, lV);
      //networkView.RPC ("SetInput", RPCMode.Server, lH, lV);
      networkView.RPC("AddForce", RPCMode.Server, lF);
    } else if (Network.isServer) {
      //DoInput (fHorizontal, fVertical);
      DoForce(fForce);
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
    return lForward + lSidewards;
  }
 
  public void DoInput (float aH, float aV)
  {
    Vector3 lForce = CalcForce(aH, aV);
    DoForce(lForce);
  }

  public void DoForce (Vector3 aF)
  {
    rigidbody.AddForce (aF);
  }
  
  [RPC]
  public void SetInput (float aH, float aV)
  {
    fHorizontal = aH;
    fVertical = aV;
  }

  [RPC]
  public void AddForce (Vector3 aForce)
  {
    fForce = aForce;
  }
}
