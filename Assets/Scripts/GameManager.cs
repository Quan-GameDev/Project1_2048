using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public CanvasGroup continue2048;
    public CanvasGroup win4096;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;

    private int score;

    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        IncreaseScore(0);
        hiscoreText.text = LoadHiscore().ToString();
 
        gameOver.alpha = 0f;
        gameOver.interactable = false;

        continue2048.alpha = 0f;
        continue2048.interactable = false;

        win4096.alpha = 0f;
        win4096.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    public void Continue()
    {
        board.enabled = false;
        continue2048.interactable = true;

        StartCoroutine(Fade(continue2048, 1f, 1f));
    }

    public void Win()
    {
        board.enabled = false;
        win4096.interactable = true;

        StartCoroutine(Fade(win4096, 1f, 1f));
    }

    public void exitGame(){
        Application.Quit();
    }

    public void continueGame(){
        
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHiscore();
    }

    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();

        if (score > hiscore) {
            PlayerPrefs.SetInt("hiscore", score);
        }
    }

    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }

}
