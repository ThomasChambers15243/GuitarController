using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GenerateNote : MonoBehaviour
{
    public bool playEffect = false;
    // Data to generate the wave
    [SerializeField]
    private double frequency;
    [SerializeField]
    private double gain;
    // Moves the sampels along the wave
    [SerializeField]
    private double increment;
    [SerializeField]
    private double period;
    private double sampling_frequency = 48000f;

    public void Innit(double newFrequency, double newGain)
    {
        frequency = newFrequency;
        gain = newGain;
    }

    /// <summary>
    /// Creates wave data and plays it
    /// </summary>
    /// <param name="data"></param>
    /// <param name="channels"></param>
    void OnAudioFilterRead(float[] data, int channels)
    {
        if (playEffect)
        {
            // update increment in case frequency has changed
            increment = frequency * 2.0 * Math.PI / sampling_frequency;

            for (var i = 0; i < data.Length; i = i + channels)
            {
                period += increment;
                // Sin Wave Generation
                data[i] = SinWaveGen(data, i);

                // Makes sure sound is played through both speakers if there are two
                if (channels == 2)
                {
                    data[i + 1] = data[i];
                }

                // Loops the period back around as the period is 2pi
                if (period > (Mathf.PI * 2))
                {
                    period = 0.0;
                }
            }
        }
    }


    /// <summary>
    /// Generates a sin wave
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    float SinWaveGen(float[] data, int index)
    {
        return (float)(gain * Mathf.Sin((float)period));
    }

    // Keeping for testing
    void OnMouseOver()
    {
        // Plays or mute's the audio
        if (Input.GetMouseButtonDown(0))
        {
            if (playEffect)
            {
                playEffect = false;
            }
            else
            {
                playEffect = true;
            }
        }
    }


    public void SetFreq(double newfrequency)
    {
        frequency = newfrequency;
    }

    public void SetGain(double newGain)
    {
        gain = newGain;
    }
} 
