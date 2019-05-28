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
		
		// Check if we have reached an achievment target.
		switch (spidersSprayedCount)
		{
			case 50:
				GooglePlayManager.UnlockAchievement(GPGSIds.achievement_50_spiders_sprayed);
				break;
			case 100:
				GooglePlayManager.UnlockAchievement(GPGSIds.achievement_100_spiders_sprayed);
				break;
			case 250:
				GooglePlayManager.UnlockAchievement(GPGSIds.achievement_250_spiders_sprayed);
				break;
			case 500:
				GooglePlayManager.UnlockAchievement(GPGSIds.achievement_500_spiders_sprayed);
				break;
			case 1000:
				GooglePlayManager.UnlockAchievement(GPGSIds.achievement_1000_spiders_sprayed);
				break;
			default:
				break;
		}
		
		//	set the player prefs SpidersSquished value to the new spidersSquishedcount.
		PlayerPrefs.SetInt("SpidersSprayed", spidersSprayedCount);

		//	Play the die SFX	
		SoundManager.Instance.DieSFX();
		//	Destroy the sprayed spider.
		Destroy(transform.parent.gameObject);
	}
}
