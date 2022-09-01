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
    int _bulletNum = 3;
    [SerializeField]
    float _followPlayerTime = 3f;
    [SerializeField]
    float _offsetAngle = 15f;
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
    GameObject _bodyBulletPrefab;
    GameObject _player;
    Vector2 _direction;
    Vector3[] _segmentPoses;
    Vector3[] _segmentVelocity;
    bool _hitted;
    bool _isStarted;
    int _movePointIndex;
    int _bodyPartIndex;
    float _atkCD;
    float _stopTimer;

    public override void InitData()
    {
        base.InitData();
        _movePointIndex = 0;
        _bodyPartIndex = 0;
        _atkCD = 0;
        _isStarted = true;
        target = GameManager.I.bossMovePoints[_movePointIndex];
        _bulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_SNAKE_BULLET);
        _bodyBulletPrefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_SNAKE_BODY_BULLET);
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
                    if ((target.position - _head.position).magnitude < 0.1f)
                    {
                        if (_movePointIndex < GameManager.I.bossMovePoints.Length - 1)
                        {
                            _isStarted = false;
                            _movePointIndex++;
                            target = GameManager.I.bossMovePoints[_movePointIndex];
                        }
                        else
                        {
                            _movePointIndex = 0;
                            CurrentState = StateMachine.FollowPlayer;
                        }
                    }
                    Move(target.position);
                    break;
                case StateMachine.FollowPlayer:
                    Move(_player.transform.position);
                    Attack();
                    _followPlayerTime -= Time.deltaTime;
                    if (_followPlayerTime <= 0)
                    {
                        _followPlayerTime = 3f;
                        CurrentState = StateMachine.Move;
                    }
                    break;
                case StateMachine.Attack:
                    Attack();
                    break;
                case StateMachine.Dead:
                    _stopTimer += Time.deltaTime;
                    if (_stopTimer > _hittedStopTime)
                    {
                        if (_bodyPartIndex < _bodyParts.Length)
                        {
                            var prefab = ResourceManager.I.Load<GameObject>(AssetPath.ENEMY_DEAD_VFX);
                            var vfxObject = ObjectPool.I.Create(prefab);
                            vfxObject.transform.position = _bodyParts[_bodyPartIndex].transform.position;
                            vfxObject.gameObject.SetActive(true);
                            _bodyParts[_bodyPartIndex].gameObject.SetActive(false);
                            _bodyPartIndex++;
                        }
                        else
                        {
                            GlobalMessenger.Launch(EventMsg.KilledTheEnemy);
                            target = null;
                            ObjectPool.I.Recycle(gameObject);
                            ObjectPool.I.Recycle(_hpBarObject);
                            GlobalMessenger.Launch(EventMsg.GameClear);
                        }
                        _stopTimer = 0;
                    }
                   
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
            if (_isStarted)
                return;

            OnHit(collider.GetComponent<Weapon>().damage);
            _hitted = true;
        }
        else if (collider.tag == "Bullet")
        {
            if (_isStarted)
                return;

            OnHit(collider.transform.GetComponent<Bullet>().owner, collider.transform.GetComponent<Bullet>().skillData);
            _hitted = true;
        }
    }



    void Move(Vector3 nextPos)
    {
        //head
        _direction = nextPos - _head.position;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _head.rotation = Quaternion.Slerp(_head.rotation, rotation, Data.NowTurnSpeed * Time.deltaTime);
        _head.position = Vector2.MoveTowards(_head.position, nextPos, Data.NowMoveSpeed * Time.deltaTime);
        if (_player == null)
        {
            var zone = Physics2D.OverlapCircleAll(transform.position, Data.NowViewRadius);
            foreach (var item in zone)
            {
                if (item.tag == "Player")
                {
                    _player = item.gameObject;
                    break;
                }
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

    }

    void RotateToPlayer() {
        if (IsFacingToPlayer())
        {
            CurrentState = StateMachine.Attack;
        }
        else
        {
            if (_player == null)
            {
                return;
            }
            Vector3 distance = _player.transform.position - _head.transform.position;
            float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _head.rotation = Quaternion.Slerp(_head.rotation, rotation, Data.NowTurnSpeed * Time.deltaTime);
        }
    }

    bool IsFacingToPlayer() {
        if (_player == null)
        {
            return false;
        }
        Vector3 distance = _player.transform.position - _head.transform.position;
        float angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg;
        if (angle < _offsetAngle)
        {
            return true;
        }
        return false;
    }

    void Attack()
    {
        if (_atkCD > Data.Skill.cd)
        {
            for (int i = 0; i < _bulletNum; i++)
            {

                var bulletObject = ObjectPool.I.Create(_bulletPrefab);
                bulletObject.transform.position = _firePoint.position;
                Bullet bullet = bulletObject.GetComponent<Bullet>();
                bullet.owner = Data;
                bullet.skillData = Data.Skill;
                var dir = (_player.transform.position - _head.position).normalized;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                if (_bulletNum % 2 == 1)
                {
                    bullet.moveDirection = Quaternion.AngleAxis(_offsetAngle * (i - _bulletNum / 2), Vector3.forward) * dir;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle - _offsetAngle * (i - _bulletNum / 2), Vector3.forward);
                }
                else
                {
                    bullet.moveDirection = Quaternion.AngleAxis(_offsetAngle * (i - _bulletNum / 2) + _offsetAngle / 2, Vector3.forward) * dir;
                    bullet.transform.rotation = Quaternion.AngleAxis(angle - _offsetAngle * (i - _bulletNum / 2) + _offsetAngle / 2, Vector3.forward);
                }
                bullet.speed = Data.NowAtkSpeed;
                bullet.gameObject.SetActive(true);
            }

            if (Data.NowHP < (Data.MaxHP / 2))
            {
                var index = Random.Range(0, _bodyParts.Length);
                for (float i = 0; i <= 2 * Mathf.PI; i += (Mathf.PI / 6))
                {
                    var bulletObject = ObjectPool.I.Create(_bodyBulletPrefab);
                    Bullet bullet = bulletObject.GetComponent<Bullet>();
                    bullet.owner = Data;
                    bullet.skillData = Data.Skill;
                    bullet.speed = Data.NowAtkSpeed;
                    bullet.moveDirection = new Vector3(Mathf.Cos(i), Mathf.Sin(i), 0);
                    bullet.transform.position = _bodyParts[index].transform.position;
                    bullet.gameObject.SetActive(true);

                }
                
            }

            _atkCD = 0;
        }

    }

}
