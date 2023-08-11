using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Panel_view : MonoBehaviour {

    public Text Title;
    public Image Icon;
    public Transform area_body;
    public GameObject prefab_item_view;
    public GameObject prefab_item_p;
    public GameObject prefab_item_image_p;

    public GameObject button_speech;
    public GameObject button_back_book;
    public GameObject button_save_book;
    public AudioSource sound_speed;

    private string id_book;
    private int id_chapter=-1;
    private int num_chapter;
    private int type_book;
    private string name_book;

    public Sprite icon_audio_play;
    public Sprite icon_audio_stop;
    public Image Img_btn_audio_read;
    public Image Img_btn_back_book;

    public GameObject panel_bottom_button_view_p;
    public Text btn_view_show_p_txt;
    private Image img_chap;
    private byte[] data_img_chap;
    private byte[] data_audio_chap;

    List<Item_view> list_p_select=new List<Item_view>();
    public List<Item_view> list_p = new List<Item_view>();

    public void Show(string title,Sprite icon)
    {
        this.data_audio_chap = null;
        this.data_img_chap = null;
        this.id_chapter =- 1;
        GameObject.Find("Bible").GetComponent<Bible>().Sound_Click.Play();
        this.Title.text = title;
        this.Icon.sprite = icon;
        this.gameObject.SetActive(true);
        this.button_speech.SetActive(false);
        this.button_back_book.SetActive(false);
        this.button_save_book.SetActive(false);
        this.panel_bottom_button_view_p.SetActive(false);
        GameObject.Find("Bible").GetComponent<Bible>().carrot.clear_contain(this.area_body);
    }

    public void Close()
    {
        this.close_button_view_p();
        GameObject.Find("Bible").GetComponent<Bible>().carrot.ads.show_ads_Interstitial();
        this.check_and_stop_audio();
        GameObject.Find("Bible").GetComponent<Bible>().Sound_Click.Play();
        this.gameObject.SetActive(false);
    }

    private void check_and_stop_audio()
    {
        this.StopAllCoroutines();
        if (this.sound_speed.clip != null)
        {
            if (this.sound_speed.isPlaying)
            {
                this.sound_speed.Stop();
            }
        }
    }

    public void style_read_work()
    {
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.left = 10;
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.right = 10;
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.top = 30;
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.bottom = 30;
        this.area_body.GetComponent<VerticalLayoutGroup>().spacing = 10;
        this.area_body.GetComponent<VerticalLayoutGroup>().childControlHeight = true;
        this.area_body.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = true;
        this.list_p.Clear();
        this.button_save_book.SetActive(true);
    }

    public void style_nomal()
    {
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.left = 3;
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.right = 3;
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.top = 3;
        this.area_body.GetComponent<VerticalLayoutGroup>().padding.bottom = 3;
        this.area_body.GetComponent<VerticalLayoutGroup>().spacing =1;
        this.area_body.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
        this.area_body.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
    }


    public void dowload_audio_view(string url_audio)
    {
        if (url_audio != "")
        {
            GameObject.Find("Bible").GetComponent<Bible>().carrot.get_mp3(url_audio,act_downloadAudio);
        }
    }

    private void act_downloadAudio(UnityWebRequest www)
    {
        this.data_audio_chap = www.downloadHandler.data;
        sound_speed.clip = DownloadHandlerAudioClip.GetContent(www);
        this.Img_btn_audio_read.sprite = this.icon_audio_play;
        this.button_speech.SetActive(true);
    }

    public void Play_read_audio()
    {
        if (sound_speed.clip != null)
        {
            if (this.sound_speed.isPlaying)
            {
                this.sound_speed.Pause();
                this.Img_btn_audio_read.sprite =  this.icon_audio_play;
            }
            else
            {
                this.sound_speed.Play();
                this.Img_btn_audio_read.sprite = this.icon_audio_stop;
            }
        }
    }

    public void set_highlight(int index)
    {
        this.id_chapter = index;
    }

    public void set_button_back_book(string id_book,string name_book,int type,int number_chapter)
    {
        this.id_book = id_book;
        this.type_book = type;
        this.num_chapter = number_chapter;
        this.name_book = name_book;
        this.button_back_book.SetActive(true);
        if (this.type_book == 1)
        {
            this.Img_btn_back_book.sprite = GameObject.Find("Bible").GetComponent<Bible>().icon_book1;
        }

        if (this.type_book == 2)
        {
            this.Img_btn_back_book.sprite = GameObject.Find("Bible").GetComponent<Bible>().icon_book2;
        }
    }

    public void back_book()
    {
        this.check_and_stop_audio();
        this.close_button_view_p();
        GameObject.Find("Bible").GetComponent<Bible>().show_list_chapter(this.num_chapter, this.type_book,this.name_book, this.id_book);
    }

    public void show_button_view_p(Item_view item_v)
    {
        this.list_p_select.Add(item_v);
        this.btn_view_show_p_txt.text = PlayerPrefs.GetString("show_quote","Shows") +" (" + this.list_p_select.Count + ")";
        this.panel_bottom_button_view_p.SetActive(true);
    }

    public void close_button_view_p()
    {
        for (int i = 0; i < this.list_p_select.Count; i++)
        {
            this.list_p_select[i].txt_name.color = Color.white;
        }
        this.list_p_select.Clear();
        this.panel_bottom_button_view_p.SetActive(false);
    }

    public void show_list_select_p()
    {
        string s_txt_show = "";
        string s_txt_chap = this.list_p_select[0].data_text;
        for (int i = 0; i < this.list_p_select.Count; i++)
        {
            s_txt_show+=this.list_p_select[i].txt_name.text + "\n";
        }
        if (this.list_p_select.Count==1)
        {
            s_txt_chap += " (" + this.list_p_select[0].id_p + ")";
        }
        else
        {
            s_txt_chap += " (" + this.list_p_select[0].id_p +","+ this.list_p_select[this.list_p_select.Count-1].id_p + ")";
        }

        
        if (this.data_img_chap == null)
        {
            GameObject.Find("Bible").GetComponent<Bible>().panel_view_quocte.show_quote_in_view_p(s_txt_show, s_txt_chap, GameObject.Find("Bible").GetComponent<Bible>().img_bk_home_item1,this.list_p_select[0].id_chapter,"vi");
        }
        else
        {
            GameObject.Find("Bible").GetComponent<Bible>().panel_view_quocte.show_quote_in_view_p(s_txt_show, s_txt_chap,img_chap,this.list_p_select[0].id_chapter,"vi");
        }
        this.gameObject.SetActive(false);
    }

    public void close_slide_list_select_p()
    {
        this.gameObject.SetActive(true);
    }

    public void save_book()
    {
        if (this.data_img_chap != null)
        {
            GameObject.Find("Bible").GetComponent<Book_offline>().save_book(this.Title.text, this.list_p, this.data_img_chap, this.data_audio_chap);
        }
        else
        {
            GameObject.Find("Bible").GetComponent<Book_offline>().save_book(this.Title.text, this.list_p, null, this.data_audio_chap);
        }
    }

    public void set_img_chap(byte[] sp_data,Image sp_chap){
        this.img_chap=sp_chap;
        this.data_img_chap=sp_data;
    }

 }
