using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_view : MonoBehaviour {

    public Image icon;
    public Text txt_name;
    public int type=0;
    public int chapter=0;
    public int num_chapter = 0;
    public int act = 0;
    public string id_book;
    public string data_text = "";
    public int id_p=-1;
    public string id_chapter;

    public void click()
    {
        GameObject.Find("Bible").GetComponent<Bible>().carrot.close();
        if (act == 0)
        {
            GameObject.Find("Bible").GetComponent<Bible>().show_list_chapter(this.chapter, this.type,this.txt_name.text,this.id_book);
        }

        if (act == 1)
        {
            GameObject.Find("Bible").GetComponent<Bible>().show_list_p(-1,this.type, this.id_book,this.data_text, this.chapter,this.num_chapter);
        }

        if (act == 2)
        {
            if (this.txt_name.color == Color.white)
            {
                this.txt_name.color = Color.yellow;
                GameObject.Find("Bible").GetComponent<Bible>().panel_view.show_button_view_p(this);
            }
            else
            {
                this.txt_name.color = Color.white;
                GameObject.Find("Bible").GetComponent<Bible>().panel_view.close_button_view_p();
            }
        }

        if (act == 3)
        {
            GameObject.Find("Bible").GetComponent<Bible>().show_list_p(this.id_p, this.type, this.id_book, this.data_text, this.chapter, this.num_chapter);
        }

        if (act == 4)
        {
            Application.OpenURL(this.data_text);
        }
    }
}
