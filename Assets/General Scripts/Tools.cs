using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static bool IsMouseOnScreen()
    {
        if (Input.mousePosition.x <= 0 ||
            Input.mousePosition.x >= Screen.width ||
            Input.mousePosition.y <= 0 ||
            Input.mousePosition.y >= Screen.height) 
            return false; 

        return true;
    }
}
