using UnityEngine;
using System.Collections;

public class FocusPlayer : MonoBehaviour
{

  public float maxDistance = 15.0f;
  public float groundHeight = 10.0f;
  public float smooth = 1.5f;
  public GameObject terrain = null;
  protected Transform fTarget = null;
  protected int playerIndex = 0;
  protected float lastTime = 0.0f;

  // Use this for initialization
  void Start ()
  {
    if (terrain == null) {
      terrain = GameObject.Find ("Terrain");
    }
  }
  
  void FixedUpdate ()
  {
    bool lFirst = false;

    float lScroll = Input.GetAxis ("Mouse ScrollWheel");
    if (lScroll != 0.0) {
      float lCameraDistance = maxDistance - lScroll;
      if (lCameraDistance < 2.0f) {
        lCameraDistance = 2.0f;
      } else if (lCameraDistance > 100.0f) {
        lCameraDistance = 100.0f;
      }
      maxDistance = lCameraDistance;
    }
    if (Network.isServer) {
      groundHeight = 100.0f;
      maxDistance = 10000.0f;
      transform.position = new Vector3 (0, 100.0f, 0);
      transform.LookAt (Vector3.zero);
      GameObject[] lPlayers = GameObject.FindGameObjectsWithTag ("Player");
      if (lPlayers.Length > 0) {
        if (playerIndex >= lPlayers.Length) {
          playerIndex = 0;
        }
        if (lastTime + 10.0f > Time.fixedTime) {
          playerIndex = Mathf.RoundToInt(Random.Range (0, lPlayers.Length));
          lastTime = Time.fixedTime;
        }
        fTarget = lPlayers[playerIndex].transform;
      }
    } else {
      if (fTarget == null) {
        lFirst = true;
        GameObject[] lPlayers = GameObject.FindGameObjectsWithTag ("Player");
        foreach (GameObject lP in lPlayers) {
          if ((!Network.isClient && !Network.isServer) || Network.player.Equals (lP.GetComponent<PlayerController> ().player)) {
            fTarget = lP.transform;
            break;
          }
        }
      }
    }
    if (fTarget != null) {
      Vector3 lView = fTarget.position - transform.position;
      float lDistance = Vector3.Magnitude (lView);
      if (lDistance > maxDistance || lFirst) {
        lView.Normalize ();
        Vector3 lPos = transform.position;
        lPos += lView * (lDistance - maxDistance) * 0.5f;
        float lGroundHeight = GetTerrainHeight (transform.position, fTarget.position.y);
        if (lGroundHeight < fTarget.position.y) {
          lGroundHeight = fTarget.position.y;
        }
        lPos.y = lGroundHeight + groundHeight;
        transform.position = Vector3.Lerp (transform.position, lPos, smooth * Time.deltaTime);
      }
      SmoothLookAt (fTarget);
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

  public float GetTerrainHeight (Vector3 aPosition, float aDefault)
  {
    float lHeight;
    if (terrain != null) {
      int lw = 1;
      int lh = 1;
      Terrain lTerrain = terrain.GetComponent<Terrain> ();
      Vector3 lOffset = aPosition - lTerrain.transform.position;
      lOffset.x += (lw / lTerrain.terrainData.size.x) / 2;
      lOffset.z += (lh / lTerrain.terrainData.size.z) / 2;
      int lx = Mathf.FloorToInt (((lOffset.x % lTerrain.terrainData.size.x) / lTerrain.terrainData.size.x) * lTerrain.terrainData.heightmapWidth);
      int ly = Mathf.FloorToInt (((lOffset.z % lTerrain.terrainData.size.z) / lTerrain.terrainData.size.z) * lTerrain.terrainData.heightmapHeight);
      //print (lTerrain.terrainData.size.ToString() + "/" + lOffset.ToString() + ":" + lx.ToString() + "," + ly.ToString());
      float[,] lHeights = lTerrain.terrainData.GetHeights (lx, ly, lw, lh);
      lHeight = terrain.transform.position.y + lHeights [0, 0] * lTerrain.terrainData.size.y;
      //lHeight = terrain.transform.position.y + (lHeight / lTerrain.terrainData.heightmapResolution) * lTerrain.terrainData.size.y;
      //Debug.Log("Terrain " + terrain.transform.position.y.ToString() + " " + lHeight.ToString() + " " + lTerrain.terrainData.heightmapResolution.ToString() + " " + lTerrain.terrainData.size.y.ToString() + " " + lHeights [0, 0].ToString());
    } else {
      lHeight = aDefault;
    }
    return lHeight;
  }
}
