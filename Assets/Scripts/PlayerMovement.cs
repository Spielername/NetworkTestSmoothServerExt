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
  protected Vector3 fLastForce = Vector3.zero;

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
      DoForce (fForce);
    }
    /*
    float v = rigidbody.velocity.magnitude;
    if (v > 10.0f) {
      int anz = Mathf.RoundToInt(v / 2);
      for (int i = 0; i < anz; i++) {
        ParticleSystem ps = transform.FindChild ("Speedy").particleSystem;
        Vector3 d = rigidbody.velocity / 2.0f;
        Vector3 p = transform.position;
        float delta = 0.2f;
        p.y += -1.0f + Random.Range (-delta, delta);
        d.x += Random.Range (-delta, delta);
        d.y += Random.Range (-delta, delta);
        d.z += Random.Range (-delta, delta);
        ps.Emit (transform.position, d, 0.2f, 5.0f, new Color32(128,128,128,128));
        ParticleSystem.Particle p = new ParticleSystem.Particle();
        ps.
      }
    }
    */
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
    float v = (rigidbody.velocity + aF).magnitude;
    if (v > maxVelocity) {
      aF.Normalize ();
      float c = rigidbody.velocity.magnitude;
      if (c < maxVelocity) {
        aF = aF * (maxVelocity - c);
      } else {
        aF = Vector3.zero;
      }
      //Debug.Log("V" + v.ToString() + " c" + c.ToString() + " aF" + aF.ToString());
    } 
    rigidbody.AddForce (aF);
  }
  
  [RPC]
  public void AddForce (Vector3 aForce)
  {
    fForce = aForce;
  }

  /*
  void OnTriggerEnter(Collider collider) {
    if (collider.gameObject.tag == "Ground") {
      Terrain t = collider.gameObject.GetComponent<Terrain>();
      if (t != null) {
        Vector3 pos = collider.ClosestPointOnBounds( transform.position );
        DropDownTerrainAt(t, pos);
      }
    }
  }
  
  void OnCollisionEnter (Collision collision)
  {
    if (collision.gameObject.tag == "Ground") {
      Terrain t = collision.gameObject.GetComponent<Terrain>();
      if (t != null) {
        ContactPoint contact = collision.contacts [0];
        Vector3 pos = contact.point;
        DropDownTerrainAt(t, pos);
      }
    }
  }

  public void DropDownTerrainAt(Terrain lTerrain, Vector3 pos) {
    int lw = 2;
    int lh = 2;
    Vector3 lOffset = pos - lTerrain.transform.position;
    lOffset.x += ( lw / lTerrain.terrainData.size.x ) / 2;
    lOffset.z += ( lh / lTerrain.terrainData.size.z ) / 2;
    int lx = Mathf.FloorToInt( ( ( lOffset.x % lTerrain.terrainData.size.x ) / lTerrain.terrainData.size.x ) * lTerrain.terrainData.heightmapWidth );
    int ly = Mathf.FloorToInt( ( ( lOffset.z % lTerrain.terrainData.size.z ) / lTerrain.terrainData.size.z ) * lTerrain.terrainData.heightmapHeight );
    //print (lTerrain.terrainData.size.ToString() + "/" + lOffset.ToString() + ":" + lx.ToString() + "," + ly.ToString());
    float[,] lHeights = lTerrain.terrainData.GetHeights(lx,ly,lw,lh);
    for(int lxi = 0; lxi < lw; lxi++) {
      for(int lyi = 0; lyi < lh; lyi++) {
        lHeights[lxi,lyi] -= 0.00005f;
      }
    }
    lTerrain.terrainData.SetHeights(lx,ly, lHeights);
    float[,,] lMaps = lTerrain.terrainData.GetAlphamaps(lx,ly, lw, lh);
    for(int lxi = 0; lxi < lw; lxi++) {
      for(int lyi = 0; lyi < lh; lyi++) {
        //lMaps[lxi,lyi,4] = 1.0f;
        lMaps[lxi,lyi,3] = 1.0f;
        lMaps[lxi,lyi,2] = 0.0f;
        lMaps[lxi,lyi,1] = 0.0f;
        lMaps[lxi,lyi,0] = 0.0f;
      }
    }
    lTerrain.terrainData.SetAlphamaps(lx, ly, lMaps);
  }
  */

}
