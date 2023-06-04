using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkyToWaterMiniGame : MiniGameBasic
{
    [SerializeField] private GameObject skyObject;
    protected override void Initialize()
    {
        base.Initialize();

        skyObject.SetActive(true);
    }
    protected override void OnKeyPressed(Key keyPressed)
    {
        base.OnKeyPressed(keyPressed);

        
    }
}
