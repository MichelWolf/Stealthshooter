using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Teleportation : MonoBehaviour {

	private RaycastHit lastRaycastHit;
	private RaycastHit checkAbove;
	private RaycastHit lastCapsuleCastHit;
	public float range = 0;

	public bool isLedge = false;
	public float ledgeDetectionRange = 1.0f;
	internal bool isTeleportPossible;
	Vector3 teleportPos;

	//Debugging
	internal Vector3 raycastHitPosition;
	internal Vector3 p1;
	internal Vector3 p2;
	internal GameObject sphereP1;
	internal GameObject sphereP2;
	public GameObject DebugTeleportPos; 

	//Um auf den radius und die höhe des Spielers zugreifen zu können
	private CharacterController charContr;
	//Um auf das Manasystem zugreifen zu können
	private Character character;
	//Unterschiedliche Sprites die den Status des teleportierens anzeigen
	[Header("Indicator")]
	public GameObject indicatorPrefab;
	private GameObject teleportIndicator;
	public Sprite teleportNormal;
	public Sprite teleportUp;
	public Sprite teleportBlocked;

	internal int layerMask;


	// Use this for initialization
	void Start () {
		teleportIndicator = Instantiate(indicatorPrefab, new Vector3(0, 0, 0), Quaternion.identity);
		teleportIndicator.SetActive (false);
		charContr = GetComponent<CharacterController>();
		character = GetComponent<Character> ();
		layerMask = 1 << 8 | 1 << 2;
		layerMask = ~layerMask;
	}

	// Update is called once per frame
	void Update () {

		if (teleportIndicator == null)
		{
			Debug.Log("Teleportation will only work with an indicator set in the fps camera");
		}
		else
		{ 
			if (Input.GetKey(KeyCode.E) && character.currentMana >= character.teleportCost)
			{
				teleportIndicator.SetActive(true);
				if (DebugTeleportPos != null)
				{
					DebugTeleportPos.SetActive (true);
				}
				// spawn teleport location
				//CheckDoesPlayerFit();

				// I hit an object: spawn indicator on that
				if (IsLookingAtObject())
				{
					if (lastRaycastHit.normal.y >= 0.95) //Fläche zeigt nach oben
					{
						//zeige Indicator an der getroffenen Stelle an, Radius von der Wand entfernt, damit Spieler bei teleport nicht in der Wand landet
						teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
						//teleportPos = teleportIndicator.gameObject.transform.position; //?
						//Je nachdem ob der Spieler an die Stelle passt wird der Indicator aktualisiert und teleportieren zugelassen oder nicht
						if (DoesPlayerFit (0))
						{
							isTeleportPossible = true;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
						} else
						{
							isTeleportPossible = false;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
						}
						teleportIndicator.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward);
					} else if (lastRaycastHit.normal.x >= 0.8 || lastRaycastHit.normal.z >= 0.8 || lastRaycastHit.normal.x <= -0.8 || lastRaycastHit.normal.z <= -0.8) //Fläche ist eine Wand, zeigt zur Seite
					{
						if (DoesPlayerFit (1))
						{
							if (isLedge == true)
							{
								teleportIndicator.gameObject.transform.position = p1;
								teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportUp;
							} else
							{
								teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
								teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
							}
							isTeleportPossible = true;
						} else
						{
							isTeleportPossible = false;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
						}
						teleportIndicator.transform.rotation = Quaternion.LookRotation (lastRaycastHit.normal);
					} else if (lastRaycastHit.normal.y <= -0.8) //Fläche zeigt nach unten
					{
						teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
						if (DoesPlayerFit (2))
						{
							isTeleportPossible = true;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
						} else
						{
							isTeleportPossible = false;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportBlocked;
						}
						teleportIndicator.transform.rotation = Quaternion.LookRotation (-Camera.main.transform.forward);
					}
					//Kein Boden/Keine Wand -> Rampe (hoffentlich)
					else
					{
						teleportIndicator.gameObject.transform.position = lastRaycastHit.point + (lastRaycastHit.normal * charContr.radius);
						if (DoesPlayerFit (1))
						{
							isTeleportPossible = true;
							teleportIndicator.GetComponentInChildren<Image> ().sprite = teleportNormal;
						} else
						{
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
				//Debugging, Spheren sind Größe der Kugeln oben und unten im CharacterConroller bei Radius 0.5f
				//Zeigt an, welcher Bereich durch DoesPlayerFit() überprüft wird
				//sphereP1.transform.position = p1;
				//sphereP2.transform.position = p2;
			}

			if (Input.GetKeyUp(KeyCode.E))
			{
				// teleport me to the indicator
				teleportIndicator.SetActive(false);
				if (isTeleportPossible && character.currentMana >= character.teleportCost) {
					//Lässt sich nur teleportieren, wenn das man ausreicht und der Spieler an die Stelle passt
					//TeleportTo (teleportIndicator.transform.position, lastRaycastHit.normal);
					TeleportTo (teleportPos, lastRaycastHit.normal);
					//zieht die Teleportkosten vom Mana des Spielers ab
					character.currentMana -= character.teleportCost;
					//updatet die UI mit den neuen Mana-Daten
					GameObject.FindObjectOfType<UI_Manager> ().UpdateMana(character.currentMana);
					//setzt den Cooldown zum regenerieren des Manas
					character.manaRegenCooldown = character.timeForManaRegen;
				}

				if (DebugTeleportPos != null)
				{
					DebugTeleportPos.SetActive (false);
				}
			}
		}
		//Debugging
		raycastHitPosition = lastRaycastHit.point;

		RaycastHit telpos;
		if (Physics.SphereCast (teleportIndicator.gameObject.transform.position + new Vector3(0, charContr.radius + 0.001f, 0), charContr.radius, Vector3.down, out telpos, charContr.height))
		{ 
			teleportPos = telpos.point + new Vector3 (0.0f, charContr.radius + 0.01f, 0.0f);
			teleportPos.x = teleportIndicator.gameObject.transform.position.x;
			teleportPos.z = teleportIndicator.gameObject.transform.position.z;
		} else
		{
			teleportPos = teleportIndicator.gameObject.transform.position - new Vector3(0, charContr.height, 0);
		}

		if (DebugTeleportPos != null)
		{
			DebugTeleportPos.transform.position = teleportPos;
		}
	}

	private bool IsLookingAtObject()
	{
		//Überprüft, ob der Spieler in der Range des Teleports ein Object trifft oder nicht
		//Von der Cameramitte nach vorne
		Vector3 origin = Camera.main.ViewportToWorldPoint (new Vector3(0.5f, 0.5f, 0.0f));
		Vector3 direction = Camera.main.transform.forward;

		if (Physics.Raycast(origin, direction, out lastRaycastHit, range, layerMask))
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
		//Gibt die Position am Ende der Teleport-Range zurück, wird genutzt wenn IsLookingAtObject() == false
		Vector3 origin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
		Vector3 direction = Camera.main.transform.forward;
		return origin + (direction * range);
	}

	internal bool DoesPlayerFit(int state)
	{
		//state == 0, check nach oben
		//state == 1, check an Kante
		//state == 2, check von oben
		//state == 3, check ohne Kante
		isLedge = false;
		if (state == 0)
		{
			Debug.Log ("Normale nach oben");
			Vector3 pos = lastRaycastHit.point + new Vector3 (0.0f, charContr.height + 0.01f, 0.0f);
			return checkFromAbove (pos);
		} else if (state == 1)
		{
			Debug.Log ("Normale zur Seite");
			//check für Ledge
			p1 = (lastRaycastHit.point + new Vector3 (0.0f, ledgeDetectionRange, 0.0f)) - (lastRaycastHit.normal * charContr.radius);
			p2 = p1 + Vector3.up * (charContr.height / 2);
			if (Physics.CheckCapsule (p1, p2, charContr.radius))
			{
				Debug.Log ("Keine Ledge");
				isLedge = false;
				p1 = lastRaycastHit.point + lastRaycastHit.normal * (charContr.radius + 0.01f);
				p2 = p1 + Vector3.up * (charContr.height / 2);

				Vector3 pos = (lastRaycastHit.point + lastRaycastHit.normal * (charContr.radius + 0.01f));
				return checkFromAbove(pos);

			} else
			{
				Debug.Log ("Alles frei!");
				isLedge = true;
				return true;
			}
		} else if (state == 2)
		{
			// check from below
			Debug.Log ("Normale nach unten");
			Vector3 pos = (lastRaycastHit.point + lastRaycastHit.normal * (charContr.radius + 0.01f));
			return checkFromBelow(pos);
			/*p2 = lastRaycastHit.point + new Vector3 (0.0f, -(charContr.radius + 0.01f), 0.0f);
			p1 = p2 - Vector3.up * (charContr.height / 2);

			if (Physics.CheckCapsule (p1, p2, charContr.radius))
			{
				return false;
			} else
			{
				Debug.Log ("Alles frei!");
				return true;
			}*/
		} else if (state == 3)
		{
			Debug.Log ("Nichts getroffen");
			Vector3 pos = GetPositionWhenNothingHit ();
			return checkFromAbove (pos);

		} else if (state == 4)
		{
			//aufstehen
			//sphereP1.SetActive (true);
			//sphereP2.SetActive (true);
			/*p1 = charContr.transform.position + Vector3.down * (charContr.height / 2) + new Vector3(3,0.01f, 0);
			p2 = p1 + Vector3.up * charContr.height / 2;
			sphereP1.transform.position = p1;
			sphereP2.transform.position = p2;*/
			RaycastHit boden;
			if (Physics.Raycast (charContr.transform.position, Vector3.down, out boden))
			{
				p1 = boden.point + new Vector3 (0.0f, charContr.radius + 0.01f, 0.0f);
				p2 = p1 + Vector3.up * (character.standHeight / 2);
				//sphereP1.transform.position = p1;
				//sphereP2.transform.position = p2;
			}
			if (Physics.CheckCapsule (p1, p2, charContr.radius, layerMask))
			{
				Debug.Log ("Aufstehen passt nicht");
				return false;
			} else
			{
				Debug.Log ("Aufstehen passt");
				return true;
			}

		}
		return true;
	}

	private bool checkFromAbove(Vector3 pos)
	{
		RaycastHit startPoint;
		if (Physics.SphereCast (pos, charContr.radius, Vector3.down, out startPoint, charContr.height))
		{ 
			// spherecast nach unten, um Boden zu finden
			// von dort CheckSphere
			p1 = startPoint.point + new Vector3 (0.0f, charContr.radius + 0.01f, 0.0f);
			p1.x = pos.x;
			p1.z = pos.z;
			p2 = p1 + Vector3.up * (charContr.height / 2);
			if (Physics.CheckCapsule (p1, p2, charContr.radius))
			{
				Debug.Log ("Boden gecheckt: passt nicht");
				return false;
			} else
			{
				Debug.Log ("Boden gecheckt: passt!");
				return true;
			}
		} 
		else
		{
			return true;
		}

	}

	private bool checkFromBelow(Vector3 pos)
	{
		RaycastHit startPoint;
		if (Physics.SphereCast (pos, charContr.radius, Vector3.up, out startPoint, charContr.height))
		{ 
			// spherecast nach unten, um Boden zu finden
			// von dort CheckSphere
			p2 = startPoint.point - new Vector3 (0.0f, charContr.radius + 0.01f, 0.0f);
			p2.x = pos.x;
			p2.z = pos.z;
			p1 = p2 + Vector3.down * (charContr.height / 2);
			if (Physics.CheckCapsule (p1, p2, charContr.radius))
			{
				Debug.Log ("Boden gecheckt: passt nicht");
				return false;
			} else
			{
				Debug.Log ("Boden gecheckt: passt!");
				return true;
			}
		} 
		else
		{
			return true;
		}

	}


	private void TeleportTo(Vector3 position, Vector3 normal)
	{
		//instant teleport an die Stelle die vorher Überprüft wurde
		//transform.position = lastRaycastHit.point; + lastRaycastHit.normal;
		transform.position = position;// + normal;
	}
}