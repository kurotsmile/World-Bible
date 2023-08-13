using UnityEngine;
using UnityEngine.UI;

public class Manager_Menu : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

    [Header("Obj Menu")]
    public Sprite sp_menu_nomal;
    public Sprite sp_menu_select;
    public Image[] icon_menu;

    private int index_menu = -1;

    public void load()
    {
        this.index_menu = PlayerPrefs.GetInt("index_menu",0);
        this.select_menu(index_menu);
    }

    public void click(int index)
    {
        this.bible.carrot.play_sound_click();
        this.index_menu = index;
        PlayerPrefs.SetInt("index_menu",this.index_menu);
        this.select_menu(index);
    }

    public void select_menu(int index)
    {
        for (int i = 0; i < this.icon_menu.Length; i++) this.icon_menu[i].sprite = sp_menu_nomal;
        this.icon_menu[index].sprite = this.sp_menu_select;

        if (index == 0) this.bible.show_list_book();
        if (index == 1) this.bible.offline.show();
        if (index == 2) this.bible.search.list();
    }
}
