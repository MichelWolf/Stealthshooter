using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleportIndicator : MonoBehaviour {

    public Material normal;
    public Material notPossible;
    private bool teleportPossible;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool IsTeleportPossible()
    {
        return teleportPossible;
    }

    public void SetTeleportPossible(bool possible)
    {
        teleportPossible = possible;

		Renderer rend = this.transform.gameObject.GetComponentInChildren<Renderer> ();
//			GetComponent<Renderer>();
        if (teleportPossible)
        {
            if (rend != null)
            {
                rend.material = normal;
            }
        }
        else
        {
            if (rend != null)
            {
                rend.material = notPossible;
            }
        }
    }
}
