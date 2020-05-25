using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigidBody; //Component of rocket that allows for physical contact with world
    AudioSource audioSource; //rocket thrust sound component

    //floats need f at end of number
    [SerializeField] float rcsThrust = 100f;  //inspector value adjuster for rotation speed
    [SerializeField] float mainThrust = 5f; //inspector value adjuster for main thrust

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();  //ref rigidbody component from Rocketship on Unity Editor
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive)
        { //todo somewhere stop sound
            Thrust();
            Rotate();
        }

    }
    void Rotate()
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
        if (state != State.Alive) { return; }//if not alive, do not execute below
        switch (collision.gameObject.tag) //switch collision based on tag of object
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextLevel", 1f);  //delay, 1f is 1 second 
                break;
            default:
                //kill player
                state = State.Dying;
                Invoke("LoadFirstLevel", 1f);
                break;
        }
    }
    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
}
