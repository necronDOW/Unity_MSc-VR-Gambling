using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataMono : MonoBehaviour
{
    public PlayerData playerData
    {
        get
        {
            if (!_playerData)
                _playerData = (PlayerData)ScriptableObject.CreateInstance(typeof(PlayerData));

            return _playerData;
        }
        private set { }
    }
    private PlayerData _playerData;
    public Scene originScene { get; private set; }

    private void Awake()
    {
        originScene = gameObject.scene;
        DontDestroyOnLoad(gameObject);
    }
}
