using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSettings : MonoBehaviour
{
    [SerializeField]
    public int octaves = 8;

    [SerializeField]
    public float frequency = 0.5f;

    [SerializeField]
    public float factor = 1f;

    [SerializeField]
    public float roughness = 2.0f;
    
    [SerializeField]
    public float persistance = 0.5f;

    [SerializeField]
    public int noiseFloor = 64;
}
