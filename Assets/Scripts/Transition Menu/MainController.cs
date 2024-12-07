using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainController : MonoBehaviour
{
    [SerializeField] public ObservableCollection<Unit> UnitList = new();
    [SerializeField] public ObservableCollection<Squad> SquadList = new();
    [SerializeField] public GameObject MessageSystem;
    [SerializeField] public GameObject ParentMenu;

    [Header("Serialized Units")]
    [SerializeField] private List<Unit> SerializedUnitList = new();
    [SerializeField] private List<Squad> SerializedSquadList = new();

    [Header("Resource Display")]
    [SerializeField] TextMeshProUGUI Gold;
    [SerializeField] TextMeshProUGUI Iron;
    [SerializeField] TextMeshProUGUI MagicGems;
    [SerializeField] TextMeshProUGUI Horses;

    [Header("Menu Buttons")]
    [SerializeField] Button OpenMenu;
    [SerializeField] Button CloseMenu;
    [SerializeField] Button PlayMission;
    [SerializeField] Button ManageArmy;
    [SerializeField] Button Marketplace;
    [SerializeField] Button Arena;
    [SerializeField] Button Companion;
    [SerializeField] Button Save;
    [SerializeField] Button Load;
    [SerializeField] Button MainMenu;

    [Header("All Interface Objects")]
    [SerializeField] GameObject ManageArmyInterface;
    [SerializeField] GameObject MarketplaceInterface;
    [SerializeField] GameObject ArenaInterface;
    [SerializeField] GameObject CompanionInterface;
    [SerializeField] GameObject SaveInterface;

    [Header("Audio Interfacing")]
    [SerializeField] AudioSource uiSound;
    [SerializeField] AudioSource musicSound;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip checkBoxClick;
    [SerializeField] AudioClip sliderSound;
    [SerializeField] AudioClip cancelClick;
    [SerializeField] List<AudioClip> musicList = new();

    public void Awake()
    {
        MenuSoundboard.SetupSoundboard(uiSound, buttonClick, checkBoxClick, sliderSound, cancelClick);

        StartCoroutine(MusicReel());

        GetComponent<UnitLoader>().Start();

        UnitList.CollectionChanged += UnitList_CollectionChanged;
        SquadList.CollectionChanged += SquadList_CollectionChanged;

        //Assign buttons
        PlayMission.onClick.AddListener(() => {
            StartCoroutine(WaitForStartGame());
        });
        ManageArmy.onClick.AddListener(() => {
            DisableAllInterfaces();
            ManageArmyInterface.SetActive(true);
            MenuSoundboard.PlayButton();
        });
        Marketplace.onClick.AddListener(() => {
            DisableAllInterfaces();
            MarketplaceInterface.SetActive(true);
            MenuSoundboard.PlayButton();
        });
        Arena.onClick.AddListener(() => {
            DisableAllInterfaces();
            ArenaInterface.SetActive(true);
            MenuSoundboard.PlayButton();
        });
        Companion.onClick.AddListener(() => {
            DisableAllInterfaces();
            CompanionInterface.SetActive(true);
            MenuSoundboard.PlayButton();
        });
        Save.onClick.AddListener(() => {
            DisableAllInterfaces();
            SaveInterface.GetComponent<SaveController>().operation = SaveController.SaveOp.save;
            SaveInterface.SetActive(true);
            MenuSoundboard.PlayButton();
        });
        Load.onClick.AddListener(() => {
            DisableAllInterfaces();
            SaveInterface.GetComponent<SaveController>().operation = SaveController.SaveOp.load;
            SaveInterface.SetActive(true);
            MenuSoundboard.PlayButton();
        });
        MainMenu.onClick.AddListener(() => {
            StartCoroutine(WaitForMainMenu());
        });

        OpenMenu.onClick.AddListener(() => {
            MenuSoundboard.PlayButton();
        });

        CloseMenu.onClick.AddListener(() => {
            MenuSoundboard.PlayCancel();
        });

        //Typically this would load units from file, but for now we generate units. This is going to be used in new game!
        for(int i = UnityEngine.Random.Range(10, 30); i >= 0; i--) UnitList.Add(UnitGenerator.generate());

        //Generating squads
        for(int i = UnityEngine.Random.Range(4, 10); i > 0; i--) SquadList.Add(UnitGenerator.GenerateSquad());
    }

    void Update()
    {
        Gold.text = UnitResourceManager.Gold.ToString();
        Iron.text = UnitResourceManager.Iron.ToString();
        MagicGems.text = UnitResourceManager.MagicGems.ToString();
        Horses.text = UnitResourceManager.Horses.ToString();

        if(Input.GetKeyUp(KeyCode.P))
        {
            UnitResourceManager.Gold += 1000;
            UnitResourceManager.Iron += 10;
            UnitResourceManager.MagicGems += 10;
            UnitResourceManager.Horses += 10;
        }

        if(Input.GetKeyUp(KeyCode.O))
        {
            UnitResourceManager.Gold -= 1000;
            UnitResourceManager.Iron -= 10;
            UnitResourceManager.MagicGems -= 10;
            UnitResourceManager.Horses -= 10;
        }
    }

    void DisableAllInterfaces()
    {
        ParentMenu.SetActive(false);
        ManageArmyInterface.SetActive(false);
        MarketplaceInterface.SetActive(false);
        ArenaInterface.SetActive(false);
        CompanionInterface.SetActive(false);
        SaveInterface.SetActive(false);
    }

    void UnitList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        SerializedUnitList = UnitList.ToList<Unit>();
    }

    void SquadList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        SerializedSquadList = SquadList.ToList<Squad>();
    }

    public void GenerateUnit()
    {
        UnitList.Add(UnitGenerator.generate());
    }

    public void GenerateSquadInList()
    {
        SquadList.Add(GenerateSquad());
    }

    public Squad GenerateSquad()
    {
        Squad generic = new Squad();
        for(int j = UnityEngine.Random.Range(1, 10); j > 0; j--) 
        {
            Unit u = UnitGenerator.generate();
            
            Pair<int, int> pair;

            do{
                pair = new Pair<int, int>(UnityEngine.Random.Range(0, 3), UnityEngine.Random.Range(0, 3));

                bool AlreadyFielded = false;
                foreach(var ugp in generic.RetrieveUnitPairs().ToList())
                {
                    if(ugp.Second.Equals(pair)) AlreadyFielded = true;
                }

                if(AlreadyFielded) continue;
                
                break;
            }while(true);

            if(generic.RetrieveUnits().Count == 0) generic.Name = u.Name + "'s Squad";
            generic.FieldUnit(u, pair);
        }
        
        return generic;
    }

    public List<Unit> RetrieveSerializedUnits()
    {
        return SerializedUnitList;
    }

    public List<Squad> RetrieveSerializedSquad()
    {
        return SerializedSquadList;
    }

    System.Collections.IEnumerator WaitForStartGame()
    {
        if(!uiSound.isPlaying) MenuSoundboard.PlayButton();
        while(uiSound.isPlaying) yield return null;

        PlayerController.LoadSquadList(this);
        SceneManager.LoadScene("Main Game");
    }

    System.Collections.IEnumerator WaitForMainMenu()
    {
        if(!uiSound.isPlaying) MenuSoundboard.PlayButton();
        while(uiSound.isPlaying) yield return null;

        SceneManager.LoadScene("Main Menu");
    }

    int musicIdx = 0;
    System.Collections.IEnumerator MusicReel()
    {
        while(true)
        {
            if(!musicSound.isPlaying)
            {
                musicSound.clip = musicList[musicIdx++];
                musicIdx = musicIdx % musicList.Count;
                musicSound.Play();
            }

            yield return null;
        }
    }

    public static class MenuSoundboard
    {
        [Header("Audio Interfacing")]
        static AudioSource uiSound;
        static AudioClip buttonClick;
        static AudioClip checkBoxClick;
        static AudioClip sliderSound;
        static AudioClip cancelClick;

        public static void SetupSoundboard(AudioSource uiSound, AudioClip button, AudioClip checkbox, AudioClip slider, AudioClip cancel)
        {
            MenuSoundboard.uiSound = uiSound;
            buttonClick = button;
            checkBoxClick = checkbox;
            sliderSound = slider;
            cancelClick = cancel;
        }

        public static void PlayButton()
        {
            uiSound.clip = buttonClick;
            uiSound.Play();
        }

        public static void PlayCheckbox()
        {
            uiSound.clip = checkBoxClick;
            uiSound.Play();
        }

        public static void PlayCancel()
        {
            uiSound.clip = cancelClick;
            uiSound.Play();
        }

        public static void PlaySlider()
        {
            uiSound.clip = sliderSound;
            uiSound.Play();
        }
    }
}
