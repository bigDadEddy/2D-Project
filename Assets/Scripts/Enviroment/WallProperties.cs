using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallProperties : MonoBehaviour {
	public bool wallSlippery;
	public bool wallGripy;

	public float wallSlideSpeedMax = 1;
	public float wallStickTime = 1f;
	[HideInInspector]
	public float timeToWallUnStick;
	public float wallJumpStickTime = .5f;
	[HideInInspector]
	public float timeToWallUnJumpStick;


	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeapOff;


	// Use this for initialization
	void Start () {
		if (wallSlippery) {
			wallJumpClimb.x = 7.5f;
			wallJumpClimb.y = 16;
			wallJumpOff.x = 8.5f;
			wallJumpOff.y = 7;
			wallLeapOff.x = 18;
			wallLeapOff.y = 17;
			wallSlideSpeedMax = 3;
			wallStickTime = 0;
		} else if (wallGripy) {
			wallJumpClimb.x = 5f;
			wallJumpClimb.y = 16;
			wallJumpOff.x = 8.5f;
			wallJumpOff.y = 7;
			wallLeapOff.x = 18;
			wallLeapOff.y = 17;
			wallSlideSpeedMax = 1;
			wallStickTime = 1f;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
