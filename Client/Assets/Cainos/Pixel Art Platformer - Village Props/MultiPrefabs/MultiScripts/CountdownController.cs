using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    public PauseMenu pauseMenu;
    void Update()
    {
        if (remainingTime > 0)
        {

            remainingTime -= Time.deltaTime;
        }

        else if (remainingTime < 0)
        {
            remainingTime = 0;
            pauseMenu.LoadMenu();
           timerText.color = Color.red;
           timerText.text = "Game Over!";
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }


}

