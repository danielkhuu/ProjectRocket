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
   
    Rigidbody rigidBody; //Component of rocket that allows for physical contact with world
    AudioSource audioSource; //rocket thrust sound component

    //floats need f at end of number
    [SerializeField] float rcsThrust = 100f;  //inspector value adjuster for rotation speed
    [SerializeField] float mainThrust = 5f; //inspector value adjuster for main thrust
    [SerializeField] float levelLoadDelay = 1f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip nextLevelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    bool collisionsDisabled = false;

    void Start()  // Start is called before the first frame update
    {
        rigidBody = GetComponent<Rigidbody>();  //ref rigidbody component from Rocketship on Unity Editor
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update() // Update is called once per frame
    {
        if (state == State.Alive)
        { //todo somewhere stop sound
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) //only respond to this method if debug build
        {
            RespondToDebugKeys();
        }

    }

    void RespondToRotateInput()
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

    void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))   //can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust*Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    void OnCollisionEnter(Collision collision) //determines collision behavior based on object tag
    {
        if (state != State.Alive || collisionsDisabled) { return; }//if not alive, do not execute below
        switch (collision.gameObject.tag) //switch collision based on tag of object
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                //kill player
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(nextLevelSound);
        mainEngineParticles.Stop();
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);  //delay, can be changed in inspector
    }
    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop(); //stop thrust sound
        audioSource.PlayOneShot(explosion);
        mainEngineParticles.Stop();
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))   //can thrust while rotating
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; //toggle
        }
    }
}
