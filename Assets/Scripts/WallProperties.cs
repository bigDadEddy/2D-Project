using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallProperties : MonoBehaviour {
	public bool wallNormal;

	public float wallSlideSpeedMax = 1;
	public float wallStickTime = .25f;
	[HideInInspector]
	public float timeToWallUnstick;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeapOff;


	// Use this for initialization
	void Start () {
		if(wallNormal){
			wallJumpClimb.x = 7.5f;
			wallJumpClimb.y = 16;
			wallJumpOff.x = 8.5f;
			wallJumpOff.y = 7;
			wallLeapOff.x = 18;
			wallLeapOff.y = 17;
			wallSlideSpeedMax = 3;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
