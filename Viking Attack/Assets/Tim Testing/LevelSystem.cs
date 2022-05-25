using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem : MonoBehaviour
{

    public int xp;
    public int currentLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateXp(5);
    }
    public void UpdateXp(int XP)
    {
        xp += XP;

        int curLvl = (int)(0.1f + Mathf.Sqrt(xp));
        if(curLvl != currentLevel)
        {
            currentLevel = curLvl;
        }
        int xpNextLevel = 100 * (currentLevel + 1) * (currentLevel + 1);
        int differencexp = xpNextLevel - xp;
        int totalDif = xpNextLevel - (100 * currentLevel * currentLevel);
    }
}
