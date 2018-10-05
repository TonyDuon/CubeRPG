using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenNavigation : MonoBehaviour {

    public GameController gameController;
    public GameObject Title;
    public GameObject Instructions;
    int state = 0;

	// Use this for initialization
	void Start () {
        Instructions.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (state)
            {
                case 0:
                    Title.SetActive(false);
                    Instructions.SetActive(true);
                    break;
                case 1:
                    gameController.LoadField();
                    break;
            }

            state++;
        }

    }
}
