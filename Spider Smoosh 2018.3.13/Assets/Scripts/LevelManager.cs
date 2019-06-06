using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	// GameManager instance Stuff.
	public static LevelManager Instance { get; set; }
	
	//	----- Game Play UI Stuff.
	//	The count down text componant.
	public TextMeshProUGUI countDownText;
	//	Spray Count Stuff.
	public TextMeshProUGUI sprayCountText;
	
	//	----- Timer Stuff.
	//	The Timer text componant.
	public TextMeshProUGUI timerText;
	//	How long the player has been playing not including the countdown.
	private float timePlayed;
	//	How long the app has been running for.
	private float timeSinceStart;
	//	How long the countdown is.
	private const float PRE_START_COUNTDOWN = 4;
	//	A bool to tell weather the countdown has finished or not.
	[HideInInspector] public bool countDownHasFinished = false;
	//	When the game started / when the player starts playing, spiders come out, timers starts ect.
	[HideInInspector] public float gamePlayHasStarted;

	//	The players current score (How long the player lasted with dieing).
	private float currentScore;

	//	The speed the spiders start with, to be increased in the coroutine.
	[HideInInspector] public float spiderSpeed = 1f;
	
	//	----- Tutorial stuff
	//	Get access to the tutorial game object.
	public GameObject tutorialPanel;

	//	----- Game Over Stuff
	//	Bool to tell weather the game is over or not.
	[HideInInspector] public bool gameOver = false;
	// The Game over panel.
	public GameObject gameOverPanel;
	//	Everything that has to be disabled when game is over
	public GameObject[] disableOnGameOverArray;
	//	The amount of spiders squished throughout all the games played
	private int squishedLifetimeBeforePlay;
	//	The amount of spiders squished this game
	private int squishedThisGame;
	//	The amount of spiders sprayed throughout all the games played
	private int sprayedLifetimeBeforePlay;
	//	The amount of spiders sprayed this game
	private int sprayedThisGame;
	//	The Squished this game text componant.
	public TextMeshProUGUI squishedThisGameText;
	//	Lifetime squishes after play
	public TextMeshProUGUI squishedLifetimeAfterPlayText;
	//	The Squished this game text componant.
	public TextMeshProUGUI sprayedThisGameText;
	//	Lifetime squishes after play
	public TextMeshProUGUI sprayedLifetimeAfterPlayText;
	//	New High Score text.
	public TextMeshProUGUI newHighScore;
	//	Bool to tell weather the game is over or not.

	void Awake()
	{   // LevelManager instance Stuff.
		//Check if instance already exists
		if (Instance == null)
		{
			//if not, set instance to this
			Instance = this;
		}
		//If instance already exists and it's not this:
		else if (Instance != this)
		{

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a LevelManager.
			Destroy(gameObject);

		}
	}

	void Start()
    {
		//	Start the BackGround SFX if it isnt already playing.
		if (SoundManager.Instance.bgMusicIsPlaying == false)
		{
			SoundManager.Instance.StartBgMusic();
		}
		//	Get the amount of spiders squished throughout all the games played before the level is played.
		squishedLifetimeBeforePlay = PlayerPrefs.GetInt("SpidersSquished");
		//	Get the amount of spiders sprayed throughout all the games played before the level is played.
		sprayedLifetimeBeforePlay = PlayerPrefs.GetInt("SpidersSprayed");

		//	Set the gameOver bool to false at runtime.
		gameOver = false;

		//Disable the new high score text at runtime.
		newHighScore.gameObject.SetActive(false);

		// Disable the tutorial panal at runtime
		tutorialPanel.SetActive(false);

		//	Disable the game over panel at runtime.
		gameOverPanel.SetActive(false);
		
		//	Set the spray count text to the player prefs spray count value.
		sprayCountText.text = PlayerPrefs.GetInt("SprayCount", 0).ToString();
		
		//	Find out how low the app has been running for, used in later calculations.
		timeSinceStart = Time.time;

		//	Disable the count down text at runtime.
		countDownText.gameObject.SetActive(false);

		//	Forloop to enable everything that gets disabled when game is over
		for (int i = 0; i < disableOnGameOverArray.Length; i++)
		{
			disableOnGameOverArray[i].SetActive(true);
		}
		//	Start the countdown coroutine
		StartCoroutine(CountDown());

		// Run the Banner routine in the admanager script.
		HideBanner();
	}

    void Update()
	{
		//Debug.Log(Time.timeScale);
		//	Timer and time formatting stuff.
		//	Exit out of this method if the pre start countdown hasnt finished
		if (Time.time - timeSinceStart < PRE_START_COUNTDOWN)
		{
			countDownHasFinished = false;
			return;
		}
		else
		{
			countDownHasFinished = true;
		}
		//	Keep track of how long the player has been playing.
		timePlayed = Time.time - (timeSinceStart + PRE_START_COUNTDOWN);
		
		//	Old Format
		//	Format the timer time.
		//string minutes = ((int)timePlayed / 60).ToString("00"); // Used to have the timer show in seconds and minutes rather that just seconds.
		//string seconds = (timePlayed % 60).ToString("00.00"); // Used to have the timer show in seconds and minutes rather that just seconds.
		//	Old Format End.
		
		//	Format the timer to show in minutes and seconds.
		string minutes = Mathf.Floor(timePlayed / 60).ToString("00");
		string seconds = Mathf.Floor(timePlayed % 60).ToString("00");
		//	Display the time.
		timerText.text = minutes + ":" + seconds;

	}

	public void UpdateSprayCount()
	{
		//	Set the spray count text to the player prefs spray count value.
		sprayCountText.text = PlayerPrefs.GetInt("SprayCount", 0).ToString();

	}

	#region Open / Close Tutorial
	//	Open the tutorial
	public void OpenSprayTutorial()
	{
		Time.timeScale = 0;
		//	Enable the tutorial panel
		tutorialPanel.SetActive(true);
	}

	public void CloseSprayTutorial()	// ----- Dont forget to call this method from the close spray tutorial button.
	{
		Time.timeScale = 1;
		//	Disable the tutorial panel
		tutorialPanel.SetActive(false);
		//	Create a key to tell if the tutorial has been opened before
		PlayerPrefs.SetString("HasSprayedBefore", "Yes");
		//	Play the button SFX
		SoundManager.Instance.ButtonSFX();
	}
	#endregion

	public void GameOver()
	{
		#region Ad Stuff
		int gameOvers = PlayerPrefs.GetInt("GameOvers", 0);
		if(gameOvers < 2)
		{
			gameOvers++;
			PlayerPrefs.SetInt("GameOvers", gameOvers);
		}
		else if(gameOvers >= 2)
		{
			PlayerPrefs.SetInt("GameOvers", 0);
			AdManager.Instance.PlayRegularAd();
		}
		#endregion

		#region Squished Display Stuff
		//	Get the amount of spiders squished this game.
		squishedThisGame = PlayerPrefs.GetInt("SpidersSquished") - squishedLifetimeBeforePlay;
		//	Display the amount of spiders squished this game.
		squishedThisGameText.text = squishedThisGame.ToString();
		//	Get the new amount of spiders squished over lifetime.
		squishedLifetimeAfterPlayText.text = PlayerPrefs.GetInt("SpidersSquished").ToString();
		#endregion

		#region Sprayed Display Stuff
		//	Get the amount of spiders sprayed this game.
		sprayedThisGame = PlayerPrefs.GetInt("SpidersSprayed") - sprayedLifetimeBeforePlay;
		//	Display the amount of spiders sprayed this game.
		sprayedThisGameText.text = sprayedThisGame.ToString();
		//	Get the new amount of spiders sprayed over lifetime.
		sprayedLifetimeAfterPlayText.text = PlayerPrefs.GetInt("SpidersSprayed").ToString();
		#endregion

		//	Create a score from the time played value.
		currentScore = timePlayed;
		//	If the current score is higher than the players high score...
		if (currentScore > PlayerPrefs.GetFloat("HighScore", 0))
		{
			//	Set the high score to the current score
			PlayerPrefs.SetFloat("HighScore", currentScore);
			//	Start the new high score text coroutine.
			StartCoroutine(NewHighScore());
		}

		//	Forloop to enable everything that gets disabled when game is over
		for (int i = 0; i < disableOnGameOverArray.Length; i++)
		{
			disableOnGameOverArray[i].SetActive(false);
		}

		//	Set the time scaleto 0 / Pause everything.
		//Time.timeScale = 0; ------- High Score anim cant play while Time.time == 0

		//	Enablethe game over panel
		gameOverPanel.SetActive(true);
		//	Set the game over bool to true.
		gameOver = true;
		//	Post high score to the leaderboard.
		PostToLeaderboard();
	}
	
	private void PostToLeaderboard()
	{
		
		//	Convert the currentScore from a float to a int
		int score = (int)currentScore;
		//	Convert the score int to a long called scoreToPost. ---- not realy converting (Long to Int dosnt seem to need converting.)
		long scoreToPost = score * 1000;
		//	Post the score to the leaderboard.
		GooglePlayManager.PostToLeaderboard(scoreToPost);
	}
	
	IEnumerator CountDown()
	{   
		countDownText.gameObject.SetActive(true);
		countDownText.text = 3.ToString();
		SoundManager.Instance.PlayCountDownSFX();
		yield return new WaitForSecondsRealtime(1);
		countDownText.text = 2.ToString();
		SoundManager.Instance.PlayCountDownSFX();
		yield return new WaitForSecondsRealtime(1);
		countDownText.text = 1.ToString();
		SoundManager.Instance.PlayCountDownSFX();
		yield return new WaitForSecondsRealtime(1);
		countDownText.text = "GO";
		SoundManager.Instance.PlayCountDownSFX();
		yield return new WaitForSecondsRealtime(1);
		countDownText.gameObject.SetActive(false);

		//	Store the Time.time value when gameplay started after the countdown finishes.
		gamePlayHasStarted = Time.time;
		//	Start the spider speed increase coroutine after the countdown finishes.
		StartCoroutine(SpiderSpeed());
		//	Start the SprayLastSpawned coroutine after the countdown finishes.
		StartCoroutine(SprayLastSpawned());
	}

	IEnumerator SpiderSpeed()
	{
		if(spiderSpeed < 2)
		{	//	If the spiders speed is less than 2
			yield return new WaitForSecondsRealtime(15);
			//	Increase the spiders speed by 0.5f
			spiderSpeed = spiderSpeed + 0.5f;
			//	Restart the routine.
			StartCoroutine(SpiderSpeed()); 
		}
		else if(spiderSpeed >= 2)
		{	//	if the spiders speed is greater than 2
			yield return new WaitForSecondsRealtime(15);
			//	Increase the spiders speed by 0.1f
			spiderSpeed = spiderSpeed + 0.1f;
			//	Restart the routine.
			StartCoroutine(SpiderSpeed());
		}
	}

	IEnumerator SprayLastSpawned()
	{	
		//	If the game isnt over and the timescale = 1.
		if(gameOver == false && Time.timeScale == 1)
		{
			yield return new WaitForSecondsRealtime(1);
			float sprayLastSpawnedValue = PlayerPrefs.GetFloat("SprayLastSpawned", 0);
			//	Add the time played to the the player prefs "SprayLastSpawned value".
			float newSprayLastSpawnedValue = sprayLastSpawnedValue + 1;
			//	Change the player prefs "SprayLastSpawned" value to the newSprayLastSpawnedValue
			PlayerPrefs.SetFloat("SprayLastSpawned", newSprayLastSpawnedValue);
			StartCoroutine(SprayLastSpawned());
		}
		else
		{	//	If the timescale isnt set to 1 (the game is paused/frozen) and the game isnt over.
			yield return new WaitForSecondsRealtime(1);
			//	Restart this routine.
			StartCoroutine(SprayLastSpawned());
		}
		
	}

	IEnumerator NewHighScore()
	{	//	If the player beats there high score, enable the New High Score text
		newHighScore.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(1.2f);
		//	Disable the new high score text
		newHighScore.gameObject.SetActive(false);
	}

	//	Run the HideBanner method when this script is loaded.
	private void HideBanner()
	{
		//Debug.Log("HideBanner method being called from Levelmanager");
		AdManager.Instance.Hidebanner();
	}

	#region Scene Loading Stuff
	public void LoadMainMenu()
	{   //	Play the button SFX
		SoundManager.Instance.ButtonSFX();
		//	Load the main menu scene.	
		SceneManager.LoadScene(1);
	}

	public void LoadLevel()
	{
		if(AdManager.Instance.adIsPlaying == false)
		{
			//	Play the button SFX
			SoundManager.Instance.ButtonSFX();
			//	Load the level scene.
			SceneManager.LoadScene(2);
		}
	}
	#endregion

	#region Debugging Buttons
	public void DeleteSprayKey()
	{
		PlayerPrefs.DeleteKey("SprayCount");
		//	Set the spray count text to the player prefs spray count value.
		sprayCountText.text = PlayerPrefs.GetInt("SprayCount", 0).ToString();
	}

	public void DeleteTutorialKey()
	{
		PlayerPrefs.DeleteKey("HasSprayedBefore");
		Debug.Log("Tutorial Key Deleted");
	}

	public void DeleteHSKey()
	{
		PlayerPrefs.DeleteKey("HighScore");
		Debug.Log("High Score Key Deleted");
	}

	public void AddSprays()
	{
		int sprayCount = PlayerPrefs.GetInt("SprayCount");
		sprayCount += 1;
		PlayerPrefs.SetInt("SprayCount", sprayCount);
	}

	public void DeleteAllKeys()
	{
		PlayerPrefs.DeleteAll();
		Debug.Log("All Keys Deleted");
	}
	#endregion
}
