using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public List<PlayerData> playerData { get; private set; }
    public DiceSpawnerScript diceSpawnerRef;
    public WalletScript walletRef;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        PlayerDataManager[] existingManagers = gameObject.scene.GetRootGameObjects().Select(x => x.GetComponent<PlayerDataManager>()).ToArray();
        if (existingManagers.Length > 1) {
            existingManagers[0].AddPlayerData(diceSpawnerRef, walletRef);
            Destroy(gameObject);
        }
        else {
            playerData = new List<PlayerData>();
            AddPlayerData(diceSpawnerRef, walletRef);
        }
    }

    public void AddPlayerData(DiceSpawnerScript diceSpawnerRef, WalletScript walletRef)
    {
        playerData.Add((PlayerData)ScriptableObject.CreateInstance(typeof(PlayerData)));

        if (diceSpawnerRef)
            diceSpawnerRef.playerData = playerData[playerData.Count() - 1];

        if (walletRef)
            walletRef.playerData = playerData[playerData.Count() - 1];

        Debug.Log("Player Data added.");
    }
}
