using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "The_Return/KeyMatrix_SO")]
public class KeyMatrix_SO : ScriptableObject
{
    [SerializeField] private List<keyData> keydatas;
    public Vector2Int GetCoordinateFromKey(Key key){return keydatas.Find(x=>x.key == key).coordinate;}
}
[System.Serializable]
public class keyData{
    public Key key;
    public Vector2Int coordinate;
}
