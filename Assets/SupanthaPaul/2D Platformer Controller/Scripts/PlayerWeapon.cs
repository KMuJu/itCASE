using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
	[SerializeField] private float scaleParam;
	[SerializeField] private float scaleTime;
	[SerializeField] private float minSize;
	[SerializeField] private bool canShrink;
	[SerializeField] private bool canGrow;
	
    private LineRenderer _lr;
    
    public LayerMask playerLayer;
    
    public float aimDistance = 5f;
    
    private Color _growColor = new Color(13f/255f, 242f/255f, 5f/255f, 1f);
    private Color _shrinkColor = new Color(214f/255f, 0f/255f, 28f/255f, 1f);
    private Color _neutralColor = new Color(1f, 1f, 1f, 0.05f);
    
    public Color activeColor;
    
    // Start is called before the first frame update
    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        activeColor = _neutralColor;
    }

    // Update is called once per frame
    void Update()
    {
        _lr.startColor = activeColor;
        _lr.endColor = activeColor;
        bool fireGrow = false, fireShrink = false;
        
        // Change color of line render
        if (Input.GetMouseButtonDown(0))
        {
            activeColor = _growColor;
            fireGrow = true && canGrow;
            fireShrink = false;
            
        }
        else if (Input.GetMouseButtonDown(1))
        {
            activeColor = _shrinkColor;
            fireShrink = true && canShrink;
            fireGrow = false;
        }
        else if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            activeColor = _neutralColor;
        }
        
        // Get player object position and start line here
        Vector2 objectPosition = transform.position;
        _lr.SetPosition(0, objectPosition);
        
        // Get cursor position
        Vector2 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Calculate direction of aim
        Vector2 direction = (cursorPosition - objectPosition).normalized;
        
        // Raycast
        Raycast(objectPosition, direction, aimDistance, playerLayer, fireGrow, fireShrink, 1);
    }

    void Raycast(Vector2 origin, Vector2 direction, float distance, LayerMask mask, bool fireGrow, bool fireShrink, int depth)
    {
        if (depth == 10)
        {
            return;
        }
        
        RaycastHit2D rc = Physics2D.Raycast(origin, direction, distance, ~mask);
        
        Vector2 endPosition;

        if (rc.collider)
        {
            endPosition = rc.point;
            
            // Set stop position for lineRender
            _lr.positionCount = depth + 1;
            _lr.SetPosition(depth, endPosition);

            if (rc.collider.CompareTag("mirror"))
            {
                Vector2 newPosition = endPosition - direction * 0.1f;
                direction.x *= -1f;
                Raycast(newPosition, direction, distance, 0, fireGrow, fireShrink, depth+1);
            }
            else if ((fireGrow || fireShrink) && (rc.collider.CompareTag("scalable") || rc.collider.CompareTag("Player")))
            {
	            var objectRigidbody = rc.collider.GetComponent<Rigidbody2D>();
                if (fireGrow)
				{
					StartCoroutine(ScaleObject(rc.collider.transform, objectRigidbody, Vector2.one));
				}
				else
				{
					StartCoroutine(ScaleDownObject(rc.collider.transform, objectRigidbody, Vector2.one));
				}
            }
        }
        else
        {
            endPosition = origin + (direction * distance);
            
            // Set stop position for lineRender
            _lr.positionCount = depth + 1;
            _lr.SetPosition(depth, endPosition);
        }
    }
    
	private IEnumerator ScaleObject(Transform objectTransform, Rigidbody2D rb, Vector2 scale)
	{
		Vector2 initialScale = objectTransform.localScale;
		float x = initialScale.x + scale.x * scaleParam * Math.Sign(initialScale.x);
		float y = initialScale.y + scale.y * scaleParam * Math.Sign(initialScale.y);
		if (Math.Sign(x) != Math.Sign(initialScale.x))
		{
			x = minSize * Mathf.Sign(initialScale.x);
		}

		if (Math.Sign(y) != Math.Sign(initialScale.y))
		{
			y = minSize * Mathf.Sign(initialScale.y);
		}
		Vector2 targetScale = new Vector2( x,y );
		
		float initialMass = rb.mass; // Store the initial mass
		float targetMass = initialMass * (targetScale.x / initialScale.x); // Compute target mass based on scale

		float elapsedTime = 0f;

		while (elapsedTime < scaleTime)
		{
			// Calculate the new scale
			Vector2 newScale = Vector2.Lerp(initialScale, targetScale, elapsedTime / scaleTime);
			// Check for collisions
			if (CheckCollisionAlongAxis(objectTransform, newScale, scale))
			{
				Debug.Log("STOP");
				yield break;
			}
			objectTransform.localScale = newScale;

			// Increment elapsed time
			elapsedTime += Time.deltaTime;
			
			rb.mass = Mathf.Lerp(initialMass, targetMass, elapsedTime / 1f);

			// Wait for the next frame
			yield return null;
		}

		// Ensure the final scale is set
		objectTransform.localScale = targetScale;
	}
	
	private IEnumerator ScaleDownObject(Transform objectTransform, Rigidbody2D rb, Vector2 scale)
	{
		Vector2 initialScale = objectTransform.localScale;
		float x = initialScale.x - scale.x * scaleParam * Math.Sign(initialScale.x);
		float y = initialScale.y - scale.y * scaleParam * Math.Sign(initialScale.y);
		if (Math.Sign(x) != Math.Sign(initialScale.x) || Math.Abs(x) < minSize)
		{
			x = minSize * Mathf.Sign(initialScale.x);
		}

		if (Math.Sign(y) != Math.Sign(initialScale.y) || Math.Abs(y) < minSize)
		{
			y = minSize * Mathf.Sign(initialScale.y);
		}
		
		Vector2 targetScale = new Vector2( x,y );
		
			
		
		float initialMass = rb.mass; // Store the initial mass
		float targetMass = initialMass * (targetScale.x / initialScale.x); // Compute target mass based on scale

		float elapsedTime = 0f;

		while (elapsedTime < scaleTime)
		{
			// Calculate the new scale
			objectTransform.localScale = Vector2.Lerp(initialScale, targetScale, elapsedTime / scaleTime);

			// Increment elapsed time
			elapsedTime += Time.deltaTime;
			
			rb.mass = Mathf.Lerp(initialMass, targetMass, elapsedTime / 1f);

			// Wait for the next frame
			yield return null;
		}

		// Ensure the final scale is set
		objectTransform.localScale = targetScale;
	}
	
	private bool CheckCollisionAlongAxis(Transform objectTransform, Vector2 newScale, Vector2 scaleDirection)
    {
        // Check for collision along the x and y axis separately
        bool collisionX = false;
        bool collisionY = false;
    
        // Check along X-axis
        if (scaleDirection.x != 0)
        {
            Vector2 boxSize = new Vector2(newScale.x, objectTransform.localScale.y* 0.5f);
            Collider2D[] collidersX = Physics2D.OverlapBoxAll(objectTransform.position, boxSize, 0f);
            collisionX = collidersX.Length > 2; // True if there are two or more colliders
			Debug.Log(collidersX.Length);
        }
    
        // Check along Y-axis
        if (scaleDirection.y != 0)
        {
            Vector2 boxSize = new Vector2(objectTransform.localScale.x * 0.5f, newScale.y);
            Collider2D[] collidersY = Physics2D.OverlapBoxAll(objectTransform.position, boxSize, 0f);
            collisionY = collidersY.Length > 2; // True if there are two or more colliders
        }
    
        return collisionX || collisionY;
    }
}
