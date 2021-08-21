using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.UI.Image;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    private BookManager book;
    public int page;
    public AudioClip backgroundMusic;
    public List<AudioClip> effectMusicList = new List<AudioClip>();
    AudioSource background;
    List<AudioSource> effectList = new List<AudioSource>();

    private CanvasScaler scaler;
    private RectTransform rectTransform;

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
            if (!background.isPlaying)
            {
                Debug.Log("Playing background for page " + page);
                background.Play();
            }
        } else
        {
            if (background.isPlaying)
            {
                Debug.Log("Stopping background for page " + page);
                background.Stop();
            }
        }
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
        if (book.isZoomedIn)
        {
            instantZoomIn();
        } else
        {
            instantZoomOut();
        }
    }
    void Awake()
    {
        Debug.Log("PAGE " + page + " start");
        var canvasTransform = this.transform.parent;
        book = canvasTransform.GetComponent<BookManager>();
        if (backgroundMusic)
        {
            background = gameObject.AddComponent<AudioSource>();
            background.clip = backgroundMusic;
            background.loop = true;
            background.ignoreListenerVolume = true;

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

    }

}
