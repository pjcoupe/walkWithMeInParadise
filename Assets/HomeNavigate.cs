using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeNavigate : MonoBehaviour
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
            bookManager.goHome();
        });
    }
}
