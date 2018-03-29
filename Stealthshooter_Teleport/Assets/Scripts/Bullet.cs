using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed = 100f;
	public int damage = 1;
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
			
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().TakeDamage (damage);
			} else if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<teleportTest> ().TakeDamage (damage);
			}
			Destroy (gameObject);
		}
	}
}
