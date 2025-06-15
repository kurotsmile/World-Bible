using Carrot;
using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Bible : MonoBehaviour
{

    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_Book book;
    public Book_Offline offline;
    public Manager_Menu menu;
    public Manager_Search search;
    public IronSourceAds ads;
    public Carrot_File file;

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
    public Sprite icon_path_file;
    public Color32 color_row_a;
    public Color32 color_row_b;
    public AudioClip sound_click_clip;

    private IList list_data_Bible;
    private bool is_ready_cache = true;

    [Header("Ads")]
    float timer_ads = 400.0f;

    private string s_path_data = "";
    private bool is_editor = false;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        carrot.Load_Carrot();

        if (PlayerPrefs.GetString("path_data", "") != "")
        {
            s_path_data = PlayerPrefs.GetString("path_data");
            this.is_editor = true;
        }
        else
        {
            s_path_data = System.IO.Path.Combine(Application.dataPath, "Resources");
            this.is_editor = false;
        }

        carrot.change_sound_click(sound_click_clip);
        offline.On_load();

        if (this.carrot.os_app == OS.Window)
            this.file.type = Carrot_File_Type.StandaloneFileBrowser;
        else
            this.file.type = Carrot_File_Type.SimpleFileBrowser;
    }

    public void load_app_online()
    {
        if (PlayerPrefs.GetString("lang", "") == "")
            carrot.Show_list_lang(Act_load);
        else
            menu.load();
    }

    public void load_app_offline()
    {
        menu.select_menu(1);
    }

    private void Act_load(string s_data)
    {
        this.Add_loading_item();
        this.carrot.delay_function(1f, menu.load);
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

    public void show_list_country()
    {
        is_ready_cache = false;
        carrot.Show_list_lang(Act_load);
    }

    public void show_list_book()
    {
        TextAsset bible_data_text = Resources.Load<TextAsset>("bible_"+this.carrot.lang.Get_key_lang());
        this.Load_list_by_data(bible_data_text.text);
    }

    private IList SortListByOrderKey(IList list)
    {
        var dictList = list.Cast<IDictionary>().ToList();
        var sorted = dictList.OrderBy(d => System.Convert.ToInt32(d["order"])).ToList();
        return sorted;
    }

    private void Load_list_by_data(string s_data)
    {
        carrot.clear_contain(tr_all_item_book);

        list_data_Bible = Json.Deserialize(s_data) as IList;
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
            IDictionary data = list_data_Bible[i] as IDictionary;
            var id_book = data["id"].ToString();
            string lang_book = data["lang"].ToString();

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
        item_book_update.gameObject.GetComponent<Image>().color = carrot.color_highlight;
        item_book_update.set_act(() =>
        {
            this.carrot.Show_msg("Save Ebook " + this.carrot.lang.Get_key_lang() + " success!");
        });

        Carrot_Box_Item item_add_book = this.Create_item();
        item_add_book.set_icon_white(carrot.icon_carrot_download);
        item_add_book.set_title(this.carrot.L("check_update", "Check and update book data"));
        item_add_book.set_tip(this.carrot.L("check_update_tip", "Download new data and updated books in the current language"));
        item_add_book.txt_tip.color = Color.black;
        item_add_book.gameObject.GetComponent<Image>().color = carrot.color_highlight;
        item_add_book.set_act(() =>
        {
            this.carrot.play_sound_click();
            this.book.ShowAddBook();
        });

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

    public void add_none(bool is_clear = true)
    {
        if (is_clear) carrot.clear_contain(tr_all_item_book);
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
        Carrot.Carrot_Box_Item item_title = Create_item();
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

    public void show_list_app_other()
    {
        carrot.show_list_carrot_app();
    }

    public void show_setting()
    {
        Carrot_Box box_setting = carrot.Create_Setting();

        Carrot_Box_Item item_path = box_setting.create_item_of_top();
        item_path.set_icon(icon_path_file);
        item_path.set_title(carrot.L("path_save", "Path save"));
        item_path.set_tip(carrot.L("path_save_tip", "Path to save books and data"));
        item_path.set_type(Box_Item_Type.box_value_txt);
        item_path.set_val(s_path_data);
        item_path.set_act(() =>
        {
            this.file.Open_folders(s_path =>
            {
                item_path.set_val(s_path[0]);
                PlayerPrefs.SetString("path_data", s_path[0]);
                this.carrot.Show_msg(this.carrot.L("app_title", "Bible world"), this.carrot.L("path_save_success", "Path saved successfully!"));
                FileBrowserHelpers.WriteTextToFile(System.IO.Path.Combine(s_path[0], "bible-"+this.carrot.lang.Get_key_lang()+".json"), Json.Serialize(this.list_data_Bible));
            });
        });
    }
}
