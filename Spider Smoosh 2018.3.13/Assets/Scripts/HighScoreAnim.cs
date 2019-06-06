using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreAnim : MonoBehaviour
{
	public Animator anim;

	void Start()
    {
		anim = GetComponent<Animator>();
		anim.Play("New_High_Score_Anim");
	}
}
