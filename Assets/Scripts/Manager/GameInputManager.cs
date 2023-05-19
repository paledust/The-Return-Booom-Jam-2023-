using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameInputManager : MonoBehaviour
{
    private KeyControl akey;
    [SerializeField, ShowOnly] private int pressedKeyCount = 0;
    void Update(){
        foreach(KeyControl key in Keyboard.current.allKeys){
            if(key.wasPressedThisFrame) {
                if(pressedKeyCount==0) EventHandler.Call_OnAnyKeyPressed();
                pressedKeyCount ++;
                EventHandler.Call_OnKeyPressed(key.keyCode);
            }
            if(key.wasReleasedThisFrame) {
                pressedKeyCount --;
                if(pressedKeyCount==0) EventHandler.Call_OnNoKeyPressed();
                EventHandler.Call_OnKeyReleased(key.keyCode);

            }
        }        
    }
}