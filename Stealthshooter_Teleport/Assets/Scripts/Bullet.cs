using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed = 100f;
	public float damage = 1f;
	public Vector3 direction;
	public Vector3 target;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		float distThisFrame = speed * Time.deltaTime;
		//Debug.Log ("Getroffen");
		transform.Translate (direction.normalized * distThisFrame, Space.World);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.name != "Gun") {
			Destroy (gameObject);
		}
	}
}
