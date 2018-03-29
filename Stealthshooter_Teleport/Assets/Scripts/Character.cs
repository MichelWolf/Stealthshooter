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

	private CharacterController charContr;
	private Vector3 velocity;
	public bool isGrounded = true;


	void Start()
	{
		charContr = GetComponent<CharacterController>();
	}

	void Update()
	{
		GetWalkingMode ();
		isGrounded = Physics.Raycast (this.transform.position, Vector3.down, charContr.height / 2 + 0.1f);
		if (isGrounded && velocity.y < 0)
			velocity.y = 0f;

		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		move = transform.TransformDirection (move);
		if (walkingMode == WalkingMode.walking) {
			charContr.Move (move * Time.deltaTime * walkSpeed);
		}else if (walkingMode == WalkingMode.running) {
			charContr.Move (move * Time.deltaTime * runSpeed);
		} else if (walkingMode == WalkingMode.crouching) {
			charContr.Move (move * Time.deltaTime * crouchSpeed);
		}
		if (move != Vector3.zero)
			transform.forward = move;

		if (Input.GetButtonDown("Jump") && isGrounded)
			velocity.y += Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);

		velocity.y += Physics.gravity.y * Time.deltaTime;
		charContr.Move(velocity * Time.deltaTime);
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