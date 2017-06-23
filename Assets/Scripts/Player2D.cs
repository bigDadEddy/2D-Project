using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (Controller2D))]
public class Player2D : MonoBehaviour {

	//1 unit equals ~.5m
	//intuitive physics variables
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float moveSpeed = 6;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .15f;

	//un-intuitive behind the scene variable
	float gravity = -20;
	float jumpVelocity = 8;
	Vector3 velocity;
	float velocityXSmoothing;

	Controller2D controller;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		jumpVelocity = Mathf.Abs (gravity) * timeToJumpApex;
	}

	void Update (){
		Vector2 input = new Vector2(Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		bool wallSliding = false;
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;
			GameObject wall;
			if (wallDirX == 1) {
				wall = controller.collisions.objectRight;
				print (wall);
			} else {
				wall = controller.collisions.objectLeft;
				print (wall);
			}
			WallProperties wallProperties;
			wallProperties = wall.GetComponent<WallProperties> ();
			print (wallProperties.wallNormal);

			if (velocity.y < -wallProperties.wallSlideSpeedMax) {
				velocity.y = -wallProperties.wallSlideSpeedMax;
			}

			if (wallProperties.timeToWallUnstick > 0) {

				velocity.x = 0;
				velocityXSmoothing = 0;

				if (input.x != 0) {
					wallProperties.timeToWallUnstick -= Time.deltaTime;
				} else {
					wallProperties.timeToWallUnstick = wallProperties.wallStickTime;
				}
			} else {
				wallProperties.timeToWallUnstick = wallProperties.wallStickTime;
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
					
		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}


		if (Input.GetKeyDown (KeyCode.UpArrow) && controller.collisions.below) {
			velocity.y = jumpVelocity;
		}

		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
	

}
