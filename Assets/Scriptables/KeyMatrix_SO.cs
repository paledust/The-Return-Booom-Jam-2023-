using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "The_Return/KeyMatrix_SO")]
public class KeyMatrix_SO : ScriptableObject
{
    [SerializeField] private List<keyData> keydatas;
    public Vector2Int GetCoordinateFromKey(Key key){
        keyData data = keydatas.Find(x=>x.key == key);
        if(data == null){
            return new Vector2Int(Random.Range(0,Service.LINE), Random.Range(0,Service.ROLL));
        }
        else{
            return keydatas.Find(x=>x.key == key).coordinate;
        }
    }
}
[System.Serializable]
public class keyData{
    public Key key;
    public Vector2Int coordinate;
}
