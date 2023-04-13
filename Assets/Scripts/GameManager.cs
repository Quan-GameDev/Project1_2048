using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public CanvasGroup win4096;
    public CanvasGroup continue2048;    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    public AudioSource audio2048;
    public AudioSource audio4096;
    public AudioSource audioGameOver;

    public GameObject game;

    private int score;
    public TileBoard a;
    public TileBoard b;
    private void Start()
    {
        
        NewGame();
    }

    public void NewGame()
    {
        board.a = 0;
        board.b = 0;
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
        gameOver.blocksRaycasts = true;
        StartCoroutine(Fade(gameOver, 1f, 1f));
        audioGameOver.Play();
    }

    public void Continue()
    {
        board.enabled = false;
        continue2048.interactable = true;
        continue2048.blocksRaycasts = true;
        audio2048.Play();
        StartCoroutine(Fade(continue2048, 1f, 1f));
        
    }

    public void Win()
    {
        board.enabled = false;
        win4096.interactable = true;
        win4096.blocksRaycasts = true;
        StartCoroutine(Fade(win4096, 1f, 1f));
        audio4096.Play();
    }

    public void exitGame(){
        Application.Quit();
    }

    public void continueGame(){
        board.enabled = true;
        continue2048.alpha = 0f;
        continue2048.interactable = false;
        board.a = 0;
        board.b = 0;
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
