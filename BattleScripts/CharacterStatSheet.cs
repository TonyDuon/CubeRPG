using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStatSheet : MonoBehaviour {

    public string Name;
    public float HP, MaxHP, MP, MaxMP, Attack, Defense, Speed;
    public bool FocusActive = false;
    public bool ShieldActive = false;
}
