using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattle : CharacterStatSheet
{
    public int Level = 5;
    public float EXP = 0;
    public float nextLevel = 100;

    public string[] skills = new string[3];
	
	void Start ()
    {
    }

    void addEXP(float EXP)
    {
        this.EXP = EXP;

        if (EXP >= nextLevel)
        {
            Level++;
            statIncrease();
            EXP -= nextLevel;
        }

    }

    void statIncrease()
    {
        MaxHP += 10;
        MaxMP += 10;
        Attack += 2;
        Defense += 1;
        Speed += 1;

        //recover on level up
        HP = MaxHP;
        MP = MaxMP;
    }


    
}
