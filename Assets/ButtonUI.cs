using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private string PVP = "2PlayerChess";
    [SerializeField] private string VSComputer = "1PlayerChess";
    [SerializeField] private string Explore = "Explore";


    public void Load2Players()
    {
        SceneManager.LoadScene(PVP);
    }

    public void Load1Player()
    {
        SceneManager.LoadScene(VSComputer);
    }

    public void LoadExplore()
    {
        SceneManager.LoadScene(Explore);
    }
}
