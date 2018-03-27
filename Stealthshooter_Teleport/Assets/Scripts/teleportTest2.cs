﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class teleportTest2 : MonoBehaviour {

    private RaycastHit lastRaycastHit;
	private RaycastHit checkAbove;
    private RaycastHit lastCapsuleCastHit;
    private GameObject spawnedThing;

    public GameObject spawnThing;
    public float range = 0;
    public bool isLedge = false;
	public float ledgeDetectionRange = 1.0f;

	//Debugging
	public Vector3 raycastHitPosition;
	public Vector3 p1;
	public Vector3 p2;

	// Use this for initialization
	void Start ()
    {
        spawnedThing = Instantiate(spawnThing, new Vector3(0, 0, 0), Quaternion.identity);
	}

	// Update is called once per frame
	void Update ()
    {
		if (spawnedThing == null)
		{
			Debug.Log("Teleportation will only work with an indicator set in the fps camera");
		}
		else
		{ 
			if (Input.GetKey(KeyCode.E))
			{
				spawnedThing.SetActive(true);
				// spawn teleport location

				// I hit an object: spawn indicator on that
				if (IsLookingAtObject())
				{
					if (lastRaycastHit.normal.y >= 0.8) //Fläche zeigt nach oben
					{
						if (DoesPlayerFit(0))
                        {
							spawnedThing.gameObject.transform.position = lastRaycastHit.point; // + lastRaycastHit.normal;
						}
                        else
                        {
							spawnedThing.SetActive(false);
						}
					} 
					else if (lastRaycastHit.normal.x >= 0.8 || lastRaycastHit.normal.z >= 0.8 || lastRaycastHit.normal.x <= -0.8 || lastRaycastHit.normal.z <= -0.8) //Fläche ist eine Wand, zeigt zur Seite
					{
						if (DoesPlayerFit(1))
                        {
							if (isLedge == true)
                            {
								spawnedThing.gameObject.transform.position = p1;
							}
                            else
                            {
								spawnedThing.gameObject.transform.position = lastRaycastHit.point;
							}
						}
                        else
                        {
							spawnedThing.SetActive(false);
						}
					}
					else if (lastRaycastHit.normal.y <= -0.8) //Fläche zeigt nach unten
					{
						if (DoesPlayerFit(2))
                        {
							spawnedThing.gameObject.transform.position = lastRaycastHit.point; // + lastRaycastHit.normal;
						}
                        else
                        {
							spawnedThing.SetActive(false);
						}
					}
				}

				else
				{
                    // I hit nothing: spawn indicator in the air (limited by range)
                    if (DoesPlayerFit(3))
                    {
                        spawnedThing.gameObject.transform.position = GetPositionWhenNothingHit(); // + lastRaycastHit.normal;
                    }
                    else
                    {
                        spawnedThing.SetActive(false);
                    }
				}
			}

			if (Input.GetKeyUp(KeyCode.E) && spawnedThing.activeSelf)
			{
                // teleport me to the indicator
                spawnedThing.SetActive(false);
				TeleportTo(spawnedThing.transform.position, lastRaycastHit.normal);
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
		CharacterController charContr = GetComponent<CharacterController>();
		isLedge = false;
		if (state == 0)
        {
			Debug.Log ("Normale nach oben");
			//p1 = lastRaycastHit.point + charContr.center + Vector3.up * -charContr.height * 0.5F;
			p1 = lastRaycastHit.point + new Vector3 (0.0f, 0.6f, 0.0f);
			p2 = p1 + Vector3.up * charContr.height;

			if (Physics.CheckCapsule (p1, p2, charContr.radius))
            {
				return false;
			}
            else
            {
				Debug.Log ("Alles frei!");
				return true;
			}
		}
        else if (state == 1)
        {
			Debug.Log ("Normale zur Seite");
			//check für Ledge
			p1 = (lastRaycastHit.point + new Vector3 (0.0f, ledgeDetectionRange, 0.0f)) - (lastRaycastHit.normal * charContr.radius);
			p2 = p1 + Vector3.up * charContr.height;
			if (Physics.CheckCapsule (p1, p2, charContr.radius))
            {
				Debug.Log ("Keine Ledge");
				isLedge = false;
				p1 = lastRaycastHit.point -lastRaycastHit.normal * charContr.radius;
				p2 = p1 + Vector3.up * charContr.height;
				if (Physics.CheckCapsule (p1, p2, charContr.radius))
                {
					return true;
				}
                else
                {
					Debug.Log ("Alles frei!");
					return false;
				}

			}
            else
            {
				Debug.Log ("Alles frei!");
				isLedge = true;
				return true;
			}
		}
        else if (state == 2)
        {
			Debug.Log ("Normale nach unten");
			//p1 = lastRaycastHit.point + charContr.center + Vector3.up * -charContr.height * 0.5F;
			p2 = lastRaycastHit.point + new Vector3 (0.0f, -0.6f, 0.0f);
			p1 = p2 - Vector3.up * charContr.height;

			if (Physics.CheckCapsule (p1, p2, charContr.radius))
            {
				return false;
			}
            else
            {
				Debug.Log ("Alles frei!");
				return true;
			}
		}
        else if (state == 3)
        {
            Debug.Log("Nichts getroffen");
            Vector3 pos = GetPositionWhenNothingHit();
            RaycastHit startPoint;
            if (Physics.Raycast(pos, Vector3.down, out startPoint)) // max distance nötig?
            {
                // raycast nach unten, um Boden zu finden
                // von dort CheckSphere
                //p1 = startPoint.point + charContr.center + Vector3.up * -charContr.height * 0.5F;//+ new Vector3(0.0f, 0.6f, 0.0f);
                p1 = startPoint.point + new Vector3(0.0f, 0.6f, 0.0f);
                p2 = p1 + Vector3.up * charContr.height;
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
                p1 = startPoint.point - new Vector3(0.0f, 0.6f, 0.0f);
                p2 = p1 + Vector3.down * charContr.height;
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
                p2 = p1 + Vector3.up * charContr.height;
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
