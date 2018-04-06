using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	public Text Ammo;
	public Slider manaSlider;
	public Slider healthSlider;

	//Zeigt Munition im UI an
	public void UpdateAmmo(int currAmmo, int maxAmmo)
	{
		Ammo.text = "" + currAmmo + "/" + maxAmmo;
	}
	//Passt den Wert der Lebensanzeige an
	public void UpdateMana(float manaValue)
	{
		manaSlider.value = manaValue;
	}
	//Passt den Wert der Manaanzeige an
	public void UpdateHealth(float healthValue)
	{
		healthSlider.value = healthValue;
	}
}
