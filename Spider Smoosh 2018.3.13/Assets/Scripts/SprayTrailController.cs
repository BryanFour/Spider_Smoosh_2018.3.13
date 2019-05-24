using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayTrailController : MonoBehaviour
{
	//	The gameobject/position we want the spray cloud / spray trail / poison to follow.
	public Transform nozzlePosition;

    void Update()
    {

		transform.position = nozzlePosition.position;
    }
}
