using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public GenerateGrid grid;
    private int numberOfTilesDestroyed;
    public TMP_Text score;
    public PowerUpManager powerUpManager;

    private void Start()
    {
        score.text = "0";        
    }

    public void HandleScore(int numberOfTilesDestroyed)
    {
        if (grid.isTilesBroken)
        {
            if(numberOfTilesDestroyed == 3)
            {
                score.text = (int.Parse(score.text) + 15).ToString();
                
            } else if(numberOfTilesDestroyed == 4)
            {
                score.text = (int.Parse(score.text) + 30).ToString();
                powerUpManager.IncreaseLiquid();
            }
            else if(numberOfTilesDestroyed >= 5)
            {
                score.text = (int.Parse(score.text) + 50).ToString();
            }
        }
    }
}
