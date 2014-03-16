using UnityEngine;
using System.Collections;

public class WhenClientMakeKinematic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  if (Network.isClient) {
      if (rigidbody != null) {
        rigidbody.isKinematic = true;
      }
      if (collider != null) {
        collider.isTrigger = true;
      }
    }
	}
}
