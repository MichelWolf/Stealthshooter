using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour {

	//Sitzt auf einem EmptyGameObject mit SphereCollider
	//SphereCollider isTrigger = true;
	void OnTriggerEnter(Collider other)
	{
		//Wenn der Collider der entered der Spieler ist wird bei dem dieser Punkt als Respawnpoint gesetzt
		if (other.gameObject.tag == "Player") {

			other.gameObject.GetComponent<teleportTest> ().spawnpoint = this.gameObject;
		}
	}
}
