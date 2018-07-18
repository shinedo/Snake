using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartUIController : MonoBehaviour {

    public Text lastText;
    public Text bestText;
    public Toggle blue;
    public Toggle yellow;
    public Toggle border;
    public Toggle noBorder;

    private void Awake()
    {
        lastText.text = "上次：长度" + PlayerPrefs.GetInt("lastLength", 0) + "," + " 分数" + PlayerPrefs.GetInt("lastScore", 0);
        bestText.text = "最好：长度" + PlayerPrefs.GetInt("bestLength", 0) + "," + " 分数" + PlayerPrefs.GetInt("bestScore", 0);
    }

    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void Start()
    {
        if (PlayerPrefs.GetString("sh", "sh01") == "sh01")
        {
            blue.isOn = true;
            PlayerPrefs.SetString("sh", "sh01");
            PlayerPrefs.SetString("sb01", "sb0101");
            PlayerPrefs.SetString("sb02", "sb0102");
        }
        else
        {
            yellow.isOn = true;
            PlayerPrefs.SetString("sh", "sh02");
            PlayerPrefs.SetString("sb01", "sb0201");
            PlayerPrefs.SetString("sb02", "sb0202");
        }

        if (PlayerPrefs.GetInt("border",1)==1)
        {
            border.isOn = true;
            PlayerPrefs.SetInt("border", 1);
        }
        else
        {
            noBorder.isOn = true;
            PlayerPrefs.SetInt("border", 0); 
        }
    }

    public void BlueSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetString("sh", "sh01");
            PlayerPrefs.SetString("sb01", "sb0101");
            PlayerPrefs.SetString("sb02", "sb0102");
        }
    }

    public void YellowSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetString("sh", "sh02");
            PlayerPrefs.SetString("sb01", "sb0201");
            PlayerPrefs.SetString("sb02", "sb0202");
        }
    }

    public void BorderSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("border", 1);
        }
    }

    public void NoBorderSelected(bool isOn)
    {
        if (isOn)
        {
            PlayerPrefs.SetInt("border", 0);
        }
    }
}
