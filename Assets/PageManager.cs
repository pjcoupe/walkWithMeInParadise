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
    public List<AudioClip> story = new List<AudioClip>();
    public int page;
    public bool allowZoomIn = true;
    public bool allowLyrics = true;
    public bool allowVoice = true;
    public AudioClip backgroundMusic;
    public List<AudioClip> effectMusicList = new List<AudioClip>();



    private BookManager book;
    private AudioSource background;
    private AudioSource storyAudioSource;
    private List<AudioSource> effectList = new List<AudioSource>();
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
    private float currentScale = maxScale;

    public Vector3 zoomedInInitialTarget = new Vector3(600, 318, 0);
    private Vector3 currentLocalPosition;
    private Vector3 endLocalPosition = new Vector3(0, 0, 0);
    private Vector3 startLocalPosition = new Vector3(0, 0, 0);
    private float delta = 0;
    private TextMeshProUGUI textMeshPro;
    

    public void toggleVoice()
    {
        book.toggleVoice();
        if (story.Count <= book.languageIndex)
        {
            Debug.Log("No story defined for page " + page);
            return;
        }
        checkToChangeStoryVoiceClipAndLanguageText();
        
    }

    public void checkToChangeStoryVoiceClipAndLanguageText(float delay = 0f)
    {
        AudioClip clip = getStoryClip();
        if (clip)
        {
            if (storyAudioSource.clip && storyAudioSource.clip != clip)
            {
                if (storyAudioSource.isPlaying)
                {
                    Debug.Log("Stoping story for page " + page);
                    storyAudioSource.Stop();
                }
            }
            storyAudioSource.clip = clip;
            storyAudioSource.loop = false;
            if (book.isVoiceOn)
            {
                if (!storyAudioSource.isPlaying)
                {
                    Debug.Log("Playing story for page " + page);
                    storyAudioSource.PlayDelayed(delay);
                }
            }
            else
            {
                if (storyAudioSource.isPlaying)
                {
                    Debug.Log("Stoping story for page " + page);
                    storyAudioSource.Stop();
                }
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


    public void toggleEffects()
    {
        book.toggleEffects();
        foreach (AudioSource effect in effectList)
        {
            if (book.isEffectsOn)
            {
                if (!effect.isPlaying)
                {
                    Debug.Log("Playing effect for page " + page);
                    effect.Play();
                }
            }
            else
            {
                if (effect.isPlaying)
                {
                    Debug.Log("Stopping effects for page " + page);
                    effect.Stop();
                }
            }
        }
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

    public void StopPage()
    {
        if (background && background.isPlaying)
        {
            Debug.Log("Stopping background for page " + page);
            background.Stop();
        }
        foreach (AudioSource effect in effectList)
        {
            if (effect.isPlaying)
            {
                Debug.Log("Stopping effect for page " + page);
                effect.Stop();
            }
        }
        if (storyAudioSource && storyAudioSource.isPlaying)
        {
            Debug.Log("Stopping voice story for page " + page);
            storyAudioSource.Stop();
        }
        
    }

    public AudioClip getStoryClip()
    {
        if (book.languageIndex >=0 && book.languageIndex < story.Count)
        {
            return story[book.languageIndex];
        }
        return null;
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
        foreach (AudioSource effect in effectList)
        {
            if (book.isEffectsOn)
            {
                Debug.Log("Playing effect for page " + page);
                effect.Play();
            } else
            {
                Debug.Log("Stopping effect for page " + page);
                effect.Stop();
            }
        }
        if (getStoryClip() && storyAudioSource)
        {
            if (book.isVoiceOn)
            {
                Debug.Log("Playing story for page " + page);
                storyAudioSource.Play();
            } else
            {
                Debug.Log("Stopping story for page " + page);
                storyAudioSource.Stop();
            }
        }
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

        Debug.Log("PAGE " + page + " start");
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

        storyAudioSource = gameObject.AddComponent<AudioSource>();
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
        foreach (AudioClip effect in effectMusicList)
        {
            AudioSource toPush = gameObject.AddComponent<AudioSource>();
            toPush.clip = effect;
            toPush.loop = true;
            toPush.ignoreListenerVolume = true;
            effectList.Add(toPush);
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
        float lerp = Lerp(heightPreferred, widthPreferred, screenAspectRatio);
        this.scaler.matchWidthOrHeight = 1f - lerp;
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
