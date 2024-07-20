using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSide : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private bool IsWhite = true;
    private bool FlipSideButtonWasClicked = false;


    public void FlipTheBoard()
    {
        IsWhite = IsWhite ? false : true;
        FlipSideButtonWasClicked = true;
    }

    public bool GetColor()
    {
        return IsWhite;
    }

    public bool GetClicked()
    {
        return FlipSideButtonWasClicked;
    }

    public void UnClick()
    {
        FlipSideButtonWasClicked = false;
    }
}
