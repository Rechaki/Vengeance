using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public UnitData owner;
    public SkillData skillData;
    public Vector3 moveDirection;


    public float speed;
    public int damage;
    public Vector3 target;

    void Start()
    {


    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    //void OnCollisionEnter2D(Collision2D collision) {
    //    //Debug.Log(collision.transform.name);
    //    if (collision.transform.tag != "Enemy" || collision.transform.tag != "EnemyBullet")
    //    {
    //        ObjectPool.I.Recycle(gameObject);
    //    }

    //}

    void OnTriggerEnter2D(Collider2D collision) {
        if (gameObject.tag == "Bullet")
        {
            if (collision.tag == "Enemy" || collision.tag == "Untagged")
            {
                ObjectPool.I.Recycle(gameObject);
            }
        }
        else if (gameObject.tag == "EnemyBullet")
        {
            if (collision.tag == "Player" || collision.tag == "Untagged")
            {
                ObjectPool.I.Recycle(gameObject);
            }
        }
    }

}
