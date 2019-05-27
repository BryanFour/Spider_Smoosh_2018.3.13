using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
	//	-----	Manual Auth Stuff.
	//	Manual GPS Auth Panel.
	public GameObject authPanel;
	//	Yes button
	public GameObject authYesbutton;
	//	No Button
	public GameObject authNoButton;
	//	The auth info text object.
	public TextMeshProUGUI authInfoText;

	public void Start()
	{
		//	Disable the auth panael at runtime.
		authPanel.SetActive(false);
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

		//	Disable the Yes/No buttons.
		authYesbutton.SetActive(false);
		authNoButton.SetActive(false);

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
		//	Make sure the buttons are enabled.
		authYesbutton.SetActive(true);
		authNoButton.SetActive(true);
	}

	IEnumerator AuthSuccess()
	{
		//	Change the info text.
		authInfoText.text = "Successfully signed into Google Play Services.";
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
		//	Change the text.
		authInfoText.text = "Unable to sign into the Google Play Services, would you like to try again?";
	}
}
