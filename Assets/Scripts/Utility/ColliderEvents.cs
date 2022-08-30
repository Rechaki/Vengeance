using System;
using UnityEngine;

public class ColliderEvents : MonoBehaviour
{
	public bool debug;

	public delegate void TriggerEvent(Collider2D collider);
	public delegate void CollisionEvent(Collision2D collision);

	public event TriggerEvent OnTriggerEnterEvent;
	public event TriggerEvent OnTriggerExitEvent;
	public event TriggerEvent OnTriggerStayEvent;
	public event CollisionEvent OnCollisionEnterEvent;
	public event CollisionEvent OnCollisionExitEvent;
	public event CollisionEvent OnCollisionStayEvent;

	void OnTriggerEnter2D(Collider2D collider) {
		if (debug)
		{
			Debug.Log($"Collider Name on <color=yellow>OnTriggerEnter2D</color>:  {collider.gameObject.name}");
		}
		TriggerEvent onTriggerEnterEvent = OnTriggerEnterEvent;
		if (onTriggerEnterEvent == null)
		{
			return;
		}
		onTriggerEnterEvent(collider);
	}

	void OnTriggerExit2D(Collider2D collider) {
		if (debug)
		{
			Debug.Log($"Collider Name on <color=yellow>OnTriggerExit2D</color>:  {collider.gameObject.name}");
		}
		TriggerEvent onTriggerExitEvent = OnTriggerExitEvent;
		if (onTriggerExitEvent == null)
		{
			return;
		}
		onTriggerExitEvent(collider);
	}

	void OnTriggerStay2D(Collider2D collider) {
		if (debug)
		{
			Debug.Log($"Collider Name on <color=yellow>OnTriggerStay2D</color>:  {collider.gameObject.name}");
		}
		TriggerEvent onTriggerStayEvent = OnTriggerStayEvent;
		if (onTriggerStayEvent == null)
		{
			return;
		}
		onTriggerStayEvent(collider);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (debug)
		{
			Debug.Log($"Collision Name on <color=green>OnCollisionEnter2D</color>:  {collision.gameObject.name}");
		}
		CollisionEvent onCollisionEnterEvent = OnCollisionEnterEvent;
		if (onCollisionEnterEvent == null)
		{
			return;
		}
		onCollisionEnterEvent(collision);
	}

	void OnCollisionExit2D(Collision2D collision) {
		if (debug)
		{
			Debug.Log($"Collision Name on <color=green>OnCollisionExit2D</color>:  {collision.gameObject.name}");
		}
		CollisionEvent onCollisionExitEvent = OnCollisionExitEvent;
		if (onCollisionExitEvent == null)
		{
			return;
		}
		onCollisionExitEvent(collision);
	}

	void OnCollisionStay2D(Collision2D collision) {
        if (debug)
        {
			Debug.Log($"Collision Name on <color=green>OnCollisionStay2D</color>:  {collision.gameObject.name}");
        }
		CollisionEvent onCollisionStayEvent = OnCollisionStayEvent;
		if (onCollisionStayEvent == null)
		{
			return;
		}
		onCollisionStayEvent(collision);
	}

	void SetActive(bool active) {
		if (debug)
		{
			Debug.Log($"ColliderEvents on {gameObject.name} active: {active}", gameObject);
		}
		if (gameObject.activeSelf == active)
		{
			return;
		}
		gameObject.SetActive(active);
	}

}
