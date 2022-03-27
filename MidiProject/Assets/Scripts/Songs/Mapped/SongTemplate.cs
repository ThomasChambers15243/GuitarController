using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongTemplate
{
    public string songName;
    public int noteCount;
    public int timeSigTop;
    public int timeSigBot;
    public string[] nNames;
    public int[] nOctave;
    public int[] nDur;

    public SongTemplate(string _songName, int _noteCount, int _timeSigTop, int _timeSigBot, string[] _nNames, int[] _nOctave, int[] _nDur)
    {        
        songName = _songName;
        noteCount = _noteCount;
        timeSigTop = _timeSigTop;
        timeSigBot = _timeSigBot;
        nNames = _nNames;
        nOctave = _nOctave;
        nDur = _nDur;
    }
}
