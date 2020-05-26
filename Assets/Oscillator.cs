using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent] //so you dont have duplicate scripts for an object
public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector = new Vector3(10f,10f,10f); //todo remove from inspector
    [SerializeField] float period = 2f; //2 seconds  Modify this to change speed of object
    
    float movementFactor; 

    Vector3 startingPos; //must be stored for absolute movement

    
    void Start()
    {
        startingPos = transform.position; //starting position based on transform in inspector
    }

    
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; } //if period = 0, don't do it
        //cannot use == 0f because floats can be different even by a small amount.
        //Epsilon ensures we can compare period to the smallest amount 
        float cycles = Time.time / period; //grows continually from 0
        const float tau = Mathf.PI * 2; //all around circle you need 2PI or 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to +1

        movementFactor = rawSinWave / 2f + .5f;
        Vector3 offset = movementFactor * movementVector; //confusing way to say movement
        transform.position = startingPos + offset; //offset from starting position
    }
}
