using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerState : BaseObjectState
{
    protected PlayerController player;
    protected LevelController level;
    
    public override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerController>();
    }

    private void Start()
    {
        level = LevelController.Instance;
    }
}
