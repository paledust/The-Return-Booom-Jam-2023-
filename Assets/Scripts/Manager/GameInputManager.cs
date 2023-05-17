using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class GameInputManager : MonoBehaviour
{
    private KeyControl akey;
    void OnKeyPress(InputValue value){
        if(value.isPressed){
            foreach(KeyControl key in Keyboard.current.allKeys){
                if(key.wasPressedThisFrame) {
                    EventHandler.Call_OnKeyPressed(key.keyCode);
                    Debug.Log("Pressed");
                    return;
                }
            }
        }
    }
}