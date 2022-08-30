using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    [SerializeField]
    Button button;

    void Start() {
        button.onClick.AddListener(LoadGameScene);
    }

    void LoadGameScene() {
        LevelManager.I.LoadScene("Game");
    }
}
