using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SongMetaDetails 
{


    // Geters
    public string GetArtist();
    public string GetDate();    
    public string GetDifficulty();
    public string GetGenre();
    // Seters
    public void SetArtist(string _artist);
    public void SetDate(string _date);
    public void SetDifficulty(string _difficulty);
    public void SetGenre(string _genre);
}
