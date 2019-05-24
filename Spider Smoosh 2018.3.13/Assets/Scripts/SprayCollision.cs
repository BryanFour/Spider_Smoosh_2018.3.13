using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayCollision : MonoBehaviour
{
	//	Spiders sprayed count
	private int spidersSprayedCount;

	private void OnParticleCollision(GameObject col)
	{
		//	get the amount of spiders sprayed.
		spidersSprayedCount = PlayerPrefs.GetInt("SpidersSprayed", 0);
		//	Add 1 to the spider squished count
		spidersSprayedCount += 1;
		//	set the player prefs SpidersSquished value to the new spidersSquishedcount.
		PlayerPrefs.SetInt("SpidersSprayed", spidersSprayedCount);

		//	Play the die SFX	
		SoundManager.Instance.DieSFX();
		//	Destroy the sprayed spider.
		Destroy(transform.parent.gameObject);
	}
}
