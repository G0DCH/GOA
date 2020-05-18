using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject help;
    public GameObject buttons;

    [SerializeField]
    public GameObject controlPanels;

    private void Start()
    {
        help.SetActive(false);
        controlPanels.SetActive(false);
        buttons.SetActive(true);
    }

    public void EndGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        //SceneManager.LoadScene("Auction");
        help.SetActive(false);
        controlPanels.SetActive(true);
        buttons.SetActive(false);
    }

    public void HelpMenu()
    {
        help.SetActive(true);
        controlPanels.SetActive(false);
        buttons.SetActive(false);
    }
}