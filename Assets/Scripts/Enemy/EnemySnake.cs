using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnake : EnemyUnit
{
    [SerializeField]
    Transform _head;
    [SerializeField]
    Transform _targetDirection;
    [SerializeField]
    Transform _wiggleDirection;
    [SerializeField]
    Transform _firePoint;
    [SerializeField]
    float _hittedStopTime = 0.5f;
    [SerializeField]
    float _hpPositionY = 0.6f;
    [SerializeField]
    float _targetDistance;
    [SerializeField]
    float _smoothSpeed;
    [SerializeField]
    float _wiggleSpeed;
    [SerializeField]
    float _wiggleMagnitude;
    [SerializeField]
    SnakeBody[] _bodyParts;
    [SerializeField]
    bool _debug = false;

    GameObject _hpBarObject;
    GameObject _bulletPrefab;
    Vector3 _targetPos;
    Vector2 _direction;
    Vector3[] _segmentPoses;
    Vector3[] _segmentVelocity;
    bool _hitted;
    float _radius;
    float _atkCD;
    float _stopTimer;

    public override void InitData()
    {
        base.InitData();
        _atkCD = Data.Skill.cd;
        _radius = Data.NowViewRadius;
        _targetPos = target.position;
        _bulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_SNAKE_BULLET);
        if (_debug)
        {
            var hpBarPefab = ResourceManager.I.Load<GameObject>(AssetPath.HP_BAR);
            _hpBarObject = ObjectPool.I.Create(hpBarPefab);
            _hpBarObject.GetComponent<HPBar>().SetValue(1);
            _hpBarObject.SetActive(true);
        }

        CurrentState = StateMachine.Move;
        _segmentPoses = new Vector3[_bodyParts.Length + 1];
        _segmentVelocity = new Vector3[_bodyParts.Length + 1];
        Data.RefreshEvent += Refresh;
        damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;
        foreach (var body in _bodyParts)
        {
            body.damageColliderEvents.OnTriggerEnterEvent += OnDamageTriggerEnter;
        }
    }

    void Update()
    {
        
        if (CurrentState == StateMachine.Dead)
        {
            return;
        }

        if (_hitted)
        {
            _stopTimer += Time.deltaTime;
            if (_stopTimer > _hittedStopTime)
            {
                _stopTimer = 0;
                _hitted = false;
            }
        }
        else
        {
            switch (CurrentState)
            {
                case StateMachine.Idle:
                    break;
                case StateMachine.Move:
                    Move();
                    break;
                case StateMachine.Attack:
                    Attack();
                    break;
                case StateMachine.Dead:
                    break;
                default:
                    break;
            }
        }

        if (_debug)
        {
            _hpBarObject.transform.position = transform.position + new Vector3(0, _hpPositionY, 0);
        }

        _atkCD += Time.deltaTime;

        StateUpdate();
    }

    void Refresh(EnemyData data)
    {
        if (data.NowHP <= 0)
        {
            CurrentState = StateMachine.Dead;
            target = null;
            ObjectPool.I.Recycle(gameObject);
            ObjectPool.I.Recycle(_hpBarObject);
        }
        else
        {
            if (_debug)
            {
                _hpBarObject.GetComponent<HPBar>().SetValue((float)data.NowHP / (float)data.MaxHP);
            }
        }
    }

    void OnDamageTriggerEnter(Collider2D collider)
    {
        if (collider.tag == "Weapon")
        {
            OnHit(collider.GetComponent<Weapon>().damage);
            _hitted = true;
        }
        else if (collider.tag == "Bullet")
        {
            OnHit(collider.transform.GetComponent<Bullet>().owner, collider.transform.GetComponent<Bullet>().skillData);
            _hitted = true;
        }
    }



    void Move()
    {
        //head
        _direction = _targetPos - _head.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _head.rotation = Quaternion.Slerp(_head.rotation, rotation, Data.NowTurnSpeed * Time.deltaTime);
        _head.position = Vector2.MoveTowards(_head.position, _targetPos, Data.NowMoveSpeed * Time.deltaTime);
        var zone = Physics2D.OverlapCircleAll(transform.position, Data.NowViewRadius);
        foreach (var item in zone)
        {
            if (item.tag == "Player")
            {
                _targetPos = item.transform.position;
                break;
            }
        }
        //body
        _wiggleDirection.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(_wiggleSpeed * Time.deltaTime) * _wiggleMagnitude);
        _segmentPoses[0] = _targetDirection.position;
        for (int i = 1; i < _segmentPoses.Length; i++)
        {
            var pos = _segmentPoses[i - 1] + (_segmentPoses[i] - _segmentPoses[i - 1]).normalized * _targetDistance;
            _segmentPoses[i] = Vector3.SmoothDamp(_segmentPoses[i], pos, ref _segmentVelocity[i], _smoothSpeed);
            _bodyParts[i - 1].transform.position = _segmentPoses[i];
        }

        if ((_targetPos - transform.position).magnitude < Data.NowViewRadius)
        {
            CurrentState = StateMachine.Attack;
        }

    }

    void Attack()
    {
        var bulletObject = ObjectPool.I.Create(_bulletPrefab);
        bulletObject.transform.position = _firePoint.position;
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.owner = Data;
        bullet.skillData = Data.Skill;
        bullet.moveDirection = (_targetPos - transform.position).normalized;
        bullet.speed = Data.NowAtkSpeed;
        bullet.gameObject.SetActive(true);
        var x = UnityEngine.Random.Range(-GameManager.I.ScreenSize.x, GameManager.I.ScreenSize.x);
        var y = UnityEngine.Random.Range(-GameManager.I.ScreenSize.y, GameManager.I.ScreenSize.y);
        target.transform.position = new Vector3(x, y, 0);
        CurrentState = StateMachine.Move;
    }

}
