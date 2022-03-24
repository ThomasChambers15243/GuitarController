using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SongPlay
{
    private List<string> songNames = new List<string>{"AtDoomsGate"};
    private MappedSong playingSong;
    private string currentSongName;

    public SongPlay(string name)
    {
        if (songNames.Contains(name))
        {
            int index = songNames.IndexOf(name);
            playingSong = new MappedSong(songNames[index]);
            currentSongName = playingSong.name;
        }
    }
    

    public IEnumerable PlayGame()
    {
        playingSong.

         yield return null;
    }
}
