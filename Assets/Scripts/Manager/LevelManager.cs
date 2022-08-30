using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Singleton<LevelManager>
{
    public string CurrentLevel { get; private set; }

    void Awake() {
        CurrentLevel = "Title";
    }

    public void LoadScene(string levelName) {
        if (levelName == "Clear")
        {
            GlobalMessenger.Launch(EventMsg.GameClear);
            return;
        }
        CurrentLevel = levelName;
        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        //EventMsgManager.Check();

    }
}
