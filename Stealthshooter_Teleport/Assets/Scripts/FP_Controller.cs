using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP_Controller : MonoBehaviour {

	private UI_Manager ui_manager;

	private RaycastHit hit;
	public float walkSpeed;
	public GameObject equippedGun;

	// Use this for initialization
	void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		ui_manager = GameObject.FindObjectOfType<UI_Manager> ();
		ui_manager.UpdateAmmo (equippedGun.GetComponent<Gun> ().currentAmmo, equippedGun.GetComponent<Gun> ().maxAmmo);
	}
	
	// Update is called once per frame
	void Update () {
		/*float translation = Input.GetAxis ("Vertical") * walkSpeed;
		float straffe = Input.GetAxis ("Horizontal") * walkSpeed;
		translation *= Time.deltaTime;
		straffe *= Time.deltaTime;

		transform.Translate (straffe, 0, translation);
		*/

		if (Input.GetKeyDown ("escape"))
			Cursor.lockState = CursorLockMode.None;

		if (Input.GetButtonDown ("Fire1")) {

			Vector3 rayOrigin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

			if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, Mathf.Infinity))
			{
				equippedGun.GetComponent<Gun>().Shoot (hit);
				ui_manager.UpdateAmmo (equippedGun.GetComponent<Gun> ().currentAmmo, equippedGun.GetComponent<Gun> ().maxAmmo);
			}
		}
		if (Input.GetKeyDown (KeyCode.R)) 
		{
			equippedGun.GetComponent<Gun>().Reload ();
			ui_manager.UpdateAmmo (equippedGun.GetComponent<Gun> ().currentAmmo, equippedGun.GetComponent<Gun> ().maxAmmo);
		}


	}
}
