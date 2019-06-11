using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
	public static MainMenuManager Instance;

	//	-----	Manual Auth Stuff.
	//	Manual GPS Auth Panel.
	public GameObject authPanel;
	//	Yes button
	public GameObject authYesbutton;
	//	No Button
	public GameObject authNoButton;
	//	Close Button
	public GameObject authPanelCloseButton;
	//	The auth info text object.
	public TextMeshProUGUI authInfoText;

	//	The Pannel that is show after watching a rewarded video. (Reward Recived panel.)
	public GameObject videoRewardPanel;

	//	Rate Stuff.
	public GameObject ratePanel;
	public GameObject rateRewardPanel;
	private bool userHasRated = false;
	private bool remind = false;
	private int currentCount;

	public void Awake()
	{
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
	}
	public void Start()
	{
		//	Start the BackGround SFX if it isnt already playing.
		if(SoundManager.Instance.bgMusicIsPlaying == false)
		{
			SoundManager.Instance.StartBgMusic();
		}
		//	Disable the auth panael at runtime.
		authPanel.SetActive(false);
		//	Disable the reward panel at runtime.
		videoRewardPanel.SetActive(false);
		//	Change the info text to the default message.
		authInfoText.text = "You are not signed into the Google Play Services, would you like to try signing in?";
		// Run the Banner routine in the admanager script.
		CallBannerRoutine();

		//	Rate Stuff.
		//	Make sure the rate and rate reward panel is disabled.
		ratePanel.SetActive(false);
		rateRewardPanel.SetActive(false);
		//	Run the rate checker.
		RateChecker();
	}

	//	Method for the show leaderboard button.
	public void ShowLeaderboard()
	{
		SoundManager.Instance.ButtonSFX();
		//	If the user is logged into google play services.
		if (Social.localUser.authenticated)
		{
			GooglePlayManager.ShowLeaderboardUI();
		}
		//  Ask the user if the would like to try logging in.
		else
		{
			//	Run the not signed in method.
			NoAuth();
		}
	}

	//	Method for the show achievements button.
	public void ShowAchievements()
	{
		SoundManager.Instance.ButtonSFX();
		//	If the user is logged into google play services.
		if (Social.localUser.authenticated)
		{
			GooglePlayManager.ShowAchievementUI();
		}
		//  Ask the user if the would like to try logging in.
		else
		{
			//	Run the not signed in method.
			NoAuth();
		}
	}

	#region Manual GPS authentication stuff.
	//	Method to attatch to the "Yes, I would like to try signing into GPS" button.
	public void AttemptManualAuthButton()
	{
		SoundManager.Instance.ButtonSFX();
		
		//	If the user has no internet connection.	
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{	
			//	Change the text.
			authInfoText.text = "Unable to sign into Google Play Services, To use Google Play features please enable your mobile data or wifi.";
			//	Disable the Yes/No Buttons
			authYesbutton.SetActive(false);
			authNoButton.SetActive(false);
			//	Enable the close button.
			authPanelCloseButton.SetActive(true);
		}
		//	If the user dose have internet conmnection.
		else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork || Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			//	Try to authenticate the user (Sign them into GPS).
			Social.localUser.Authenticate((bool success) =>
			{   //	If the authentication was successfull.
				if (success == true)
				{
					Debug.Log("Logged into Google Play Games Services");
					//	run routine to change the text to inform the user that the login was successfull then close the auth panel.
					StartCoroutine(AuthSuccess());
				}
				//  If the authentication failed.
				else
				{
					Debug.LogError("Unable to sign into Google Play Services");
					//	Run the auth failure method.
					AuthFailure();
				}
			});
		}	
	}

	//Method to attatch to the "No, I would not like to try signing into GPS" button.
	public void CloseAuthPanelButton()
	{
		SoundManager.Instance.ButtonSFX();
		//	Close the Auth Panel
		authPanel.SetActive(false);
	}
	#endregion

	private void NoAuth()
	{
		//	Enable the Manual Auth Panel.
		authPanel.SetActive(true);
		//	Change the info text.
		authInfoText.text = "You are not signed into the Google Play Services, would you like to try signing in?";
		//	Make sure the Yes/No buttons are enabled.
		authYesbutton.SetActive(true);
		authNoButton.SetActive(true);
		//	Disnable the close button.
		authPanelCloseButton.SetActive(false);
	}

	IEnumerator AuthSuccess()
	{
		//	Change the info text.
		authInfoText.text = "Successfully signed into Google Play Services.";
		//	Disable the Yes/No Buttons
		authYesbutton.SetActive(false);
		authNoButton.SetActive(false);
		//	Wait for 2 seconds.
		yield return new WaitForSeconds(2);
		//	Close the manual auth panel.
		authPanel.SetActive(false);
	}

	private void AuthFailure()
	{
		//	Enable the Yes/No Buttons
		authYesbutton.SetActive(true);
		authNoButton.SetActive(true);
		//	Disnable the close button.
		authPanelCloseButton.SetActive(false);
		//	Change the text.
		authInfoText.text = "Unable to sign into the Google Play Services, would you like to try again?";
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------
	#region Reward Video Stuff
	public void RewardPlayer()
	{
		int sprayCount = PlayerPrefs.GetInt("SprayCount", 0);
		//Debug.Log("Spray count was " + sprayCount);
		sprayCount++;
		//Debug.Log("Spray clount now is " + sprayCount);
		PlayerPrefs.SetInt("SprayCount", sprayCount);
		//Debug.Log("Spray count from player prefs is " + PlayerPrefs.GetInt("SprayCount", 0));
		videoRewardPanel.SetActive(true);
	}

	public void CloseRewardPanel()
	{
		videoRewardPanel.SetActive(false);
	}
	#endregion

	//	Start the Banner routine when this script is loaded.
	private void CallBannerRoutine()
	{
		//Debug.Log("RunBannerRoutine method being called from mainmenumanager");
		StartCoroutine(AdManager.Instance.ShowBannerWhenReady());
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------

	#region Rate Stuff.
	private void RateChecker()
	{
		//	If the user has not rated the app.
		if (PlayerPrefs.GetString("UserHasRated", "No") == "No")
		{
			//	Set the has rated bool to false.
			userHasRated = false;
			//	Get the current rate count.
			currentCount = PlayerPrefs.GetInt("CurrentRateCount", 0);
		}
		//	If the user has rated the app.
		else if (PlayerPrefs.GetString("UserHasRated", "No") == "Yes")
		{
			userHasRated = true;
		}

		//-----------------------------------------------------------------------------------------------

		//	If the user has asked to be reminded.  // "No" is the default value.
		if (PlayerPrefs.GetString("Remind", "No") == "Yes")
		{
			//	Set the remind bool to true.
			remind = true;
		}
		//	If the player has not asked to be reminded.  // "No" is the default value.
		else if (PlayerPrefs.GetString("Remind", "No") == "No")
		{
			//	Set the remind bool to false.
			remind = false;
		}

		//-----------------------------------------------------------------------------------------------

		//	If the user has rated the app.
		if (userHasRated == true)
		{
			return;
		}
		//	If the user has not rated the app and has not asked to be reminded and it not time to ask them to rate.
		else if (userHasRated == false && remind == false && currentCount <= 1)
		{
			//	Increment the current count by 1.
			currentCount++;
			//	Set the player prefs to the new current count.
			PlayerPrefs.SetInt("CurrentRateCount", currentCount);
		}
		//	If the user has not rated and has not asked to be reminded and it is time to ask the to rate.
		else if (userHasRated == false && remind == false && currentCount >= 2)
		{
			//	Enable the rate panel.
			ratePanel.SetActive(true);
		}
		//	If the user has not rated the app and has asked to be reminded and its not time to rate the app.
		else if (userHasRated == false && remind == true && currentCount <= 0)
		{
			//	Increment the current count by 1.
			currentCount++;
			//	Set the player prefs to the new current count.
			PlayerPrefs.SetInt("CurrentRateCount", currentCount);
		}
		//	If the user has not rated and has asked to be reminded and it is time to ask the to rate.
		else if (userHasRated == false && remind == true && currentCount >= 1)
		{
			//	Enable the rate panel.
			ratePanel.SetActive(true);
		}
	}

	public void YesRateButton()
	{   //	Play the button SFX
		SoundManager.Instance.ButtonSFX();
		//	Reward the player with 3 bug sprays.
		//	Get the current spray count
		int currentSprayCount = PlayerPrefs.GetInt("SprayCount");
		//Debug.Log("Current spray count is " + currentSprayCount);
		//	Add 3 to the current spray count.
		int newSprayCount = currentSprayCount + 3;
		//Debug.Log("Spray count afterthe reward is added to it is " + newSprayCount);
		//	Set the new spray count to the player prefs spray count.
		PlayerPrefs.SetInt("SprayCount", newSprayCount);
		//Debug.Log("The new spray count from the player prefs is " + PlayerPrefs.GetInt("SprayCount"));

		//	Disable the rate panel.
		ratePanel.SetActive(false);
		//	Set the UserHasRate player prefs string to "Yes"
		PlayerPrefs.SetString("UserHasRated", "Yes");
		//	Open the rate URL.
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.BurningHairStudios.SpiderSmoosh");
		//	Start the show rewward panel routine.
		StartCoroutine(OpenRewardPanel());
	}

	public void NoRateButton()
	{
		//	Play the button SFX
		SoundManager.Instance.ButtonSFX();
		//	Set the current rate count to 0.
		PlayerPrefs.SetInt("CurrentRateCount", 0);
		//	Set the player prefs remind sting to yes.
		PlayerPrefs.SetString("Remind", "Yes");
		//	Disable the rate panel.
		ratePanel.SetActive(false);
	}

	public void CloseRateRewardPanel()
	{
		//	Play the button SFX
		SoundManager.Instance.ButtonSFX();
		//	Close the rate reward panel.
		rateRewardPanel.SetActive(false);
	}

	IEnumerator OpenRewardPanel()
	{
		yield return new WaitForSecondsRealtime(1);
		rateRewardPanel.SetActive(true);
	}
	#endregion
	public void DeleteRateKey()
	{
		PlayerPrefs.DeleteAll();
	}
}
