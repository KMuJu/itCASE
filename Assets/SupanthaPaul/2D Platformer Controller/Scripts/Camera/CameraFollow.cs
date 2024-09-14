using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupanthaPaul
{
	public class CameraFollow : MonoBehaviour
	{
	    [SerializeField]
		private Transform target;
		[SerializeField]
		private float smoothSpeed = 0.125f;
		public Vector3 offset;
		[Header("Camera bounds")]
		public Vector3 minCamerabounds;
		public Vector3 maxCamerabounds;


		private void FixedUpdate()
		{
		}

		
	}
}
