using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_offline : MonoBehaviour
{
    public Sprite icon;
    public GameObject Prefab_item_book_offline;
    public int length=0;
    void Start()
    {
        this.length = PlayerPrefs.GetInt("book_offline_length", 0);
    }

    public void save_book(string name,List<Item_view> list_p,byte[] data_img,byte[] data_audio)
    {
        PlayerPrefs.SetString("book_offline_name_" + this.length,name);
        PlayerPrefs.SetInt("book_offline_p_count" + this.length,list_p.Count);
        for (int i = 0; i < list_p.Count; i++)
        {
            PlayerPrefs.SetString("book_offline_p_" + this.length+"_"+i, list_p[i].txt_name.text);
            PlayerPrefs.SetString("book_offline_p_id_" + this.length+"_"+i, list_p[i].id_chapter);
        }

        if (data_img!=null)
        {
            this.GetComponent<Bible>().carrot.save_file("book_offline_"+this.length+".png", data_img);
        }

        if (data_audio!=null)
        {
            this.GetComponent<Tools>().save_audio("book_offline_" + this.length + ".mp3", data_audio);
        }
        this.GetComponent<Bible>().carrot.show_msg(PlayerPrefs.GetString("book_offline", "Book Offline"), PlayerPrefs.GetString("save_book_success", "save_book_success"),Carrot.Msg_Icon.Success);
        this.length++;
        PlayerPrefs.SetInt("book_offline_length", this.length);
    }


    public void show_list_book_offline()
    {
        if (this.length > 0)
        {
            this.GetComponent<Bible>().panel_view.Show(PlayerPrefs.GetString("book_offline"), this.icon);
            this.GetComponent<Bible>().panel_view.style_nomal();

            bool is_num_book = false;
            for (int i = 0; i < this.length; i++)
            {
                if (PlayerPrefs.GetString("book_offline_name_" + i, "") != "")
                {
                    is_num_book = true;
                    GameObject item_view = Instantiate(this.Prefab_item_book_offline);
                    item_view.transform.SetParent(this.GetComponent<Bible>().panel_view.area_body);
                    item_view.transform.localPosition = new Vector3(item_view.transform.localPosition.x, item_view.transform.localPosition.y, 0f);
                    item_view.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_view.GetComponent<Item_book_offline>().index = i;
                    item_view.GetComponent<Item_book_offline>().txt_name.text = PlayerPrefs.GetString("book_offline_name_" + i);
                    item_view.GetComponent<Item_book_offline>().is_home=false;
                }
            }

            if (is_num_book == false)
            {
                this.length = 0;
                PlayerPrefs.DeleteKey("book_offline_length");
            }
        }
        else
        {
            this.GetComponent<Bible>().carrot.show_msg(PlayerPrefs.GetString("book_offline", "Book Offline"), PlayerPrefs.GetString("book_offline_none", "book_offline_none"), Carrot.Msg_Icon.Alert);
        }
    }

    public void show_book_offline_contain(int index)
    {
        string s_name_book = PlayerPrefs.GetString("book_offline_name_" + index);
        this.GetComponent<Bible>().panel_view.Show(s_name_book, this.icon);
        this.GetComponent<Bible>().panel_view.style_read_work();
        this.GetComponent<Bible>().panel_view.button_save_book.SetActive(false);


        GameObject item_image = Instantiate(this.GetComponent<Bible>().panel_view.prefab_item_image_p);
        item_image.transform.SetParent(this.GetComponent<Bible>().panel_view.area_body);
        item_image.transform.localPosition = new Vector3(item_image.transform.localPosition.x, item_image.transform.localPosition.y, 0f);
        item_image.transform.localScale = new Vector3(1f, 1f, 1f);
        this.GetComponent<Bible>().carrot.load_file_img("book_offline_" + index + ".png",item_image.GetComponent<Item_image_p>().img);

        for (int i = 0; i < PlayerPrefs.GetInt("book_offline_p_count" +index); i++)
        {
            GameObject item_p = Instantiate(this.GetComponent<Bible>().panel_view.prefab_item_p);
            item_p.transform.SetParent(this.GetComponent<Bible>().panel_view.area_body);
            item_p.transform.localPosition = new Vector3(item_p.transform.localPosition.x, item_p.transform.localPosition.y, 0f);
            item_p.transform.localScale = new Vector3(1f, 1f, 1f);
            item_p.GetComponent<Item_view>().txt_name.text = PlayerPrefs.GetString("book_offline_p_" + index+"_"+i);
            item_p.GetComponent<Item_view>().act = 2;
            item_p.GetComponent<Item_view>().id_p = i + 1;
            item_p.GetComponent<Item_view>().data_text = s_name_book + " - " + PlayerPrefs.GetString("chapter");
            item_p.GetComponent<Item_view>().id_chapter = PlayerPrefs.GetString("book_offline_p_id_" + index+"_"+i);
        }

        this.GetComponent<Bible>().panel_view.dowload_audio_view(this.GetComponent<Tools>().get_url_audio("book_offline_" + index + ".mp3"));
    }

    public void delete(int index,bool is_home)
    {
        PlayerPrefs.DeleteKey("book_offline_name_"+index);
        for (int i = 0; i < PlayerPrefs.GetInt("book_offline_p_count"+index,0); i++)
        {
            PlayerPrefs.DeleteKey("book_offline_p_" + index+"_"+i);
            PlayerPrefs.DeleteKey("book_offline_p_id_" + index+"_"+i);
        }
        PlayerPrefs.DeleteKey("book_offline_p_count"+index);
        this.GetComponent<Tools>().delete_file("book_offline_" + index + ".png");
        this.GetComponent<Tools>().delete_file("book_offline_" + index + ".mp3");

        if(is_home)
        this.show_list_offline_in_home();
        else
        this.show_list_book_offline();
    }

    public void show_list_offline_in_home(){
        this.length = PlayerPrefs.GetInt("book_offline_length", 0);
        if (this.length > 0)
        {
            foreach(Transform child in this.GetComponent<Bible>().area_main_body_contain){
                if(child.gameObject.name=="book_offline") Destroy(child.gameObject);
            }
            for (int i = 0; i < this.length; i++)
            {
                if (PlayerPrefs.GetString("book_offline_name_" + i, "") != "")
                {
                    GameObject item_view = Instantiate(this.Prefab_item_book_offline);
                    item_view.transform.SetParent(this.GetComponent<Bible>().area_main_body_contain);
                    item_view.transform.localPosition = new Vector3(item_view.transform.localPosition.x, item_view.transform.localPosition.y, 0f);
                    item_view.transform.localScale = new Vector3(1f, 1f, 1f);
                    item_view.GetComponent<Item_book_offline>().index = i;
                    item_view.GetComponent<Item_book_offline>().txt_name.text = PlayerPrefs.GetString("book_offline_name_" + i);
                    item_view.GetComponent<Item_book_offline>().is_home=true;
                    item_view.GetComponent<Item_book_offline>().name="book_offline";
                }
            }
        }
    }
}
