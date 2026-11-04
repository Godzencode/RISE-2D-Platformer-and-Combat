using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private GameObject VictoryPanel;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }



    public void TriggerLosePanel()
    {
        LosePanel.transform.LeanMoveLocalY(0f , 0.8f).setEaseOutElastic();
        Debug.Log("Its Workinh I guess!!");
    }

    public void TriggerVictoryPanel()
    {
        VictoryPanel.transform.LeanMoveLocalY(0f , 0.8f).setEaseOutBack();
    }

    public void LoadMenu()
        {
            Debug.Log(" Load Menu");
            SceneManager.LoadScene("MenuScene");
        }

    public void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Debug.Log("Load Next Level ." );
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitingGame()
    {
        Application.Quit();
        Debug.Log("Quiting game");
    }
}
