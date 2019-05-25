
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

    void Start()
    {
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;

		AuthenticateUser();    
    }
	
	void AuthenticateUser()
	{
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.Activate();
		Social.localUser.Authenticate((bool success) =>
		{
			if (success == true)
			{
				Debug.Log("Logged into Google Play Games Services");
				SceneManager.LoadScene("MainMenu");
			}
			else
			{
				Debug.LogError("Unable to sign into Google Play Services");
				signInText.text = "Could not sign into Google Play Services";
				signInText.color = Color.red; //<------------------------------------ Not working, why???
				StartCoroutine(LoadMainMenuAfterFail());
			}
		});
	}

	IEnumerator LoadMainMenuAfterFail()
	{
		yield return new WaitForSecondsRealtime(2);
		SceneManager.LoadScene("MainMenu");
	}

	public static void PostToLeaderboard(long newScore)
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

	public static void ShowLeaderboardUI()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_high_score);
	}

	public static void ShowAchievementUI()
	{
		if (Social.localUser.authenticated)
		{
			Social.ShowAchievementsUI();
		}
	}

	public static void UnlockAchievement(string achievementID)
	{
		Social.ReportProgress(achievementID, 100.0f, (bool success) =>
		{
			Debug.Log("Achievement Unlocked " + success.ToString());
		});
	}
}
