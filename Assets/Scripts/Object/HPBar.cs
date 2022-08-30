using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    Transform _pivot;

    public void SetValue(float num)
    {
        if (num <= 0)
        {
            num = 0;
        }
        _pivot.localScale = new Vector3(num, 1, 1);
    }
}
