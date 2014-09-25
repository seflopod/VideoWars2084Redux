using UnityEngine;

namespace VideoWars.Components
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class FixedPositionComponent : MonoBehaviour
	{
		public bool fixXAxis = true;
		public bool fixYAxis = true;
		
		private Vector3 _fixedPos;
		
		private void Awake()
		{
			_fixedPos = transform.position;
		}
		
		private void FixedUpdate()
		{
			Vector3 curPos = transform.position;
			Vector2 curVel = rigidbody2D.velocity;
			if(fixXAxis && curPos.x != _fixedPos.x)
			{
				curPos.x = _fixedPos.x;
				curVel.x = 0f;
			}
			
			if(fixYAxis && curPos.y != _fixedPos.y)
			{
				curPos.y = _fixedPos.y;
				curVel.y = 0f;
			}
			rigidbody2D.velocity = curVel;
			//transform.position = curPos;
			rigidbody2D.MovePosition(new Vector2(curPos.x, curPos.y));
		}
	}
}
