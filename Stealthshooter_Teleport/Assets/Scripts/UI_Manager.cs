using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public Text Ammo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateAmmo(int currAmmo, int maxAmmo)
	{
		Ammo.text = "" + currAmmo + "/" + maxAmmo;
	}
}
