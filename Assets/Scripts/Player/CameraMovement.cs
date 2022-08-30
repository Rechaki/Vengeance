using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
	[SerializeField]
    Transform _player;
	[SerializeField]
	float _distance;

	void LateUpdate() {
		transform.localPosition = _player.position - transform.forward * _distance;
	}
}
