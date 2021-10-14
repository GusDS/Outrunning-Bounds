using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMgr :MonoSingleton<InputMgr>
{
    public static Action OnSpotlightsClick;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnSpotlightsClick();
        }
    }
}
