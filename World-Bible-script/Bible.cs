using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Purchasing;

public class Bible : MonoBehaviour {

    public Carrot.Carrot carrot;
    public GameObject panel_main;
    public Panel_view panel_view;
    public GameObject panel_setting;
    public Transform area_main_body_contain;
    public Transform area_contain_list_book1;
    public Transform area_contain_list_book2;
    public GameObject prefab_book_home;
    public Image img_bk_home_item1;
    public Text txt_title_to_day;
    public Text txt_p_of_day;

    public Sprite icon_book1;
    public Sprite icon_book2;
    public Sprite icon_p;
    public Sprite icon_search;

    public GameObject btn_removeads;
    public GameObject panel_setting_removeads;
    public AudioSource Sound_Click;

    private Book_home_item[] arr_item_book1;
    private Book_home_item[] arr_item_book2;

    public Panel_view_quote panel_view_quocte;

    [Header("Ads")]
    public string ads_id_app_vungle;
    public string ads_id_trunggiang_vungle;
    float timer_ads= 400.0f;
#pragma warning disable 0649
    string gameId = "3222624";
#pragma warning restore 0649
    private int count_click_ads = 0;

    void Start () {
        this.carrot.Load_Carrot(this.check_app_exit);
        this.carrot.shop.onCarrotPaySuccess = this.onBuySuccessCarrotPay;
        this.carrot.shop.onCarrotRestoreSuccess = this.onRestoreSuccessCarrotPay;
        this.panel_view.gameObject.SetActive(false);
        this.panel_view_quocte.gameObject.SetActive(false);
        this.panel_setting.SetActive(false);
    }
    private void load_ads(){
        if (PlayerPrefs.GetInt("is_buy_ads", 0) == 0)
        {
#if UNITY_WSA
            Vungle.init(this.ads_id_app_vungle);
            Vungle.loadAd(this.ads_id_trunggiang_vungle);
#else
            if (Advertisement.isSupported) Advertisement.Initialize(gameId,false);
#endif
        }
        this.check_ads();
    }

    public void load_app_online(){
        this.load_ads();
        if (PlayerPrefs.GetString("lang", "") == "")
        {
            this.carrot.show_list_lang(this.act_load);
        }
        else
        {
            this.show_list_book();
        }
        
        this.txt_title_to_day.text=PlayerPrefs.GetString("quote_of_day","Bible verses of the day");
    }


    public void load_app_offline(){
        this.GetComponent<Book_offline>().show_list_offline_in_home();
        this.check_ads();
        this.txt_p_of_day.text =PlayerPrefs.GetString("p_of_day","...");
        this.txt_title_to_day.text=PlayerPrefs.GetString("offline_title","You are using the application in offline mode, the main functions will be displayed when the application is connected to the network");
    }

    private void check_ads(){
        this.btn_removeads.SetActive(false);
        this.panel_setting_removeads.SetActive(false);
        if (PlayerPrefs.GetInt("is_buy_ads", 0) == 0)
        {
            if(this.carrot.is_online()){
                this.btn_removeads.SetActive(true);
                this.panel_setting_removeads.SetActive(true);
            }
        }
    }

    private void act_load(string s_data){
        this.show_list_book();
    }

    private void check_app_exit(){
        if (this.panel_view.gameObject.activeInHierarchy)
        {
            this.panel_view.Close();
            this.carrot.set_no_check_exit_app();
        }
        else if (this.panel_view_quocte.gameObject.activeInHierarchy)
        {
            this.panel_view_quocte.btn_close.onClick.Invoke();
            this.carrot.set_no_check_exit_app();
        }
        else if(this.panel_setting.activeInHierarchy){
            this.close_setting();
            this.carrot.set_no_check_exit_app();
        }
    }

    void Update()
    {
        timer_ads -= Time.deltaTime;
        ShowAd();
    }

    private void ShowAd()
    {
        if (PlayerPrefs.GetInt("is_buy_ads", 0) == 0)
        {
           if (timer_ads <= 0)
            {
                Advertisement.Show();
                timer_ads = 300.0f;
           }
        }
    }

    public void check_click_show_ads()
    {
        if (PlayerPrefs.GetInt("is_buy_ads", 0) == 0)
        {
            this.count_click_ads++;
            if (this.count_click_ads >= 16)
            {
#if UNITY_WSA
                Vungle.playAd(this.ads_id_trunggiang_vungle);
#else
                if(Advertisement.IsReady())Advertisement.Show("video");
#endif
                this.count_click_ads = 0;
            }
        }
    }

	public void show_list_country(){
        this.Sound_Click.Play();
        this.carrot.show_list_lang(this.act_load);
	}

    [ContextMenu ("Delete All data")]
    public void delete_all_data()
    {
        PlayerPrefs.DeleteAll();
        this.carrot.delete_all_data();
        this.Sound_Click.Play();
        this.Start();
    }

    public void show_list_book()
    {
        WWWForm frm_book=this.carrot.frm_act("load_book");
        this.carrot.send(frm_book,Load_list_book);
    }

    private void Load_list_book(string s_data)
    {
        IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data);
        IList all_item = (IList)data["list_book1"];
        this.txt_p_of_day.text = data["p_of_day"].ToString();
        PlayerPrefs.SetString("p_of_day",data["p_of_day"].ToString());
        PlayerPrefs.SetString("p_of_day_id",data["p_of_day_id"].ToString());
        PlayerPrefs.SetString("p_of_day_lang",data["p_of_day_lang"].ToString());

        this.arr_item_book1 = new Book_home_item[all_item.Count];
        this.carrot.clear_contain(this.area_contain_list_book1);
        for(int i=0;i< all_item.Count;i++)
        {
        IDictionary item = (IDictionary)all_item[i];
        GameObject item_book_1 = Instantiate(this.prefab_book_home);
        item_book_1.transform.SetParent(this.area_contain_list_book1);
        item_book_1.transform.localPosition = new Vector3(item_book_1.transform.localPosition.x, item_book_1.transform.localPosition.y, 0f);
        item_book_1.transform.localScale = new Vector3(1f, 1f, 1f);
        item_book_1.GetComponent<Book_home_item>().id_book = item["id"].ToString();
        item_book_1.GetComponent<Book_home_item>().txt_name.text = item["name"].ToString();
        item_book_1.GetComponent<Book_home_item>().icon.sprite = this.icon_book1;
        item_book_1.GetComponent<Book_home_item>().chapter= int.Parse(item["chapter"].ToString());
        item_book_1.GetComponent<Book_home_item>().type = 1;
        this.arr_item_book1[i] = item_book_1.GetComponent<Book_home_item>();
        }


        IList all_item2 = (IList)data["list_book2"];
        this.arr_item_book2 = new Book_home_item[all_item2.Count];
        this.carrot.clear_contain(this.area_contain_list_book2);
        for (int i = 0; i < all_item2.Count; i++)
        {
        IDictionary item = (IDictionary)all_item2[i];
        GameObject item_book_2 = Instantiate(this.prefab_book_home);
        item_book_2.transform.SetParent(this.area_contain_list_book2);
        item_book_2.transform.localPosition = new Vector3(item_book_2.transform.localPosition.x, item_book_2.transform.localPosition.y, 0f);
        item_book_2.transform.localScale = new Vector3(1f, 1f, 1f);
        item_book_2.GetComponent<Book_home_item>().id_book = item["id"].ToString();
        item_book_2.GetComponent<Book_home_item>().txt_name.text = item["name"].ToString();
        item_book_2.GetComponent<Book_home_item>().icon.sprite = this.icon_book2;
        item_book_2.GetComponent<Book_home_item>().chapter = int.Parse(item["chapter"].ToString());
        item_book_2.GetComponent<Book_home_item>().type = 2;
        this.arr_item_book2[i] = item_book_2.GetComponent<Book_home_item>();
        }
        this.carrot.get_img(data["bk_home_item1"].ToString(),this.img_bk_home_item1);
    }

    public void show_search()
    {
        this.check_click_show_ads();
        this.Sound_Click.Play();
        WWWForm frm_search = this.carrot.frm_act("search_book");
        this.carrot.show_search(frm_search,act_search,PlayerPrefs.GetString("search_tip","You can search for any biblical content here!"));
    }

    public void show_list_book1()
    {
        this.panel_view.Show(PlayerPrefs.GetString("book_1"), this.icon_book1);
        this.panel_view.style_nomal();
        for (int i = 0; i < this.arr_item_book1.Length; i++)
        {
        GameObject item_view = Instantiate(this.panel_view.prefab_item_view);
        item_view.transform.SetParent(this.panel_view.area_body);
        item_view.transform.localPosition = new Vector3(item_view.transform.localPosition.x, item_view.transform.localPosition.y, 0f);
        item_view.transform.localScale = new Vector3(1f, 1f, 1f);
        item_view.GetComponent<Item_view>().id_book = this.arr_item_book1[i].id_book;
        item_view.GetComponent<Item_view>().txt_name.text = this.arr_item_book1[i].txt_name.text;
        item_view.GetComponent<Item_view>().icon.sprite= this.arr_item_book1[i].icon.sprite;
        item_view.GetComponent<Item_view>().chapter = this.arr_item_book1[i].chapter;
        item_view.GetComponent<Item_view>().type =1;
        }
    }

    public void show_list_book2()
    {
        this.panel_view.Show(PlayerPrefs.GetString("book_2"), this.icon_book2);
        this.panel_view.style_nomal();
        for (int i = 0; i < this.arr_item_book2.Length; i++)
        {
        GameObject item_view = Instantiate(this.panel_view.prefab_item_view);
        item_view.transform.SetParent(this.panel_view.area_body);
        item_view.transform.localPosition = new Vector3(item_view.transform.localPosition.x, item_view.transform.localPosition.y, 0f);
        item_view.transform.localScale = new Vector3(1f, 1f, 1f);
        item_view.GetComponent<Item_view>().id_book = this.arr_item_book2[i].id_book;
        item_view.GetComponent<Item_view>().txt_name.text = this.arr_item_book2[i].txt_name.text;
        item_view.GetComponent<Item_view>().icon.sprite = this.arr_item_book2[i].icon.sprite;
        item_view.GetComponent<Item_view>().chapter = this.arr_item_book2[i].chapter;
        item_view.GetComponent<Item_view>().type =2;
        }
    }

    public void show_list_chapter(int num_chap,int type,string name_book,string id_book)
    {
        this.check_click_show_ads();
        Sprite icon_book = null;
        if (type == 1)
        {
        this.panel_view.Show(PlayerPrefs.GetString("book_1")+" - "+name_book, this.icon_book1);
        icon_book = this.icon_book1;
        }
        else
        {
        this.panel_view.Show(PlayerPrefs.GetString("book_2")+" - "+name_book, this.icon_book2);
        icon_book = this.icon_book2;
        }
        this.panel_view.style_nomal();

        for (int i=1;i<= num_chap; i++)
        {
        GameObject item_view = Instantiate(this.panel_view.prefab_item_view);
        item_view.transform.SetParent(this.panel_view.area_body);
        item_view.transform.localPosition = new Vector3(item_view.transform.localPosition.x, item_view.transform.localPosition.y, 0f);
        item_view.transform.localScale = new Vector3(1f, 1f, 1f);
        item_view.GetComponent<Item_view>().txt_name.text = PlayerPrefs.GetString("chapter", "chapter") + " " + i;
        item_view.GetComponent<Item_view>().data_text = name_book;
        item_view.GetComponent<Item_view>().icon.sprite = icon_book;
        item_view.GetComponent<Item_view>().type = type;
        item_view.GetComponent<Item_view>().id_book = id_book;
        item_view.GetComponent<Item_view>().chapter = i;
        item_view.GetComponent<Item_view>().num_chapter = num_chap;
        item_view.GetComponent<Item_view>().act = 1;
        }
    }

    private int cur_show_p_id_hightlight=-1;
    private int cur_show_p_type=-1;
    private string cur_show_p_id_book="";
    private string cur_show_p_name_book="";
    private int cur_show_p_chap=-1;
    private int cur_show_num_chap=-1;
    public void show_list_p(int id_hightlight,int type, string id_book,string name_book, int chap,int num_chap)
    {
        this.cur_show_p_id_hightlight=id_hightlight;
        this.cur_show_p_type=type;
        this.cur_show_p_id_book=id_book;
        this.cur_show_p_name_book=name_book;
        this.cur_show_p_chap=chap;
        this.cur_show_num_chap=num_chap;
        this.check_click_show_ads();
        WWWForm frm = this.carrot.frm_act("read_book");
        frm.AddField("id_book", id_book);
        frm.AddField("id_chapter", chap);
        this.carrot.send(frm,act_get_list_p);
    }

    private Item_image_p p_img_temp;
    private void act_get_list_p(string s_data)
    {
        IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data);
        IList all_item = (IList)data["list_data"];

        Sprite icon_book = null;
        string nam_view_book = "";
        if (this.cur_show_p_type == 1){
            nam_view_book = PlayerPrefs.GetString("book_1")+" - "+ this.cur_show_p_name_book + " - " + this.cur_show_num_chap;
            icon_book = this.icon_book1;
        }
        else
        {
            nam_view_book = PlayerPrefs.GetString("book_2") + " - " + this.cur_show_p_name_book + " - " + this.cur_show_num_chap;
            icon_book = this.icon_book2;
        }

        this.panel_view.Show(nam_view_book, icon_book);
        this.panel_view.style_read_work();
        this.panel_view.set_button_back_book(this.cur_show_p_id_book, this.cur_show_p_name_book, this.cur_show_p_type,this.cur_show_num_chap);

        if (data["image"].ToString()!="")
        {
            GameObject item_image = Instantiate(this.panel_view.prefab_item_image_p);
            item_image.transform.SetParent(this.panel_view.area_body);
            item_image.transform.localPosition = new Vector3(item_image.transform.localPosition.x, item_image.transform.localPosition.y, 0f);
            item_image.transform.localScale = new Vector3(1f, 1f, 1f);
            this.carrot.get_img(data["image"].ToString(),act_set_img_chap);
            this.p_img_temp=item_image.GetComponent<Item_image_p>();
        }

        for (int i = 0; i < all_item.Count; i++)
        {
            IDictionary item = (IDictionary)all_item[i];
            GameObject item_p= Instantiate(this.panel_view.prefab_item_p);
            item_p.transform.SetParent(this.panel_view.area_body);
            item_p.transform.localPosition = new Vector3(item_p.transform.localPosition.x, item_p.transform.localPosition.y, 0f);
            item_p.transform.localScale = new Vector3(1f, 1f, 1f);
            item_p.GetComponent<Item_view>().txt_name.text = item["name"].ToString();
            item_p.GetComponent<Item_view>().act = 2;
            item_p.GetComponent<Item_view>().id_p = i+1;
            item_p.GetComponent<Item_view>().id_chapter=item["id"].ToString();
            item_p.GetComponent<Item_view>().data_text = this.cur_show_p_name_book + " - " + PlayerPrefs.GetString("chapter")+":"+this.cur_show_num_chap+" ";
            this.panel_view.list_p.Add(item_p.GetComponent<Item_view>());
            if(this.cur_show_p_id_hightlight == i)item_p.GetComponent<Item_view>().click();
        }

        if (data["audio"].ToString()!=null)  this.panel_view.dowload_audio_view(data["audio"].ToString());
    }

    private void act_set_img_chap(Texture2D data_img){
        this.p_img_temp.img.sprite = this.carrot.Texture2DtoSprite(data_img);
        this.panel_view.set_img_chap(data_img.EncodeToPNG(),this.p_img_temp.img);
    }

    public void rate_app()
    {
        this.Sound_Click.Play();
        this.carrot.show_rate();
    }

    public void app_share()
    {
        this.Sound_Click.Play();
        this.carrot.show_share();
    }

    public void buy_product(int index){
        this.carrot.show_loading();
        this.Sound_Click.Play();
        this.carrot.buy_product(index);
    }


    public void buy_success(Product product)
    {
        this.onBuySuccessCarrotPay(product.definition.id);
    }

    private void onBuySuccessCarrotPay(string id_product)
    {
        if (id_product == this.carrot.shop.get_id_by_index(0))
        {
            this.carrot.show_msg(PlayerPrefs.GetString("remove_ads", "remove_ads"), PlayerPrefs.GetString("buy_success", "buy_success"), Carrot.Msg_Icon.Success);
            this.act_inapp_removeAds();
        }
    }

    private void onRestoreSuccessCarrotPay(string[] arr_id)
    {
        for(int i = 0; i < arr_id.Length; i++)
        {
            string s_id_product = arr_id[i];
            if (s_id_product == this.carrot.shop.get_id_by_index(0)) this.act_inapp_removeAds();
        }
    }

    private void act_inapp_removeAds()
    {
        PlayerPrefs.SetInt("is_buy_ads", 1);
        this.check_ads();
    }

    public void restore_product(){
        this.Sound_Click.Play();
        this.carrot.restore_product();
    }

    public void View_quote_home_item1()
    {
        string s_id_p=PlayerPrefs.GetString("p_of_day_id");
        string s_lang_p=PlayerPrefs.GetString("p_of_day_lang");
        this.panel_view_quocte.show_quote(this.txt_p_of_day.text, this.img_bk_home_item1,s_id_p,s_lang_p);
    }


    private void act_search(string s_data){
        IDictionary data = (IDictionary)Carrot.Json.Deserialize(s_data);
        IList all_item = (IList)data["list_data"];
        this.panel_view.style_nomal();
        this.carrot.show_list_box(this.carrot.inp_search.text,this.icon_search);
        for(int i = 0; i < all_item.Count; i++){
            IDictionary book = (IDictionary)all_item[i];
            GameObject item_book = Instantiate(this.panel_view.prefab_item_view);
            item_book.transform.SetParent(this.carrot.area_body_box);
            item_book.transform.localPosition = new Vector3(item_book.transform.localPosition.x, item_book.transform.localPosition.y, 0f);
            item_book.transform.localScale = new Vector3(1f, 1f, 1f);
            item_book.GetComponent<Item_view>().txt_name.text = book["name"].ToString();
            item_book.GetComponent<Item_view>().chapter = int.Parse(book["chapter"].ToString());
            item_book.GetComponent<Item_view>().id_book= book["id"].ToString();
            if (book["type"].ToString() == "0")
            {
                item_book.GetComponent<Item_view>().type = 1;
                item_book.GetComponent<Item_view>().icon.sprite = this.icon_book1;
            }
            else
            {
                item_book.GetComponent<Item_view>().type = 2;
                item_book.GetComponent<Item_view>().icon.sprite = this.icon_book2;
            }
        }

        IList all_p = (IList)data["list_p"];
        for(int i = 0; i < all_p.Count; i++)
        {
            IDictionary p = (IDictionary)all_p[i];
            GameObject item_p = Instantiate(this.panel_view.prefab_item_view);
            item_p.transform.SetParent(this.carrot.area_body_box);
            item_p.transform.localPosition = new Vector3(item_p.transform.localPosition.x, item_p.transform.localPosition.y, 0f);
            item_p.transform.localScale = new Vector3(1f, 1f, 1f);
            item_p.GetComponent<Item_view>().txt_name.text = p["name"].ToString();
            item_p.GetComponent<Item_view>().icon.sprite = this.icon_p;
            if (p["type"].ToString() == "0")
            {
                item_p.GetComponent<Item_view>().type = 1;
            }
            else
            {
                item_p.GetComponent<Item_view>().type = 2;
            }
            item_p.GetComponent<Item_view>().chapter = int.Parse(p["chapter"].ToString());
            item_p.GetComponent<Item_view>().num_chapter = int.Parse(p["num_chapter"].ToString());
            item_p.GetComponent<Item_view>().id_book = p["id_book"].ToString();
            item_p.GetComponent<Item_view>().id_p =int.Parse(p["id_p"].ToString());
            item_p.GetComponent<Item_view>().data_text = p["name_book"].ToString();
            item_p.GetComponent<Item_view>().act = 3;
        }
    }

    public void show_list_app_other(){
        this.Sound_Click.Play();
        this.carrot.show_list_carrot_app();
    }

    public void show_setting(){
        this.Sound_Click.Play();
        this.panel_setting.SetActive(true);
    }

    public void close_setting(){
        this.Sound_Click.Play();
        this.panel_setting.SetActive(false);
    }
}
