using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static bool Gamebool = false;
    public GameObject GameOverUI;
    void Update()
    {
        if(Gamebool){
            GameOverUI.SetActive(true);
            Debug.Log("게임오버");
            Time.timeScale = 0f;
        }

        else Time.timeScale = 1f;
    }

    public void Retry(){
        Debug.Log("재시작");
        SoundManager.instance.PlaySFX("Click");
        Gamebool = false;
        GameOverUI.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToLevelSelect(){
        Debug.Log("레벨 선택 이동");
        SoundManager.instance.PlaySFX("Click");
        SoundManager.instance.PlayBGM("Main");
        SceneManager.LoadScene("LevelSelect");
        Gamebool = false;
    }

    public void QuitGame(){
        Debug.Log("게임 나가기 성공");
        SoundManager.instance.PlaySFX("Click");
        Application.Quit();
    }
}