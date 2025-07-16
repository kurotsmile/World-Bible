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

    [Header("Ui")]
    public Text TxtValCountOldBible;
    public Text TxtValCountNewBible;
    public Text txtPassage;
    public GameObject panelHome;
    public GameObject panelMain;

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
    public Sprite icon_speech;
    public Sprite icon_up;
    public Sprite icon_down;
    public Sprite icon_history;


    [Header("Sound")]
    public AudioClip sound_click_clip;
    public AudioSource soundBk;

    [Header("Color")]
    public Color32 color_row_a;
    public Color32 color_row_b;
    public Color32 color_text_title;
    public Color32 color_row_title_a;
    public Color32 color_row_title_b;

    [Header("Ads")]
    float timer_ads = 400.0f;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        carrot.Load_Carrot();
        carrot.change_sound_click(sound_click_clip);
        carrot.game.load_bk_music(this.soundBk);
        this.ads.On_Load();

        this.carrot.act_buy_ads_success = this.ads.RemoveAds;
        this.carrot.game.act_click_watch_ads_in_music_bk = this.ads.ShowRewardedVideo;
        this.ads.onRewardedSuccess = this.carrot.game.OnRewardedSuccess;

        offline.On_load();

        if (this.carrot.os_app == OS.Window)
            this.file.type = Carrot_File_Type.StandaloneFileBrowser;
        else
            this.file.type = Carrot_File_Type.SimpleFileBrowser;

        if (PlayerPrefs.GetString("lang", "") == "")
            carrot.Show_list_lang(Act_load);
        else
            Act_load("");

        this.panelHome.SetActive(true);
        this.panelMain.SetActive(false);
    }

    public void Act_load(string s_data = "")
    {
        this.Add_loading_item();
        this.carrot.delay_function(1f, ()=> {
            book.OnLoad();
            menu.load();
        });
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
        carrot.Show_list_lang(Act_load);
    }

    public void ShowListBook(string Stype="")
    {
        this.panelHome.SetActive(false);
        this.panelMain.SetActive(true);
        this.book.ShowList(Stype);
    }

    public IList SortListByOrderKey(IList list)
    {
        var dictList = list.Cast<IDictionary>().ToList();
        var sorted = dictList.OrderBy(d => System.Convert.ToInt32(d["order"])).ToList();
        return sorted;
    }

    public Carrot_Box_Item Create_item()
    {
        GameObject obj_item = Instantiate(prefab_book_item);
        obj_item.transform.SetParent(tr_all_item_book);
        obj_item.transform.localPosition = new Vector3(obj_item.transform.localPosition.x, obj_item.transform.localPosition.y, 0f);
        obj_item.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_item.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Carrot_Box_Item item_box = obj_item.GetComponent<Carrot_Box_Item>();
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

    public Carrot_Box_Item add_title(string s_title)
    {
        Carrot_Box_Item item_title = Create_item();
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
        item_path.set_title(carrot.L("path_save", "Set data directory path"));
        item_path.set_tip(carrot.L("path_save_tip", "Data editing path (data export and import)"));
        item_path.set_type(Box_Item_Type.box_value_txt);
        item_path.set_val(book.GetPathData());
        item_path.set_act(() =>
        {
            this.file.Open_folders(s_path =>
            {
                this.book.SetPathData(s_path[0]);
                item_path.set_val(s_path[0]);
                PlayerPrefs.SetString("path_data", s_path[0]);

                string s_path_file = System.IO.Path.Combine(s_path[0], "bible-" + this.carrot.lang.Get_key_lang() + ".json");
                if (FileBrowserHelpers.FileExists(s_path_file))
                {
                    string s_data = FileBrowserHelpers.ReadTextFromFile(s_path_file);
                    this.book.Load_list_by_data(s_data);
                    this.carrot.Show_msg(this.carrot.L("app_title", "Bible world"), this.carrot.L("path_save_success", "Path saved successfully!"));
                }
                else
                {
                    FileBrowserHelpers.WriteTextToFile(s_path_file, Json.Serialize(this.book.list_data_Bible));
                    this.carrot.Show_msg(this.carrot.L("app_title", "Bible world"), this.carrot.L("path_save_success", "Path saved successfully!"));
                }
            });
        });
    }

    public void ShowHome()
    {
        this.panelHome.SetActive(true);
        this.panelMain.SetActive(false);
        this.txtPassage.text = this.book.GetPassage();
        this.TxtValCountNewBible.text = this.book.GetLengthBibleByType("new_testament").ToString()+" "+carrot.L("book","Book");
        this.TxtValCountOldBible.text = this.book.GetLengthBibleByType("old_testament").ToString()+" "+carrot.L("book","Book");
    }

    public void BtnShowNewPassage()
    {
        carrot.play_sound_click();
        book.GetAndShowNewPassage();
    }

    public void BtnShowBibleNew()
    {
        carrot.play_sound_click();
        ShowListBook("new_testament");
        menu.Select_Menu_No_func(1);
    }

    public void BtnShowBibleOld()
    {
        carrot.play_sound_click();
        ShowListBook("old_testament");
        menu.Select_Menu_No_func(1);
    }
}
