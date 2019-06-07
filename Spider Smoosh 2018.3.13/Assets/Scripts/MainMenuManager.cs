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
	public GameObject rewardPanel;


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
		rewardPanel.SetActive(false);
		//	Change the info text to the default message.
		authInfoText.text = "You are not signed into the Google Play Services, would you like to try signing in?";
		// Run the Banner routine in the admanager script.
		CallBannerRoutine();
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
		rewardPanel.SetActive(true);
	}

	public void CloseRewardPanel()
	{
		rewardPanel.SetActive(false);
	}
	#endregion

	//	Start the Banner routine when this script is loaded.
	private void CallBannerRoutine()
	{
		//Debug.Log("RunBannerRoutine method being called from mainmenumanager");
		StartCoroutine(AdManager.Instance.ShowBannerWhenReady());
	}
}
