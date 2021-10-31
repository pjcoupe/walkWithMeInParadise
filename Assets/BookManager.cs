using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using System.Collections;
using System.Threading.Tasks;

public class BookManager : MonoBehaviour, IStoreListener
{
    public float currentPanelOnScreenLerp = 0f;
    public float targetPanelOnScreenLerp = 0f;
    public float panelMoveSpeed = 3f;
    public int advertisingPageIndex = 5;

    private string _androidGameId = "4295140";
    private string _iOsGameId = "4295141";
    private bool _testMode = true;

    public AudioSource storyAudioSource;
    private AudioSource click;

    public void playClick()
    {
        click.Play();
    }
    
    [SerializeField] bool _enablePerPlacementMode = true;
    private string _gameId;




    private bool allowedToMoveToNextPage(int proposedPage)
    {
        return true;   
    }

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

    public void setLanguage(int index, AudioClip welcomeClip, string languageName)
    {
        click.Play();
        if (this.languageIndex != index)
        {
            this.languageName = languageName;
            Debug.Log("Set Language from " + this.languageIndex + " to " + index + " (" + languageName + ")");
            this.languageIndex = index;
            if (welcomeClip)
            {
                audioSource.PlayOneShot(welcomeClip);
            }
            page.checkToChangeStoryVoiceClipAndLanguageText();
        } else
        {
            Debug.Log("Already on languageIndex " + index + " " + languageName);
        }
    }
    private AudioSource audioSource;

    private int totalPages = 10 + 1;
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
    private Transform home;
    private Transform lyrics;
    private Transform playNextPage;
    private Transform iap;
    private Transform playPrevPage;
    private bool enableJoystickAndZoomGlobally = false;
    

    private bool _isZoomedIn = false;
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
           joyStick.gameObject.SetActive(_isZoomedIn && enableJoystickAndZoomGlobally);
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
        click.Play();
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
        click.Play();
        _isVoicePlaying = !_isVoicePlaying;
        voiceOff.gameObject.SetActive(_isVoicePlaying);
        voiceOn.gameObject.SetActive(!_isVoicePlaying);
    }

    public void toggleMusic()
    {
        click.Play();
        _isMusicOn = !_isMusicOn;
        musicOff.gameObject.SetActive(_isMusicOn);
        musicOn.gameObject.SetActive(!_isMusicOn);
    }
    public void toggleZoom()
    {
        click.Play();
        _isZoomedIn = !_isZoomedIn;
        joyStick.gameObject.SetActive(_isZoomedIn && enableJoystickAndZoomGlobally);
        zoomOut.gameObject.SetActive(_isZoomedIn && enableJoystickAndZoomGlobally);
        zoomIn.gameObject.SetActive(!_isZoomedIn && enableJoystickAndZoomGlobally);
    }


    public bool showZoomControl
    {
        set
        {
            joyStick.gameObject.SetActive(value && enableJoystickAndZoomGlobally);
            zoomOut.gameObject.SetActive(value && isZoomedIn && enableJoystickAndZoomGlobally);
            zoomIn.gameObject.SetActive(value && !isZoomedIn && enableJoystickAndZoomGlobally);
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

    public string languageName
    {
        get
        {
            string savedLanguage = PlayerPrefs.GetString("languageName");
            if (savedLanguage == "" || savedLanguage == null)
            {
                savedLanguage = "English";
                PlayerPrefs.SetString("languageName", savedLanguage);
            }
            return savedLanguage;
        }
        set
        {
            PlayerPrefs.SetString("languageName", value);
        }
    }
    public int languageIndex
    {
        get
        {
            int savedLanguage = PlayerPrefs.GetInt("languageIndex");
            PlayerPrefs.SetInt("languageIndex", savedLanguage);
            
            return savedLanguage;
        }
        set
        {
            PlayerPrefs.SetInt("languageIndex", value);
        }
    }

    public bool isUnlocked()
    {
        return PlayerPrefs.GetString("Unlocked") == SystemInfo.deviceUniqueIdentifier;
    }

    public void unlockBook()
    {
        myIAPManager.ProcessPurchase(null);        
    }

    public void CheckLockUnlockBook()
    {
        if (isUnlocked())
        {
            iap.gameObject.SetActive(false);
        } else
        {
            // we aren't unlocked disable lock if <= page 3
            if (page.page < 3)
            {
                iap.gameObject.SetActive(false);
            } else
            {
                iap.gameObject.SetActive(true);
            }
        }
    }


    private MyIAPManager myIAPManager;
    // Start is called before the first frame update
    void Awake()
    {
        storyAudioSource = gameObject.AddComponent<AudioSource>();
        click = Camera.main.GetComponent<AudioSource>();
        myIAPManager = new MyIAPManager();
        audioSource = gameObject.AddComponent<AudioSource>();
        var canvasTransform = this.transform;
        playPrevPage = canvasTransform.Find("Prev");
        playNextPage = canvasTransform.Find("Next");
        iap = canvasTransform.Find("iap");
        home = canvasTransform.Find("Home");
        lyrics = canvasTransform.Find("Lyrics");
        joyStick = canvasTransform.Find("Joystick");
        zoomOut = canvasTransform.Find("ZoomOut");
        zoomIn = canvasTransform.Find("ZoomIn");
        musicOff = canvasTransform.Find("MusicOff");
        musicOn = canvasTransform.Find("MusicOn");
        voiceOff = canvasTransform.Find("VoiceOff");
        voiceOn = canvasTransform.Find("VoiceOn");
    }


    public void registerPage(PageManager childPage)
    {
        pageList.Add(childPage);
        //Debug.Log("Registered page " + childPage.page);
        if (pageList.Count == totalPages)
        {
            int currentPage = PlayerPrefs.GetInt("CurrentPage");
            currentPage = 0;
            gotoPage(currentPage);
        }
    }

    public void goHome()
    {
        click.Play();
        gotoPage(0);
    }

    public void movePrevious()
    {
        click.Play();
        gotoPage(page.page - 1);
    }
    public void moveNext()
    {
        click.Play();
        gotoPage(page.page + 1);
    }

    public async Task FadeAndPlayNext(AudioSource audioSource, AudioClip nextClipToPlayAfterFadeCompleted, float duration = 0.5f, float targetVolume = 0f)
    {
        if (audioSource.isPlaying)
        {
            float currentTime = 0;
            float start = audioSource.volume;
            Debug.Log("Fading clip " + audioSource.clip.name);
            while (currentTime < duration && audioSource.isPlaying)
            {
                currentTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
                await Task.Delay(1);
                //yield return null;
            }
            audioSource.Stop();
            audioSource.volume = start;
            audioSource.clip = null;
        }
        if (nextClipToPlayAfterFadeCompleted)
        {
            audioSource.volume = 1;
            audioSource.clip = nextClipToPlayAfterFadeCompleted;
            Debug.Log("Playing next clip " + nextClipToPlayAfterFadeCompleted.name);
            audioSource.Play();
        }
        //yield break;
    }

    private void gotoPage(int pageNum)
    {
        int previousPageNum = pageNumber;
        if (pageNum == previousPageNum)
        {
            return;
        }
        if (!allowedToMoveToNextPage(pageNum))
        {
            Debug.Log("Cannot goto page " + pageNum);
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
        CheckLockUnlockBook();
        showZoomControl = !isCover;
        showMusicControl = page.backgroundMusic;
        showHomeControl = !isCover;
        showVoiceControl = page.getStoryClip();
        showPlayPrevPageControl = !isCover;
        showPlayNextPageControl = true;
        page.checkToChangeStoryVoiceClipAndLanguageText();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        ((IStoreListener)myIAPManager).OnInitializeFailed(error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        return ((IStoreListener)myIAPManager).ProcessPurchase(purchaseEvent);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        ((IStoreListener)myIAPManager).OnPurchaseFailed(product, failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        ((IStoreListener)myIAPManager).OnInitialized(controller, extensions);
    }
}
