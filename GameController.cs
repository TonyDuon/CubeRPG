using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GameController mainGC;
    public Vector3 playerPosition = new Vector3(0, 1, 0);
    public Vector3 playerEulerAngles = new Vector3(0, 0, 0);

    public string previousScene;

    // Use this for initialization
    void Start() {
        if (mainGC == null)
        {
            mainGC = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            GameObject.Destroy(this);
        }



    }

    //// Update is called once per frame
    //void Update () {

    //}

    public void LoadBattle(Vector3 playerPosition, Vector3 playerEulerAngles)
    {
        this.playerPosition = playerPosition;
        this.playerEulerAngles = playerEulerAngles;
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Battle");

    }

    public void LoadField()
    {
        SceneManager.LoadScene(previousScene);
    }

    public void MoveScene(string SceneName, Vector3 startingPosition, Vector3 startingRotation)
    {
        SceneManager.LoadScene(SceneName);
        this.playerPosition = startingPosition;
        this.playerEulerAngles = startingRotation;
    }

    public void StartGame()
    {
        
    }
}
