using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace SupanthaPaul
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private Transform startPosition;
		[SerializeField] private Camera camera;
		[SerializeField] private float speed;
		[Header("Jumping")]
		[SerializeField] private float jumpForce;
		[SerializeField] private float fallMultiplier;
		[SerializeField] private Transform groundCheck;
		[SerializeField] private float groundCheckRadius;
		[FormerlySerializedAs("whatIsGround")] [SerializeField] private LayerMask layer;
		[FormerlySerializedAs("whatIsGround")] [SerializeField] private LayerMask playerMask;
		[SerializeField] private int extraJumpCount = 1;
		[SerializeField] private GameObject jumpEffect;
		

		// Access needed for handling animation in Player script and other uses
		[HideInInspector] public bool isGrounded;
		[HideInInspector] public float moveInput;
		[HideInInspector] public bool canMove = true;
		[HideInInspector] public bool isDashing = false;
		[HideInInspector] public bool actuallyWallGrabbing = false;
		// controls whether this instance is currently playable or not
		[HideInInspector] public bool isCurrentlyPlayable = true;

		[SerializeField] private float jumpParam;
		[SerializeField] private bool canShrink;
		[SerializeField] private bool canGrow;

	

		private Rigidbody2D m_rb;
		private ParticleSystem m_dustParticle;
		private bool m_facingRight = true;
		private readonly float m_groundedRememberTime = 0.25f;
		private float m_groundedRemember = 0f;
		private bool m_hasDashedInAir = false;
		private bool m_onWall = false;
		private bool m_onRightWall = false;
		private bool m_onLeftWall = false;

		// 0 -> none, 1 -> right, -1 -> left
		private int m_onWallSide = 0;
		private int m_playerSide = 1;

		private Camera cam;


		void Start()
		{
			Debug.Log(canShrink ? "Shrinking" : "Growing");
			// create pools for particles
			PoolManager.instance.CreatePool(jumpEffect, 2);

			// if it's the player, make this instance currently playable
			if (transform.CompareTag("Player"))
				isCurrentlyPlayable = true;

			m_rb = GetComponent<Rigidbody2D>();
			m_dustParticle = GetComponentInChildren<ParticleSystem>();
            Restart();
		}

		private void Restart()
		{
			transform.position = startPosition.position;
			transform.rotation = startPosition.rotation;
			transform.localScale = startPosition.localScale;
		}

		private void FixedUpdate()
		{
			// check if grounded
			isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, layer);
			var position = transform.position;

			// calculate player and wall sides as integers
			CalculateSides();

			// if this instance is currently playable
			if (isCurrentlyPlayable)
			{
				// horizontal movement
				if(canMove)
					m_rb.velocity = new Vector2(moveInput * speed, m_rb.velocity.y);
				else
					m_rb.velocity = new Vector2(0f, m_rb.velocity.y);
				// better jump physics
				if (m_rb.velocity.y < 0f)
				{
					m_rb.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
				}

				// Flipping
				if (!m_facingRight && moveInput > 0f)
					Flip();
				else if (m_facingRight && moveInput < 0f)
					Flip();

				// enable/disable dust particles
				float playerVelocityMag = m_rb.velocity.sqrMagnitude;
				if(m_dustParticle.isPlaying && playerVelocityMag == 0f)
				{
					m_dustParticle.Stop();
				}
				else if(!m_dustParticle.isPlaying && playerVelocityMag > 0f)
				{
					m_dustParticle.Play();
				}

			}
		}

		private void Update()
		{
			// horizontal input
			moveInput = InputSystem.HorizontalRaw();

			

			// grounded remember offset (for more responsive jump)
			m_groundedRemember -= Time.deltaTime;
			if (isGrounded)
				m_groundedRemember = m_groundedRememberTime;

			if (!isCurrentlyPlayable) return;
			// if not currently dashing and hasn't already dashed in air once
			
			// if has dashed in air once but now grounded
			if (m_hasDashedInAir && isGrounded)
				m_hasDashedInAir = false;
			
			// Jumping
			if(InputSystem.Jump() && (isGrounded || m_groundedRemember > 0f))	// normal single jumping
			{
				m_rb.velocity = new Vector2(m_rb.velocity.x, jumpForce + transform.localScale.y * jumpParam);
				// jumpEffect
				PoolManager.instance.ReuseObject(jumpEffect, groundCheck.position, Quaternion.identity);
			}
			
			//Vector2 pos = camera.WorldToScreenPoint(transform.position);
			//if (pos.y < 0)
			//{
			//	Restart();	
			//}
			
		}

		private void ScaleObjectRaycast(bool scale)
		{
			
			// Get the mouse position in screen space
			Vector3 mouseScreenPosition = Input.mousePosition;

			// Convert the mouse position to world space
			Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
			mouseWorldPosition.z = 0; // Ensure z is 0 for 2D raycasting

			// Calculate the direction from the object's position to the mouse position
			Vector2 direction = (mouseWorldPosition - transform.position).normalized;

			// Perform the raycast
			//Raycast(transform.position, direction, scale);
		}

		//private void Raycast(Vector2 origin, Vector2 direction, bool scale, bool mirrored = false)
		//{
		//	LayerMask mask = layer;
		//	RaycastHit2D hit;
		//	if (!mirrored)
		//	{
		//		hit = Physics2D.Raycast(origin, direction, Mathf.Infinity, layer);
		//	}
		//	else
		//	{
		//		hit = Physics2D.Raycast(origin + direction, direction, Mathf.Infinity);
		//	}
		//	if (hit.collider)
		//	{
		//		// Check if the hit object has a specific tag or component
		//		if (hit.collider.CompareTag("scalable") || hit.collider.CompareTag("Player"))
		//		{
		//			var objectRigidbody = hit.collider.GetComponent<Rigidbody2D>();
		//			
		//			if (scale)
		//			{
		//				StartCoroutine(ScaleObject(hit.collider.transform, objectRigidbody, Vector2.one));
		//			}
		//			else
		//			{
		//				StartCoroutine(ScaleDownObject(hit.collider.transform, objectRigidbody, Vector2.one));
		//			}
		//		} else if (hit.collider.CompareTag("mirror"))
		//		{
		//			Vector2 position = hit.point - direction*0.1f;
		//			Vector2 dir = new Vector2(direction.x*-1f, direction.y);
		//			Raycast(position, dir, scale, true);
		//		}
		//	}
		//}
		

		void Flip()
		{
			m_facingRight = !m_facingRight;
			Vector3 scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;
		}

		void CalculateSides()
		{
			if (m_onRightWall)
				m_onWallSide = 1;
			else if (m_onLeftWall)
				m_onWallSide = -1;
			else
				m_onWallSide = 0;

			if (m_facingRight)
				m_playerSide = 1;
			else
				m_playerSide = -1;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
		}
	}
}
