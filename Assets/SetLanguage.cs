using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLanguage : MonoBehaviour
{
    public int languageIndex = 0;

    private Button button;
    private BookManager bookManager;
    public AudioClip welcomeToMyLanguage;

    // Start is called before the first frame update
    void Start()
    {
        bookManager = transform.parent.gameObject.GetComponentInChildren<BookManager>();
        button = transform.gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            bookManager.setLanguage(languageIndex, welcomeToMyLanguage, gameObject.name);
            button.Select();
            
        });
        if (bookManager.languageIndex == languageIndex)
        {
            button.Select();
        }
    }

    private void FixedUpdate()
    {
        if (bookManager.languageIndex == languageIndex)
        {
            button.Select();
        }
    }

}
