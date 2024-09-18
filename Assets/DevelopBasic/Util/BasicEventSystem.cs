using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

//A basic C# Event System
public static class EventHandler
{
#region Input
    public static Action<Key> OnKeyPressed;
    public static void Call_OnKeyPressed(Key key){OnKeyPressed?.Invoke(key);}
    public static Action<Key> OnKeyReleased;
    public static void Call_OnKeyReleased(Key key){OnKeyReleased?.Invoke(key);}
    public static Action OnAnyKeyPressed;
    public static void Call_OnAnyKeyPressed(){OnAnyKeyPressed?.Invoke();}
    public static Action OnNoKeyPressed;
    public static void Call_OnNoKeyPressed(){OnNoKeyPressed?.Invoke();}
#endregion

#region Mini Game
    public static Action<MiniGameBasic> OnEndMiniGame;
    public static void Call_OnEndMiniGame(MiniGameBasic miniGame){OnEndMiniGame?.Invoke(miniGame);}
    public static Action OnNextMiniGame;
    public static void Call_OnNextMiniGame(){OnNextMiniGame?.Invoke();}
#endregion

#region Game Basic
    public static Action E_BeforeUnloadScene;
    public static void Call_BeforeUnloadScene()=>E_BeforeUnloadScene?.Invoke();
    public static Action E_AfterLoadScene;
    public static void Call_AfterLoadScene()=>E_AfterLoadScene?.Invoke();
#endregion
}

//A More Strict Event System
namespace SimpleEventSystem{
    public abstract class Event{
        public delegate void Handler(Event e);
    }
    public class E_OnTestEvent:Event{
        public float value;
        public E_OnTestEvent(float data){value = data;}
    }

    public class EventManager{
        private static  EventManager instance;
        public static EventManager Instance{
            get{
                if(instance == null) instance = new EventManager();
                return instance;
            }
        }

        private Dictionary<Type, Event.Handler> RegisteredHandlers = new Dictionary<Type, Event.Handler>();
        public void Register<T>(Event.Handler handler) where T: Event{
            Type type = typeof(T);

            if(RegisteredHandlers.ContainsKey(type)){
                RegisteredHandlers[type] += handler;
            }
            else{
                RegisteredHandlers[type] = handler;
            }
        }
        public void UnRegister<T>(Event.Handler handler) where T: Event{
            Type type = typeof(T);
            Event.Handler handlers;

            if(RegisteredHandlers.TryGetValue(type, out handlers)){
                handlers -= handler;
                if(handlers == null){
                    RegisteredHandlers.Remove(type);
                }
                else{
                    RegisteredHandlers[type] = handlers;
                }
            }
        }
        public void FireEvent<T>(T e) where T:Event {
            Type type = e.GetType();
            Event.Handler handlers;

            if(RegisteredHandlers.TryGetValue(type, out handlers)){
                handlers?.Invoke(e);
            }
        }
        public void ClearList(){RegisteredHandlers.Clear();}
    }
}
