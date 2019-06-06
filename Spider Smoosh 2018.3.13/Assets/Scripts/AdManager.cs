using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
//https://www.youtube.com/watch?v=XHTTjyRzxmw

public class AdManager : MonoBehaviour
{
	public static AdManager Instance;

	[Header("Config")]
	[SerializeField] private string gameID = "3149495";
	[SerializeField] private bool testMode = false;
	[SerializeField] private string rewardedVideoPlacementID = "rewardedVideo";
	[SerializeField] private string regularPlacementID = "video";
	private string bannerAdID = "bannerAd";

	//	Bool to tell other scripts that an a is playing.
	[HideInInspector]
	public bool adIsPlaying = false;			//---- Probs not needed.

	void Awake()
	{
		#region Instance Stuff
		// GameManager instance Stuff.
		//Check if instance already exists
		if (Instance == null)
		{
			//if not, set instance to this
			Instance = this;
		}
		//If instance already exists and it's not this:
		else if (Instance != this)
		{

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);

		}
		DontDestroyOnLoad(gameObject);
		// GameManager instance Stuff End.
		#endregion

		Advertisement.Initialize(gameID, testMode);
	}

	private void Start()
	{
		//StartCoroutine(ShowBannerWhenReady());  --- Now ran from the mainmenu script.
	}

	#region Regular Ad.
	public void RequestRegularAd(Action<ShowResult> callback)
	{
#if UNITY_ADS
		if (Advertisement.IsReady(regularPlacementID))
		{
			ShowOptions so = new ShowOptions();
			so.resultCallback = callback;
			Advertisement.Show(regularPlacementID, so);
			//	Stop the Backgroud SFX from playing.
			SoundManager.Instance.StopBgMusic();
		}
		else
		{
			Debug.Log("Ad not ready yet.");
		}
#else
		Debug.Log("Ads not supported");
#endif
	}

	public void PlayRegularAd()
	{   
		//	Change the ad is playing bool to true.
		adIsPlaying = true;
		//	Start the CoRoutine that waits for 1 seconds before playing the ad, we do this to allow the new high score text to be shown before playing the ad.	
		StartCoroutine(RegularAdWaitTime());
	}

	IEnumerator RegularAdWaitTime()
	{
		//	Wait 1 second before playing the regular ad.
		yield return new WaitForSecondsRealtime(1);
		RequestRegularAd(OnAdClosed);
	}

	private void OnAdClosed(ShowResult result)
	{
		//Debug.Log("Regular ad closed");
		adIsPlaying = false;
		//	Start the Background SFX.
		SoundManager.Instance.StartBgMusic();
	}
	#endregion

	#region Rewarded Ad.
	public void RequestRewardedAd(Action<ShowResult> callback)
	{
#if UNITY_ADS
		if (Advertisement.IsReady(rewardedVideoPlacementID))
		{
			ShowOptions so = new ShowOptions();
			so.resultCallback = callback;
			Advertisement.Show(rewardedVideoPlacementID, so);
			//	Change the ad is playing bool to true.
			adIsPlaying = true;
			//	Stop the Backgroud SFX from playing.
			SoundManager.Instance.StopBgMusic();
		}
		else
		{
			Debug.Log("Ad not ready yet.");
		}
#else
		Debug.Log("Ads not supported");
#endif
	}
	public void PlayRewardedAd()
	{
		RequestRewardedAd(OnRewardedAdClosed);
	}

	private void OnRewardedAdClosed(ShowResult result)
	{
		//Debug.Log("Rewarded ad closed");
		switch (result)
		{
			case ShowResult.Finished:
				//Debug.Log("Ad finished, reward player");
				MainMenuManager.Instance.RewardPlayer();
				adIsPlaying = false;
				//	Start the Background SFX.
				SoundManager.Instance.StartBgMusic();
				break;
			case ShowResult.Skipped:
				//Debug.Log("Ad skipped, no reward");
				adIsPlaying = false;
				//	Start the Background SFX.
				SoundManager.Instance.StartBgMusic();
				break;
			case ShowResult.Failed:
				//Debug.Log("Ad failed");
				adIsPlaying = false;
				//	Start the Background SFX.
				SoundManager.Instance.StartBgMusic();
				break;
		}
	}
	#endregion



	public IEnumerator ShowBannerWhenReady()
	{   /*
		//	While the adverticement is not ready.
		while (!Advertisement.IsReady(bannerAdID))
		{
			yield return new WaitForSeconds(0.05f);
		}
		//	If the adverticement is ready.
		Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
		Advertisement.Banner.Show(bannerAdID);
		*/
		//	While the adverticement is not ready.
		while (!Advertisement.IsReady(bannerAdID))
		{
			yield return new WaitForSeconds(0.05f);
		}
		if (Advertisement.Banner.isLoaded || Advertisement.IsReady(bannerAdID))  //---- https://forum.unity.com/threads/can-i-make-banner-ads-optional-for-the-player.594811/#post-4143193
		{
			//Debug.Log("Showing banner");
			Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
			Advertisement.Banner.Show(bannerAdID);
		}
		else
		{
			//Debug.Log("Bannner not ready");
			Advertisement.Banner.Load(bannerAdID);
			yield return new WaitForSeconds(0.05f);
			StartCoroutine(ShowBannerWhenReady());
		}
		
	}

	public void Hidebanner()
	{
		Advertisement.Banner.Hide();
	}
}
