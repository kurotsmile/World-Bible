using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Book : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;
    private TextMeshProUGUI textPro;

    public void view(string id)
    {
        this.bible.carrot.play_sound_click();
        this.bible.carrot.ads.show_ads_Interstitial();
        this.bible.carrot.show_loading();

        this.bible.carrot.db.Collection("bible").Document(id).GetSnapshotAsync().ContinueWithOnMainThread(task=>{

            DocumentSnapshot book=task.Result;

            if (task.IsCompleted)
            {
                this.bible.carrot.hide_loading();
                if (book.Exists)
                {
                    IDictionary data = book.ToDictionary();
                    Carrot.Carrot_Box box_list_chapter = this.bible.carrot.Create_Box();
                    if (data["type"] != null)
                    {
                        string s_type_book = data["type"].ToString();
                        if(s_type_book== "old_testament")
                            box_list_chapter.set_icon_white(this.bible.icon_book_old_testament);
                        else
                            box_list_chapter.set_icon_white(this.bible.icon_book_new_Testament);
                    }
                    else
                    {
                        box_list_chapter.set_icon_white(this.bible.icon_book_new_Testament);
                    }

                    box_list_chapter.set_title(data["name"].ToString());

                    IList contents = (IList)data["contents"];
                    foreach(IDictionary chapter in contents)
                    {
                        Carrot.Carrot_Box_Item item_chapter=box_list_chapter.create_item();
                        item_chapter.set_icon_white(this.bible.icon_chapter);
                        if(chapter["name"] !=null) item_chapter.set_title(chapter["name"].ToString());
                        IList paragraphs = (IList)chapter["paragraphs"];

                        if (chapter["paragraphs"] != null)
                        {
                            item_chapter.set_tip(paragraphs.Count+ " paragraph");
                            item_chapter.set_act(() => this.view_paragraphs_list(chapter["name"].ToString(),paragraphs));
                        }

                        Carrot.Carrot_Box_Btn_Item btn_page=item_chapter.create_item();
                        btn_page.set_icon(this.bible.icon_book_open);
                        btn_page.set_act(() => this.view_paragraphs_page(chapter["name"].ToString(), paragraphs));
                        btn_page.set_color(this.bible.carrot.color_highlight);
                    }
                }
            }

            if (task.IsFaulted)
            {
                this.bible.carrot.hide_loading();
                this.bible.carrot.show_msg("Bible", "Operation error, please try again next time!", Carrot.Msg_Icon.Error);
            }
        });
    }

    private void view_paragraphs_list(string s_title,IList paragraphs)
    {
        this.bible.carrot.play_sound_click();
        Carrot.Carrot_Box box_paragraphs = this.bible.carrot.Create_Box();
        box_paragraphs.set_icon_white(this.bible.icon_chapter);
        box_paragraphs.set_title(s_title);

        for (int i = 0; i < paragraphs.Count; i++)
        {
            string s_paragraph = paragraphs[i].ToString();
            Carrot.Carrot_Box_Item item_p=box_paragraphs.create_item("p_" + i);
            item_p.set_title(s_paragraph);
            item_p.set_tip("sentence "+(i+1).ToString());
            item_p.set_icon(this.bible.icon_paragraph);
        }
    }

    private void view_paragraphs_page(string s_title, IList paragraphs)
    {
        this.bible.carrot.play_sound_click();
        Carrot.Carrot_Box box_paragraphs = this.bible.carrot.Create_Box();
        box_paragraphs.set_icon_white(this.bible.icon_chapter);
        box_paragraphs.set_title(s_title);

        string s_page = "";
        for (int i = 0; i < paragraphs.Count; i++)
        {
            s_page= s_page+paragraphs[i].ToString()+" ";
        }

        GameObject obj_txt = Instantiate(this.bible.prefab_paragraph_item);
        this.textPro = obj_txt.GetComponent<TextMeshProUGUI>();
        this.textPro.text = s_page;

        obj_txt.transform.SetParent(box_paragraphs.area_all_item);
        obj_txt.transform.localPosition = new Vector3(0f, 0f, 0f);
        obj_txt.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_txt.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        this.bible.carrot.delay_function(1f, this.fix_size_paragraph);
    }

    private void fix_size_paragraph()
    {
        this.textPro.overflowMode = TextOverflowModes.Masking;
    }
}
