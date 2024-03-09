using TMPro;
using UnityEngine;

public class ShotPopup1 : MonoBehaviour
{
    //Create a shot popup 
    public static ShotPopup Create(Vector3 postion, string equ)
    {
        Transform shotPopupTransform = Instantiate(GameAssets.i.pfShotPopup, postion, Quaternion.identity);

        ShotPopup shotPopup = shotPopupTransform.GetComponent<ShotPopup>();
        shotPopup.Setup(equ);

        return shotPopup;
    }
    private const float DISAPPEAR_TIMER_MAX = 2f;
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void Setup(string equ)
    {
        textMesh.SetText(equ.ToString());
        disappearTimer = DISAPPEAR_TIMER_MAX;
        textColor = textMesh.color;
        moveVector = new Vector3(1, 1) * 30f;
    }

    private void Update()
    {

        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;


        if (disappearTimer > DISAPPEAR_TIMER_MAX * .5f)
        {
            //first half of the popup
            float increaseScaleAmount = 1f;
            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;

        }
        else
        {
            float decreaseScaleAmount = 1f;
            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;

        }
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            //start disappearing
            float disappearSpeed = 5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }

        }
    }

}
