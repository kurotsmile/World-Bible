using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using UnityEngine;

public class Bible : MonoBehaviour {

    [Header("Obj Main")]
    public Carrot.Carrot carrot;
    public Manager_Book book;

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

    public GameObject btn_removeads;
    public AudioClip sound_click_clip;

    [Header("Ads")]
    float timer_ads= 400.0f;

    void Start () {
        this.carrot.Load_Carrot(this.check_app_exit);
        this.carrot.change_sound_click(this.sound_click_clip);
    }

    public void load_app_online(){
        if (PlayerPrefs.GetString("lang", "") == "")
            this.carrot.show_list_lang(this.act_load);
        else
            this.show_list_book();
    }


    public void load_app_offline(){

    }

    private void act_load(string s_data){
        this.show_list_book();
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
        this.add_loading_item();
        Query queryBible=this.carrot.db.Collection("bible");
        queryBible = queryBible.WhereEqualTo("lang", this.carrot.lang.get_key_lang());
        queryBible.GetSnapshotAsync().ContinueWithOnMainThread(task=>
        {
            if (task.IsCompleted)
            {

                this.carrot.clear_contain(this.tr_all_item_book);
                foreach(DocumentSnapshot doc in task.Result.Documents)
                {
                    IDictionary data = doc.ToDictionary();
                    data["id"] = doc.Id;
                    var id_ebook = doc.Id;
                    if (data["contents"] != null) data.Remove("contents");
                    Carrot.Carrot_Box_Item item_book = this.create_item();
                    if (data["name"] != null)
                    {
                        item_book.set_title(data["name"].ToString());
                        item_book.set_tip(data["type"].ToString());
                    }

                    if (data["type"] != null)
                    {
                        string s_type_book = data["type"].ToString();
                        if (s_type_book == "old_testament")
                            item_book.set_icon_white(this.icon_book_old_testament);
                        else
                            item_book.set_icon_white(this.icon_book_new_Testament);
                    }

                    Carrot.Carrot_Box_Btn_Item btn_save=item_book.create_item();
                    btn_save.set_icon(this.icon_book_save);
                    btn_save.set_icon_color(Color.white);
                    btn_save.set_color(this.carrot.color_highlight);

                    item_book.set_act(() => this.book.view(id_ebook));
                }
            }

            if (task.IsFaulted)
            {
                this.carrot.show_msg("Bible", "Unknown task error, please try again next time!", Carrot.Msg_Icon.Error);
            }
        });
    }

    private Carrot.Carrot_Box_Item create_item()
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

    private void add_loading_item()
    {
        this.carrot.clear_contain(this.tr_all_item_book);
        GameObject obj_loading = Instantiate(this.prefab_loading_item);
        obj_loading.transform.SetParent(this.tr_all_item_book);
        obj_loading.transform.localPosition = new Vector3(obj_loading.transform.localPosition.x, obj_loading.transform.localPosition.y, 0f);
        obj_loading.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_loading.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void show_search()
    {
        this.carrot.ads.show_ads_Interstitial();
        this.carrot.show_search(null,PlayerPrefs.GetString("search_tip","You can search for any biblical content here!"));
    }

    public void rate_app()
    {
        this.carrot.show_rate();
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
