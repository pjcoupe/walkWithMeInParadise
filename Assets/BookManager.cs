using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookManager : MonoBehaviour
{
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
            foreach (var p in pageList)
            {
                p.StopPage();
                p.gameObject.SetActive(false);                
            }
            _page = value;
            if (_page)
            {
                _page.gameObject.SetActive(true);
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
    private int pageNumber = -1;
    private PageManager[] pageList;
    private PageManager cover;
    private Transform joyStick;
    private Transform zoomOut;
    private Transform zoomIn;
    private Transform musicOff;
    private Transform musicOn;
    private Transform effectsOff;
    private Transform effectsOn;
    private Transform home;
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
        var canvasTransform = this.transform;
        playPrevPage = canvasTransform.Find("Prev");
        playNextPage = canvasTransform.Find("Next");
        home = canvasTransform.Find("Home");
        joyStick = canvasTransform.Find("Joystick");
        zoomOut = canvasTransform.Find("ZoomOut");
        zoomIn = canvasTransform.Find("ZoomIn");

        musicOff = canvasTransform.Find("MusicOff");
        musicOn = canvasTransform.Find("MusicOn");

        effectsOff = canvasTransform.Find("EffectsOff");
        effectsOn = canvasTransform.Find("EffectsOn");

        pageList = this.gameObject.GetComponentsInChildren<PageManager>();
        goHome();
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
        showMusicControl = true;
        showEffectsControl = true;
        showHomeControl = !isCover;
        showPlayPrevPageControl = !isCover;
        showPlayNextPageControl = true;
    }

    
}
