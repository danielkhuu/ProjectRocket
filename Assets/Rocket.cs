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
    Rigidbody rigidBody; //Component of rocket that allows for physical contact with world
    AudioSource audioSource; //rocket thrust sound component

    //floats need f at end of number
    [SerializeField] float rcsThrust = 100f;  //inspector value adjuster for rotation speed
    [SerializeField] float mainThrust = 5f; //inspector value adjuster for main thrust

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();  //ref rigidbody component from Rocketship on Unity Editor
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Thrust();
        Rotate();
    }
    private void Rotate()
    {
        rigidBody.freezeRotation = true; //take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;  //multiply by frame time. Longer frame time = faster rotate speed


        if (Input.GetKey(KeyCode.A))  //cannot rotate both ways at same time
        {
            transform.Rotate(Vector3.forward * rotationThisFrame); 
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        rigidBody.freezeRotation = false; //resume physics control of rotation
        /*
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Rotate(Vector2.right);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.Rotate(Vector2.left);
        }
        */
    }

    void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))   //can thrust while rotating
        {
            rigidBody.AddRelativeForce(Vector3.up* mainThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
    }

    void OnCollisionEnter(Collision collision) //determines collision behavior based on object tag
    {
        switch (collision.gameObject.tag) //switch collision based on tag of object
        {
            case "Friendly":
                print("Friendly object");
                break;
            case "Fuel":
                print("Fuel");
                break;
            default:
                print("Dead");
                //kill player
                break;
        }
    }
}
