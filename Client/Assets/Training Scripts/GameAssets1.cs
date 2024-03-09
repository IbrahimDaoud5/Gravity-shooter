using UnityEngine;
//scrip that makes the access for assets in the scene easier
public class GameAssets1 : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _i;
        }
    }

    public Transform pfShotPopup;
}
