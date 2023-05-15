using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour
{
    protected void Awake() => Keyboard.current.onTextInput += OnKeyPressed;

    protected void OnDestroy() => Keyboard.current.onTextInput -= OnKeyPressed;
    void OnKeyPressed(char key) => EventHandler.Call_OnKeyPressed((KeyCode) key);
}