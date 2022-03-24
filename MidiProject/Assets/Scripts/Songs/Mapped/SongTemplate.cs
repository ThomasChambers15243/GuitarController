using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTemplate
{
    public string songName;
    public int noteCount;
    public string[] nNames;
    public int[] nOctave;
    public int[] nDur;

    public SongTemplate(string _songName, int _noteCount, string[] _nNames, int[] _nOctave, int[] _nDur)
    {
        songName = _songName;
        noteCount = _noteCount;
        nNames = _nNames;
        nOctave = _nOctave;
        nDur = _nDur;
    }
}
