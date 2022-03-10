using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NeckHolder neckHolder = new NeckHolder();
        for(int i = 0; i < 6; i++)
        {
            Debug.Log(neckHolder.strings[i].tunning);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
