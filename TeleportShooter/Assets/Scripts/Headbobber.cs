using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbobber: MonoBehaviour 
{

	private float timer = 0.0f;
	[Header("Gehen")]
	[Tooltip("Werte für den Headbob beim normalen gehen")]
	public float walkBobbingSpeed = 0.18f;
	public float walkBobbingAmount = 0.2f;
	[Header("Rennen")]
	public float runBobbingSpeed = 0.18f;
	public float runBobbingAmount = 0.2f;
	[Header("Schleichen")]
	public float crouchBobbingSpeed = 0.18f;
	public float crouchBobbingAmount = 0.2f;
	[Header("Kopfhöhe")]
	public float midpoint = 0f;

	public float standHeight = 1.8f;
	public float crouchHeight = 1.2f;

	void Update () {
		if (FindObjectOfType<Character> ().isGrounded)
		{
			float waveslice = 0.0f;
			float horizontal = Input.GetAxis ("Horizontal");
			float vertical = Input.GetAxis ("Vertical");

			Vector3 bobs = transform.localPosition; 

			if (Mathf.Abs (horizontal) == 0 && Mathf.Abs (vertical) == 0)
			{
				timer = 0.0f;
			} else
			{
				waveslice = Mathf.Sin (timer);
				if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.walking)
				{
					timer = timer + walkBobbingSpeed;
				}
				else if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.running)
				{
					timer = timer + runBobbingSpeed;
				}
				else if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.crouching)
				{
					timer = timer + crouchBobbingSpeed;
				}
				if (timer > Mathf.PI * 2)
				{
					timer = timer - (Mathf.PI * 2);
				}
			}
			if (waveslice != 0)
			{
				float translateChange = 0f;
				if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.walking)
				{
					translateChange = waveslice * walkBobbingAmount;
				}
				else if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.running)
				{
					translateChange = waveslice * runBobbingAmount;
				}
				else if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.crouching)
				{
					translateChange = waveslice * crouchBobbingAmount;
				}

				float totalAxes = Mathf.Abs (horizontal) + Mathf.Abs (vertical);
				totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
				translateChange = totalAxes * translateChange;
				bobs.y = midpoint + translateChange;
			} else
			{
				bobs.y = midpoint;
			}

			transform.localPosition = bobs;
		}
		Vector3 parentPos = transform.parent.gameObject.transform.localPosition;
		if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.walking)
		{
			if (parentPos.y < standHeight)
			{
				parentPos.y += 0.05f;
			}
			parentPos.y = Mathf.Clamp (parentPos.y, 0, standHeight);
		}
		else if (FindObjectOfType<Character> ().walkingMode == Character.WalkingMode.crouching)
		{
			if (parentPos.y > crouchHeight)
			{
				parentPos.y -= 0.05f;
			}
			parentPos.y = Mathf.Clamp (parentPos.y, crouchHeight, Mathf.Infinity);
		}
		transform.parent.gameObject.transform.localPosition = parentPos;
	}



}