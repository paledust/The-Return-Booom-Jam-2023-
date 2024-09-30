using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Service{
    public const int ROLL = 4;
    public const int LINE = 10;
#region HelperFunction
    /// <summary>
    /// Return a list of all active and inactive objects of T type in loaded scenes.
    /// </summary>
    /// <typeparam name="T">Object Type</typeparam>
    /// <returns></returns>
    public static T[] FindComponentsOfTypeIncludingDisable<T>(){
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
        var MatchObjects = new List<T> ();

        for(int i=0; i<sceneCount; i++){
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);
            
            var RootObjects = scene.GetRootGameObjects ();

            foreach (var obj in RootObjects) {
                var Matches = obj.GetComponentsInChildren<T> (true);
                MatchObjects.AddRange (Matches);
            }
        }

        return MatchObjects.ToArray ();
    }
    public static void Shuffle<T>(ref T[] elements){
        var rnd = new System.Random();
        for(int i=0; i<elements.Length; i++){
            int index = rnd.Next(i+1);
            T tmp = elements[i];
            elements[i] = elements[index];
            elements[index] = tmp;
        }
    }
    public static float MapZeroOne_To_NegativeOneToOne(float rnd){
        float temp = rnd;
        temp = temp*2-1;
        return temp;
    }
    public static float SmoothToValue(float rawValue, float targetValue, float step, float allowance){
        if(Mathf.Abs(rawValue-targetValue)<=allowance) return targetValue;
        else return Mathf.Lerp(rawValue, targetValue, step);
    }
#endregion
}