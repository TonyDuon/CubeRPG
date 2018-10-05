using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    [HideInInspector] public bool isInteracting = false;
    //[HideInInspector] public Transform tf;
    [HideInInspector] public Rigidbody rb;

    public GameController gameController;
    public GameObject dialogPanel;

    public string[] currentDialog;
    public int DialogCounter = 0;
    public bool interactable = false;
    public bool triggerBattleFromDialog = false;

    //public float playerForce = 200;
    public float movementSpeed = 7f;
    public float turnSpeed = 2;

    public bool encounter = false;
    public int encounterRate = 50;
    public float nextActionTime = 0.0f;
    public float period = 0.1f;

    // Use this for initialization
    void Start () {
        rb = this.GetComponent<Rigidbody>();

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        transform.position = gameController.playerPosition;
        transform.eulerAngles = gameController.playerEulerAngles;

        //tf = this.GetComponent<Transform>();
	}

    void printDialog(int counter)
    {
        if (counter < currentDialog.Length)
        {
            dialogPanel.SetActive(true);
            dialogPanel.GetComponentInChildren<Text>().text = currentDialog[counter];
        }
        else
        {
            isInteracting = false; //dialog is finished
            DialogCounter = 0;
            if (triggerBattleFromDialog)
            {
                gameController.LoadBattle(transform.position, transform.eulerAngles);
            }
        }
        
    }

	// Update is called once per frame
	void FixedUpdate () {

        if (isInteracting)
        {
            //space to log window and advance dialog
            if (Input.GetKeyDown(KeyCode.Space))
            {
                dialogPanel.SetActive(false);
                DialogCounter++;
                printDialog(DialogCounter);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (interactable)
            {
                isInteracting = true;
                printDialog(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            gameController.LoadBattle(transform.position, transform.eulerAngles);
        }
        else
        {
            //used to store direction in +1 or -1
            int horizontal = 0;
            int vertical = 0;

            //get the input and cast it from float to int
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));


            if (horizontal != 0 || vertical != 0)
            {
                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(horizontal, 0, vertical));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

                

                transform.position += new Vector3(horizontal*movementSpeed* Time.deltaTime, 0, vertical*movementSpeed* Time.deltaTime);
                if (encounter)
                {
                    period += 0.01f;
                    if (period > 1)
                    {
                        period = 0;
                        checkEncounter();
                    }                 
                }



            }
        }
	}

    void checkEncounter()
    {
        int rng = UnityEngine.Random.Range(1, 101);
        if (rng < encounterRate)
        {
            encounterRate = (int) Mathf.Floor( encounterRate /= 2 ); //reduce encounter rate per encounter
            gameController.LoadBattle(transform.position, transform.eulerAngles);
        }

    }

    void OnTriggerEnter(Collider collisionInfo)
    {

        if (collisionInfo.tag == "Interactable")
        {
            NPC_Script newDialog = collisionInfo.GetComponentInParent<NPC_Script>();
            currentDialog = newDialog.Dialog;
            DialogCounter = 0;

            if (newDialog.triggerBattle)
            {
                triggerBattleFromDialog = true;
            }
            
            interactable = true;
        }
        else if (collisionInfo.tag == "ChangeScene")
        {
            ChangeScene newScene = collisionInfo.GetComponent<ChangeScene>();
            gameController.MoveScene(newScene.SceneName, newScene.startingPosition, newScene.startingRotation);
        }
        else if (collisionInfo.tag == "Encounter")
        {
            encounter = true;
        }
    }

    void OnTriggerExit(Collider collisionInfo)
    {
        if (collisionInfo.tag == "Interactable")
        {
            currentDialog = null;
            interactable = false;
            triggerBattleFromDialog = false;
        }
        else if (collisionInfo.tag == "Encounter")
        {
            encounter = false;
        }
    }
}
