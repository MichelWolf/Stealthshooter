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
		if (currentAmmo > 0 && fireCooldownLeft <= 0) {
			GameObject b = (GameObject)Instantiate (bullet, gunEnd.position, this.transform.rotation);
			b.GetComponent<Bullet> ().direction = hit.point - gunEnd.position;
			b.GetComponent<Bullet> ().target = hit.point;
			currentAmmo--;
			fireCooldownLeft = fireCooldown;
		}

	}
	public IEnumerator Reload(){
		isReloading = true;
		yield return new WaitForSeconds (reloadTime);
		currentAmmo = maxAmmo;
		isReloading = false;
		GameObject.FindObjectOfType<UI_Manager> ().UpdateAmmo(currentAmmo, maxAmmo);
	}
}