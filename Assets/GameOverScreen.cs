using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text Winner;
    public void Setup(string winner)
    {
        gameObject.SetActive(true);
        Winner.text = winner + " Wins";
    }
}
