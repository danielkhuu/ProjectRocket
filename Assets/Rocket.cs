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

    [SerializeField] float rcsThrust = 100f;  //inspector value adjuster for rotation speed
    [SerializeField] float mainThrust = 5f; //inspector value adjuster for main thrust
    [SerializeField] float levelLoadDelay = 1f; //1 second delay

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip nextLevelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    bool isTransitioning= false; //determins if player is respawning or playing the level
    bool collisionsDisabled = false; //disables collision (used for debugging mode)

      AudioSource audioSource; //rocket thrust sound component

    void Start()  // Start is called before the first frame update
    {
        /*references rigidbody component from Rocketship on Unity Editor
         * rigidBody component on rocket allows rocket to collide with other objects in scene
         * */
        rigidBody = GetComponent<Rigidbody>();  
        audioSource = GetComponent<AudioSource>();    
    }
   
    void Update() // Update is called once per frame. Reads key input on each frame
    {
        if (!isTransitioning)       //while player is not respawning, read input
        { 
            RespondToThrustInput();
            RespondToRotateInput();
        }
        if (Debug.isDebugBuild) //only respond to this method if debug build
        {
            RespondToDebugKeys();
        }

    }

    void RespondToRotateInput()         //reads A and D key for left and right rotation
    {
        rigidBody.angularVelocity = Vector3.zero; //remove rotation due to physics
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;  //multiply by frame time. Longer frame time = faster rotate speed
 
        if (Input.GetKey(KeyCode.A))  //cannot rotate both ways at same time
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        

        /*   //if I want to move rocket forward and backwards on z axis
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

    void RespondToThrustInput()        //reads space key and calls ApplyThrust method
    {
        if (Input.GetKey(KeyCode.Space))   
        {
            ApplyThrust();
        }
        else        //only apply thrust if space key is pressed
        {
            StopApplyingThrust();
        }
    }
    void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust*Time.deltaTime);  //applies upward force on rocket
        if (!audioSource.isPlaying)         
        {
            audioSource.PlayOneShot(mainEngine); //plays thrust sound 
        }
        mainEngineParticles.Play();              //engine particles
    }        //applies upward movement on rocket, plays engine sound and particles
    void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    } //stops engine sound and particles

    void OnCollisionEnter(Collision collision) //determines collision behavior based on object tag
    {
        if (isTransitioning || collisionsDisabled) { return; }//if respawning, do not execute below
        switch (collision.gameObject.tag) //switch collision behavior based on tag of object
        {
            case "Friendly":            //ex: launch pad should not kill player
                break;
            case "Finish":              //collision with landing pad will execute next-level sequence
                StartSuccessSequence();
                break;
            default:               
                StartDeathSequence();   //collision with anything else kills player
                break;
        }
    }

    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop();                      //stops thrust sound
        mainEngineParticles.Stop();              //stops engine particles
        audioSource.PlayOneShot(nextLevelSound); //plays next-level sound
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);  //call LoadNextLevel method w/ delay
    }        //win effects and moves to next level
    private void StartDeathSequence()
    {
        isTransitioning = true;
        audioSource.Stop();                 //stops thrust sound
        mainEngineParticles.Stop();        
        audioSource.PlayOneShot(explosion); //play explosion sound
        deathParticles.Play();              
        Invoke("RestartLevel", levelLoadDelay); //call RestartLevel method w/ delay
    }         //death effects and restarts level

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; //gets current scene number
        int nextSceneIndex = currentSceneIndex + 1; //adds 1 to current scene to get next scene
        SceneManager.LoadScene(nextSceneIndex);     //load the scene

        //if reached last level, loop back to first level
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {   
            nextSceneIndex = 0;  
        }
        SceneManager.LoadScene(nextSceneIndex); //loads first level
    }
    private void RestartLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; 
        SceneManager.LoadScene(currentSceneIndex);
    }

    void RespondToDebugKeys() //Debug mode 
    {    /*L for force next level
           C to toggle collision */
        if (Input.GetKeyDown(KeyCode.L))  
        {
            LoadNextLevel();
        }
        else if(Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; //toggle
        }
    }
}

//Daniel Khuu 5/26/2020 0:43 Finished Code for "Project Boost" Unity Project 
