using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Book_home_item : MonoBehaviour {

    public Text txt_name;
    public Image icon;
    public string id_book;
    public int chapter = 0;
    public int type = 0;
    public int act = 0;

    public void click()
    {
        if (act == 0)
        {
            GameObject.Find("Bible").GetComponent<Bible>().show_list_chapter(this.chapter, this.type, this.txt_name.text, this.id_book);
        }

        if (act == 1)
        {
            Application.OpenURL(id_book);
        }
    }
}
