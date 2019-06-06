using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreAnim : MonoBehaviour
{
	public Animator anim;

	private void Awake()
	{
		Debug.Log("HighScoreAnim script started");
	}

	void Start()
    {
		anim = GetComponent<Animator>();
		anim.Play("New_High_Score_Anim");
		Debug.Log("Anim played.");
	}

    void Update()
    {
		  
    }
}
