using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpraySpawner : MonoBehaviour
{
	//	The collectable spray can prefab.
	public GameObject collectableSpray;
	//	The array of collectable spray can spawn points.
	public Transform[] spawnPoints;
	//	Number of seconds between spray spawns
	private float spraySpawnInterval = 120;


	void Start()
    {  
		StartCoroutine(TimeToSpawnChecker());
    }

	private void SpawnSpray()
	{	//	If the countdown hasnt finished and the game isnt paused/frozen and gameover is false, exit this method
		if(LevelManager.Instance.countDownHasFinished == false && LevelManager.Instance.gameOver == false && Time.timeScale == 1)
		{
			return;
		}
		//	----- If the countdown has finished and the game isnt paused/frozen...
		//	Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range(0, spawnPoints.Length);
		
		//	Set the spray last spawned player pref value to 0
		PlayerPrefs.SetFloat("SprayLastSpawned", 0);
		
		//	Instanciate a spray can at a random spawn points position.
		Instantiate(collectableSpray, spawnPoints[spawnPointIndex].position, collectableSpray.transform.rotation);
	}

	IEnumerator TimeToSpawnChecker()
	{
		yield return new WaitForSecondsRealtime(1);
		//	If the time since the last spray was spawned is greater than the spray spawn interval
		if (PlayerPrefs.GetFloat("SprayLastSpawned", 0) >= spraySpawnInterval)
		{	//	Spawn a spray can.
			SpawnSpray();
			//	Restart the routine to check if its time to spawn a spray can (check every 1 second)
			StartCoroutine(TimeToSpawnChecker());
		}
		else
		{	//	Restart the routine to check if its time to spawn a spray can (check every 1 second)
			StartCoroutine(TimeToSpawnChecker());
		}
	}
}
