using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tunnings
{
    // Notes

    public static string[] allOctaveNotes = {"a", "a#", "b", "c", "c#", "d", "d#", "e", "f", "f#", "g", "g#"};

    // Tunnings from thin string as 0th index to thick string as 5th index

    public static string[] standardTunning = { "e", "b", "g", "d", "a", "e" };

    public static string[] dropDTunning = { "d", "b", "g", "d", "a", "e" };

    // Voltage from the first fret to the 6th fret
    public static float[] voltageFromFret1 = { 4.64f, 4.34f, 4.03f, 3.57f, 3.23f, 2.85f };

}
