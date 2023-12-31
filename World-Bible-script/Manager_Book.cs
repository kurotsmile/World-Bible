using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Book : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

    [Header("Obj Book")]
    public Sprite icon_list;
    private TextMeshProUGUI textPro;

    private IDictionary data_book_cur = null;
    private IDictionary data_chapter_cur = null;
    private Carrot.Carrot_Box box_paragraphs_view = null;
    private bool type_view_page = false;
    private int index_show_chapter = -1;

    public void view(string id)
    {
        this.bible.carrot.play_sound_click();
        this.bible.carrot.ads.show_ads_Interstitial();
        this.bible.carrot.show_loading();

        this.bible.carrot.db.Collection("bible").Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task=>{

            DocumentSnapshot book=task.Result;

            if (task.IsCompleted)
            {
                this.bible.carrot.hide_loading();
                if (book.Exists)
                {
                    IDictionary data = book.ToDictionary();
                    data["id"] = book.Id;
                    data["type_item"] = "online";
                    this.view_book_by_data(data);
                }
            }

            if (task.IsFaulted)
            {
                this.bible.carrot.hide_loading();
                this.bible.carrot.show_msg(PlayerPrefs.GetString("app_title", "Bible world"), PlayerPrefs.GetString("error_unknown","Operation error, please try again next time!"), Carrot.Msg_Icon.Error);
            }
        });
    }

    public void view_book_by_data(IDictionary data)
    {
        this.data_book_cur = data;
        Carrot.Carrot_Box box_list_chapter = this.bible.carrot.Create_Box();
        if (data["type"] != null)
        {
            string s_type_book = data["type"].ToString();
            if (s_type_book == "old_testament")
                box_list_chapter.set_icon_white(this.bible.icon_book_old_testament);
            else
                box_list_chapter.set_icon_white(this.bible.icon_book_new_Testament);
        }
        else
        {
            box_list_chapter.set_icon_white(this.bible.icon_book_new_Testament);
        }

        box_list_chapter.set_title(data["name"].ToString());

        IList contents = (IList)data["contents"];
        for(int i=0;i<contents.Count;i++)
        {
            IDictionary chapter = (IDictionary) contents[i];
            chapter["index"] = i;
            Carrot.Carrot_Box_Item item_chapter = box_list_chapter.create_item();
            item_chapter.set_icon_white(this.bible.icon_chapter);
            if (chapter["name"] != null) item_chapter.set_title(chapter["name"].ToString());
            IList paragraphs = (IList)chapter["paragraphs"];

            if (chapter["paragraphs"] != null)
            {
                item_chapter.set_tip(paragraphs.Count + " paragraph");
                item_chapter.set_act(() => this.view_paragraphs_page(chapter));
            }

            Carrot.Carrot_Box_Btn_Item btn_page = item_chapter.create_item();
            btn_page.set_icon(this.bible.icon_book_open);
            btn_page.set_act(() => this.view_paragraphs_page(chapter));
            btn_page.set_color(this.bible.carrot.color_highlight);


            Carrot.Carrot_Box_Btn_Item btn_list = item_chapter.create_item();
            btn_list.set_icon(this.icon_list);
            btn_list.set_act(() => this.view_paragraphs_list(chapter));
            btn_list.set_color(this.bible.carrot.color_highlight);
        }

        Carrot.Carrot_Box_Btn_Panel panel = box_list_chapter.create_panel_btn();


        if (data["type_item"] != null)
        {
            string type_item = data["type_item"].ToString();

            if (type_item == "online")
            {
                Carrot.Carrot_Button_Item btn_save = panel.create_btn("btn_save");
                btn_save.set_icon_white(this.bible.offline.icon_offline_book);
                btn_save.set_label(PlayerPrefs.GetString("save","Save"));
                btn_save.set_bk_color(this.bible.carrot.color_highlight);
                btn_save.set_act_click(() => bible.offline.add(data));
            }

            if (type_item == "offline")
            {
                Carrot.Carrot_Button_Item btn_del = panel.create_btn("btn_del");
                btn_del.set_icon_white(this.bible.carrot.sp_icon_del_data);
                btn_del.set_label(PlayerPrefs.GetString("del","Delete"));
                btn_del.set_bk_color(this.bible.carrot.color_highlight);
                btn_del.set_act_click(() => bible.offline.delete(int.Parse(data["index"].ToString())));
            }
        }


        Carrot.Carrot_Button_Item btn_share = panel.create_btn("btn_share");
        btn_share.set_icon_white(this.bible.carrot.sp_icon_share);
        btn_share.set_bk_color(this.bible.carrot.color_highlight);
        btn_share.set_label(PlayerPrefs.GetString("share", "Share"));
        btn_share.set_act_click(() => this.share());

        Carrot.Carrot_Button_Item btn_close = panel.create_btn("btn_close");
        btn_close.set_icon_white(this.bible.carrot.icon_carrot_cancel);
        btn_close.set_bk_color(this.bible.carrot.color_highlight);
        btn_close.set_label(PlayerPrefs.GetString("cancel", "Cancel"));
        btn_close.set_act_click(() => box_list_chapter.close());
    }

    private void view_paragraphs_list(IDictionary chapter)
    {
        this.index_show_chapter = int.Parse(chapter["index"].ToString());
        this.type_view_page = false;
        this.data_chapter_cur = chapter;
        this.bible.carrot.play_sound_click();
        Carrot.Carrot_Box box_paragraphs = this.box_view(this.data_book_cur["name"].ToString() + " ("+PlayerPrefs.GetString("chapter", "Chapter") +" "+ (this.index_show_chapter + 1)+")");
        IList paragraphs = (IList)chapter["paragraphs"];
        for (int i = 0; i < paragraphs.Count; i++)
        {
            string s_paragraph = paragraphs[i].ToString();
            Carrot.Carrot_Box_Item item_p=box_paragraphs.create_item("p_" + i);
            item_p.set_title(s_paragraph);
            item_p.set_tip("sentence "+(i+1).ToString());
            item_p.set_icon(this.bible.icon_paragraph);

            Carrot.Carrot_Box_Btn_Item btn_copy=item_p.create_item();
            btn_copy.set_icon(this.bible.icon_copy);
            btn_copy.set_color(this.bible.carrot.color_highlight);
            btn_copy.set_act(() =>this.show_copy(s_paragraph));
        }

        this.nav_page(box_paragraphs);
    }

    private void view_paragraphs_page(IDictionary chapter)
    {
        this.index_show_chapter = int.Parse(chapter["index"].ToString());
        this.type_view_page = true;
        this.data_chapter_cur = chapter;
        this.bible.carrot.play_sound_click();
        Carrot.Carrot_Box box_paragraphs = this.box_view(this.data_book_cur["name"].ToString()+ " (" + PlayerPrefs.GetString("chapter", "Chapter") + " "+(this.index_show_chapter+1)+ ")");
        IList paragraphs = (IList)chapter["paragraphs"];
        string s_page = "";
        for (int i = 0; i < paragraphs.Count; i++)
        {
            s_page= s_page+paragraphs[i].ToString()+" ";
        }

        GameObject obj_txt;
        if(this.bible.carrot.lang.get_key_lang()=="ko") obj_txt=Instantiate(this.bible.prefab_paragraph_item_ko);
        else if (this.bible.carrot.lang.get_key_lang() == "zh") obj_txt = Instantiate(this.bible.prefab_paragraph_item_zh);
        else obj_txt = Instantiate(this.bible.prefab_paragraph_item);

        this.textPro = obj_txt.GetComponent<TextMeshProUGUI>();
        this.textPro.text = s_page;

        obj_txt.transform.SetParent(box_paragraphs.area_all_item);
        obj_txt.transform.localPosition = new Vector3(0f, 0f, 0f);
        obj_txt.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_txt.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        this.bible.carrot.delay_function(1f, this.fix_size_paragraph);
    }

    private void nav_page(Carrot.Carrot_Box box)
    {
        Carrot.Carrot_Box_Btn_Panel panel = box.create_panel_btn();
        Carrot.Carrot_Button_Item btn_prev = panel.create_btn("btn_prev");
        btn_prev.set_icon(this.bible.icon_prev_page);
        btn_prev.set_label(PlayerPrefs.GetString("prev","Previous"));
        btn_prev.set_bk_color(this.bible.carrot.color_highlight);
        btn_prev.set_act_click(() => this.prev_page());

        Carrot.Carrot_Button_Item btn_next = panel.create_btn("btn_next");
        btn_next.set_icon(this.bible.icon_next_page);
        btn_next.set_label(PlayerPrefs.GetString("next","Next"));
        btn_next.set_bk_color(this.bible.carrot.color_highlight);
        btn_next.set_act_click(() => this.nex_page());
    }

    private Carrot.Carrot_Box box_view(string s_title)
    {
        if (this.box_paragraphs_view != null) this.box_paragraphs_view.close();
        this.box_paragraphs_view = this.bible.carrot.Create_Box();
        this.box_paragraphs_view.set_icon_white(this.bible.icon_chapter);
        this.box_paragraphs_view.set_title(s_title);

        Carrot.Carrot_Box_Btn_Item btn_page= this.box_paragraphs_view.create_btn_menu_header(this.bible.icon_book_open);
        btn_page.set_act(() => this.view_paragraphs_page_chapter_cur());
        if (this.type_view_page == true) btn_page.set_icon_color(this.bible.carrot.color_highlight);
        Carrot.Carrot_Box_Btn_Item btn_list = this.box_paragraphs_view.create_btn_menu_header(this.icon_list);
        btn_list.set_act(() => this.view_paragraphs_list_chapter_cur());
        if (this.type_view_page == false) btn_list.set_icon_color(this.bible.carrot.color_highlight);

        return this.box_paragraphs_view;
    }

    private void view_paragraphs_page_chapter_cur()
    {
        this.view_paragraphs_page(this.data_chapter_cur);
    }

    private void view_paragraphs_list_chapter_cur()
    {
        this.view_paragraphs_list(this.data_chapter_cur);
    }

    private void fix_size_paragraph()
    {
        this.textPro.overflowMode = TextOverflowModes.Masking;
        this.nav_page(this.box_paragraphs_view);
    }

    public void show_list_book_by_type(string s_type)
    {
        this.bible.carrot.play_sound_click();
        foreach(Transform tr in this.bible.tr_all_item_book)
        {
            if (tr.gameObject.name== "old_testament"|| tr.gameObject.name == "new_testament")
            {
                if (tr.gameObject.name == s_type)
                    tr.gameObject.SetActive(true);
                else
                    tr.gameObject.SetActive(false);
            }
        }
    }

    public Carrot.Carrot_Box_Item item_book(IDictionary data)
    {
        Carrot.Carrot_Box_Item item_book = this.bible.create_item();
        if (data["name"] != null)
        {
            string s_type = data["type"].ToString();
            item_book.set_title(data["name"].ToString());
            item_book.set_tip(PlayerPrefs.GetString(s_type, s_type));
        }

        if (data["type"] != null)
        {
            string s_type_book = data["type"].ToString();
            if (s_type_book == "old_testament")
                item_book.set_icon_white(this.bible.icon_book_old_testament);
            else
                item_book.set_icon_white(this.bible.icon_book_new_Testament);
        }
        return item_book;
    }

    private void share()
    {
        string url_link = this.bible.carrot.mainhost + "/?p=bible&id=" + this.data_book_cur["id"].ToString();
        this.bible.carrot.show_share(url_link, PlayerPrefs.GetString("bible_share", "Share this bible book with everyone!"));
    }

    private void nex_page()
    {
        this.index_show_chapter++;
        this.show_chapter();
    }

    private void prev_page()
    {
        this.index_show_chapter--;
        this.show_chapter();
    }

    private void show_chapter()
    {
        IList contents = (IList)this.data_book_cur["contents"];
        IDictionary chapter = (IDictionary)contents[this.index_show_chapter];
        if (this.type_view_page)
            this.view_paragraphs_page(chapter);
        else
            this.view_paragraphs_list(chapter);
    }

    private void show_copy(string s_text)
    {
        this.bible.carrot.show_input(PlayerPrefs.GetString("copy","Copy"), PlayerPrefs.GetString("copy_tip","Copy this bible passage"), s_text);
    }
}
