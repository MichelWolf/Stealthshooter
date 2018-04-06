using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed = 100f;
	public int damage = 1;
	public Vector3 direction;

	// Update is called once per frame
	void Update () 
	{
		float distThisFrame = speed * Time.deltaTime;
		//Wird pro Frame in direction bewegt
		transform.Translate (direction.normalized * distThisFrame, Space.World);
		Destroy (gameObject, 10);
	}

	void OnTriggerEnter(Collider other) {
		//Gun collider wird ignoriert, da die Kugel dort erstellt wird und dadurch manchmal am anfang schon im Collider stecken würde und verschwinden würde
		if (other.gameObject.name != "Gun") {
			//Wenn das getroffene Objekt ein Gegner ist, lass ihn Schaden nehmen
			if (other.gameObject.tag == "Enemy") {
				other.gameObject.GetComponent<Enemy> ().TakeDamage (damage);
			//Wenn das getroffene Objekt der Spieler ist, lass ihn Schaden nehmen
			} else if (other.gameObject.tag == "Player") {
				other.gameObject.GetComponent<Character> ().TakeDamage (damage);
			}
			//Zerstöre die Kugel on Collision
			Destroy (gameObject);
		}
	}
}
