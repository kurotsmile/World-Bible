using Carrot;
using System.Collections;
using UnityEngine;

public class Manager_Search : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

    [Header("Obj Search")]
    public Sprite icon_search_return;
    private Carrot_Window_Input window_search_obj = null;

    public void show_search()
    {
        this.bible.ads.show_ads_Interstitial();
        this.window_search_obj=this.bible.carrot.show_search(Submit_search, this.bible.carrot.L("search_tip", "You can search for any biblical content here!"));
    }

    public void list()
    {
        this.bible.carrot.clear_contain(this.bible.tr_all_item_book);
        Carrot.Carrot_Box_Item item_inp_search=this.bible.Create_item();
        item_inp_search.set_icon(this.bible.icon_search);
        item_inp_search.set_type(Carrot.Box_Item_Type.box_value_input);
        item_inp_search.check_type();
        item_inp_search.set_title(this.bible.carrot.L("search", "Search"));
        item_inp_search.set_tip(this.bible.carrot.L("search_tip", "You can search for any biblical content here!"));
        item_inp_search.inp_val.onSubmit.RemoveAllListeners();
        item_inp_search.inp_val.onSubmit.AddListener(this.Submit_search);
    }

    private void Submit_search(string s_key)
    {
        this.bible.Add_loading_item();
        string s_data = PlayerPrefs.GetString("data_bible_" +bible.carrot.lang.Get_key_lang());
        if (s_data != "")
        {
            Fire_Collection fc = new(s_data);
            if (!fc.is_null)
            {
                if (this.window_search_obj != null) this.window_search_obj.close();
                this.bible.menu.Select_Menu_No_func(2);
                this.bible.carrot.clear_contain(this.bible.tr_all_item_book);
                Carrot_Box_Item item_search_results = bible.Create_item();
                item_search_results.set_icon(bible.icon_search);
                item_search_results.set_title(this.bible.carrot.L("search_results","Search Results"));
                item_search_results.set_tip(s_key);

                Carrot_Box_Btn_Item btn_clear = item_search_results.create_item();
                btn_clear.set_icon(bible.carrot.sp_icon_del_data);
                btn_clear.set_icon_color(Color.white);
                btn_clear.set_color(bible.carrot.color_highlight);
                btn_clear.set_act(() => this.list());

                int count_found = 0;

                for (int i = 0; i < fc.fire_document.Length; i++)
                {
                    IDictionary data = fc.fire_document[i].Get_IDictionary();
                    if (data["name"].ToString().Contains(s_key))
                    {
                        data["title"] = data["name"].ToString();
                        data["tip"] = this.bible.carrot.L("book", "Book")+" (" + data["name"].ToString() + ")";
                        data["type_search"] = "book";
                        this.Add_item_search(data);
                        count_found++;
                    }

                    IList contents = (IList)data["contents"];
                    for (int y = 0; y < contents.Count; y++)
                    {
                        IDictionary chapter = (IDictionary)contents[y];
                        chapter["index"] = y;
                        IList paragraphs = (IList)chapter["paragraphs"];
                        for (int z = 0; z < paragraphs.Count; z++)
                        {
                            if (paragraphs[z].ToString().Contains(s_key))
                            {
                                data["title"] = paragraphs[z].ToString();
                                data["tip"] = this.bible.carrot.L("book","Book")+" ("+data["name"].ToString()+") -> "+this.bible.carrot.L("chapter", "Chapter")+" : "+(y+1)+" -> "+ this.bible.carrot.L("paragraph", "Paragraph")+" ("+ (z+1)+")";
                                data["type_search"] = "paragraph";
                                data["data_chapter"] = chapter;
                                this.Add_item_search(data);
                                count_found++;
                            }
                        }
                    }
                }

                if (count_found == 0)
                {
                    bible.add_none();
                }
                else
                {
                    item_search_results.set_tip(s_key+" ("+count_found+")");
                }
                
            }
            else
            {
                bible.carrot.Show_msg("No data");
            }
        }
        else
        {
            bible.carrot.Show_msg("No data");
        }
    }

    private void Add_item_search(IDictionary data)
    {
        var id_book = data["id"].ToString();
        Carrot_Box_Item item_book = bible.Create_item();
        item_book.set_title(data["title"].ToString());
        item_book.set_tip(data["tip"].ToString());

        string s_type_book = data["type"].ToString();
        if (s_type_book == "old_testament")
            item_book.set_icon_white(bible.icon_book_old_testament);
        else
            item_book.set_icon_white(bible.icon_book_new_Testament);

        string s_type_search = data["type_search"].ToString();

        if (s_type_book == "book")
        {
            item_book.set_act(() => bible.book.View(data,-1));
        }
        else
        {
            IDictionary data_paragraphs=(IDictionary)data["data_chapter"];
            bible.book.Set_data_book_cur(data);
            item_book.set_act(() => bible.book.View_paragraphs_page(data_paragraphs));
        }

        item_book.gameObject.name = id_book;
        Carrot_Box_Btn_Item btn_save = item_book.create_item();
        btn_save.set_icon(bible.icon_book_save);
        btn_save.set_icon_color(Color.white);
        btn_save.set_color(bible.carrot.color_highlight);
        btn_save.set_act(() => bible.offline.Add(data));

    }
}
