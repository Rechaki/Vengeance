using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanelAlpha : MonoBehaviour
{
    [SerializeField]
    float _changeValue = 0.1f;

    CanvasGroup _canvasGroup;

    void OnEnable()
    {
        if (_canvasGroup == null)
        {
            _canvasGroup = transform.GetComponent<CanvasGroup>();
        }

        if (_canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is Null!!!");
        }
        else
        {
            _canvasGroup.alpha = 0;
        }
    }

    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (_canvasGroup.alpha <= 1)
            {
                _canvasGroup.alpha += _changeValue;
            }
        }
    }
}
