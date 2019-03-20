using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    private float score = 0.0f;

    private int difficultyLevel = 1;
    private int maxDifficultyLevel = 5;
    private int prevDifficulty = 0;
    private int scoreToNextLevel = 20;
    public Text scoreText;

    private bool isDeath = false;
    public DeathMenu deathMenu;

    public Image active1;
    public Text counter1;
    private bool count = false;
    private int time_left;
    private bool doIt = true;

    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {

        if (isDeath)
            return;

        if (score>= scoreToNextLevel)
            LevelUp();


        score += Time.deltaTime * difficultyLevel;
        scoreText.text = ((int)score).ToString();

        if (count == true)
        {

            if (doIt == true)
            {
                time_left = (int)Time.time;
                doIt = false;
            }
            counter1.text = (time_left + 10 - (int)Time.time).ToString();
        }
    }

    void LevelUp()
    {
        if (difficultyLevel == maxDifficultyLevel)
            return;

        scoreToNextLevel *= 4;
        difficultyLevel++;

        GetComponent<PlayerMotor>().SetSpeed(difficultyLevel);

        Debug.Log(difficultyLevel);
    }

    public void OnDeath()
    {
        isDeath = true;

        if(PlayerPrefs.GetFloat("Highscore") < score)
            PlayerPrefs.SetFloat("Highscore", score);

        deathMenu.ToggleEndMenu(score);
    }

    public void Money()
    {
        score = score + difficultyLevel*10;
    }

    public void SlowDown()
    {
        //for 10 sec difficulty is equal to 1
        StartCoroutine(Slow());
    }

    IEnumerator Slow()
    {
        active1.color = new Color(0, 255, 0, 255);
        counter1.color = new Color(0, 255, 0, 255);
        count = true;
        doIt = true;

        prevDifficulty = difficultyLevel;
        difficultyLevel = 1;
        GetComponent<PlayerMotor>().SetSpeed(difficultyLevel);

        yield return new WaitForSeconds(10);

        difficultyLevel = prevDifficulty;
        GetComponent<PlayerMotor>().SetSpeed(difficultyLevel);

        count = false;
        doIt = false;
        counter1.color = new Color(128, 207, 75, 0);
        active1.color = new Color(128, 207, 75, 0);
    }
}
