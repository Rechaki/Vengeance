using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    public ColliderEvents damageColliderEvents;

    [SerializeField]
    Transform _target;
    [SerializeField]
    float _turnSpeed;

    Vector2 _direction;

    void Update() {
        _direction = _target.position - transform.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _turnSpeed * Time.deltaTime);
    }

    public void Attack(Bullet bullet)
    {
        for (float i = 0; i <= 2 * Mathf.PI; i += (Mathf.PI / 6))
        {
            bullet.transform.position = transform.position;
            bullet.moveDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i), 0);
            Vector3 vec = new Vector3(Mathf.Cos(i), Mathf.Sin(i), 0);
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, vec);
            bullet.gameObject.SetActive(true);

        }
    }
}
