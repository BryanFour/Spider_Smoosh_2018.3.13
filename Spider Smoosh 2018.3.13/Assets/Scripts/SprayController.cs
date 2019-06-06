using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
// Follow Mouse - https://www.youtube.com/watch?v=7OJQ6MbHuvQ

public class SprayController : MonoBehaviour
{
	//	Move With Mouse Stuff.
	private float actualDistance;
	//	Bool to stop players activating can while acan is already active
	[HideInInspector] public bool canActive = false;
	// How long we can spray for
	private float lengthOfSpray = 10;
	//	How long the spray cooldown is.
	private float sprayCoolDownLength = 5;
	//	where we are in the cooldown
	private int coolDownValue = 5;
	//	The poit in time that we started the cooldown
	private float coolDownStartTime;
	//	A bool to tell if we are on cooldown or not.
	private bool onCoolDown;
	//	Cans children Plus the spray trail gameobject
	public Transform[] childrenArray;
	//	CoolDown title Text componant
	public TextMeshProUGUI coolDownTitleText;
	//	CoolDown value text componant
	public TextMeshProUGUI coolDownValueText;
	//	Spray Button Text
	public TextMeshProUGUI sprayButtonText;
	//	Has the player sprayed before.
	private bool hasSprayedBefore = false;	

	void Start()
    {
		// Disable all the can parts at runtime.
		for (int i = 0; i < childrenArray.Length; i++)
		{
			childrenArray[i].gameObject.SetActive(false);
		}
		//	Trick the app into letting us use the spray immediately
		coolDownStartTime = Time.time - sprayCoolDownLength;

		//	----- Spray Button Text Stuff.
		//	Enable the spray button text at runtime.
		sprayButtonText.gameObject.SetActive(true);
		//	Disable the cooldown title text on runtime.
		coolDownTitleText.gameObject.SetActive(false);
		//	Disable the cooldown value text on runtime.
		coolDownValueText.gameObject.SetActive(false);

		//	Move With Mouse Stuff.
		Vector3 toObjectVevtor = transform.position - Camera.main.transform.position;
		Vector3 linearDistanceVector = Vector3.Project(toObjectVevtor, Camera.main.transform.forward);
		actualDistance = linearDistanceVector.magnitude;
	}

	void Update()
    {
		//	Move with Mouse Stuff.
		Vector3 mousePosition = Input.mousePosition;
		mousePosition.z = actualDistance;
		transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
	}

	public void ActivateCan()	// ----- Dont forget to call this method from the close spray tutorial button.
	{
		#region Has Sprayed Before
		//	If the player has sprayed before.	
		if (PlayerPrefs.HasKey("HasSprayedBefore") == true)
		{
			hasSprayedBefore = true;
		}
		//	If the player hasnt sprayed before
		else if (PlayerPrefs.HasKey("HasSprayedBefore") == false)
		{
			hasSprayedBefore = false;
		}
		#endregion

		// Play the button press SFX
		SoundManager.Instance.ButtonSFX();
		//	If the spray can is not active and we have atleast 1 in our inventory and the pre start countdown has finished and we are not on cooldown and the can is not active already.
		if (!canActive && PlayerPrefs.GetInt("SprayCount") > 0 && LevelManager.Instance.countDownHasFinished == true && onCoolDown == false)
		{	
			//	If the bug spray isn't on cooldown and we have atleast 1 in our inventory and the player has seen the spray tutorial.
			if (sprayCoolDownLength < Time.time - coolDownStartTime && hasSprayedBefore == true)
			{
				// Enable all the can parts
				for (int i = 0; i < childrenArray.Length; i++)
				{
					childrenArray[i].gameObject.SetActive(true);
				}
				//	Bool to stop players activating can while acan is already active
				canActive = true;
				//	PlayerPrefs Stuff.
				int sprayCount = PlayerPrefs.GetInt("SprayCount");
				sprayCount -= 1;
				PlayerPrefs.SetInt("SprayCount", sprayCount);
				LevelManager.Instance.UpdateSprayCount();
				//	Start the spray can SFX.
				SoundManager.Instance.StartSpraySFX();
				StartCoroutine(SprayDuration());
			}
			else
			{
				// If the player hasn't use the bug spray before, call the first time spraying method from the levelmanager
				LevelManager.Instance.OpenSprayTutorial();
			}
		}
	}

	IEnumerator SprayDuration()
	{	
		//	Wait for the spray to finish spraying (The length of the spray)
		yield return new WaitForSecondsRealtime(lengthOfSpray);
		//	Bool to stop players activating can while acan is already active
		canActive = false;
		//	After the can has finished spraying set the cooldown bool to true (We are now on cooldown).
		onCoolDown = true;
		//	The time that the cooldown started.
		coolDownStartTime = Time.time;
		// Disable all the can parts
		for (int i = 0; i < childrenArray.Length; i++)
		{
			childrenArray[i].gameObject.SetActive(false);
		}
		
		//	Disable the spray button text.
		sprayButtonText.gameObject.SetActive(false);
		//	Enable the cooldown title text.
		coolDownTitleText.gameObject.SetActive(true);
		//	Enable the cooldown value text.
		coolDownValueText.gameObject.SetActive(true);
		
		//	Start the cooldown coroutine.
		StartCoroutine(CoolDown());
	}

	IEnumerator CoolDown()
	{
		if(coolDownValue >= 1)
		{
			//	Display the cooldown value.
			coolDownValueText.text = coolDownValue.ToString();
			//	Decrement the cool down value by 1.
			coolDownValue -= 1;
			yield return new WaitForSecondsRealtime(1);
			//	Restart the cooldown routine after waiting 1 second.
			StartCoroutine(CoolDown());
		}
		else
		{
			//	Enable the spray button text.
			sprayButtonText.gameObject.SetActive(true);
			//	Disable the cooldown title text.
			coolDownTitleText.gameObject.SetActive(false);
			//	Disable the cooldown value text.
			coolDownValueText.gameObject.SetActive(false);
			
			//	Set the cooldownvalue to 5
			coolDownValue = 5;
			//	Set the cooldown bool to false
			onCoolDown = false;
		}
	}

	
}
