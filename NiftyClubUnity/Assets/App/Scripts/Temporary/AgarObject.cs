using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent (typeof (Renderer))]
public class AgarObject : MonoBehaviour
{
	[BoxGroup ("Parameters"), SerializeField, Tooltip ("The speed that the player will move.")]
	float speed = 1f;

	[BoxGroup ("Parameters"), SerializeField, Tooltip ("Multiplier for the scaling of the player.")]
	float scale = 1f;

	[BoxGroup ("Links"), SerializeField] private Renderer spriteRenderer;

	Vector3 movePosition;

	#region Unity Methods
	
	void Awake ()
	{
		movePosition = transform.position;
	}

	void Update ()
	{
		if (speed != 0f)
		{
			transform.position = Vector3.MoveTowards (
				transform.position,
				movePosition,
				speed * Time.deltaTime);
		}
	}

	#endregion

	public void SetColor (Color32 color)
	{
		spriteRenderer.material.color = color;
	}

	public void SetRadius (float radius)
	{
		transform.localScale = new Vector3 (radius * scale, radius * scale, 1);
	}

	public void SetMovePosition (Vector3 newPosition)
	{
		movePosition = newPosition;
	}
}