using Carrot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bible : MonoBehaviour {

    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_Book book;
    public Book_Offline offline;
    public Manager_Menu menu;
    public Manager_Search search;
    public IronSourceAds ads;

    [Header("Obj Bible")]
    public Transform tr_all_item_book;
    public GameObject prefab_book_item;
    public GameObject prefab_loading_item;
    public GameObject prefab_paragraph_item;
    public GameObject prefab_paragraph_item_ko;
    public GameObject prefab_paragraph_item_zh;

    public Sprite icon_book_old_testament;
    public Sprite icon_book_new_Testament;
    public Sprite icon_book_save;
    public Sprite icon_book_open;
    public Sprite icon_chapter;
    public Sprite icon_paragraph;
    public Sprite icon_search;
    public Sprite icon_sad;
    public Sprite icon_next_page;
    public Sprite icon_prev_page;
    public Sprite icon_copy;

    public Color32 color_row_a;
    public Color32 color_row_b;
    public AudioClip sound_click_clip;

    private IList<IDictionary> list_data_Bible;
    private bool is_ready_cache = true;

    [Header("Ads")]
    float timer_ads= 400.0f;

    void Start () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        carrot.Load_Carrot();
        carrot.change_sound_click(sound_click_clip);
        offline.On_load();
    }

    public void load_app_online(){
        if (PlayerPrefs.GetString("lang", "") == "")
            carrot.Show_list_lang(Act_load);
        else
            menu.load();
    }

    public void load_app_offline(){
        menu.select_menu(1);
    }

    private void Act_load(string s_data){
        menu.load();
    }

    void Update()
    {
        timer_ads -= Time.deltaTime;
        ShowAd();
    }

    private void ShowAd()
    {
        if (timer_ads <= 0)
        {
            this.ads.ShowInterstitialAd();
            timer_ads = 300.0f;
        }
    }

	public void show_list_country(){
        is_ready_cache = false;
        carrot.Show_list_lang(Act_load);
	}

    public void show_list_book()
    {
        if (carrot.is_offline()) is_ready_cache = true;

        if (is_ready_cache == false)
        {
            Get_data_by_lang_from_sever(carrot.lang.Get_key_lang());
        }
        else
        {
            string s_data = PlayerPrefs.GetString("data_bible_" + carrot.lang.Get_key_lang());
            if (s_data == "")
            {
                Get_data_by_lang_from_sever(carrot.lang.Get_key_lang());
            }
            else
            {
                Load_list_by_data(s_data);
            }
        }
    }

    private void Get_data_by_lang_from_sever(string s_key_lang)
    {
        Add_loading_item();
        StructuredQuery q = new("bible");
        q.Add_where("lang", Query_OP.EQUAL, s_key_lang);
        carrot.server.Get_doc(q.ToJson(), Get_data_from_sever_done, Get_data_from_sever_fail);
    }

    private void Get_data_from_sever()
    {
        Add_loading_item();
        StructuredQuery q = new("bible");
        q.Add_where("lang", Query_OP.EQUAL, carrot.lang.Get_key_lang());
        carrot.server.Get_doc(q.ToJson(), Get_data_from_sever_done, Get_data_from_sever_fail);
    }

    IList<IDictionary> SortListByOrderKey(IList<IDictionary> list)
    {
        return list.OrderBy(dict => int.Parse(dict["order"].ToString())).ToList();
    }

    private void Get_data_from_sever_done(string s_data)
    {
        PlayerPrefs.SetString("data_bible_" + carrot.lang.Get_key_lang(), s_data);
        is_ready_cache = true;
        Load_list_by_data(s_data);
    }

    private void Get_data_from_sever_fail(string s_error)
    {
        string s_data = PlayerPrefs.GetString("data_bible_" + carrot.lang.Get_key_lang());
        if (s_data != "")
        {
            Load_list_by_data(s_data);
        }
        else
        {
            add_none();
            carrot.Show_msg(this.carrot.L("app_title", "Bible world"),this.carrot.L("error_unknown", "Operation error, please try again next time!"), Msg_Icon.Error);
        }
    }

    private void Load_list_by_data(string s_data)
    {
        carrot.clear_contain(tr_all_item_book);

        Fire_Collection fc = new(s_data);
        if (!fc.is_null)
        {
            list_data_Bible = new List<IDictionary>();
            for (int i = 0; i < fc.fire_document.Length; i++)
            {
                IDictionary data = fc.fire_document[i].Get_IDictionary();
                list_data_Bible.Add(data);
            }

            list_data_Bible = SortListByOrderKey(list_data_Bible);

            IList list_book_Old_testament = (IList)Json.Deserialize("[]");
            Carrot_Box_Item item_Bible_Old = add_title(this.carrot.L("old_testament", "Old testament"));
            item_Bible_Old.set_icon_white(icon_book_old_testament);
            item_Bible_Old.set_tip("Old testament");

            IList list_book_New_testament = (IList)Json.Deserialize("[]");
            Carrot_Box_Item item_Bible_New = add_title(this.carrot.L("new_testament", "New Testament"));
            item_Bible_New.set_icon_white(icon_book_new_Testament);
            item_Bible_New.set_tip("New Testament");

            int index_item = 0;
            for (int i = 0; i < list_data_Bible.Count; i++)
            {
                IDictionary data = list_data_Bible[i];
                var id_book = data["id"].ToString();
                Carrot_Box_Item item_book = book.Item_book(data);
                item_book.set_act(() => book.View(data));

                if (data["type"] != null)
                {
                    string s_type_book = data["type"].ToString();
                    if (s_type_book == "old_testament")
                        list_book_Old_testament.Add(data);
                    else
                        list_book_New_testament.Add(data);
                }

                if (index_item % 2 == 0)
                    item_book.gameObject.GetComponent<Image>().color = color_row_a;
                else
                    item_book.gameObject.GetComponent<Image>().color = color_row_b;

                item_book.gameObject.name = data["type"].ToString();

                Carrot_Box_Btn_Item btn_save = item_book.create_item();
                btn_save.set_icon(icon_book_save);
                btn_save.set_icon_color(Color.white);
                btn_save.set_color(carrot.color_highlight);
                btn_save.set_act(() => offline.Add(data));

                index_item++;
            }

            Carrot_Box_Item item_book_update = this.Create_item();
            item_book_update.set_icon_white(carrot.icon_carrot_download);
            item_book_update.set_title(this.carrot.L("check_update", "Check and update book data"));
            item_book_update.set_tip(this.carrot.L("check_update_tip", "Download new data and updated books in the current language"));
            item_book_update.txt_tip.color = Color.black;
            item_book_update.set_act(() => Get_data_from_sever());
            item_book_update.gameObject.GetComponent<Image>().color = carrot.color_highlight;

            item_Bible_Old.set_tip(list_book_Old_testament.Count + " " + this.carrot.L("book", "Book"));
            Carrot_Box_Btn_Item btn_list_old = item_Bible_Old.create_item();
            btn_list_old.set_icon(book.icon_list);
            Destroy(btn_list_old.GetComponent<Button>());
            item_Bible_Old.set_act(() => book.Show_list_book_by_type("old_testament"));

            item_Bible_New.set_tip(list_book_New_testament.Count + " " + this.carrot.L("book", "Book"));
            Carrot_Box_Btn_Item btn_list_new = item_Bible_New.create_item();
            btn_list_new.set_icon(book.icon_list);
            Destroy(btn_list_new.GetComponent<Button>());
            item_Bible_New.set_act(() => book.Show_list_book_by_type("new_testament"));
        }
        else
        {
            Get_data_by_lang_from_sever("en");
        }
    }

    public Carrot.Carrot_Box_Item Create_item()
    {
        GameObject obj_item = Instantiate(prefab_book_item);
        obj_item.transform.SetParent(tr_all_item_book);
        obj_item.transform.localPosition = new Vector3(obj_item.transform.localPosition.x, obj_item.transform.localPosition.y, 0f);
        obj_item.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_item.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Carrot.Carrot_Box_Item item_box = obj_item.GetComponent<Carrot.Carrot_Box_Item>();
        item_box.on_load(carrot);
        item_box.check_type();
        return item_box;
    }

    public void Add_loading_item()
    {
        carrot.clear_contain(tr_all_item_book);
        GameObject obj_loading = Instantiate(prefab_loading_item);
        obj_loading.transform.SetParent(tr_all_item_book);
        obj_loading.transform.localPosition = new Vector3(obj_loading.transform.localPosition.x, obj_loading.transform.localPosition.y, 0f);
        obj_loading.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_loading.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void add_none(bool is_clear=true)
    {
        if(is_clear)carrot.clear_contain(tr_all_item_book);
        Carrot_Box_Item item_none = Create_item();
        item_none.set_icon(icon_sad);
        item_none.set_title("List is empty");
        item_none.set_tip("There are no items on this list yet!");
        item_none.set_lang_data("list_none", "list_none_tip");
        item_none.load_lang_data();
        item_none.GetComponent<Image>().color = carrot.color_highlight;
    }

    public Carrot.Carrot_Box_Item add_title(string s_title)
    {
        Carrot.Carrot_Box_Item item_title=Create_item();
        item_title.set_title(s_title);
        item_title.txt_name.color = Color.white;
        item_title.txt_tip.color = Color.white;
        item_title.gameObject.GetComponent<Image>().color = carrot.color_highlight;
        return item_title;
    }

    public void show_search()
    {
        search.show_search();
    }

    public void app_share()
    {
        carrot.show_share();
    }

    public void show_list_app_other(){
        carrot.show_list_carrot_app();
    }

    public void show_setting(){
        carrot.Create_Setting();
    }
}
