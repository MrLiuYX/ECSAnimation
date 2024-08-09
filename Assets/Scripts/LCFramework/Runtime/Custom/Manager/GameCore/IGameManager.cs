using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IGameManager
{
    void InitManager();
    void InitData();
    void ResetData();
    void SaveData();

    void OnEnterGame();
    void OnExitGame();
}
