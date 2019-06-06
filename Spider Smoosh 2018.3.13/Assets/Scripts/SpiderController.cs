using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
	private Vector3 targetPosition;
	//	Create a float to hold the LevelManagers gamePlayHasStarted value.
	private float gamePlayHasStarted;       //---------------------Dont seem to be using this.
	//	The float to be given the speed from the LevelManagers spider speed coroutine.
	private float spiderSpeed;
	//	Get access to the spider animation clip, Drop the spider prefab in here to get it.
	public Animation spiderAnim;
	//	The green blood splater partical system prefab. Only used for the debugging onmouse down event.
	public GameObject bloodSplatter;
	//	Spiders squished count
	private int spidersSquishedCount;


	void Start()
    {
		//	Get the gamePlayHasStarted value from the level managers GamePlayHasStarted.
		gamePlayHasStarted = LevelManager.Instance.gamePlayHasStarted;	//---------------------Dont seem to be using this.
		//	Create a vector3 (Where the lady bug will be.)
		targetPosition = new Vector3(0, 0, 0);
		//	Have the spider face the lady bug/Target Position on creation.
		transform.LookAt(targetPosition);
	}

	void Update()
	{	//	If the count down has not finished, exit out of this method.
		if (LevelManager.Instance.countDownHasFinished == false)
		{
			return;
		}
		//	If a spider reaches the target position.
		if (transform.position == targetPosition)
		{
			//	Stop the spray can sound if its playing.
			SoundManager.Instance.StopSpraySFX();
			//	Run the game over method in the level manager.
			LevelManager.Instance.GameOver();
			//	Destroy the spider so the game over method dosnt loop
			Destroy(gameObject);
		}
		//	If the game isnt over. 
		if (LevelManager.Instance.gameOver == false)
		{
			//	Get the spiders speed from the LevelManagers spider speed coroutine.
			spiderSpeed = LevelManager.Instance.spiderSpeed;
			//	Set the spiders animation clips speed to the same value as the spider move speed.
			spiderAnim["walk"].speed = spiderSpeed;
			// Calculate the distance to move/step.
			float step = spiderSpeed * Time.deltaTime;
			// Move towards the lady bug/Target Position.
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
		}
		//	If the game is over. 
		else if(LevelManager.Instance.gameOver == true)
		{
			//	Stop the animation from playing by setting the anim speed to 0.
			spiderAnim["walk"].speed = 0;
		}
	}
	/*
	// Destroy with mouse click --DEBUG Input--
	private void OnMouseDown()
	{
		if(LevelManager.Instance.gameOver == false)
		{
			SoundManager.Instance.PlaySquishSFX();
			//	Instanciate the splatterFX
			Instantiate(bloodSplatter, gameObject.transform.position, bloodSplatter.transform.rotation);
			//	get the amount of spiders squished.
			spidersSquishedCount = PlayerPrefs.GetInt("SpidersSquished", 0);
			//	Add 1 to the spider squished count
			spidersSquishedCount += 1;
			//	set the player prefs SpidersSquished value to the new spidersSquishedcount.
			PlayerPrefs.SetInt("SpidersSquished", spidersSquishedCount);
			//	Destroy the object that was hit by the ray
			Destroy(gameObject);
		}
	}
	*/
}
