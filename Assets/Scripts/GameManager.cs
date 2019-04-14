using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public float gameStartDelay = 2f;

    public UnityEvent awakeGame;
    public UnityEvent startGame;
    
    IEnumerator StartGameOnDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        startGame.Invoke();
    }

    public void StartGame()
    {
        awakeGame.Invoke();
        StartCoroutine(StartGameOnDelay(gameStartDelay));
    }
}
