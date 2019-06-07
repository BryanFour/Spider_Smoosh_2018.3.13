
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
// https://www.youtube.com/watch?v=M6nwu00-VR4 - Leaderboard
// https://www.youtube.com/watch?v=XN29LTFtk3c - Achievments


public class GooglePlayManager : MonoBehaviour
{
	public TextMeshProUGUI signInText;
	//	The close button.
	public GameObject closeButton;
	//	AudioSource for the Cancel Button
	private AudioSource buttonAudioSource;
	//	The Button SFX Audio Clip.
	public AudioClip buttonSFX;

	void Start()
    {
		//	Create the AudioSource for the button SFX.
		buttonAudioSource = gameObject.AddComponent<AudioSource>();

		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;

		AuthenticateUser();    
    }
	
	void AuthenticateUser()
	{
		//	If the user dose not have internet connectivity.
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			signInText.text = "Unable to sign into Google Play Services, To use Google Play features please enable your mobile data or wifi.";
			closeButton.SetActive(true);
		}
		//	If the user has internet connectivity.
		else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
			PlayGamesPlatform.InitializeInstance(config);
			PlayGamesPlatform.Activate();

			//	Try to authenticate the user (Sign them into GPS).
			Social.localUser.Authenticate((bool success) =>
			{
				//	If the authentication was successfull.
				if (success == true)
				{
					Debug.Log("Logged into Google Play Games Services");
					signInText.text = "Sign in successful.";
					closeButton.SetActive(false);
					StartCoroutine(LoadMainMenu());
				}
				//  If the authentication failed.
				else if (success == false)
				{
					Debug.LogError("Unable to sign into Google Play Services");
					signInText.text = "Could not sign into Google Play Services";
					signInText.color = Color.red; //<------------------------------------ Not working, why???
					closeButton.SetActive(false);
					StartCoroutine(LoadMainMenu());
				}
				//	If the User is already signed in.
				else if (PlayGamesPlatform.Instance.IsAuthenticated())   //<------------------ Remove me if it breaks everything.
				{
					Debug.LogError("Unable to sign in, User is already authenticated.");
					signInText.text = "You are already signed into Google Play Services.";
					closeButton.SetActive(false);
					StartCoroutine(LoadMainMenu());
				}
			});
		}
	}

	IEnumerator LoadMainMenu()
	{
		yield return new WaitForSecondsRealtime(2);
		SceneManager.LoadScene("MainMenu");
	}

	public static void PostToLeaderboard(long newScore)
	{
		if (Social.localUser.authenticated)
		{
			Social.ReportScore(newScore, GPGSIds.leaderboard_high_score, (bool success) =>
			{
				if (success)
				{
					Debug.Log("Posted new score to leaderboard.");
				}
				else
				{
					Debug.Log("Unable to post new score to leaderboard.");
				}
			});
		}
	}

	public static void ShowLeaderboardUI()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_score);
	}

	public static void ShowAchievementUI()
	{
		Social.ShowAchievementsUI();
	}

	public static void UnlockAchievement(string achievementID)
	{
		if (Social.localUser.authenticated)
		{
			Social.ReportProgress(achievementID, 100.0f, (bool success) =>
			{
				Debug.Log("Achievement Unlocked " + success.ToString());
			});
		}
	}

	public static void SignOut()
	{
		PlayGamesPlatform.Instance.SignOut();
	}

	public void CloseButton()
	{
		Debug.Log("Cancel Button Pressed");
		//	Play the button SFX
		buttonAudioSource.PlayOneShot(buttonSFX);
		//	Load the level scene.
		SceneManager.LoadScene(1);
	}
}
