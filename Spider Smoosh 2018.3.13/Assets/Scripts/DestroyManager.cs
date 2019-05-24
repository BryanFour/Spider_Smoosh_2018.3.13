using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//	Destroy on touch - https://www.youtube.com/watch?v=SMxHtIKDfV4

public class DestroyManager : MonoBehaviour
{
	//	The green blood splatter particle system prefab.
	public GameObject bloodSplatter;
	//	Spiders squished count
	private int spidersSquishedCount;

	void Update()
    {
		//	If we are touching and not dragging.
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			if (Physics.Raycast(ray, out hit))												//------ EVERYTHING INSIDE THIS IF STATEMENT WILL ONLY HAPPEN IF WE TOUCH THE SCREEN... MOUSE CLICKS DO NOTHING!!!!!!!!!
				if (hit.collider.gameObject.tag == "Spider" && LevelManager.Instance.gameOver == false)
				{
					//	Play the SquishSFX/
					SoundManager.Instance.PlaySquishSFX();
					//	Instanciate the splatterFX
					Instantiate(bloodSplatter, hit.transform.position, bloodSplatter.transform.rotation);
					//	get the amount of spiders squished.
					spidersSquishedCount = PlayerPrefs.GetInt("SpidersSquished", 0);
					//	Add 1 to the spider squished count
					spidersSquishedCount += 1;
					//	set the player prefs SpidersSquished value to the new spidersSquishedcount.
					PlayerPrefs.SetInt("SpidersSquished", spidersSquishedCount);
					//	Destroy the object that was hit by the ray
					Destroy(hit.transform.gameObject);
				}
		}
	}

}
