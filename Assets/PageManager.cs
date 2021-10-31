using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using UnityEngine;
using TMPro;

public class PageManager : MonoBehaviour
{
    public List<string> lyricText = new List<string>();
    private AudioClip story;
    public int page;
    public bool allowZoomIn = true;
    public bool allowLyrics = true;
    public bool allowVoice = true;
    public AudioClip backgroundMusic;



    private BookManager book;
    private AudioSource background;
    private AudioSource storyAudioSource
    {
        get
        {
            return book.storyAudioSource;
        }
    }
    private CanvasScaler scaler;
    private RectTransform rectTransform;
    private RectTransform topPanel;
    private RectTransform bottomPanel;
    
    

    const float maxScale = 2f;
    const float minScale = 1.125f;
    const float moveDampener = 10f;
    private Vector3 bottomLeft = new Vector3(960f * (maxScale - 1f), 418f * (maxScale - 1f), 1f); // was 540
    private Vector3 topRight = new Vector3(-960f * (maxScale - 1f), -418f * (maxScale - 1f), 1f);
    private bool zooming = false;
    
    private float zoomStart;
    private float zoomEnd;
    

    // zoom related
    private float currentScale = minScale;

    public Vector3 zoomedInInitialTarget = new Vector3(600, 318, 0);
    private Vector3 currentLocalPosition;
    private Vector3 endLocalPosition = new Vector3(0, 0, 0);
    private Vector3 startLocalPosition = new Vector3(0, 0, 0);
    private float delta = 0;
    private TextMeshProUGUI textMeshPro;
    

    public void toggleVoice()
    {
        book.toggleVoice();
        checkToChangeStoryVoiceClipAndLanguageText();
    }



    public async void checkToChangeStoryVoiceClipAndLanguageText()
    {
        AudioClip clip = getStoryClip();
        if (!book.isVoiceOn)
        {
            clip = null;
        }
        if (clip != storyAudioSource.clip)
        {
            Debug.Log("check1 " + (clip ? clip.name : "null"));
            await book.FadeAndPlayNext(storyAudioSource, clip); //StartCoroutine(book.FadeAndPlayNext(storyAudioSource, clip));
        } else
        {
            if (!storyAudioSource.isPlaying && book.isVoiceOn)
            {
                Debug.Log("check2 " + (clip ? clip.name : "null"));
                await book.FadeAndPlayNext(storyAudioSource, clip); //StartCoroutine(book.FadeAndPlayNext(storyAudioSource, clip));
            } else
            {
                Debug.Log("checkToChangeStoryVoice is same as audio " + (clip ? "has clip" : "null"));
            }
            
        }
        
        if (textMeshPro && textMeshPro.text != getLyricText())
        {
            textMeshPro.text = getLyricText();
        }
    }

    public string getLyricText()
    {
        if (book.languageIndex >= 0 && book.languageIndex < lyricText.Count)
        {
            return lyricText[book.languageIndex];
        }
        return "";
    }




    public void toggleMusic()
    {
        book.toggleMusic();
        if (book.isMusicOn)
        {
            if (background && !background.isPlaying)
            {
                Debug.Log("Playing background for page " + page);
                background.Play();
            }
        } else
        {
            if (background && background.isPlaying)
            {
                Debug.Log("Stopping background for page " + page);
                background.Stop();
            }
        }
    }

    public void toggleLyrics()
    {
        book.toggleLyrics();
        
    }

    public void toggleZoom()
    {
        if (!zooming)
        {
            zooming = true;
            delta = 0;

            if (book.isZoomedIn)
            {
                zoomStart = maxScale;
                zoomEnd = minScale;
                currentScale = maxScale - 0.01f;
                startLocalPosition = currentLocalPosition;
                endLocalPosition = new Vector3(0, 0, 0);
            }
            else
            {
                zoomStart = minScale;
                zoomEnd = maxScale;
                currentScale = minScale + 0.01f;
                startLocalPosition = currentLocalPosition;
                endLocalPosition = zoomedInInitialTarget;
            }
            book.toggleZoom();
        }
    }

    private void instantZoomOut()
    {
        currentScale = minScale;
        currentLocalPosition = new Vector3(0,0,0);
    }

    private void instantZoomIn()
    {
        currentScale = maxScale;
        currentLocalPosition = zoomedInInitialTarget;
    }

    private void doZoom()
    {
        delta += 3f * Time.fixedDeltaTime;
        if (delta > 1f)
        {
            delta = 1f;
        }
        currentScale = TimeLerp(zoomStart, zoomEnd, delta);
        if (currentScale >= maxScale)
        {
            zooming = false;
            currentScale = maxScale;
        }
        else if (currentScale <= minScale)
        {
            zooming = false;
            currentScale = minScale;
        }
        currentLocalPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, delta);
    }

    private float TimeLerp(float start, float end, float time0To1)
    {
        float range = end - start;
        if (time0To1 < 0f)
        {
            time0To1 = 0f;
        }
        else if (time0To1 > 1f)
        {
            time0To1 = 1f;
        }
        return start + (time0To1 * range);

    }

    private float Lerp(float start, float end, float current)
    {
        var max = Mathf.Max(start, end);
        var min = Mathf.Min(start, end);
        if (current > max)
        {
            current = max;
        }
        else if (current < min)
        {
            current = min;
        }
        return (current - min) / (max - min);
    }

    public async void StopPage()
    {
        if (background && background.isPlaying)
        {
            //Debug.Log("Stopping background for page " + page);
            background.Stop();
        }

        if (storyAudioSource && storyAudioSource.isPlaying && storyAudioSource.clip == story)
        {
            Debug.Log("check3 " + (storyAudioSource.clip ? storyAudioSource.clip.name : "null"));
            await book.FadeAndPlayNext(storyAudioSource, null); // StartCoroutine(book.FadeAndPlayNext(storyAudioSource, null));
        }
        
    }

    public void unloadStoryClip()
    {
        if (story)
        {
            Resources.UnloadAsset(story);
            story = null;
        }
    }
    public AudioClip getStoryClip()
    {
        string path = "voice/" + book.languageName + "/Page" + page;
        story = Resources.Load(path) as AudioClip;
        if (!story)
        {
            Debug.Log("Failed to get story " + path);
        }
        return story;
    }
    public void PlayPage()
    {
        if (background)
        {
            if (book.isMusicOn)
            {
                Debug.Log("Playing background for page " + page);
                background.Play();
            } else
            {
                Debug.Log("Stopping background for page " + page);
                background.Stop();
            }
        }
        checkToChangeStoryVoiceClipAndLanguageText();

        book.showVoiceControl = allowVoice;
        book.showLyricControl = allowLyrics;
        book.showZoomControl = allowZoomIn;
        if (book.isZoomedIn && !allowZoomIn)
        {
            instantZoomOut();
        } else if (book.isZoomedIn)
        {
            instantZoomIn();
        } else
        {
            instantZoomOut();
        }
    }

    private RectTransform panelToUse;
    private Vector2 onScreenPanelPos;
    private Vector2 offScreenPanelPos;
    
    void Awake()
    {
        // note I got flags from https://www.countryflags.com/united-states-flag-icon/

        //Debug.Log("PAGE " + page + " start");
        Transform parent = transform.parent;
        textMeshPro = parent.GetComponentInChildren<TextMeshProUGUI>();
        Transform top = parent.Find("TopPanel");
        if (top)
        {
            topPanel = top.GetComponent<RectTransform>();
            onScreenPanelPos = new Vector2(topPanel.anchoredPosition.x, topPanel.anchoredPosition.y);
            offScreenPanelPos = onScreenPanelPos + new Vector2(0, 500f);
            panelToUse = topPanel;
        }
        else
        {
            Transform bottom = parent.Find("BottomPanel");
            if (bottom)
            {
                bottomPanel = bottom.GetComponent<RectTransform>();
                onScreenPanelPos = new Vector2(bottomPanel.anchoredPosition.x, bottomPanel.anchoredPosition.y);
                offScreenPanelPos = onScreenPanelPos + new Vector2(0, -500f);
                panelToUse = bottomPanel;
            }
        }

        
        var canvasTransform = this.transform.parent.parent;
        book = canvasTransform.GetComponent<BookManager>();
        if (backgroundMusic)
        {
            book.showMusicControl = true;
            background = gameObject.AddComponent<AudioSource>();
            background.clip = backgroundMusic;
            background.loop = true;
            background.ignoreListenerVolume = true;
        } else
        {
            book.showMusicControl = false;
        }

        scaler = canvasTransform.GetComponent<CanvasScaler>();
        float width = Screen.width;
        float height = Screen.height;
        float screenAspectRatio = Mathf.Max(width, height) / Mathf.Min(width, height);
        float widthPreferred = 16f / 9f;
        float heightPreferred = 4f / 3f;
        if (screenAspectRatio > widthPreferred)
        {
            screenAspectRatio = widthPreferred;
        }
        else if (screenAspectRatio < heightPreferred)
        {
            screenAspectRatio = heightPreferred;
        }
        
        this.scaler.matchWidthOrHeight = 1f;
        currentLocalPosition = zoomedInInitialTarget;
        this.rectTransform = transform.GetComponent<RectTransform>();
        if (book.isCover)
        {
            currentScale = minScale;
        }
        this.rectTransform.localScale = new Vector3(currentScale, currentScale, 1);
        book.registerPage(this);
    }



    void FixedUpdate()
    {
        //Debug.Log("Screen " + Screen.currentResolution);
        var gamepad = Gamepad.current;

        if (gamepad != null && !zooming)
        {
            Vector2 move = gamepad.leftStick.ReadValue();
            if (move.magnitude > 0)
            {
                Vector3 targetPos = currentLocalPosition + new Vector3(move.x * -moveDampener, move.y * -moveDampener, 0);
                float targetPosX = Mathf.Max(topRight.x, Mathf.Min(bottomLeft.x, targetPos.x));
                float targetPosY = Mathf.Max(topRight.y, Mathf.Min(bottomLeft.y, targetPos.y));
                currentLocalPosition = new Vector3(targetPosX, targetPosY, 1);
            }
        }
        else if (zooming)
        {
            doZoom();
        }
        this.rectTransform.localPosition = currentLocalPosition;
        this.rectTransform.localScale = new Vector3(currentScale, currentScale, 1);
        if (panelToUse)
        {
            if (book.currentPanelOnScreenLerp < book.targetPanelOnScreenLerp)
            {
                book.currentPanelOnScreenLerp = Mathf.Clamp01(book.currentPanelOnScreenLerp + book.panelMoveSpeed * 0.01f);
            }
            else if (book.currentPanelOnScreenLerp > book.targetPanelOnScreenLerp)
            {
                book.currentPanelOnScreenLerp = Mathf.Clamp01(book.currentPanelOnScreenLerp - book.panelMoveSpeed * 0.01f);
            }
            panelToUse.anchoredPosition = Vector2.Lerp(onScreenPanelPos, offScreenPanelPos, book.currentPanelOnScreenLerp);
        }
        
    }

 
}
