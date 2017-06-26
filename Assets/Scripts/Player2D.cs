using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Controller2D))]
public class Player2D : MonoBehaviour {

	//1 unit equals ~.5m
	//intuitive physics variables
	public float maxJumpHeight = 4;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	float moveSpeed = 6;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .15f;

	//un-intuitive behind the scene variable
	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);
	}

	void Update (){
		Vector2 input = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		bool wallSliding = false;
		bool wallStuck = false;

		//all wallsliding is contained in this if
		//WallProperties is only accessable inside this statement
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
				wallSliding = true;

				WallProperties wallProperties;
				wallProperties = wall.GetComponent<WallProperties> ();
				//print (wallProperties.wallSlippery);

				if (velocity.y < -wallProperties.wallSlideSpeedMax) {
					velocity.y = -wallProperties.wallSlideSpeedMax;
				}

				if (wallProperties.timeToWallUnJumpStick > 0) {
					velocity.x = 0;
					velocityXSmoothing = 0;

					if (input.x != 0 && input.x != wallDirX) {
						wallProperties.timeToWallUnJumpStick -= Time.deltaTime;

					} else {
						wallProperties.timeToWallUnJumpStick = wallProperties.wallJumpStickTime;
					}
				} else {
					wallProperties.timeToWallUnJumpStick = wallProperties.wallJumpStickTime;
				}

				if (Input.GetKeyDown (KeyCode.UpArrow)) {
					if (wallDirX == input.x) {
						velocity.x = -wallDirX * wallProperties.wallJumpClimb.x;
						velocity.y = wallProperties.wallJumpClimb.y;
					} else if (input.x == 0) {
						velocity.x = -wallDirX * wallProperties.wallJumpOff.x;
						velocity.y = wallProperties.wallJumpOff.y;
					} else {
						velocity.x = -wallDirX * wallProperties.wallLeapOff.x;
						velocity.y = wallProperties.wallLeapOff.y;
					}
				} 
				if (input.x == 0 && input.y == 0) {
					
					if (wallProperties.timeToWallUnStick > 0) {
						wallProperties.timeToWallUnStick -= Time.deltaTime;
						wallStuck = true;
					}
				} else{
					wallProperties.timeToWallUnStick = wallProperties.wallStickTime;
				}

			}
		}


		if (Input.GetKeyDown (KeyCode.UpArrow) && controller.collisions.below) {
			velocity.y = maxJumpVelocity;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}

		if (!(wallStuck && input.y == 0 && input.x == 0)) {
			velocity.y += gravity * Time.deltaTime;		
		} else {
			velocity.x = 0;
			velocity.y = 0;

		}

		controller.Move (velocity * Time.deltaTime, input);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}
}