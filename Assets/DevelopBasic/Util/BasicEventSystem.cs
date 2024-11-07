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
    public static Action<Vector3> E_OnStoneTouchWater;
    public static void Call_OnStoneTouchWater(Vector3 position)=>E_OnStoneTouchWater?.Invoke(position);
    public static Action<MovingCloud> E_OnCloudOutOfBoundry;
    public static void Call_OnCloudOutOfBoundry(MovingCloud cloud)=>E_OnCloudOutOfBoundry?.Invoke(cloud);
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
