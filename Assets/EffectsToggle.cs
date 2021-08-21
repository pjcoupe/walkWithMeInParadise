using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectsToggle : MonoBehaviour
{
    private Button button;
    private BookManager bookManager;

    // Start is called before the first frame update
    void Awake()
    {
        bookManager = transform.parent.gameObject.GetComponentInChildren<BookManager>();
        button = transform.gameObject.GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            bookManager.page.toggleEffects();
            Debug.Log("Toggle Effects");
        });
    }


}
