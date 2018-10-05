using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class BattleController : MonoBehaviour {

    public GameController gameController;

    public Text statusText;
    public EventSystem ActionEvent;

    public GameObject DialogPanel;
    public GameObject ActionPanel;
    public GameObject SkillPanel;
    public GameObject ItemPanel;
    public GameObject EnemyPanel;
    public GameObject PlayerPanel;

    public GameObject[] enemyType;
    public GameObject[] playerType;
    public GameObject characterPointer;

    [HideInInspector] public Button[] ActionMenu = new Button[3];
    [HideInInspector] public Button[] SkillMenu = new Button[3];
    [HideInInspector] public Button[] ItemMenu = new Button[3];
    [HideInInspector] public Button[] EnemyMenu = new Button[3];
    [HideInInspector] public Button[] PlayerMenu = new Button[3];
    [HideInInspector] public GameObject[] players;
    [HideInInspector] public GameObject[] enemies;
    [HideInInspector] public GameObject[] turnOrder;
    [HideInInspector] public int turnIndex = 0;

    [HideInInspector] public bool playerTurn = false;
    [HideInInspector] public bool enemyTurn = false;
    [HideInInspector] public bool ActionPhase = false;
    [HideInInspector] public bool WinState = false;
    [HideInInspector] public bool characterDefeated = false;
    [HideInInspector] public bool bossFight = false;
    [HideInInspector] public int currentState = 0;

    [HideInInspector] public int playerIndex = 0;
    [HideInInspector] public int enemyIndex = 0;

    [HideInInspector] public GameObject attacker;
    [HideInInspector] public GameObject target;
    [HideInInspector] public string ActionName;
    [HideInInspector] public Vector3 originalPosition;

    // Use this for initialization
    void Start () {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        spawnAllies();  //Spawn allies on the field
        spawnEnemies(); //Spawn enemies on the field
        SetTurnOrder(); //sets the turn order based on everyone's speed values

        ActionMenu = ActionPanel.GetComponentsInChildren<Button>();
        ActionMenu[0].onClick.AddListener(TaskOnAttack);
        ActionMenu[1].onClick.AddListener(TaskOnSkill);
        ActionMenu[2].onClick.AddListener(TaskOnItem);

        SkillMenu = SkillPanel.GetComponentsInChildren<Button>();
        SkillMenu[0].onClick.AddListener(TaskOn1stSkill);
        SkillMenu[1].onClick.AddListener(TaskOn2ndSkill);
        SkillMenu[2].onClick.AddListener(TaskOn3rdSkill);

        ItemMenu = ItemPanel.GetComponentsInChildren<Button>();
        ItemMenu[0].onClick.AddListener(TaskOnPotion);
        ItemMenu[1].onClick.AddListener(TaskOnFullHeal);

        EnemyMenu = EnemyPanel.GetComponentsInChildren<Button>();
        EnemyMenu[0].onClick.AddListener(() => { TaskOnTargetEnemy(playerIndex, 0); });
        EnemyMenu[1].onClick.AddListener(() => { TaskOnTargetEnemy(playerIndex, 1); });
        EnemyMenu[2].onClick.AddListener(() => { TaskOnTargetEnemy(playerIndex, 2); });

        PlayerMenu = PlayerPanel.GetComponentsInChildren<Button>();
        PlayerMenu[0].onClick.AddListener(() => { TaskOnTargetPlayer(playerIndex, 0); });
        PlayerMenu[1].onClick.AddListener(() => { TaskOnTargetPlayer(playerIndex, 1); });

        PlayerMenu[0].GetComponentInChildren<Text>().text = players[0].GetComponent<PlayerBattle>().Name;
        if (players.Length > 1)
        {
            PlayerMenu[1].GetComponentInChildren<Text>().text = players[1].GetComponent<PlayerBattle>().Name;
        }
        

        UpdateEnemies(); //sets targetables enemies
        UpdateStatus(); //draws all character health on GUI
    }

    void spawnEnemies()
    {
        if (gameController.previousScene == "BossRoom")
        {
            Instantiate(enemyType[3], new Vector3(-3, 1, 0), Quaternion.identity);
            bossFight = true;
        }
        else
        {
            int rng = UnityEngine.Random.Range(1, 3);

            //if (rng > 2)
            //{
            //    Instantiate(enemyType[0], new Vector3(-3, 1, 0), Quaternion.identity);
            //    Instantiate(enemyType[1], new Vector3(-3, 1, -2), Quaternion.identity);
            //    Instantiate(enemyType[2], new Vector3(-3, 1, 2), Quaternion.identity);
            //}else
            if (rng > 1)
            {
                Instantiate(enemyType[0], new Vector3(-3, 1, -1), Quaternion.identity);
                Instantiate(enemyType[1], new Vector3(-3, 1, 1), Quaternion.identity);
            }
            else
            {
                Instantiate(enemyType[0], new Vector3(-3, 1, 0), Quaternion.identity);
            }
        }
    }

    void spawnAllies()
    {
        if (gameController.previousScene == "BossRoom")
        {
            Instantiate(playerType[0], new Vector3(3, 1, -1), Quaternion.identity);
            Instantiate(playerType[1], new Vector3(3, 1, 1), Quaternion.identity);
        }
        else
        {
            Instantiate(playerType[0], new Vector3(3, 1, 0), Quaternion.identity);
        }
    }

    void SetTurnOrder()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] characterOrder = new GameObject[players.Length + enemies.Length];

        Array.Copy(players, characterOrder, players.Length);
        Array.Copy(enemies, 0, characterOrder, players.Length, enemies.Length);

        float[] StatOrder = new float[players.Length + enemies.Length];
        for (int i = 0; i < characterOrder.Length; i++)
        {
            StatOrder[i] = characterOrder[i].GetComponent<CharacterStatSheet>().Speed;
        }


        for (int i = 0; i < StatOrder.Length-1; i++)
        {
            for (int j = i+1; j < StatOrder.Length; j++)
            {
                if (StatOrder[i] < StatOrder[j])
                {
                    float temp1 = StatOrder[i];
                    StatOrder[i] = StatOrder[j];
                    StatOrder[j] = temp1;

                    GameObject temp2 = characterOrder[i];
                    characterOrder[i] = characterOrder[j];
                    characterOrder[j] = temp2;
                }
            }
        }

        this.turnOrder = characterOrder;
    }

    void TaskOnAttack()
    {
        ActionName = "Attack";
        AdvancePlayerPanel(3);
    }

    void TaskOnSkill()
    {
        AdvancePlayerPanel(1);
    }

    void TaskOn1stSkill()
    {
        ActionName = players[playerIndex].GetComponent<PlayerBattle>().skills[0];
        AdvancePlayerPanel(3);
    }

    void TaskOn2ndSkill()
    {
        ActionName = players[playerIndex].GetComponent<PlayerBattle>().skills[1];
        AdvancePlayerPanel(4);
    }

    void TaskOn3rdSkill()
    {
        ActionName = players[playerIndex].GetComponent<PlayerBattle>().skills[2];
        AdvancePlayerPanel(4);
    }

    void TaskOnItem()
    {
        AdvancePlayerPanel(2);
    }

    void TaskOnPotion()
    {
        ActionName = "Potion";
        AdvancePlayerPanel(4);
    }

    void TaskOnFullHeal()
    {
        ActionName = "FullHeal";
        AdvancePlayerPanel(4);
    }

    void TaskOnTargetEnemy(int playerIndex, int enemyIndex)
    {
        this.enemyIndex = enemyIndex;
        AdvancePlayerPanel(-1);
        AdvanceActionPhase(0, players[playerIndex], enemies[enemyIndex]);
    }

    void TaskOnTargetPlayer(int playerIndex, int targetIndex)
    {
        AdvancePlayerPanel(-1);
        AdvanceActionPhase(0, players[playerIndex], players[targetIndex]);
    }

    void AdvanceActionPhase(int phase, GameObject attacker, GameObject target)
    {
        switch (phase)
        {
            case -1: break;
            case 0:
                ActionPhase = true;
                this.attacker = attacker;
                this.target = target;

                //move attacker to center
                originalPosition = attacker.GetComponent<Transform>().position;
                attacker.GetComponent<Transform>().position = new Vector3(0, 1, 0);
                //open dialog window and announce action
                DialogPanel.SetActive(true);

                if (ActionName == "Focus")
                {
                    DialogPanel.GetComponentInChildren<Text>().text =
                        attacker.GetComponent<CharacterStatSheet>().Name + " used Focus!";
                }
                else
                {
                    DialogPanel.GetComponentInChildren<Text>().text =
                        attacker.GetComponent<CharacterStatSheet>().Name + " " + ActionName + "s " +
                        target.GetComponent<CharacterStatSheet>().Name + "!";
                }

                break;
            case 1:
                //If I had animation it would occur here

                //calculate damage
                float damage = ProcessAction(attacker, target);
                UpdateStatus();
                //open dialog window and announce damage dealt
                DialogPanel.SetActive(true);

                if (damage == 0)
                {
                    if (ActionName == "Focus")
                    {
                        DialogPanel.GetComponentInChildren<Text>().text =
                            target.GetComponent<CharacterStatSheet>().Name + " attack increased for 1 turn!";
                    }
                    else if (ActionName == "Shield")
                    {
                        DialogPanel.GetComponentInChildren<Text>().text =
                            target.GetComponent<CharacterStatSheet>().Name + " defense increased!";
                    }

                }
                else if (damage > -1)
                {
                    DialogPanel.GetComponentInChildren<Text>().text =
                    attacker.GetComponent<CharacterStatSheet>().Name + " deals " + damage + " damage!";
                }
                else
                {
                    DialogPanel.GetComponentInChildren<Text>().text =
                    attacker.GetComponent<CharacterStatSheet>().Name + " recovers " + Math.Abs(damage) + " HP!";
                }
                
                break;
            case 2:
                //return attacker's to their original position
                attacker.GetComponent<Transform>().position = originalPosition;
                ActionPhase = false;
                currentState = 0;

                WinState = CheckForEnemies();
                if (WinState)
                {
                    AdvanceWinDialog(0);
                }

                break;
            default: break;
        }
    }

    void AdvanceWinDialog(int state)
    {
        switch (state)
        {
            case 0:
                //play victory music here
                DialogPanel.SetActive(true);
                DialogPanel.GetComponentInChildren<Text>().text = "YOU WON THE BATTLE!";
                //TODO: add exp to all players and add additonal windows on level up
                break;
            case 1:

                ExitBattle();
                break;
            default:
                break;
        }
    }

    void AdvancePlayerPanel(int state)
    {
        switch (state)
        {
            case 0: //Attack-Skill-Item panel
                SkillPanel.SetActive(false);
                ItemPanel.SetActive(false);
                EnemyPanel.SetActive(false);
                PlayerPanel.SetActive(false);

                ActionPanel.SetActive(true);
                ActionMenu[0].interactable = true;
                ActionMenu[1].interactable = true;
                ActionMenu[2].interactable = true;
                ActionEvent.SetSelectedGameObject(GameObject.FindGameObjectsWithTag("Button")[0]);
                break;

            case 1:
                //When Skill is selected, disable main panel and open up skill panel
                ActionMenu[0].interactable = false;
                ActionMenu[1].interactable = false;
                ActionMenu[2].interactable = false;

                SkillPanel.SetActive(true);
                ActionEvent.SetSelectedGameObject(GameObject.FindGameObjectsWithTag("Button")[1]);
                break;

            case 2:
                //When Item is selected, disable main panel and open up item panel
                ActionMenu[0].interactable = false;
                ActionMenu[1].interactable = false;
                ActionMenu[2].interactable = false;

                ItemPanel.SetActive(true);
                ActionEvent.SetSelectedGameObject(GameObject.FindGameObjectsWithTag("Button")[1]);
                break;

            case 3:
                //Open targetable enemy panel from either skill panel or Attack
                SkillPanel.SetActive(false);
                ActionMenu[0].interactable = false;
                ActionMenu[1].interactable = false;
                ActionMenu[2].interactable = false;

                EnemyPanel.SetActive(true);
                ActionEvent.SetSelectedGameObject(GameObject.FindGameObjectsWithTag("Button")[1]);
                break;

            case 4:
                //Open targetable player panel from either skill panel or item panel
                SkillPanel.SetActive(false);
                ItemPanel.SetActive(false);

                PlayerPanel.SetActive(true);
                if (players.Length == 1)
                {
                    PlayerMenu[1].gameObject.SetActive(false);
                }
                ActionEvent.SetSelectedGameObject(GameObject.FindGameObjectsWithTag("Button")[1]);
                break;

            case -1: 
                //User has confirmed thier action for this turn, remove all panels and cursor
                ActionPanel.SetActive(false);
                SkillPanel.SetActive(false);
                ItemPanel.SetActive(false);
                EnemyPanel.SetActive(false);
                PlayerPanel.SetActive(false);
                characterPointer.GetComponent<Transform>().position = new Vector3(0, 0, 0);
                playerTurn = false;
                break;
            default: break;

        }
    }

	// Update is called once per frame
	void Update () {

        //if none of the booleans are true, decide who acts next in the turn order
        if (!ActionPhase && !playerTurn && !enemyTurn && !WinState)
        {
            //reorder turn order if a character has been defeated
            if (characterDefeated)
            {
                characterDefeated = false;
                SetTurnOrder();
                UpdateEnemies();
            }

            //once everyone had thier turn, loop turn order counter back to 0
            if (turnIndex > turnOrder.Length-1)
            {
                turnIndex = 0;
            }
            //if the next turn is a defeated character skip thier turn (may be redundant)
            if (turnOrder[turnIndex] == null)
            {
                turnIndex++;
            }
            else if (turnOrder[turnIndex].tag == "Player") //if the next turn is a playable character
            {
                
                playerTurn = true;
                
                AdvancePlayerPanel(0);
                for (int i = 0; i < players.Length; i++)
                {
                    if (turnOrder[turnIndex] == players[i])
                    {
                        playerIndex = i;
                        characterPointer.GetComponent<Transform>().position = players[i].GetComponent<Transform>().position + new Vector3(0,1,0);
                        SkillMenu[0].GetComponentInChildren<Text>().text = players[playerIndex].GetComponent<PlayerBattle>().skills[0];
                        SkillMenu[1].GetComponentInChildren<Text>().text = players[playerIndex].GetComponent<PlayerBattle>().skills[1];
                        SkillMenu[2].GetComponentInChildren<Text>().text = players[playerIndex].GetComponent<PlayerBattle>().skills[2];
                        break;
                    }
                }
                turnIndex++;
            }
            else if (turnOrder[turnIndex].tag == "Enemy") //if the next turn is an enemy
            {
                enemyTurn = true;

                for (int i = 0; i < enemies.Length; i++)
                {
                    if (turnOrder[turnIndex] == enemies[i])
                    {
                        enemyIndex = i;
                        break;
                    }
                }
                turnIndex++;
            }
        }
        else if (WinState) //if the battle is over print dialog
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //close the panel
                DialogPanel.SetActive(false);
                currentState++;
                AdvanceWinDialog(currentState);
            }   
        }
        else if (ActionPhase) //if an action is occuring, print dialog
        {
            //space to advance dialog during battle
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //close the panel
                DialogPanel.SetActive(false);
                currentState++;
                AdvanceActionPhase(currentState, attacker, target);
            } 
        }
        else if (playerTurn) //during player's turn read for player input
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //return to default selection
                AdvancePlayerPanel(0);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                //currentState = -1;
            }

        }
        else if (enemyTurn) //during enemy's turn, attack player
        {
            //Potential TODO would be to randomise enemy attack pattern here by using skills
            enemyTurn = false;
            ActionName = "Attack";
            AdvanceActionPhase(0, enemies[enemyIndex], players[0]);
            
        }

    }

    //Recieves the attacker and target to process action based on action string
    float ProcessAction(GameObject player, GameObject target)
    {
        CharacterStatSheet attackerStat = player.GetComponent<CharacterStatSheet>();
        CharacterStatSheet targetStat = target.GetComponent<CharacterStatSheet>();
        float damage = 0;
        bool buff = false;

        switch (ActionName)
        {
            case "Attack":
                damage = attackerStat.Attack - targetStat.Defense;
                if (attackerStat.FocusActive)
                {
                    attackerStat.FocusActive = false;
                    attackerStat.Attack /= 2.5f;
                }
                break;
            case "X-Cut": 
                //double attack
                damage = (attackerStat.Attack - targetStat.Defense)* 2;
                if (attackerStat.FocusActive)
                {
                    attackerStat.FocusActive = false;
                    attackerStat.Attack /= 2.5f;
                }
                break;
            case "Pierce": 
                //ignores defense
                damage = attackerStat.Attack;
                if (attackerStat.FocusActive)
                {
                    attackerStat.FocusActive = false;
                    attackerStat.Attack /= 2.5f;
                }
                break;
            case "Heal": 
                //recovers HP, scales with attack
                damage = attackerStat.Attack * -1.5f;
                if (attackerStat.FocusActive)
                {
                    attackerStat.FocusActive = false;
                    attackerStat.Attack /= 2.5f;
                }
                break;
            case "Focus": 
                //double attack stat for the next attack/heal. Does not stack. 
                buff = true;
                if (targetStat.FocusActive == false)
                {
                    targetStat.FocusActive = true;
                    targetStat.Attack *= 2.5f;
                }
                break;
            case "Shield": 
                //buffs defense for target. Does not stack
                buff = true;
                if (targetStat.ShieldActive == false)
                {
                    targetStat.ShieldActive = true;
                    targetStat.Defense *= 1.5f;
                }
                break;
            case "Potion":
                damage = -25f;
                break;
            case "FullHeal":
                damage = -50f;
                break;
            default: break;
        }
        
        if (buff) //if focus or shield skill was used
        {
            return 0;
        }
        else
        {
            targetStat.HP -= (float) Math.Floor(damage);

            if (targetStat.HP > targetStat.MaxHP) //prevent over-healing
            {
                targetStat.HP = targetStat.MaxHP;
            }
        }

        return (float) Math.Floor(damage);
    }

    //Check enemy remaining HP and updates player HP on the status screen
    void UpdateStatus()
    {

        statusText.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            CharacterStatSheet playerStat = players[i].GetComponent<CharacterStatSheet>();
            statusText.text += playerStat.Name+"   " + playerStat.HP + "/"+playerStat.MaxHP+" \n";
        }


        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                CharacterStatSheet enemyStat = enemies[i].GetComponent<CharacterStatSheet>();

                if (enemyStat.HP <= 0)
                {
                    GameObject.Destroy(enemies[i]);
                    //enemies[i].SetActive(false);
                    enemies[i] = null;
                    characterDefeated = true;
                }
                else
                {
                    //statusText.text += enemyStat.Name + "   " + enemyStat.HP + "/" + enemyStat.MaxHP + " \n";
                }
            }
        }
    }

    //Update the buttons used for player targetting
    void UpdateEnemies()
    {
        EnemyPanel.SetActive(true);
        for (int i = 0; i < EnemyMenu.Length; i++)
        {
            if (i > enemies.Length - 1) //if they are less than 3 enemies, remove button
            {
                EnemyMenu[i].gameObject.SetActive(false);
            }
            else if (enemies[i] == null) //if an enemy has been defeated, remove button
            {
                EnemyMenu[i].gameObject.SetActive(false);
            }
            else
            {
                //print enemy name on the selectable button
                EnemyMenu[i].GetComponentInChildren<Text>().text = enemies[i].GetComponent<CharacterStatSheet>().Name;
            }
        }
        EnemyPanel.SetActive(false);
    }

    bool CheckForEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if(enemies[i] != null) return false;
        }
        return true; //all enemies defeated!
    }

    void ExitBattle()
    {
        StartCoroutine(FadeScene());
    }

    IEnumerator FadeScene()
    {
        GameObject fadeToBlack = GameObject.FindGameObjectWithTag("FadeToBlack");
        CanvasGroup CG = fadeToBlack.GetComponent<CanvasGroup>();
        Text demoEndText = fadeToBlack.GetComponentInChildren<Text>();
        demoEndText.enabled = false;

        while (CG.alpha < 1)
        {
            CG.alpha += Time.deltaTime / 2;
            yield return null;
        }
        //as soon as the screen fades to black, transistion back to next screen
        if (bossFight)
        {
            demoEndText.enabled = true;
        }
        else
        {
            gameController.LoadField();
        }
        
        yield return null;

    }
}
