using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameInputManager : Singleton<GameInputManager>
{
    [SerializeField, ShowOnly] private bool haskeyPressed = false;
    void Update(){
        bool keyFlag = false;
        foreach(KeyControl key in Keyboard.current.allKeys){
            if(key.wasPressedThisFrame) {
                EventHandler.Call_OnKeyPressed(key.keyCode);
            }
            if(key.wasReleasedThisFrame) {
                EventHandler.Call_OnKeyReleased(key.keyCode);
            }

            if(key.isPressed && !keyFlag){
                keyFlag = true;
            }
        }

        if(!haskeyPressed && keyFlag){
            EventHandler.Call_OnAnyKeyPressed();
            haskeyPressed = true;
        }
        else if(haskeyPressed && !keyFlag){
            EventHandler.Call_OnNoKeyPressed();
            haskeyPressed = false;
        }
    }
}