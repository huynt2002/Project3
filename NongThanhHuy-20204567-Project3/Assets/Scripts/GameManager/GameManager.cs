using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public bool pause { get; private set; }
    public bool playerInfo { get; private set; }
    public bool diaLog { get; private set; }
    MainCanvasManager mainCanvasManager;
    public bool isBossFight;
    public bool isLose;
    public bool isWin;

    public delegate void EndGame();
    public EndGame lose;

    void Start()
    {
        instance = this;
        mainCanvasManager = GetComponent<MainCanvasManager>();
        playerInfo = false;
        pause = false;
        isBossFight = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLose || isWin)
        {
            lose.Invoke();
        }
    }

    public void RestartGame()
    {
        StartCoroutine(Restart());
    }

    IEnumerator Restart()
    {
        isBossFight = false;
        isLose = false;
        isWin = false;
        Resume();
        LevelManager.instance?.Transis();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("PlayScene");
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void PauseResume(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (pause)
            {
                Resume();
            }
            else if (playerInfo)
            {
                PlayerInfoHide();
            }
            else if (diaLog)
            {

            }
            else
            {
                Pause();
            }
        }
    }

    public void PlayerInfo(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (pause || diaLog)
            {
                return;
            }
            if (playerInfo)
            {
                PlayerInfoHide();
            }
            else
            {
                PlayerInfoShow();
            }
        }
    }

    public void PlayerInfoShow()
    {
        Time.timeScale = 0;
        playerInfo = true;
        PlayerStats.instance.GetComponent<PlayerInput>().enabled = false;
        mainCanvasManager.ManagePlayerInfoCanvas();
    }

    public void PlayerInfoHide()
    {
        Time.timeScale = 1;
        playerInfo = false;
        PlayerStats.instance.GetComponent<PlayerInput>().enabled = true;
        mainCanvasManager.ManagePlayerInfoCanvas();
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (pause || !playerInfo || diaLog)
            {
                return;
            }
            if (playerInfo)
            {
                var i = mainCanvasManager.GetItemIndex();
                if (i != -1)
                {
                    InventoryManager.instance.DropItem(InventoryManager.instance.items[i]);
                    PlayerInfoHide();
                }
            }
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        pause = true;
        PlayerStats.instance.GetComponent<PlayerInput>().enabled = false;
        mainCanvasManager.ManagePauseCanvas();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        pause = false;
        PlayerStats.instance.GetComponent<PlayerInput>().enabled = true;
        mainCanvasManager.ManagePauseCanvas();
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Setting()
    {

    }

    public void Dialog()
    {
        diaLog = true;
        PlayerStats.instance.GetComponent<PlayerInput>().enabled = false;
    }

    public void DisableDialog()
    {
        diaLog = false;
        PlayerStats.instance.GetComponent<PlayerInput>().enabled = true;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
