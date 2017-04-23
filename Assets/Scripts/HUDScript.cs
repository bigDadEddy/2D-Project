using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScript : MonoBehaviour {

	public GUIText score;

	float playerScore = 0;

	// Update is called once per frame
	void Update () {
		playerScore += Time.deltaTime;

		//update the public value
		score.text = string.Format ("Score: " + (int)(playerScore * 100));
	}

	public void ChangeScore (int amount){
		playerScore += amount;
	}
}
