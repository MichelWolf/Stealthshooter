using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCamera : MonoBehaviour {

	public bool isControllable = false;
	public GameObject player;

	public float smoothing = 5.0f;

	void Update(){
		if (isControllable) {		
			//Rotiere die Camera, wenn sie steuerbar ist
			this.gameObject.transform.parent.transform.eulerAngles += new Vector3(Input.GetAxisRaw("Mouse Y") * smoothing * -1, Input.GetAxisRaw("Mouse X") * smoothing, 0) * Time.deltaTime;
		}
	}
		
	void OnTriggerStay (Collider other)
	{
		//Solange sich ein Trigger-Collider in diesem Collider befindet
		if (other.gameObject == player) {
			//Wenn der andere Collider der Spieler ist
			Debug.Log ("Spieler in Collider");
			Vector3 direction = player.transform.position - transform.position;
			RaycastHit hit;
			//Sende Raycast zu Position des Spielers
			if (Physics.Raycast (transform.position, direction, out hit)) {
				//Wenn der Spieler getroffen wurde ist er auch sichtbar
				if (hit.collider.gameObject == player) {
					Debug.Log ("Spieler in Sichtbereich");
				}
				//Ansonsten befindet sich der Spieler in Deckung (hinter einer Wand)
			}
		}
	}
}