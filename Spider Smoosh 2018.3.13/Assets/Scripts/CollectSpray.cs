using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSpray : MonoBehaviour
{
	//	The amount of sprays the player has.
	private int sprayCount;
	//	The speed the collectable spray cans fall.
	private float fallSpeed = 5;

	void Start()
    {
		//	Get the amount of sprays the player has from the PlayerPrefs.
		sprayCount = PlayerPrefs.GetInt("SprayCount");
	}

    void Update()
    {
		//	make the collectable sprays "fall" along the z-axis relative to the world space.
		transform.Translate(new Vector3(0, 0, -fallSpeed) * Time.deltaTime, Space.World);

		//	----- Collect Spray Can Stuff.
		//	If the player is touching the screen in atleast 1 place and isnt dragging there finger
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			if (Physics.Raycast(ray, out hit))
				if (hit.collider.gameObject.tag == "CollectableSpray")
				{   //	Add 1 to the spray count.
					sprayCount += 1;
					//	Save the playerprefs
					PlayerPrefs.SetInt("SprayCount", sprayCount);
					//	Update the Spray Count Text.
					LevelManager.Instance.UpdateSprayCount();
					//	Play the collect spray SFX (The Rattle).
					SoundManager.Instance.SprayRattleSFX();
					//	Destroy the collected spray can.
					Destroy(hit.transform.gameObject);
				}
		}
		//	If the collectabe cans position is less than or equal to -10 on the z-axis, destroy the can because its off screen.
		if(transform.position.z <= -10)
		{
			Destroy(gameObject);
		}
	}
	/*
	// Collect then Destroy with mouse click --DEBUG Input--
	public void OnMouseDown()
	{
		//	Add 1 to the spray count.
		sprayCount += 1;
		//	Save the playerprefs
		PlayerPrefs.SetInt("SprayCount", sprayCount);
		//	Update the Spray Count Text.
		LevelManager.Instance.UpdateSprayCount();
		//	Play the collect spray SFX (The Rattle).
		SoundManager.Instance.SprayRattleSFX();
		//	Destroy the collected spray can.
		Destroy(gameObject);
	}
	
	*/
}
