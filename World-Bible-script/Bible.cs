using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bible : MonoBehaviour {

    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_Book book;
    public Book_Offline offline;
    public Manager_Menu menu;
    public Manager_Search search;

    [Header("Obj Bible")]
    public Transform tr_all_item_book;
    public GameObject prefab_book_item;
    public GameObject prefab_loading_item;
    public GameObject prefab_paragraph_item;

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

    private IList list_data_Bible;
    private bool is_ready_cache = false;

    [Header("Ads")]
    float timer_ads= 400.0f;

    void Start () {
        this.carrot.Load_Carrot(this.check_app_exit);
        this.carrot.change_sound_click(this.sound_click_clip);
        this.offline.load();
    }

    public void load_app_online(){
        if (PlayerPrefs.GetString("lang", "") == "")
            this.carrot.show_list_lang(this.act_load);
        else
            this.menu.load();
    }

    public void load_app_offline(){
        this.menu.select_menu(1);
    }

    private void act_load(string s_data){
        this.menu.load();
    }

    private void check_app_exit(){

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
            this.carrot.ads.show_ads_Interstitial();
            timer_ads = 300.0f;
        }
    }

	public void show_list_country(){
        this.carrot.show_list_lang(this.act_load);
	}

    public void show_list_book()
    {
        if (this.carrot.is_offline()) this.is_ready_cache = true;

        if (this.is_ready_cache == false)
        {
            this.get_data_from_sever();
        }
        else
        {
            string s_data = PlayerPrefs.GetString("data_bible_" + this.carrot.lang.get_key_lang());
            if (s_data == "")
            {
                this.get_data_from_sever();
            }
            else
            {
                IList list_bible = (IList)Carrot.Json.Deserialize(s_data);
                this.load_list_by_data(list_bible);
            }
        }

    }

    private void get_data_from_sever()
    {
        this.add_loading_item();
        Query queryBible = this.carrot.db.Collection("bible");
        queryBible = queryBible.WhereEqualTo("lang", this.carrot.lang.get_key_lang());
        queryBible.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result.Count > 0)
                {
                    this.list_data_Bible = (IList)Carrot.Json.Deserialize("[]");
                    foreach (DocumentSnapshot doc in task.Result.Documents)
                    {
                        IDictionary data = doc.ToDictionary();
                        data["id"] = doc.Id;
                        this.list_data_Bible.Add(data);
                    }
                    this.load_list_by_data(this.list_data_Bible);
                    PlayerPrefs.SetString("data_bible_" + this.carrot.lang.get_key_lang(), Carrot.Json.Serialize(this.list_data_Bible));
                    this.is_ready_cache = true;
                }
                else
                {
                    this.add_none();
                }
            }

            if (task.IsFaulted)
            {
                this.add_none();
                this.carrot.show_msg(PlayerPrefs.GetString("app_title", "Bible world"), PlayerPrefs.GetString("error_unknown", "Operation error, please try again next time!"), Carrot.Msg_Icon.Error);
            }
        });
    }

    private void load_list_by_data(IList list)
    {
        this.carrot.clear_contain(this.tr_all_item_book);

        IList list_book_Old_testament = (IList)Carrot.Json.Deserialize("[]");
        Carrot.Carrot_Box_Item item_Bible_Old = this.add_title(PlayerPrefs.GetString("old_testament", "Old testament"));
        item_Bible_Old.set_icon_white(this.icon_book_old_testament);
        item_Bible_Old.set_tip("Old testament");

        IList list_book_New_testament = (IList)Carrot.Json.Deserialize("[]");
        Carrot.Carrot_Box_Item item_Bible_New = this.add_title(PlayerPrefs.GetString("new_testament", "New Testament"));
        item_Bible_New.set_icon_white(this.icon_book_new_Testament);
        item_Bible_New.set_tip("New Testament");

        int index_item = 0;
        for(int i=0;i<list.Count;i++)
        {
            IDictionary data =(IDictionary) list[i];
            var id_book = data["id"].ToString();
            if (data["contents"] != null) data.Remove("contents");
            Carrot.Carrot_Box_Item item_book = this.book.item_book(data);
            item_book.set_act(() => this.book.view(id_book));

            if (data["type"] != null)
            {
                string s_type_book = data["type"].ToString();
                if (s_type_book == "old_testament")
                    list_book_Old_testament.Add(data);
                else
                    list_book_New_testament.Add(data);
            }

            if (index_item % 2 == 0)
                item_book.gameObject.GetComponent<Image>().color = this.color_row_a;
            else
                item_book.gameObject.GetComponent<Image>().color = this.color_row_b;

            item_book.gameObject.name = data["type"].ToString();

            Carrot.Carrot_Box_Btn_Item btn_save = item_book.create_item();
            btn_save.set_icon(this.icon_book_save);
            btn_save.set_icon_color(Color.white);
            btn_save.set_color(this.carrot.color_highlight);
            btn_save.set_act(() => this.offline.get_and_save(id_book));

            index_item++;
        }

        item_Bible_Old.set_tip(list_book_Old_testament.Count + " " + PlayerPrefs.GetString("book", "Book"));
        Carrot.Carrot_Box_Btn_Item btn_list_old = item_Bible_Old.create_item();
        btn_list_old.set_icon(this.book.icon_list);
        Destroy(btn_list_old.GetComponent<Button>());
        item_Bible_Old.set_act(() => book.show_list_book_by_type("old_testament"));

        item_Bible_New.set_tip(list_book_New_testament.Count + " " + PlayerPrefs.GetString("book", "Book"));
        Carrot.Carrot_Box_Btn_Item btn_list_new = item_Bible_New.create_item();
        btn_list_new.set_icon(this.book.icon_list);
        Destroy(btn_list_new.GetComponent<Button>());
        item_Bible_New.set_act(() => book.show_list_book_by_type("new_testament"));
    }

    public Carrot.Carrot_Box_Item create_item()
    {
        GameObject obj_item = Instantiate(this.prefab_book_item);
        obj_item.transform.SetParent(this.tr_all_item_book);
        obj_item.transform.localPosition = new Vector3(obj_item.transform.localPosition.x, obj_item.transform.localPosition.y, 0f);
        obj_item.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_item.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        Carrot.Carrot_Box_Item item_box = obj_item.GetComponent<Carrot.Carrot_Box_Item>();
        item_box.on_load(this.carrot);
        item_box.check_type();
        return item_box;
    }

    public void add_loading_item()
    {
        this.carrot.clear_contain(this.tr_all_item_book);
        GameObject obj_loading = Instantiate(this.prefab_loading_item);
        obj_loading.transform.SetParent(this.tr_all_item_book);
        obj_loading.transform.localPosition = new Vector3(obj_loading.transform.localPosition.x, obj_loading.transform.localPosition.y, 0f);
        obj_loading.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_loading.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void add_none(bool is_clear=true)
    {
        if(is_clear)this.carrot.clear_contain(this.tr_all_item_book);
        Carrot.Carrot_Box_Item item_none = this.create_item();
        item_none.set_icon(this.icon_sad);
        item_none.set_title("List is empty");
        item_none.set_tip("There are no items on this list yet!");
        item_none.set_lang_data("list_none", "list_none_tip");
        item_none.load_lang_data();
        item_none.GetComponent<Image>().color = this.carrot.color_highlight;
    }

    public Carrot.Carrot_Box_Item add_title(string s_title)
    {
        Carrot.Carrot_Box_Item item_title=this.create_item();
        item_title.set_title(s_title);
        item_title.txt_name.color = Color.white;
        item_title.txt_tip.color = Color.white;
        item_title.gameObject.GetComponent<Image>().color = this.carrot.color_highlight;
        return item_title;
    }

    public void show_search()
    {
        this.search.show_search();
    }

    public void app_share()
    {
        this.carrot.show_share();
    }

    public void show_list_app_other(){
        this.carrot.show_list_carrot_app();
    }

    public void show_setting(){
        this.carrot.Create_Setting();
    }
}
