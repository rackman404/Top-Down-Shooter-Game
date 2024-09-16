using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIMenuController : MonoBehaviour
{
    public GUIMainController GUImainControl;

    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button restartB;
    public Button manualB;
    public Button quitToMainB;
    public Button saveB;
    public Button loadB;
    public Button resumeB;
    
    [Header("Manual Panel")]
    public Button manualHideB;
    public GameObject manualPanel;

    [Header("Main Menu")]
    public GameObject mainMenuPanel;

    public Button mainMenuStartB;
    public Button mainMenuLoadB;
    public Button mainMenuQuitToDeskB;

    // Start is called before the first frame update
    void Start()
    {
        manualPanel.SetActive(false);

        //external game events
        restartB.onClick.AddListener(GUImainControl.CallReset);
        saveB.onClick.AddListener(GUImainControl.CallSave);
        loadB.onClick.AddListener(GUImainControl.CallLoad);
        resumeB.onClick.AddListener(GUImainControl.CallResume);
        quitToMainB.onClick.AddListener(GUImainControl.CallExitLevel);

        mainMenuStartB.onClick.AddListener(GUImainControl.CallStartLevel);
        mainMenuLoadB.onClick.AddListener(GUImainControl.CallLoadMainMenu);
        mainMenuQuitToDeskB.onClick.AddListener(GUImainControl.CallExitToDesktop);

        //internal menu events
        manualB.onClick.AddListener(ShowManual);
        manualHideB.onClick.AddListener(HideManual);
    }

    public void MainMenuMode(){
        mainMenuPanel.SetActive(true);
        manualPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void GameMenuMode(){
        mainMenuPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    void ShowManual(){
        manualPanel.SetActive(true);
    }

    void HideManual(){
        manualPanel.SetActive(false);
    }


}
