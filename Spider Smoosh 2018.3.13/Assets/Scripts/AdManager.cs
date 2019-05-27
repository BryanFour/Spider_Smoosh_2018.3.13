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
	[SerializeField] private bool testMode = true;
	[SerializeField] private string rewardedVideoPlacementID = "rewardedVideo";
	[SerializeField] private string regularPlacementID = "video";

	//	Error Debugging
	//public GameObject error1;
	//public GameObject error2;

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
		//error1.SetActive(false);
		//error2.SetActive(false);
	}
	
	public void RequestRegularAd(Action<ShowResult> callback)
	{
#if UNITY_ADS
		if (Advertisement.IsReady(regularPlacementID))
		{
			ShowOptions so = new ShowOptions();
			so.resultCallback = callback;
			Advertisement.Show(regularPlacementID, so);
		}
		else
		{
			Debug.Log("Ad not ready yet.");
		}
#else
		Debug.Log("Ads not supported");
#endif
	}
	
	/*
	public void RequestRegularAd()
	{
#if UNITY_ADS
		if (Advertisement.IsReady(regularPlacementID))
		{
			//ShowOptions so = new ShowOptions();
			//so.resultCallback = callback;
			Advertisement.Show(regularPlacementID);
		}
		else
		{
			Debug.Log("Ad not ready yet.");
		}
#else
		Debug.Log("Ads not supported");
#endif
	}
	*/

	public void RequestRewardedAd(Action<ShowResult> callback)
	{
#if UNITY_ADS
		if (Advertisement.IsReady(rewardedVideoPlacementID))
		{
			ShowOptions so = new ShowOptions();
			so.resultCallback = callback;
			Advertisement.Show(rewardedVideoPlacementID, so);
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
		RequestRegularAd(OnAdClosed);
		//RequestRegularAd();
	}
	
	public void PlayRewardedAd()
	{
		RequestRewardedAd(OnRewardedAdClosed);
	}

	private void OnAdClosed(ShowResult result)
	{
		Debug.Log("Regular ad closed");
	}
	
	private void OnRewardedAdClosed(ShowResult result)  ////// https://forum.unity.com/threads/unity-rewarded-ads-not-calling-back-first-time.608053/
	{
		Debug.Log("Rewarded ad closed");
		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log("Ad finished, reward player");
				MainMenuManager.Instance.RewardPlayer();
				break;
			case ShowResult.Skipped:
				Debug.Log("Ad skipped, no reward");
				//error1.SetActive(true);
				break;
			case ShowResult.Failed:
				Debug.Log("Ad failed");
				//error2.SetActive(true);
				break;
		}
	}
	
	//	Error debugging stuff.
	public void CloseErrorOne()
	{
		//error1.SetActive(false);
	}

	public void CloseErrorTwo()
	{
		//error2.SetActive(false);
	}
}
