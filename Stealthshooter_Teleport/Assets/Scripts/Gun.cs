using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

	private RaycastHit hit;

	public float gunDamage = 1.0f;
	public float fireCooldown = 0.5f;
	float fireCooldownLeft = 0f;
	public GameObject bullet;

	public int maxAmmo = 7;
	public int currentAmmo;
	public float reloadTime = 1f;
	internal bool isReloading = false;

	// die Kugeln werden am gunEnd erstellt
	public Transform gunEnd;

	// Use this for initialization
	void Start () 
	{
		currentAmmo = maxAmmo;
	}

	// Update is called once per frame
	void Update () 
	{
		if (fireCooldownLeft > 0) {
			fireCooldownLeft -= Time.deltaTime;
		}
	}
	public void Shoot(RaycastHit hit){
		//Wenn genug Munition vorhanden ist und die Firerate es zulässt
		if (currentAmmo > 0 && fireCooldownLeft <= 0) {
			//erstellt eine Kugel am gunEnd
			GameObject b = (GameObject)Instantiate (bullet, gunEnd.position, this.transform.rotation);
			//setzt die Richtung der Kugel, Vector von GunEnd zu der vom Raycast getroffenen Position 
			b.GetComponent<Bullet> ().direction = hit.point - gunEnd.position;
			//zieht Munition ab
			currentAmmo--;
			//setzt den Cooldown zum nächsten Schießen
			fireCooldownLeft = fireCooldown;
		}

	}
	public IEnumerator Reload(){
		//wird benötigt um schießen während des nachladens zu verbieten
		isReloading = true;
		//nachladen dauert eine reloadTime
		yield return new WaitForSeconds (reloadTime);
		//Munition wird wieder gefüllt
		currentAmmo = maxAmmo;
		//Nachladen abgeschlossen
		isReloading = false;
		//UI aktualisieren
		GameObject.FindObjectOfType<UI_Manager> ().UpdateAmmo(currentAmmo, maxAmmo);
	}
}