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
    public Sprite icon_speech;
    public Sprite icon_up;
    public Sprite icon_down;

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
        
        book.OnLoad();
        offline.On_load();

        if (this.carrot.os_app == OS.Window)
            this.file.type = Carrot_File_Type.StandaloneFileBrowser;
        else
            this.file.type = Carrot_File_Type.SimpleFileBrowser;

        if (PlayerPrefs.GetString("lang", "") == "")
            carrot.Show_list_lang(Act_load);
        else
            Act_load("");
    }

    public void Act_load(string s_data="")
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
        carrot.Show_list_lang(Act_load);
    }

    public void show_list_book()
    {
        string PathFileData = this.book.GetPathData() + "/bible-" + this.carrot.lang.Get_key_lang() + ".json";
        if (this.book.IsEditor())
        {
            if (FileBrowserHelpers.FileExists(PathFileData))
            {
                string sData = FileBrowserHelpers.ReadTextFromFile(PathFileData);
                this.Load_list_by_data(sData);
            }
            else
            {
                TextAsset bible_data_text = Resources.Load<TextAsset>("bible_" + this.carrot.lang.Get_key_lang());
                if (bible_data_text==null)
                    this.Load_list_by_data("");
                else
                    this.Load_list_by_data(bible_data_text.text);
            }  
        }
        else
        {
            TextAsset bible_data_text = Resources.Load<TextAsset>("bible_" + this.carrot.lang.Get_key_lang());
            this.Load_list_by_data(bible_data_text.text);
        }
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

        if(s_data=="")
            this.book.list_data_Bible = Json.Deserialize("[]") as IList;
        else
            this.book.list_data_Bible = Json.Deserialize(s_data) as IList;
        this.book.list_data_Bible = SortListByOrderKey(this.book.list_data_Bible);

        IList list_book_Old_testament = (IList)Json.Deserialize("[]");
        Carrot_Box_Item item_Bible_Old = add_title(this.carrot.L("old_testament", "Old testament"));
        item_Bible_Old.set_icon_white(icon_book_old_testament);
        item_Bible_Old.set_tip("Old testament");
        item_Bible_Old.GetComponent<Image>().color = this.color_row_title_a;

        IList list_book_New_testament = (IList)Json.Deserialize("[]");
        Carrot_Box_Item item_Bible_New = add_title(this.carrot.L("new_testament", "New Testament"));
        item_Bible_New.set_icon_white(icon_book_new_Testament);
        item_Bible_New.set_tip("New Testament");
        item_Bible_New.GetComponent<Image>().color = this.color_row_title_b;

        int index_item = 0;
        for (int i = 0; i < this.book.list_data_Bible.Count; i++)
        {
            var index_data = i;
            IDictionary data = this.book.list_data_Bible[i] as IDictionary;
            var bookData = data;
            Carrot_Box_Item item_book = book.Item_book(data);
            item_book.set_act(() => book.View(data,index_data));
            data["index_data"] = index_data;
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

            if (book.IsEditor())
            {
                Carrot_Box_Btn_Item btn_edit = item_book.create_item();
                btn_edit.set_icon(this.carrot.user.icon_user_edit);
                btn_edit.set_icon_color(Color.white);
                btn_edit.set_color(this.carrot.color_highlight);
                btn_edit.set_act(()=>
                {
                    this.carrot.play_sound_click();
                    this.book.ShowAddBook(bookData);
                });

                Carrot_Box_Btn_Item btn_del = item_book.create_item();
                btn_del.set_icon(this.carrot.sp_icon_del_data);
                btn_del.set_icon_color(Color.white);
                btn_del.set_color(Color.red);
                btn_del.set_act(()=>
                {
                    this.carrot.Show_msg("Delete Book", "Are you sure you want to delete this '" + data["name"].ToString() + "' book?",()=>
                    {
                        this.book.DeleteEbook(index_data);
                        this.Act_load("");
                    });
                });
            }

            index_item++;
        }

        if (this.book.IsEditor())
        {
            Carrot_Box_Item item_add_book = this.Create_item();
            item_add_book.set_icon_white(carrot.icon_carrot_add);
            item_add_book.set_title("Add Book");
            item_add_book.set_tip("Add new a Book");
            item_add_book.txt_tip.color = Color.black;
            item_add_book.gameObject.GetComponent<Image>().color = carrot.color_highlight;
            item_add_book.set_act(() =>
            {
                this.carrot.play_sound_click();
                this.book.ShowAddBook();
            });
        }

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
                    this.Load_list_by_data(s_data);
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
}
