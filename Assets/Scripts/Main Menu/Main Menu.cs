using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] AudioSource buttonClick;
    [SerializeField] AudioClip defaultButton;
    [SerializeField] AudioClip cancelButton;
    [SerializeField] AudioClip checkBoxButton;
    [SerializeField] AudioClip sliderSound;

    public void play()
    {
        SceneManager.LoadScene("Main Game");
    }

    public void NewGame_()
    {
        PlayClick();
        //New Game+ Options

        //Save Screen Interface

        //Choose save slot

        //When finished, load new game interface here and preload squadlist
    }

    public void NewGame()
    {
        StartCoroutine(WaitForNewGame());
    }

    public void LoadGame()
    {
        PlayClick();
        //Load screen interface

        //When finished, load transition interface here
    }

    public void PlayClick()
    {
        buttonClick.clip = defaultButton;
        buttonClick.Play();
    }

    public void PlayCancel()
    {
        buttonClick.clip = cancelButton;
        buttonClick.Play();
    }

    public void PlayCheckbox()
    {
        buttonClick.clip = checkBoxButton;
        buttonClick.Play();
    }

    public void PlaySlider()
    {
        buttonClick.clip = sliderSound;
        buttonClick.Play();
    }

    public void quit()
    {
        StartCoroutine(WaitForQuit());
    }

    public void Update()
    {
        if(Input.GetKeyUp(GlobalSettings.ControlMap[SettingKey.QuickLoad])) Debug.Log("QuickLoad the game");
    }

    IEnumerator WaitForQuit()
    {
        if(!buttonClick.isPlaying) PlayClick();

        while(buttonClick.isPlaying)
        {
            yield return null;
        }

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    IEnumerator WaitForNewGame()
    {
        if(!buttonClick.isPlaying) PlayClick();

        while(buttonClick.isPlaying)
        {
            yield return null;
        }

        SceneManager.LoadScene("Transition Phase");

        UnitResourceManager.Gold = 2500;
        UnitResourceManager.Iron = 10;
        UnitResourceManager.MagicGems = 10;
        UnitResourceManager.Horses = 10;
        
        //Save screen interface

        //Choose save slot

        //When finished, load new game interface here
    }
}