using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tunnings
{
    // Notes
    
    public static string[] allOctaveNotes = {"c", "c#", "d", "d#", "e", "f", "f#", "g", "g#", "a", "a#", "b"};

    // Tunnings from thin string as 0th index to thick string as 5th index

    public static string[] standardTunning = { "e", "b", "g", "d", "a", "e" };

    public static string[] dropDTunning = { "d", "b", "g", "d", "a", "e" };

    // Voltage from the first fret to the 6th fret
    public static float[] voltageFromFret1 = { 4.64f, 4.34f, 3.95f, 3.57f, 3.23f, 2.85f };
    public static float[] voltageFromBreadBoard = {4.44f,3.88f,3.32f, 2.76f, 2.19f,1.62f };
    // Voltage from breaboard voltage ciruit
    public static float[] voltageFromBreadBoardm = { 4.44f, 3.87f, 3.31f, 2.74f, 2.18f, 1.62f };

}
