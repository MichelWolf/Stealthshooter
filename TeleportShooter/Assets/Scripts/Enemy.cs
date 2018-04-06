using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public int health = 100; //Leben des Gegners

	//Gegner bekommt Schaden 
	public void TakeDamage(int damage)
	{
		health -= damage;
		if (health <= 0) 
		{
			Die ();
		}
	}
	//Gegner stribt
	public void Die()
	{
		Destroy (this.gameObject);
	}
}
