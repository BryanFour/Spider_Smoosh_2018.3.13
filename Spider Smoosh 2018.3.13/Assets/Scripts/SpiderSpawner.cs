using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderSpawner : MonoBehaviour
{	
	//	The array of spider prefabs.
	public GameObject[] spiders;
	//	The array of spider spawn points.
	public Transform[] spawnPoints;
	//	The spiders spawn rate to be decreased in the decrease spawn rate routine. 
	private float spawnRate = 1f;

	private void Start()
	{
		StartCoroutine(SpawnSpiders());
		StartCoroutine(DecreaseSpawnRate());
	}

	IEnumerator DecreaseSpawnRate()
	{
		if(spawnRate > 0.5f && LevelManager.Instance.countDownHasFinished == true && Time.timeScale == 1)
		{
			yield return new WaitForSecondsRealtime(15);
			spawnRate = spawnRate - 0.25f;
			StartCoroutine(DecreaseSpawnRate());
		}
		else if (LevelManager.Instance.countDownHasFinished == false || Time.timeScale == 0)
		{
			yield return new WaitForSecondsRealtime(0.5f);
			StartCoroutine(DecreaseSpawnRate());
		}
	}

	IEnumerator SpawnSpiders()
	{
		//if(LevelManager.Instance.countDownHasFinished == true && LevelManager.Instance.gameOver == false)
		if (LevelManager.Instance.countDownHasFinished == true && Time.timeScale == 1)
		{
			//	Choose a random spider.
			int randomSpider = Random.Range(0, spiders.Length);
			//	Find a random index between zero and one less than the number of spawn points.
			int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
			//	Instantiate the randomly chosen spider at the randomly chosen spawn point with the spawn points rotation.
			Instantiate(spiders[randomSpider], spawnPoints[randomSpawnPoint].position, spawnPoints[randomSpawnPoint].rotation);
			yield return new WaitForSecondsRealtime(spawnRate);
			//	Restart the routine.
			StartCoroutine(SpawnSpiders());
		}
		//	if the countdown hasnt finished or the game is paused/frozen...
		else if (LevelManager.Instance.countDownHasFinished == false || Time.timeScale == 0)
		{
			yield return new WaitForSecondsRealtime(0.5f);
			//	Restart the routine after waiting 0.5 seconds.
			StartCoroutine(SpawnSpiders());
		}
			
	}
}
