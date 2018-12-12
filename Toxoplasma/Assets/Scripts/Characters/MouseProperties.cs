using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseProperties : MonoBehaviour
{

    [Range(0f, 10f)]
    public float fear = 0f;
    [Range(0f, 10f)]
    public float anger = 0f;
    [Range(0f, 10f)]
    public float hunger = 0f;

    void Start()
    {

    }

    void Update()
    {

    }
}
