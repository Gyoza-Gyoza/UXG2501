using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadingManager 
{
    private static int databaseCount; 
    private static int databaseLoaded; 

    public static void LoadDatabase()
    {
        databaseCount++; 
    }
    public static bool DatabaseLoaded()
    {
        databaseLoaded++;

        if (databaseLoaded >= databaseCount)
        {
            PhaseManager.Instance.CurrentPhase = new Experimentation();
            return true;
        }
        else return false;
    }
}
