using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMsgTips : MonoBehaviour
{
    [SerializeField]
    float _autoHideTime = 2.0f;

    float _timer;

    void OnEnable()
    {
        _timer = 0;
    }

    void Update()
    {
        if (_timer > _autoHideTime)
        {
            gameObject.SetActive(false);
        }
        _timer += Time.deltaTime;    
    }
}
