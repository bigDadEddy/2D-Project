using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Controller2D))]
public class Player2D : MonoBehaviour {

	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	public float moveSpeed = 6;
	public float accelerationTimeAirborne = .2f;
	public float accelerationTimeGrounded = .15f;

	Controller2D controller;

	Vector2 directionalInput;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;
	Vector3 velocity;

	bool wallStuck = false;

	void Start () {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	public void SetDirectionalInput (Vector2 input){
		directionalInput = input;
	}

	public void OnJumpInputDown(){
		if (Input.GetKeyDown (KeyCode.UpArrow) && controller.collisions.below) {
			if (controller.collisions.slidingDownSlope) {
				if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { //not jumping aginst max slope
					velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
					velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
				}
			}
			else{
				velocity.y = maxJumpVelocity;
			}
		}
	}

	public void OnJumpInputUp(){
		if (Input.GetKeyUp (KeyCode.Space)) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
	}

	void Update (){
		calculateVelocity ();

		CalculateWallClimbing ();
		OnJumpInputDown ();
		OnJumpInputUp ();

		calculateGravity ();

		controller.Move (velocity * Time.deltaTime, directionalInput);

		//reset gravity force when on ground
		if(controller.collisions.above || controller.collisions.below){
			if (controller.collisions.slidingDownSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
			}
		}
	}

	void calculateVelocity(){
		float targetVelocityX = directionalInput.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
	}

	void calculateGravity(){ 
		if (wallStuck) {
			velocity.y = 0;
		} 
		else {
			velocity.y += gravity * Time.deltaTime;	
		}

		wallStuck = false;
	}

	public void CalculateWallClimbing(){
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {

			int wallDirX = (controller.collisions.left) ? -1 : 1;

			GameObject wall;
			if (wallDirX == 1) {
				wall = controller.collisions.objectRight;
				//print (wall);
			} else {
				wall = controller.collisions.objectLeft;
				//print (wall);
			}

			if (wall.GetComponent<WallProperties> () != null) {
				WallProperties wallProperties;
				wallProperties = wall.GetComponent<WallProperties> ();
				//print (wallProperties.wallSlippery);

				if (velocity.y < -wallProperties.wallSlideSpeedMax) {
					velocity.y = -wallProperties.wallSlideSpeedMax;
				}

				if (wallProperties.timeToWallUnJumpStick > 0) {
					velocity.x = 0;
					velocityXSmoothing = 0;

					if (directionalInput.x != 0 && directionalInput.x != wallDirX) {
						wallProperties.timeToWallUnJumpStick -= Time.deltaTime;

					} else {
						wallProperties.timeToWallUnJumpStick = wallProperties.wallJumpStickTime;
					}
				} else {
					wallProperties.timeToWallUnJumpStick = wallProperties.wallJumpStickTime;
				}

				if (Input.GetKeyDown (KeyCode.UpArrow)) {
					if (wallDirX == directionalInput.x) {
						velocity.x = -wallDirX * wallProperties.wallJumpClimb.x;
						velocity.y = wallProperties.wallJumpClimb.y;
					} else if (directionalInput.x == 0) {
						velocity.x = -wallDirX * wallProperties.wallJumpOff.x;
						velocity.y = wallProperties.wallJumpOff.y;
					} else {
						velocity.x = -wallDirX * wallProperties.wallLeapOff.x;
						velocity.y = wallProperties.wallLeapOff.y;
					}
				} 
				if (directionalInput.x == 0 && directionalInput.y == 0) {

					if (wallProperties.timeToWallUnStick > 0) {
						wallProperties.timeToWallUnStick -= Time.deltaTime;
						wallStuck = true;
					}
				} else{
					wallProperties.timeToWallUnStick = wallProperties.wallStickTime;
				}

			}
		}
	}
}