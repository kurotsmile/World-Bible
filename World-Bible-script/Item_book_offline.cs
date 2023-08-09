using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_book_offline : MonoBehaviour
{

    public Text txt_name;
    public int index;
    public bool is_home;
    public void click()
    {
        GameObject.Find("Bible").GetComponent<Book_offline>().show_book_offline_contain(this.index);
    }


    public void delete()
    {

        GameObject.Find("Bible").GetComponent<Book_offline>().delete(this.index,this.is_home);

    }
}
