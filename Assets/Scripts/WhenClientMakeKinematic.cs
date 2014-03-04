using UnityEngine;
using System.Collections;

public class WhenClientMakeKinematic : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  if (Network.isClient) {
      rigidbody.isKinematic = true;
      collider.isTrigger = true;
    }
	}
}
