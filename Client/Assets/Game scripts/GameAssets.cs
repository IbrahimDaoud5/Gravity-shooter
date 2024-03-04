using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;
//scrip that makes the access for assets in the scene easier
public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i {  
        get {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
            } 
    }

    public Transform pfShotPopup;
}
