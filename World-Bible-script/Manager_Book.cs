using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager_Book : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

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
                        if (chapter["paragraphs"] != null)
                        {
                            IList paragraphs = (IList)chapter["paragraphs"];
                            item_chapter.set_tip(paragraphs.Count+ " paragraph");
                            item_chapter.set_act(() => this.view_paragraphs(chapter["name"].ToString(),paragraphs));
                        }
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

    private void view_paragraphs(string s_title,IList paragraphs)
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
}
