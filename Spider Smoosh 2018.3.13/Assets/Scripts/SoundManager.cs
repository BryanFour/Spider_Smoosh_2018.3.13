using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
	// SoundManager instance Stuff.
	public static SoundManager Instance { get; set; }

	private AudioSource squishAudioSource;
	private AudioSource bgAudioSource;
	private AudioSource sprayingAudioSource;
	private AudioSource rattleAudioSource;
	private AudioSource dieAudioSource;
	private AudioSource buttonAudioSource;
	private AudioSource countDownAudioSource;
	private AudioSource highScoreAudioSource;

	public AudioClip countDownBeep;
	public AudioClip buttonSFX;
	public AudioClip[] squishSFX;
	public AudioClip bgMusic;
	public AudioClip highScoreClaps;

	[HideInInspector]
	public bool bgMusicIsPlaying = false;

	//	Spray Can Stuff
	public AudioClip spraySFX;
	public AudioClip sprayRattle;
	private float rattleVolume = 0.5f;

	// Spider Die Stuff.
	public AudioClip[] dieSFX;
	private float dieVolume = 0.18f;
	
	void Awake()
	{   // SoundManager instance Stuff.
		//Check if instance already exists
		if (Instance == null)
		{
			//if not, set instance to this
			Instance = this;
		}
		//If instance already exists and it's not this:
		else if (Instance != this)
		{

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a SoundManager.
			Destroy(gameObject);

		}
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		highScoreAudioSource = gameObject.AddComponent<AudioSource>();
		squishAudioSource = gameObject.AddComponent<AudioSource>();
		rattleAudioSource = gameObject.AddComponent<AudioSource>();
		dieAudioSource = gameObject.AddComponent<AudioSource>();
		buttonAudioSource = gameObject.AddComponent<AudioSource>();
		countDownAudioSource = gameObject.AddComponent<AudioSource>();

		sprayingAudioSource = gameObject.AddComponent<AudioSource>();
		sprayingAudioSource.clip = spraySFX;

		bgAudioSource = gameObject.AddComponent<AudioSource>();
		bgAudioSource.loop = true;
		bgAudioSource.clip = bgMusic;
	}

	#region Die SFX
	public void DieSFX()
	{
		int randomDie = Random.Range(0, dieSFX.Length);
		dieAudioSource.PlayOneShot(dieSFX[randomDie], dieVolume);
	}
	#endregion

	#region Rattle SFX
	public void SprayRattleSFX()
	{
		rattleAudioSource.PlayOneShot(sprayRattle, rattleVolume);
	}
	#endregion

	#region Spray SFX
	public void StartSpraySFX()
	{
		sprayingAudioSource.Play();
	}
	
	public void StopSpraySFX()
	{
		sprayingAudioSource.Stop();
	}
	#endregion

	#region BackGround SFX
	public void StartBgMusic()
	{
		bgAudioSource.Play();
		bgMusicIsPlaying = true;
	}

	public void StopBgMusic()
	{
		bgAudioSource.Stop();
		bgMusicIsPlaying = false;
	}
	#endregion

	#region Squish SFX
	public void PlaySquishSFX()
	{
		//	Get a random Squish SFX.
		int randomSFX = Random.Range(0, squishSFX.Length);
		//
		squishAudioSource.PlayOneShot(squishSFX[randomSFX]);
		
	}
	#endregion

	#region Button SFX
	public void ButtonSFX()
	{
		buttonAudioSource.PlayOneShot(buttonSFX);
	}
	#endregion

	#region HighScore
	public void StartHighScoreSFX()
	{
		highScoreAudioSource.PlayOneShot(highScoreClaps, .5f);
	}

	public void StopHighScoreSFX()
	{
		highScoreAudioSource.Stop();
	}
	#endregion

	#region
	public void PlayCountDownSFX()
	{
		countDownAudioSource.PlayOneShot(countDownBeep, 0.5f);
	}
	#endregion
}
