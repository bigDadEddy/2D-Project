using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController {
	
	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;
	[HideInInspector]
	public float maxSlopeAngle = 60;

	public override void Start(){
		base.Start ();
		collisions.faceDir = 1;
	}

	public void Move(Vector2 movement, bool standingOnPlatform){
		Move (movement, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 movement, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		playerInput = input;

		if (movement.y < 0) {
			DescendSlope (ref movement);
		}
			
		if (movement.x != 0) {
			collisions.faceDir = (int)Mathf.Sign (movement.x);
		}

		HorizontalCollisions (ref movement);

		if (movement.y != 0) {
			VerticalCollisions (ref movement);
		}
		if (standingOnPlatform) {
			collisions.below = true;
		}

		transform.Translate (movement);
	}

	void HorizontalCollisions(ref Vector2 movement) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (movement.x) + skinWidth;

		if (Mathf.Abs (movement.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit) {

				if (hit.distance == 0) {
					continue;
				}

				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle) {
					ClimbSlope (ref movement, slopeAngle, hit.normal);
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
					movement.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						movement.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (movement.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
					if (collisions.left == true) {
						collisions.objectLeft = hit.transform.gameObject;
					}
					if (collisions.right == true) {
						collisions.objectRight = hit.transform.gameObject;
					}
				}
			}
		}
	}

	void VerticalCollisions(ref Vector2 movement) {
		float directionY = Mathf.Sign (movement.y);
		float rayLength = Mathf.Abs (movement.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + movement.x);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength,Color.red);

			if (hit) {
				if (hit.collider.tag == "FallThrough") {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke ("ResetFallingThroughPlatform", .5f);
						continue;
					}
				}
				movement.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					movement.x = movement.y / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign (movement.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
				if (collisions.below == true) {
					collisions.objectBelow = hit.transform.gameObject;
				}
				if (collisions.above == true) {
					collisions.objectAbove = hit.transform.gameObject;
				}
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (movement.x);
			rayLength = Mathf.Abs (movement.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * movement.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					movement.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = hit.normal;
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 movement, float slopeAngle, Vector2 slopeNormal){
		float moveDistance = Mathf.Abs (movement.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (movement.y <= climbVelocityY) {
			movement.y = climbVelocityY;
			movement.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (movement.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;

		}
	}

	void DescendSlope(ref Vector2 movement){
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (movement.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (movement.y) + skinWidth, collisionMask);
		if (maxSlopeHitLeft ^ maxSlopeHitRight) {
			SlideDownSlope (maxSlopeHitLeft, ref movement);
			SlideDownSlope (maxSlopeHitRight, ref movement);
		}

		if (!collisions.slidingDownSlope){
		float directionX = Mathf.Sign (movement.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
					if (Mathf.Sign (hit.normal.x) == directionX) {
						if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (movement.x)) {
							float moveDistance = Mathf.Abs (movement.x);
							float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
							movement.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (movement.x);
							movement.y -= descendVelocityY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
							collisions.slopeNormal = hit.normal;
						}
					}
				}
			}
		}
	}

	void SlideDownSlope(RaycastHit2D hit, ref Vector2 movement){
		if (hit) {
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle) {
				movement.x = hit.normal.x * ((Mathf.Abs (movement.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad));

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}
	}

	void ResetFallingThroughPlatform(){
		collisions.fallingThroughPlatform = false;
	}


	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public GameObject objectAbove, objectBelow;
		public GameObject objectLeft, objectRight;


		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal; 
		public int faceDir;
		public bool fallingThroughPlatform;


		public void Reset(){
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
			slopeNormal = Vector2.zero;
		}
	}
}
