using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
	[Header("Bewegung")]
	public float walkSpeed = 5f;
	public float runSpeed = 10f;
	public float crouchSpeed = 3f;
	//Spieler soll nur entweder gehen, schleichen oder rennen können
	public enum WalkingMode {crouching, walking, running};
	public WalkingMode walkingMode = WalkingMode.walking;
	//Spieler soll beim schleichen kleiner sein
	public float standHeight = 2f;
	public float crouchHeight = 1.5f;
	[Header("Gravity")]
	public float verticalVelocity;
	public float gravity = 20.0f;
	public float jumpForce = 10.0f;
	public bool isGrounded = true;

	//Manasystem
	[Header("Manasystem")]
	public float maxMana = 100;
	public float currentMana;
	public float manaRegen = 0.1f;
	public float timeForManaRegen = 5f;
	internal float manaRegenCooldown = 0f;
	public int teleportCost = 10;
	//Lebenssystem
	[Header("Lebenssystem")]
	public float maxHealth = 100;
	public float currentHealth;
	public float healthRegen = 0.1f;
	public float timeForHealthRegen = 10f;
	internal float healthRegenCooldown = 0f;
	//Respawnsystem
	[Header("Respawn")]
	public float respawnTime = 5f;
	public GameObject spawnpoint;

	public GameObject equippedGun;
	private RaycastHit hit;

	//CCTV testobject
	public GameObject camTest;

	private CharacterController charContr;

	private Vector3 moveDirection = Vector3.zero;

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
		//Raycast von mitte des Spielers bis Hälfte der Spielerhöhe + 0.1f nach unten. wenn etwas getroffen wird ist der Spieler am Boden 
		isGrounded = Physics.Raycast (this.transform.position, Vector3.down, 0.1f);

		//wenn der Spieler läuft, Bewegungen mit walkSpeed
		if (walkingMode == WalkingMode.walking)
		{
			//charContr.height = standHeight;
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= walkSpeed;
		} //wenn der Spieler rennt, Bewegungen mit runSpeed
		else if (walkingMode == WalkingMode.running) 
		{
			//charContr.height = standHeight;
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= runSpeed;
		} //wenn der Spieler schleicht, Bewegungen mit crouchSpeed
		else if (walkingMode == WalkingMode.crouching) 
		{
			//charContr.height = crouchHeight;
			moveDirection = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= crouchSpeed;
		}
		charContr.Move (moveDirection * Time.deltaTime);
		//Wenn der Spieler die "Jump"-Taste (space) drückt und sich auf dem Boden befindet
		if (isGrounded)
		{
			verticalVelocity = -gravity * Time.deltaTime;
		}
		if (Input.GetButtonDown ("Jump") && isGrounded)
		{
			verticalVelocity = jumpForce;
		}
		verticalVelocity -= gravity * Time.deltaTime;
		charContr.Move(new Vector3(0,verticalVelocity,0) * Time.deltaTime);

		//wenn C gedrückt ist wechsel auf CCTV-Steuerung mit der Maus
		if (Input.GetKeyDown (KeyCode.C))
		{
			camTest.GetComponentInChildren<CCTVCamera> ().isControllable = true;
			Camera.main.gameObject.GetComponent<camMouseLook> ().enabled = false;
		} else if (Input.GetKeyUp (KeyCode.C))
		{
			camTest.GetComponentInChildren<CCTVCamera> ().isControllable = false;
			Camera.main.gameObject.GetComponent<camMouseLook> ().enabled = true;
		}

		//wenn "Fire1" (LMB) gedrückt wird und die Waffe nicht nachlädt
		if (Input.GetButtonDown ("Fire1") && !(equippedGun.GetComponent<Gun>().isReloading)) {

			Vector3 rayOrigin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));
			//Raycast in Richtung der Camera, bis etwas getroffen wird
			if (Physics.Raycast(rayOrigin, Camera.main.transform.forward, out hit, Mathf.Infinity))
			{
				//Wenn etwas getroffen wird, schieß auf das getroffene
				equippedGun.GetComponent<Gun>().Shoot (hit);
				//aktualisiere die UI mit dem neuen Munitionsstand
				GameObject.FindObjectOfType<UI_Manager> ().UpdateAmmo (equippedGun.GetComponent<Gun> ().currentAmmo, equippedGun.GetComponent<Gun> ().maxAmmo);
			}
		}
		//wenn R gedrückt wird und die Waffe nicht nachlädt
		if (Input.GetKeyDown (KeyCode.R) && !(equippedGun.GetComponent<Gun>().isReloading)) 
		{
			//starte den Prozess des nachladens
			StartCoroutine(equippedGun.GetComponent<Gun>().Reload ());
		}

		//Manaregeneration
		if (manaRegenCooldown > 0f) {
			manaRegenCooldown -= Time.deltaTime;
		}
		if (currentMana < maxMana && manaRegenCooldown <= 0f) {
			currentMana += manaRegen;
			GameObject.FindObjectOfType<UI_Manager> ().UpdateMana(currentMana);
		}
		//Lebensregeneration
		if (healthRegenCooldown > 0f) {
			healthRegenCooldown -= Time.deltaTime;
		}
		if (currentHealth < maxHealth && healthRegenCooldown <= 0f) {
			currentHealth += healthRegen;
			GameObject.FindObjectOfType<UI_Manager> ().UpdateHealth(currentHealth);
		}
		//Debug.Log (transform.position);
	}

	//Muss überarbeitet werden, lohnt sich nicht zu kommentieren
	//Muss überarbeitet werden, lohnt sich nicht zu kommentieren
	public void GetWalkingMode()
	{
		if (Input.GetKey (KeyCode.LeftShift))
		{
			if (walkingMode == WalkingMode.crouching)
			{
				if (!GetComponent<Teleportation> ().DoesPlayerFit (4))
				{
					return;
				} 
			}
			//walkingMode = WalkingMode.running;
			SwitchWalkingMode(WalkingMode.running);
			return;
		} 
		if (Input.GetKeyUp (KeyCode.LeftShift) && walkingMode == WalkingMode.running)
		{
			//walkingMode = WalkingMode.walking;
			SwitchWalkingMode(WalkingMode.walking);
		}
		if (Input.GetKeyDown (KeyCode.LeftControl))
		{
			if (walkingMode == WalkingMode.crouching)
			{
				//Testen ob Spieler aufstehen kann
				if (GetComponent<Teleportation> ().DoesPlayerFit (4))
				{
					//walkingMode = WalkingMode.walking;
					SwitchWalkingMode(WalkingMode.walking);
				} 
				else
				{
					return;
				}
			} 
			else
			{
				//walkingMode = WalkingMode.crouching;
				SwitchWalkingMode(WalkingMode.crouching);
			}
			return;
		}
	}

	void SwitchWalkingMode(WalkingMode newMode)
	{
		WalkingMode oldMode = walkingMode;
		walkingMode = newMode;
		// do stuff
		if (oldMode == WalkingMode.crouching && newMode != WalkingMode.crouching)
		{
			charContr.height = standHeight;
			charContr.center = new Vector3(0f, standHeight / 2, 0f);
			//StartCoroutine(StandUp());
		}
		else if (newMode == WalkingMode.crouching)
		{
			charContr.height = crouchHeight;
			charContr.center = new Vector3(0f, crouchHeight / 2, 0f);
		}
	}

	public void TakeDamage(int damage){
		//Spieler bekommt Schaden
		currentHealth -= damage;
		//Cooldown zum regenerieren wird gesetzt
		healthRegenCooldown = timeForHealthRegen;
		//UI wird aktualisiert
		GameObject.FindObjectOfType<UI_Manager> ().UpdateHealth(currentHealth);
		//wenn der Spieler kein Leben übrig hat
		if (currentHealth <= 0) {
			//starte den Respawn-Prozess
			StartCoroutine(Respawn ());
		}
	}

	public IEnumerator Respawn(){
		//deaktiviere dieses Skript 
		this.gameObject.GetComponent<Character> ().enabled = false;
		//warte bis Respawn-Timer abgelaufen ist
		yield return new WaitForSeconds (respawnTime);
		//aktiviere dieses Skriot wieder
		this.gameObject.GetComponent<Character> ().enabled = true;
		//Setzt den Spieler auf die Position des aktuellen Spawnpoints
		this.transform.position = spawnpoint.transform.position;
		//Setzt Leben und Mana des Spielers wieder auf voll
		currentHealth = maxHealth;
		currentMana = maxMana;
		//Aktualisiert die UI mit neuen Werten
		GameObject.FindObjectOfType<UI_Manager> ().UpdateHealth(currentHealth);
		GameObject.FindObjectOfType<UI_Manager> ().UpdateMana(currentMana);
	}
	/*
	public IEnumerator StandUp()
	{
		float value = 0.05f;
		if (charContr.height < standHeight && walkingMode != WalkingMode.crouching)
		{
			charContr.height = Mathf.Clamp(charContr.height + value, 0, standHeight);
			charContr.center = new Vector3(0f, standHeight / 2, 0f);
			yield return new WaitForSeconds(0.001f);
			StartCoroutine(StandUp());
		}

	}
	*/
}