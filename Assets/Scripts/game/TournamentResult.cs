using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TournamentResult : MonoBehaviour
{
    public TMP_Text result;
    public TMP_Text ranking;
    // Start is called before the first frame update
    void Start()
    {
        if (Globals.tournamentInfo.ranking > 0 && Globals.tournamentInfo.ranking < 7 )
        {
            result.text = "Congratulations!";
            switch (Globals.tournamentInfo.ranking)
            {
                case 1:
                    ranking.text = Globals.tournamentInfo.ranking.ToString() + "st";
                    break;
                case 2:
                    ranking.text = Globals.tournamentInfo.ranking.ToString() + "nd";
                    break;
                case 3:
                    ranking.text = Globals.tournamentInfo.ranking.ToString() + "rd";
                    break;
                default:
                    ranking.text = Globals.tournamentInfo.ranking.ToString() + "th";
                    break;
            }
            
        }
        else
        {
            result.text = "You lose";
            ranking.text = "";
        }
    }

    public void GoToMain()
    {
        SceneManager.LoadScene("Home");
    }
}
