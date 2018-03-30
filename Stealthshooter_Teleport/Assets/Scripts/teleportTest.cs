using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public class teleportTest : MonoBehaviour {

	private RaycastHit lastRaycastHit;
	private RaycastHit checkAbove;
	private RaycastHit lastCapsuleCastHit;
	public float range = 0;
	public GameObject indicatorPrefab;
	private GameObject teleportIndicator;
	public bool isLedge = false;
	public float ledgeDetectionRange = 1.0f;
	internal bool isTeleportPossible;

	//Debugging
	public Vector3 raycastHitPosition;
	public Vector3 p1;
	public Vector3 p2;
	public GameObject sphereP1;
	public GameObject sphereP2;


	private CharacterController charContr;
	private Character character;

	public Sprite teleportNormal;
	public Sprite teleportUp;
	public Sprite teleportBlocked;


	// Use this for initialization
	void Start () {
		teleportIndicator = Instantiate(indicatorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		teleportIndicator.SetActive (false);
		charContr = GetComponent<CharacterController>();
		character = GetComponent<Character> ();
	}

	// Update is called once per frame
	void Update () {
		if (teleportIndicator == null)
		{
			Debug.Log("Teleportation will only work with an indicator set in the fps camera");
		}
		else
		{ 
			if (Input.GetKey(KeyCode.E))
			{
				teleportIndicator.SetActive(true);
				// spawn teleport location
				//CheckDoesPlayerFit();

				// I hit an object: spawn indicator on that
				if (IsLookingAtObject())
				{
					if (lastRaycastHit.normal.y >= 0.8) //Fläche zeigt nach oben
					{
						teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
						if (DoesPlayerFit (0)) {
							isTeleportPossible = true;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
						} else {
							isTeleportPossible = false;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
						}
						teleportIndicator.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward);
					} 
					else if (lastRaycastHit.normal.x >= 0.8 || lastRaycastHit.normal.z >= 0.8 || lastRaycastHit.normal.x <= -0.8 || lastRaycastHit.normal.z <= -0.8) //Fläche ist eine Wand, zeigt zur Seite
					{
						if (DoesPlayerFit (1)) {
							if (isLedge == true) {
								teleportIndicator.gameObject.transform.position = p1;
								teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportUp;
							} else {
								teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
								teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
							}
							isTeleportPossible = true;
						} else {
							isTeleportPossible = false;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
						}
						teleportIndicator.transform.rotation = Quaternion.LookRotation (lastRaycastHit.normal);
					}
					else if (lastRaycastHit.normal.y <= -0.8) //Fläche zeigt nach unten
					{
						teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
						if (DoesPlayerFit (2)) {
							isTeleportPossible = true;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
						}else {
							isTeleportPossible = false;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
						}
						teleportIndicator.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward);
					}
				}

				else
				{
					teleportIndicator.gameObject.transform.position = GetPositionWhenNothingHit();
					// I hit nothing: spawn indicator in the air (limited by range)
					if (DoesPlayerFit(3))
					{
						isTeleportPossible = true;
						teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
					}
					else
					{
						isTeleportPossible = false;
						teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
					}
					teleportIndicator.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward);
				}
				sphereP1.transform.position = p1;
				sphereP2.transform.position = p2;
			}

			if (Input.GetKeyUp(KeyCode.E))
			{
				// teleport me to the indicator
				teleportIndicator.SetActive(false);
				if (isTeleportPossible && character.currentMana >= character.teleportCost) {
					TeleportTo (teleportIndicator.transform.position, lastRaycastHit.normal);
					character.currentMana -= character.teleportCost;
					GameObject.FindObjectOfType<UI_Manager> ().UpdateMana(character.currentMana);
					character.manaRegenCooldown = character.timeForManaRegen;
				}
			}
		}
		//Debug
		raycastHitPosition = lastRaycastHit.point;
	}

	private bool IsLookingAtObject()
	{
		Vector3 origin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));
		Vector3 direction = Camera.main.transform.forward;

		if (Physics.Raycast(origin, direction, out lastRaycastHit, range))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private Vector3 GetPositionWhenNothingHit()
	{
		Vector3 origin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
		Vector3 direction = Camera.main.transform.forward;
		return origin + (direction * range);
	}

	private bool DoesPlayerFit(int state)
	{
		//state == 0, check nach oben
		//state == 1, check an Kante
		//state == 2, check von oben
		//CharacterController charContr = GetComponent<CharacterController>();
		isLedge = false;
		if (state == 0) {
			Debug.Log ("Normale nach oben");
			//p1 = lastRaycastHit.point + charContr.center + Vector3.up * -charContr.height * 0.5F;
			p1 = lastRaycastHit.point + new Vector3 (0.0f, charContr.radius + 0.01f, 0.0f);
			p2 = p1 + Vector3.up * (charContr.height / 2);

			if (Physics.CheckCapsule (p1, p2, charContr.radius)) {
				return false;
			} else {
				Debug.Log ("Alles frei!");
				return true;
			}
		} else if (state == 1) {
			Debug.Log ("Normale zur Seite");
			//check für Ledge
			p1 = (lastRaycastHit.point + new Vector3 (0.0f, ledgeDetectionRange, 0.0f)) - (lastRaycastHit.normal * charContr.radius);
			p2 = p1 + Vector3.up * (charContr.height / 2);
			if (Physics.CheckCapsule (p1, p2, charContr.radius)) {
				Debug.Log ("Keine Ledge");
				isLedge = false;
				p1 = lastRaycastHit.point + lastRaycastHit.normal * (charContr.radius + 0.01f);
				p2 = p1 + Vector3.up * (charContr.height / 2);
				if (Physics.CheckCapsule (p1, p2, charContr.radius)) {
					return false;
				} else {
					Debug.Log ("Alles frei!");
					return true;
				}

			} else {
				Debug.Log ("Alles frei!");
				isLedge = true;
				return true;
			}
		} else if (state == 2) {
			Debug.Log ("Normale nach unten");
			//p1 = lastRaycastHit.point + charContr.center + Vector3.up * -charContr.height * 0.5F;
			p2 = lastRaycastHit.point + new Vector3 (0.0f, -(charContr.radius + 0.01f), 0.0f);
			p1 = p2 - Vector3.up * (charContr.height / 2);

			if (Physics.CheckCapsule (p1, p2, charContr.radius)) {
				return false;
			} else {
				Debug.Log ("Alles frei!");
				return true;
			}
		}else if (state == 3)
		{
			Debug.Log("Nichts getroffen");
			Vector3 pos = GetPositionWhenNothingHit();
			RaycastHit startPoint;
			if (Physics.Raycast(pos, Vector3.down, out startPoint)) // max distance nötig?
			{
				// raycast nach unten, um Boden zu finden
				// von dort CheckSphere
				//p1 = startPoint.point + charContr.center + Vector3.up * -charContr.height * 0.5F;//+ new Vector3(0.0f, 0.6f, 0.0f);
				p1 = startPoint.point + new Vector3(0.0f, charContr.radius + 0.01f, 0.0f);
				//Debug.Log(p1);
				//Debug.Log (p1 + Vector3.up * (charContr.height / 2));
				p2 = p1 + Vector3.up * (charContr.height / 2);
				//DebugExtension.DebugCapsule(startPoint.point, startPoint.point + Vector3.up * charContr.height, charContr.radius, 5);
				if (Physics.CheckCapsule(p1, p2, charContr.radius))
				{
					Debug.Log("Boden gecheckt: passt nicht");
					return false;
				}
				else
				{
					Debug.Log("Boden gecheckt: passt!");
					return true;
				}
			}
			else if (Physics.Raycast(pos, Vector3.up, out startPoint))
			{
				// falls nicht gefunden, Raycast nach oben, um Decke zu finden
				// von dort CheckSphere
				//p1 = startPoint.point + charContr.center + Vector3.up * charContr.height * 0.5F;//new Vector3(0.0f, 0.6f, 0.0f);
				p1 = startPoint.point - new Vector3(0.0f, charContr.radius + 0.05f, 0.0f);
				p2 = p1 + Vector3.down * (charContr.height / 2);
				if (Physics.CheckCapsule(p1, p2, charContr.radius))
				{
					Debug.Log("Decke gecheckt: passt nicht");
					return false;
				}
				else
				{
					Debug.Log("Decke gecheckt: passt!");
					return true;
				}
			}
			else
			{
				// falls gar nichts gefunden: Check Sphere mit Mittelpunkt an aktueller Stelle der Kugel
				p1 = pos + Vector3.down * charContr.height / 2;
				p2 = p1 + Vector3.up * charContr.height / 2;
				if (Physics.CheckCapsule(p1, p2, charContr.radius))
				{
					Debug.Log("Generell gecheckt: passt nicht");
					return false;
				}
				else
				{
					Debug.Log("Generell gecheckt: passt!");
					return true;
				}
			}

		}
		return true;
	}

	private void TeleportTo(Vector3 position, Vector3 normal)
	{
		//transform.position = lastRaycastHit.point; + lastRaycastHit.normal;
		transform.position = position;// + normal;
	}
}