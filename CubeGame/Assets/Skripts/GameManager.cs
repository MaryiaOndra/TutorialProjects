using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject completeLevelUI;

    bool gameHasEnded = false;
    float invokeTime = 1f;
    
    public void CompleteLevel() 
    {
        completeLevelUI.SetActive(true);
    }

    public void EndGame()
    {
        if (gameHasEnded.Equals(false))
        {
            gameHasEnded = true;
            Invoke("Restart", invokeTime);
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
