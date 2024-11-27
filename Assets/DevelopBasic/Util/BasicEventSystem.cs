using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

//A basic C# Event System
public static class EventHandler
{
#region Input
    public static Action<Key> E_OnKeyPressed;
    public static void Call_OnKeyPressed(Key key){E_OnKeyPressed?.Invoke(key);}
    public static Action<Key> E_OnKeyReleased;
    public static void Call_OnKeyReleased(Key key){E_OnKeyReleased?.Invoke(key);}
    public static Action E_OnAnyKeyPressed;
    public static void Call_OnAnyKeyPressed(){E_OnAnyKeyPressed?.Invoke();}
    public static Action E_OnNoKeyPressed;
    public static void Call_OnNoKeyPressed(){E_OnNoKeyPressed?.Invoke();}
#endregion

#region Mini Game
    public static Action<MiniGameBasic> E_OnEndMiniGame;
    public static void Call_OnEndMiniGame(MiniGameBasic miniGame){E_OnEndMiniGame?.Invoke(miniGame);}
    public static Action E_OnNextMiniGame;
    public static void Call_OnNextMiniGame(){E_OnNextMiniGame?.Invoke();}
    public static Action<FloatingFlower> E_OnFloatingFlowerBloom;
    public static void Call_OnFloatingFlowerBloom(FloatingFlower flower)=>E_OnFloatingFlowerBloom?.Invoke(flower);
    public static Action E_OnFlowerFlow;
    public static void Call_OnFlowerFlow()=>E_OnFlowerFlow?.Invoke();
#endregion

#region Game Basic
    public static Action E_BeforeUnloadScene;
    public static void Call_BeforeUnloadScene()=>E_BeforeUnloadScene?.Invoke();
    public static Action E_AfterLoadScene;
    public static void Call_AfterLoadScene()=>E_AfterLoadScene?.Invoke();
#endregion
}