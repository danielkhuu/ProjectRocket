using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigidBody;
    AudioSource audioSource;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();  //ref rigidbody component from Rocketship on Unity Editor
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }
    private void ProcessInput()
    {
        if(Input.GetKey(KeyCode.Space))   //can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up);
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }

        if (Input.GetKey(KeyCode.A))  //cannot rotate both ways at same time
        {
            transform.Rotate(Vector3.forward); 
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward);
        }

    }
}
