using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsAppearMiniGame : MiniGameBasic
{
    [SerializeField] private GameObject waterGroup;
    protected override void Initialize()
    {
        base.Initialize();
        waterGroup.SetActive(true);
    }
    void Update()
    {
        
    }
}
