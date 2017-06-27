using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour {

	public LayerMask collisionMask;

	public const float skinWidth = .015f;
	const float dstBetweenRays = .25f;
	[HideInInspector]
	public int horizontalRayCount;
	[HideInInspector]
	public int verticalRayCount;
	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	[HideInInspector]
	public BoxCollider2D Collider;
	public RaycastOrigins raycastOrigins;

	public virtual void Awake() {
		Collider = GetComponent<BoxCollider2D> ();
	}

	public virtual void Start(){
		CalculateRaySpacing ();
	}

	public void UpdateRaycastOrigins(){
		Bounds bounds = Collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public void CalculateRaySpacing() {
		Bounds bounds = Collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.RoundToInt (bounds.size.x / dstBetweenRays);
		verticalRayCount = Mathf.RoundToInt (bounds.size.y / dstBetweenRays);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);


	}

	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
}
