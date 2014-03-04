using UnityEngine;
using System.Collections;

public class FocusPlayerServer : MonoBehaviour
{

  public float smooth = 1.5f;
  protected Transform fTarget = null;
  protected int playerIndex = 0;
  protected float lastTime = 0.0f;

  // Use this for initialization
  void Start ()
  {
  }
  
  void FixedUpdate ()
  {
    if (Network.isServer) {
      transform.position = new Vector3 (0, 100.0f, 0);
      transform.LookAt (Vector3.zero);
      GameObject[] lPlayers = GameObject.FindGameObjectsWithTag ("Player");
      if (lPlayers.Length > 0) {
        if (playerIndex >= lPlayers.Length) {
          playerIndex = 0;
        }
        if (lastTime + 10.0f > Time.fixedTime) {
          playerIndex = Mathf.RoundToInt (Random.Range (0, lPlayers.Length));
          lastTime = Time.fixedTime;
        }
        fTarget = lPlayers [playerIndex].transform;
      }
      if (fTarget != null) {
        SmoothLookAt (fTarget);
      }
    }
  }

  void SmoothLookAt (Transform lPlayer)
  {
    // Create a vector from the camera towards the player.
    Vector3 relPlayerPosition = lPlayer.position - transform.position;
    
    // Create a rotation based on the relative position of the player being the forward vector.
    Quaternion lookAtRotation = Quaternion.LookRotation (relPlayerPosition, Vector3.up);
    
    // Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
    transform.rotation = Quaternion.Lerp (transform.rotation, lookAtRotation, smooth * Time.deltaTime);
  }
}
