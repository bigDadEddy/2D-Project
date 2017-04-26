using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BumperScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			Player.TotalScore += Player.score-50;
			//Player.LastScore = Player.score;
			Player.SaveGameInfo();
			SceneManager.LoadScene (1);
			return;
		}

		if (other.gameObject.transform.parent) {
			Destroy (other.gameObject.transform.parent.gameObject);
		} else {
			Destroy (other.gameObject);
		}
	}
}
