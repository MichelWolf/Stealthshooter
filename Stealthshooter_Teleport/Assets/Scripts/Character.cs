using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	public float walkSpeed = 5f;
	public float runSpeed = 10f;
	public float crouchSpeed = 3f;
	public enum WalkingMode {crouching, walking, running};
	public WalkingMode walkingMode = WalkingMode.walking;
	public float speed = 5f;

	public float jumpHeight = 2f;
	public GameObject camTest;

	private CharacterController charContr;
	private Vector3 velocity;
	public bool isGrounded = true;

	public int maxMana = 100;
	internal int currentMana;
	public int teleportCost = 10;
	public float timeForManaRegen = 5f;
	internal float manaRegenCooldown = 0f;

	public int maxHealth = 100;
	internal int currentHealth;
	public float timeForHealthRegen = 10f;
	internal float healthRegenCooldown = 0f;

	public float respawnTime = 5f;
	public GameObject spawnpoint;

	public GameObject equippedGun;
	private RaycastHit hit;

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		charContr = GetComponent<CharacterController>();
		currentMana = maxMana;
		currentHealth = maxHealth;
	}

	void Update()
	{
		GetWalkingMode ();
		isGrounded = Physics.Raycast (this.transform.position, Vector3.down, charContr.height / 2 + 0.1f);
		if (isGrounded && velocity.y < 0)
			velocity.y = 0f;

		if (walkingMode == WalkingMode.walking) {
			float translation = Input.GetAxis ("Vertical") * walkSpeed;
			float straffe = Input.GetAxis ("Horizontal") * walkSpeed;
			translation *= Time.deltaTime;
			straffe *= Time.deltaTime;

			transform.Translate (straffe, 0, translation);
		}else if (walkingMode == WalkingMode.running) {
			float translation = Input.GetAxis ("Vertical") * runSpeed;
			float straffe = Input.GetAxis ("Horizontal") * runSpeed;
			translation *= Time.deltaTime;
			straffe *= Time.deltaTime;

			transform.Translate (straffe, 0, translation);
		} else if (walkingMode == WalkingMode.crouching) {
			float translation = Input.GetAxis ("Vertical") * crouchSpeed;
			float straffe = Input.GetAxis ("Horizontal") * crouchSpeed;
			translation *= Time.deltaTime;
			straffe *= Time.deltaTime;

			transform.Translate (straffe, 0, translation);
		}

		if (Input.GetButtonDown("Jump") && isGrounded)
			velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

		velocity.y += Physics.gravity.y * Time.deltaTime;
		charContr.Move(velocity * Time.deltaTime);

		if (Input.GetKeyDown (KeyCode.C)) {
			camTest.GetComponentInChildren<CCTVCamera>().isControllable = true;
			Camera.main.gameObject.GetComponent<camMouseLook>().enabled = false;
			//this.gameObject.GetComponent<camMouseLook> ().enabled = false;
		}else if (Input.GetKeyUp (KeyCode.C)) {
			camTest.GetComponentInChildren<CCTVCamera>().isControllable = false;
			//this.gameObject.GetComponent<camMouseLook> ().enabled = true;
			Camera.main.gameObject.GetComponent<camMouseLook>().enabled = true;
		}

		if (Input.GetButtonDown ("Fire1") && !(equippedGun.GetComponent<Gun>().isReloading)) {

			Vector3 rayOrigin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));

			if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, Mathf.Infinity))
			{
				equippedGun.GetComponent<Gun>().Shoot (hit);
				GameObject.FindObjectOfType<UI_Manager> ().UpdateAmmo (equippedGun.GetComponent<Gun> ().currentAmmo, equippedGun.GetComponent<Gun> ().maxAmmo);
			}
		}
		if (Input.GetKeyDown (KeyCode.R) && !(equippedGun.GetComponent<Gun>().isReloading)) 
		{
			StartCoroutine(equippedGun.GetComponent<Gun>().Reload ());
			//ui_manager.UpdateAmmo (equippedGun.GetComponent<Gun> ().currentAmmo, equippedGun.GetComponent<Gun> ().maxAmmo);
		}
	}

	public void GetWalkingMode()
	{
		if (Input.GetKey (KeyCode.LeftShift)) {
			walkingMode = WalkingMode.running;
			return;
		} else if (Input.GetKey (KeyCode.LeftControl)) {
			walkingMode = WalkingMode.crouching;
			return;
		}
		walkingMode = WalkingMode.walking;
	}
}