using UnityEngine;
using System.Collections;

public class ClientSmoothSynchronisation : MonoBehaviour
{

  private float lastSynchronizationTime = 0f;
  private float syncDelay = 0f;
  private float syncTime = 0f;
  private Vector3 syncStartPosition = Vector3.zero;
  private Vector3 syncEndPosition = Vector3.zero;
  private Quaternion syncStartQ = Quaternion.identity;
  private Quaternion syncEndQ = Quaternion.identity;
  
  void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
  {
    Vector3 lSyncPosition = Vector3.zero;
    Vector3 lSyncVelocity = Vector3.zero;
    Vector3 lSyncScale = Vector3.zero;
    Quaternion lSyncRotation = Quaternion.identity;
    //Color lSyncColor = Color.black;
    
    if (stream.isWriting) {
      lSyncPosition = rigidbody.position;
      lSyncVelocity = rigidbody.velocity;
      lSyncRotation = rigidbody.rotation;
      lSyncScale = transform.localScale;
      //lSyncColor = renderer.material.color;
      
      stream.Serialize (ref lSyncPosition);
      stream.Serialize (ref lSyncVelocity);
      stream.Serialize (ref lSyncRotation);
      //NetworkManager.SerializeColor (stream, ref lSyncColor);
      stream.Serialize (ref lSyncScale);
    } else {
      stream.Serialize (ref lSyncPosition);
      stream.Serialize (ref lSyncVelocity);
      stream.Serialize (ref lSyncRotation);
      //NetworkManager.SerializeColor (stream, ref lSyncColor);
      stream.Serialize (ref lSyncScale);
      
      syncTime = 0f;
      syncDelay = Time.time - lastSynchronizationTime;
      lastSynchronizationTime = Time.time;
      
      syncEndPosition = lSyncPosition + lSyncVelocity * syncDelay;
      syncStartPosition = rigidbody.position;
      syncEndQ = lSyncRotation;
      syncStartQ = rigidbody.rotation;
      //rigidbody.rotation = lSyncRotation;
      //renderer.material.color = lSyncColor;
      transform.localScale = lSyncScale;
    }
  }
  
  void Awake ()
  {
    lastSynchronizationTime = Time.time;
  }

  // Use this for initialization
  void Start ()
  {
  
  }
  
  // Update is called once per frame
  void FixedUpdate ()
  {
    if (Network.isClient) {
      SyncedMovement ();
    }
  }

  private void SyncedMovement ()
  {
    syncTime += Time.deltaTime;
    rigidbody.position = Vector3.Lerp (syncStartPosition, syncEndPosition, syncTime / syncDelay);
    rigidbody.rotation = Quaternion.Lerp (syncStartQ, syncEndQ, syncTime / syncDelay);
  }
}
