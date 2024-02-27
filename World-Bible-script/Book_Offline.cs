using Carrot;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Book_Offline : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

    [Header("Obj Offline Book")]
    public Sprite icon_offline_book;

    private int length = 0;
    private int index_del = -1;
    private Carrot_Window_Msg msg;

    public void On_load()
    {
        this.length = PlayerPrefs.GetInt("length_book", 0);
    }

    public void add(IDictionary data)
    {
        PlayerPrefs.SetString("book_" + this.length, Json.Serialize(data));
        this.length++;
        PlayerPrefs.SetInt("length_book", this.length);
        this.msg=this.bible.carrot.show_msg(PlayerPrefs.GetString("save","Storage"),PlayerPrefs.GetString("save_success", "Save the book successfully, you can read it in offline mode!"), Carrot.Msg_Icon.Success);
    }

    public void show()
    {
        this.bible.add_loading_item();
        this.bible.carrot.delay_function(1.2f, this.list);
    }

    public void get_and_save(IDictionary data)
    {
        this.bible.carrot.show_loading();
        this.add(data);
    }

    private void list()
    {
        this.bible.carrot.clear_contain(this.bible.tr_all_item_book);
        if (this.length > 0)
        {
            this.add_title();
            for (int i = this.length - 1; i >= 0; i--)
            {
                string s_data_book = PlayerPrefs.GetString("book_" + i, "");
                if (s_data_book != "")
                {
                    var index_item = i;
                    IDictionary data = (IDictionary) Carrot.Json.Deserialize(s_data_book);
                    data["index"] = i;
                    data["type_item"]="offline";
                    Carrot.Carrot_Box_Item item_book=this.bible.create_item();
                    item_book.set_icon(this.icon_offline_book);
                    item_book.set_title(data["name"].ToString());

                    if (data["contents"] != null)
                    {
                        IList contents = (IList) data["contents"];
                        item_book.set_tip(contents.Count + " Chapter");
                    }

                    Carrot.Carrot_Box_Btn_Item btn_del=item_book.create_item();
                    btn_del.set_icon(this.bible.carrot.sp_icon_del_data);
                    btn_del.set_color(this.bible.carrot.color_highlight);
                    btn_del.set_act(() => delete(index_item));

                    if (i % 2 == 0) item_book.GetComponent<Image>().color = this.bible.color_row_a;
                    else item_book.GetComponent<Image>().color = this.bible.color_row_b;

                    item_book.set_act(() => this.bible.book.View_book_by_data(data));
                }
            }
        }
        else
        {
            this.add_title();
            this.bible.add_none(false);
        }
    }

    private void add_title()
    {
        Carrot.Carrot_Box_Item item_title = this.bible.add_title(PlayerPrefs.GetString("save_list", "List of saved books"));
        item_title.set_icon(this.bible.book.icon_list);
        item_title.set_tip(PlayerPrefs.GetString("save_list_tip", "You can read these books without an internet connection"));
    }

    public void delete(int index)
    {
        this.index_del = index;
        this.msg=this.bible.carrot.show_msg("Delete", "Are you sure you want to remove this item?", this.delete_yes, this.delete_no);
    }

    private void delete_yes()
    {
        PlayerPrefs.DeleteKey("book_" + this.index_del);
        if (this.msg != null) this.msg.close();
        this.list();
    }

    private void delete_no()
    {
        if (this.msg != null) this.msg.close();
    }
}
