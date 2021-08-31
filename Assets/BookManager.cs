using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BookManager : MonoBehaviour
{
    public float currentPanelOnScreenLerp = 0f;
    public float targetPanelOnScreenLerp = 0f;
    public float panelMoveSpeed = 3f;

    private PageManager _page;
    public PageManager page
    {
        get
        {
            return _page;
        }
        private set
        {
            if (_page == value)
            {
                return;
            }
            _page = value;
            currentPanelOnScreenLerp = targetPanelOnScreenLerp;
            Debug.Log("Setting page to " + _page.page);
            foreach (var p in pageList)
            {
                p.StopPage();
                p.transform.parent.gameObject.SetActive(false);                
            }
            
            if (_page)
            {
                _page.transform.parent.gameObject.SetActive(true);
                _page.PlayPage();
            }
        }
    }
    public bool isCover
    {
        get
        {
            return page == cover;
        }
    }
    public int languageIndex
    {
        get
        {
            return _languageIndex;
        }
        
    }
    public void setLanguage(int index, AudioClip welcomeClip)
    {
        if (this.languageIndex != index)
        {
            
            this._languageIndex = index;
            if (welcomeClip)
            {
                audioSource.PlayOneShot(welcomeClip);
            }
            page.checkToChangeStoryVoiceClipAndLanguageText(1f);
        }
    }
    private AudioSource audioSource;

    private int totalPages = 10 + 1;
    private int _languageIndex = 0;
    private int pageNumber = -1;
    private List<PageManager> pageList = new List<PageManager>();
    private PageManager cover;
    private Transform joyStick;
    private Transform zoomOut;
    private Transform zoomIn;
    private Transform musicOff;
    private Transform musicOn;
    private Transform voiceOff;
    private Transform voiceOn;
    private Transform effectsOff;
    private Transform effectsOn;
    private Transform home;
    private Transform lyrics;
    private Transform playNextPage;
    private Transform playPrevPage;
    

    private bool _isZoomedIn = true;
    public bool isZoomedIn
    {
        get
        {
            return _isZoomedIn;
        }
        set
        {
           if (_isZoomedIn != value)
           {
                toggleZoom();
           }
           joyStick.gameObject.SetActive(_isZoomedIn);
        }
    }
    private bool _isMusicOn = true;
    public bool isMusicOn
    {
        get
        {
            return _isMusicOn;
        }
        set
        {
            if (_isMusicOn != value)
            {
                toggleMusic();
            }
        }
    }
    private bool _isVoicePlaying = true;
    public bool isVoiceOn
    {
        get
        {
            return _isVoicePlaying;
        }
        set
        {
            if (_isVoicePlaying != value)
            {
                toggleVoice();
            }
        }
    }
    private bool _isEffectsOn = true;
    public bool isEffectsOn
    {
        get
        {
            return _isEffectsOn;
        }
        set
        {
            if (_isEffectsOn != value)
            {
                toggleEffects();
            }
        }
    }

    private bool _isLyrics = true;
    public bool isLyrics
    {
        get
        {
            return _isLyrics;
        }
        set
        {
            if (_isLyrics != value)
            {
                toggleLyrics();
            }
        }
    }

    
    public void toggleLyrics()
    {

        _isLyrics = !_isLyrics;
        
        Image image = lyrics.GetComponent<Image>();
        if (image)
        {
            image.color = _isLyrics ? Color.white : Color.yellow;
        }
        targetPanelOnScreenLerp = Mathf.Clamp01(1f - targetPanelOnScreenLerp);
    }

    public void toggleVoice()
    {
        _isVoicePlaying = !_isVoicePlaying;
        voiceOff.gameObject.SetActive(_isVoicePlaying);
        voiceOn.gameObject.SetActive(!_isVoicePlaying);
    }

    public void toggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        musicOff.gameObject.SetActive(_isMusicOn);
        musicOn.gameObject.SetActive(!_isMusicOn);
    }
    public void toggleZoom()
    {
        _isZoomedIn = !_isZoomedIn;
        joyStick.gameObject.SetActive(_isZoomedIn);
        zoomOut.gameObject.SetActive(_isZoomedIn);
        zoomIn.gameObject.SetActive(!_isZoomedIn);
    }
    public void toggleEffects()
    {
        _isEffectsOn = !_isEffectsOn;
        effectsOff.gameObject.SetActive(_isEffectsOn);
        effectsOn.gameObject.SetActive(!_isEffectsOn);
    }

    public bool showZoomControl
    {
        set
        {
            joyStick.gameObject.SetActive(value);
            zoomOut.gameObject.SetActive(value && isZoomedIn);
            zoomIn.gameObject.SetActive(value && !isZoomedIn);
        }
    }

    public bool showMusicControl
    {
        set
        {
            musicOff.gameObject.SetActive(value && isMusicOn);
            musicOn.gameObject.SetActive(value && !isMusicOn);
        }
    }

    public bool showEffectsControl
    {
        set
        {
            effectsOff.gameObject.SetActive(value && isEffectsOn);
            effectsOn.gameObject.SetActive(value && !isEffectsOn);
        }
    }

    public bool showVoiceControl
    {
        set
        {
            voiceOff.gameObject.SetActive(value && isVoiceOn);
            voiceOn.gameObject.SetActive(value && !isVoiceOn);
        }
    }

    public bool showLyricControl
    {
        set
        {
            lyrics.gameObject.SetActive(value);
        }
    }

    public bool showHomeControl
    {
        set
        {
            home.gameObject.SetActive(value);
        }
    }

    public bool showPlayNextPageControl
    {
        set
        {
            playNextPage.gameObject.SetActive(value);
        }
    }

    public bool showPlayPrevPageControl
    {
        set
        {
            playPrevPage.gameObject.SetActive(value);
        }
    }



    // Start is called before the first frame update
    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        
        
        var canvasTransform = this.transform;
        playPrevPage = canvasTransform.Find("Prev");
        playNextPage = canvasTransform.Find("Next");
        home = canvasTransform.Find("Home");
        lyrics = canvasTransform.Find("Lyrics");
        joyStick = canvasTransform.Find("Joystick");
        zoomOut = canvasTransform.Find("ZoomOut");
        zoomIn = canvasTransform.Find("ZoomIn");

        musicOff = canvasTransform.Find("MusicOff");
        musicOn = canvasTransform.Find("MusicOn");

        voiceOff = canvasTransform.Find("VoiceOff");
        voiceOn = canvasTransform.Find("VoiceOn");

        effectsOff = canvasTransform.Find("EffectsOff");
        effectsOn = canvasTransform.Find("EffectsOn");

    }

    public void registerPage(PageManager childPage)
    {
        pageList.Add(childPage);
        Debug.Log("Registered page " + childPage.page);
        if (pageList.Count == totalPages)
        {
            goHome();
        }
    }

    public void goHome()
    {
        gotoPage(0);
    }

    public void movePrevious()
    {
        gotoPage(page.page - 1);
    }
    public void moveNext()
    {
        gotoPage(page.page + 1);
    }

    private void gotoPage(int pageNum)
    {
        if (pageNum == pageNumber)
        {
            return;
        }

        bool foundPage = false;
        foreach(var p in pageList)
        {
            if (p.page == pageNum)
            {
                page = p;
                foundPage = true;
            }
            if (p.page == 0)
            {
                cover = p;
            }
            
        }
        if (!foundPage)
        {
            page = cover;
        }
        pageNumber = page.page;
        showZoomControl = !isCover;
        showMusicControl = page.backgroundMusic;
        showEffectsControl = page.effectMusicList.Count > 0;
        showHomeControl = !isCover;
        showVoiceControl = page.getStoryClip();
        showPlayPrevPageControl = !isCover;
        showPlayNextPageControl = true;
        page.checkToChangeStoryVoiceClipAndLanguageText();
    }

    
}
