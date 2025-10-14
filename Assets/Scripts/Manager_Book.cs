using Carrot;
using SimpleFileBrowser;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Manager_Book : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

    [Header("Obj Book")]
    public Sprite icon_list;
    public IList list_data_Bible;
    public AudioSource audioSpeaker;
    private TextMeshProUGUI textPro;

    private IDictionary data_book_cur = null;
    private IDictionary data_chapter_cur = null;
    private Carrot_Box box_paragraphs_view = null;
    private Carrot_Box BoxChapterView = null;
    private Carrot_Window_Msg BoxMsg = null;
    private Carrot_Window_Input BoxInp = null;
    private bool type_view_page = false;
    private int IndexBookEdit = -1;
    private int index_show_chapter = -1;
    private string s_path_data = "";
    private bool is_editor = false;
    private bool isChange = false;

    public void OnLoad()
    {
        if (PlayerPrefs.GetString("path_data", "") != "")
        {
            s_path_data = PlayerPrefs.GetString("path_data");
            this.is_editor = true;
        }
        else
        {
            s_path_data = System.IO.Path.Combine(Application.dataPath, "Resources");
            this.is_editor = false;
        }

       
        string PathFileData = this.GetPathData() + "/bible_" + bible.carrot.lang.Get_key_lang() + ".json";
        if (IsEditor())
        {
            if (FileBrowserHelpers.FileExists(PathFileData))
            {
                string sData = FileBrowserHelpers.ReadTextFromFile(PathFileData);
                Load_list_by_data(sData);
            }
            else
            {
                TextAsset bible_data_text = Resources.Load<TextAsset>("bible_" + bible.carrot.lang.Get_key_lang());
                if (bible_data_text == null)
                    Load_list_by_data("");
                else
                    Load_list_by_data(bible_data_text.text);
            }
            Debug.Log("Path data Lang: " +PathFileData);
        }
        else
        {
            TextAsset bible_data_text = Resources.Load<TextAsset>("bible_" + bible.carrot.lang.Get_key_lang());
            if (bible_data_text != null)
                Load_list_by_data(bible_data_text.text);
            else
                Load_list_by_data("[]");
        }
    }

    public void Load_list_by_data(string s_data)
    {
        if (s_data == "")
            this.list_data_Bible = Json.Deserialize("[]") as IList;
        else
            this.list_data_Bible = Json.Deserialize(s_data) as IList;
        this.list_data_Bible = bible.SortListByOrderKey(this.list_data_Bible);
    }

    public int GetLengthBibleByType(string sType = "old_testament")
    {
        int count = 0;
        for (int i = 0; i < bible.book.list_data_Bible.Count; i++)
        {
            IDictionary data = bible.book.list_data_Bible[i] as IDictionary;
            if (data["type"].ToString() == sType) count++;
        }
        return count;
    }

    public void ShowList(string sType = "")
    {
        bible.carrot.clear_contain(bible.tr_all_item_book);
        IList list_book_Old_testament = (IList)Json.Deserialize("[]");
        Carrot_Box_Item item_Bible_Old = bible.add_title(bible.carrot.L("old_testament", "Old testament"));
        item_Bible_Old.set_icon_white(bible.icon_book_old_testament);
        item_Bible_Old.set_tip("Old testament");
        item_Bible_Old.GetComponent<Image>().color = bible.color_row_title_a;

        IList list_book_New_testament = (IList)Json.Deserialize("[]");
        Carrot_Box_Item item_Bible_New = bible.add_title(bible.carrot.L("new_testament", "New Testament"));
        item_Bible_New.set_icon_white(bible.icon_book_new_Testament);
        item_Bible_New.set_tip("New Testament");
        item_Bible_New.GetComponent<Image>().color = bible.color_row_title_b;

        int index_item = 0;
        for (int i = 0; i < bible.book.list_data_Bible.Count; i++)
        {
            var index_data = i;
            IDictionary data = bible.book.list_data_Bible[i] as IDictionary;
            var bookData = data;
            data["index_data"] = index_data;
            string s_type_book = "";
            if (data["type"] != null)
            {
                s_type_book = data["type"].ToString();
                if (s_type_book == "old_testament")
                    list_book_Old_testament.Add(data);
                else
                    list_book_New_testament.Add(data);
            }

            if (sType != "")
            {
                if (s_type_book != sType) continue;
            }

            Carrot_Box_Item item_book = bible.book.Item_book(data);
            item_book.set_act(() => {
                IDictionary dataHistory=Json.Deserialize("{}") as IDictionary;
                dataHistory["name"] = bookData["name"].ToString();
                dataHistory["date"] = DateTime.Now.ToString();
                dataHistory["index"] = index_data.ToString();
                bible.offline.AddHistory(dataHistory);
                bible.book.View(data, index_data);
            });
            if (index_item % 2 == 0)
                item_book.gameObject.GetComponent<Image>().color = bible.color_row_a;
            else
                item_book.gameObject.GetComponent<Image>().color = bible.color_row_b;

            item_book.gameObject.name = s_type_book;

            Carrot_Box_Btn_Item btn_save = item_book.create_item();
            btn_save.set_icon(bible.icon_book_save);
            btn_save.set_icon_color(Color.white);
            btn_save.set_color(bible.carrot.color_highlight);
            btn_save.set_act(() => bible.offline.Add(data));

            if (bible.book.IsEditor())
            {
                Carrot_Box_Btn_Item btn_edit = item_book.create_item();
                btn_edit.set_icon(bible.carrot.user.icon_user_edit);
                btn_edit.set_icon_color(Color.white);
                btn_edit.set_color(bible.carrot.color_highlight);
                btn_edit.set_act(() =>
                {
                    bible.carrot.play_sound_click();
                    bible.book.ShowAddBook(bookData);
                });

                Carrot_Box_Btn_Item btn_del = item_book.create_item();
                btn_del.set_icon(bible.carrot.sp_icon_del_data);
                btn_del.set_icon_color(Color.white);
                btn_del.set_color(Color.red);
                btn_del.set_act(() =>
                {
                    bible.carrot.Show_msg("Delete Book", "Are you sure you want to delete this '" + data["name"].ToString() + "' book?", () =>
                    {
                        bible.book.DeleteEbook(index_data);
                        bible.Act_load("");
                    });
                });
            }

            index_item++;
        }

        if (bible.book.IsEditor())
        {
            Carrot_Box_Item item_add_book = bible.Create_item();
            item_add_book.set_icon_white(bible.carrot.icon_carrot_add);
            item_add_book.set_title("Add Book");
            item_add_book.set_tip("Add new a Book");
            item_add_book.txt_tip.color = Color.black;
            item_add_book.gameObject.GetComponent<Image>().color = bible.carrot.color_highlight;
            item_add_book.set_act(() =>
            {
                bible.carrot.play_sound_click();
                bible.book.ShowAddBook();
            });
        }

        item_Bible_Old.set_tip(list_book_Old_testament.Count + " " + bible.carrot.L("book", "Book"));
        Carrot_Box_Btn_Item btn_list_old = item_Bible_Old.create_item();
        btn_list_old.set_icon(bible.book.icon_list);
        Destroy(btn_list_old.GetComponent<Button>());
        item_Bible_Old.set_act(() => bible.book.ShowList("old_testament"));

        item_Bible_New.set_tip(list_book_New_testament.Count + " " + bible.carrot.L("book", "Book"));
        Carrot_Box_Btn_Item btn_list_new = item_Bible_New.create_item();
        btn_list_new.set_icon(bible.book.icon_list);
        Destroy(btn_list_new.GetComponent<Button>());
        item_Bible_New.set_act(() => bible.book.ShowList("new_testament"));

    }

    public string GetPathData()
    {
        return this.s_path_data;
    }

    public void SetPathData(string s_path_new)
    {
        this.s_path_data = s_path_new;
        this.is_editor = true;
    }

    public bool IsEditor()
    {
        return this.is_editor;
    }

    public void View(IDictionary data, int index)
    {
        this.IndexBookEdit = index;
        bible.carrot.play_sound_click();
        data["type_item"] = "online";
        View_book_by_data(data);
    }

    public void View_book_by_data(IDictionary data)
    {
        data_book_cur = data;
        isChange = false;
        if (this.BoxChapterView != null) this.BoxChapterView.close();
        this.BoxChapterView = bible.carrot.Create_Box();

        if (data["type"] != null)
        {
            string s_type_book = data["type"].ToString();
            if (s_type_book == "old_testament")
                this.BoxChapterView.set_icon_white(bible.icon_book_old_testament);
            else
                this.BoxChapterView.set_icon_white(bible.icon_book_new_Testament);
        }
        else
        {
            this.BoxChapterView.set_icon_white(bible.icon_book_new_Testament);
        }

        this.BoxChapterView.set_title(data["name"].ToString());
        if (this.is_editor)
        {
            this.BoxChapterView.create_btn_menu_header(this.bible.carrot.icon_carrot_add).set_act(() =>
            {
                if (this.data_book_cur["contents"] == null)
                {
                    IList list_C = Json.Deserialize("[]") as IList;
                    IDictionary chapter = Json.Deserialize("{}") as IDictionary;
                    IList paragraphs = Json.Deserialize("[]") as IList;
                    chapter["name"] = this.data_book_cur["name"].ToString() + (list_C.Count + 1);
                    chapter["paragraphs"] = paragraphs;
                    list_C.Add(chapter);
                    this.data_book_cur["contents"] = list_C;
                    this.list_data_Bible[this.IndexBookEdit] = this.data_book_cur;
                    this.UpdateDataFile();
                    this.View_book_by_data(this.data_book_cur);
                }
                else
                {
                    IList list_C = this.data_book_cur["contents"] as IList;
                    IDictionary chapter = Json.Deserialize("{}") as IDictionary;
                    IList paragraphs = Json.Deserialize("[]") as IList;
                    chapter["name"] = this.data_book_cur["name"].ToString() + (list_C.Count + 1);
                    chapter["paragraphs"] = paragraphs;
                    list_C.Add(chapter);
                    this.data_book_cur["contents"] = list_C;
                    this.list_data_Bible[IndexBookEdit] = this.data_book_cur;
                    this.UpdateDataFile();
                    this.View_book_by_data(this.data_book_cur);
                }
                this.bible.carrot.play_sound_click();
                this.isChange = true;
                 bible.carrot.delay_function(1.2f, () => this.BoxChapterView.UI.scrollRect.verticalNormalizedPosition = 0f);
            });

            this.BoxChapterView.set_act_before_closing(() =>
            {
                if (this.isChange) this.bible.Act_load("");
            });
        }

        if (data["contents"] != null)
        {
            IList contents = (IList)data["contents"];
            for (int i = 0; i < contents.Count; i++)
            {
                var index_p = i;
                IDictionary chapter = (IDictionary)contents[i];
                var dataChapter = chapter;
                var nameChapter = chapter["name"].ToString();
                chapter["index"] = i;
                Carrot_Box_Item item_chapter = this.BoxChapterView.create_item();
                item_chapter.set_icon_white(bible.icon_chapter);
                if (chapter["name"] != null) item_chapter.set_title(chapter["name"].ToString());
                IList paragraphs = (IList)chapter["paragraphs"];

                if (chapter["paragraphs"] != null)
                {
                    item_chapter.set_tip(paragraphs.Count + " " + this.bible.carrot.L("paragraph", "Paragraph"));
                    item_chapter.set_act(() => View_paragraphs_page(chapter));
                }

                Carrot.Carrot_Box_Btn_Item btn_page = item_chapter.create_item();
                btn_page.set_icon(bible.icon_book_open);
                btn_page.set_act(() => View_paragraphs_page(chapter));
                btn_page.set_color(bible.carrot.color_highlight);

                Carrot.Carrot_Box_Btn_Item btn_list = item_chapter.create_item();
                btn_list.set_icon(icon_list);
                btn_list.set_act(() => View_paragraphs_list(chapter));
                btn_list.set_color(bible.carrot.color_highlight);

                if (this.IsEditor())
                {

                    Carrot_Box_Btn_Item btn_rename = item_chapter.create_item();
                    btn_rename.set_icon(this.bible.carrot.icon_carrot_write);
                    btn_rename.set_icon_color(Color.white);
                    btn_rename.set_color(this.bible.carrot.color_highlight);
                    btn_rename.set_act(() =>
                    {
                        this.bible.carrot.play_sound_click();
                        this.BoxInp = this.bible.carrot.Show_input("Rename the chapter '" + nameChapter + "'", "Enter new name");
                        this.BoxInp.set_act_done((s_new_name) =>
                        {
                            dataChapter["name"] = s_new_name;
                            item_chapter.txt_name.text = s_new_name;
                            this.BoxInp.close();
                            this.UpdateDataFile();
                        });
                    });

                    Carrot_Box_Btn_Item btn_edit = item_chapter.create_item();
                    btn_edit.set_icon(this.bible.carrot.user.icon_user_edit);
                    btn_edit.set_icon_color(Color.white);
                    btn_edit.set_color(this.bible.carrot.color_highlight);
                    btn_edit.set_act(() =>
                    {
                        this.bible.carrot.play_sound_click();
                        this.BoxEditOrAddParagraph(dataChapter, index_p);
                    });

                    Carrot_Box_Btn_Item btn_del = item_chapter.create_item();
                    btn_del.set_icon(this.bible.carrot.sp_icon_del_data);
                    btn_del.set_icon_color(Color.white);
                    btn_del.set_color(Color.red);
                    btn_del.set_act(() =>
                    {
                        this.BoxMsg = this.bible.carrot.Show_msg("Delete Chapter", "Are you sure you want to delete this '" + (index_p+1) + "' chapter?", () =>
                        {
                            contents.RemoveAt(index_p);
                            this.isChange = true;
                            this.UpdateDataFile();
                            BoxMsg.close();
                            this.View_book_by_data(this.data_book_cur);
                        });
                    });
                }
            }
        }

        Carrot_Box_Btn_Panel panel = this.BoxChapterView.create_panel_btn();

        if (data["type_item"] != null)
        {
            string type_item = data["type_item"].ToString();

            if (type_item == "online")
            {
                Carrot.Carrot_Button_Item btn_save = panel.create_btn("btn_save");
                btn_save.set_icon_white(bible.offline.icon_offline_book);
                btn_save.set_label(this.bible.carrot.L("save", "Save"));
                btn_save.set_bk_color(bible.carrot.color_highlight);
                btn_save.set_act_click(() => bible.offline.Add(data));
            }

            if (type_item == "offline")
            {
                Carrot.Carrot_Button_Item btn_del = panel.create_btn("btn_del");
                btn_del.set_icon_white(bible.carrot.sp_icon_del_data);
                btn_del.set_label(this.bible.carrot.L("del", "Delete"));
                btn_del.set_bk_color(bible.carrot.color_highlight);
                btn_del.set_act_click(() => bible.offline.delete(int.Parse(data["index"].ToString())));
            }
        }
        Carrot_Button_Item btn_close = panel.create_btn("btn_close");
        btn_close.set_icon_white(bible.carrot.icon_carrot_cancel);
        btn_close.set_bk_color(bible.carrot.color_highlight);
        btn_close.set_label(this.bible.carrot.L("cancel", "Cancel"));
        btn_close.set_act_click(() => this.BoxChapterView.close());
    }

    private void BoxEditOrAddParagraph(IDictionary dataChapter, int index_c)
    {
        Carrot_Box box_paragraphs = this.bible.carrot.Create_Box();
        box_paragraphs.set_icon(this.bible.icon_paragraph);
        box_paragraphs.set_title("Add Content ("+dataChapter["name"].ToString()+")");

        if (dataChapter["paragraphs"] != null)
        {
            IList paragraphs = (IList)dataChapter["paragraphs"];
            for (int i = 0; i < paragraphs.Count; i++)
            {
                var index_p = i;
                string s_paragraph = paragraphs[i].ToString();
                Carrot_Box_Item item_p = box_paragraphs.create_item();
                item_p.set_icon(this.bible.icon_chapter);
                item_p.set_type(Box_Item_Type.box_value_input);
                item_p.set_title("Paragraph " + (i + 1));
                item_p.set_tip(s_paragraph);
                item_p.set_val(s_paragraph);

                this.AddBtnDelParagraphItem(item_p).set_act(() =>
                {
                    this.bible.carrot.play_sound_click();
                    Destroy(item_p.gameObject);
                });
            }
        }

        box_paragraphs.create_btn_menu_header(this.bible.carrot.icon_carrot_add).set_act(() =>
        {
            int leng_p = box_paragraphs.area_all_item.childCount + 1;
            Carrot_Box_Item item_p_new = box_paragraphs.create_item();
            item_p_new.set_icon(this.bible.icon_chapter);
            item_p_new.set_type(Box_Item_Type.box_value_input);
            item_p_new.set_title("Paragraph " + leng_p);
            item_p_new.set_tip("New Paragraph");
            this.AddBtnDelParagraphItem(item_p_new);
            bible.carrot.delay_function(1f, () =>
            {
                box_paragraphs.UI.scrollRect.verticalNormalizedPosition = 0f;
            });
        });

        box_paragraphs.create_btn_menu_header(this.bible.carrot.icon_carrot_done).set_act(() =>
        {
            IList book_contents = this.data_book_cur["contents"] as IList;
            IDictionary chapterData = book_contents[index_c] as IDictionary;
            IDictionary item_content = Json.Deserialize("{}") as IDictionary;
            item_content["name"] = chapterData["name"].ToString();
            IList list_paragraphs = Json.Deserialize("[]") as IList;
            foreach (Transform tr in box_paragraphs.area_all_item)
            {
                string val = tr.gameObject.GetComponent<Carrot_Box_Item>().get_val();
                val = val.Replace("\r\n", "").Replace("\n", "").Replace("\r", "").Trim();
                list_paragraphs.Add(val);
            }
            item_content["paragraphs"] = list_paragraphs;
            book_contents[index_c] = item_content;
            this.data_book_cur["contents"] = book_contents;
            this.list_data_Bible[this.IndexBookEdit] = this.data_book_cur;
            this.UpdateDataFile();
            BoxMsg = this.bible.carrot.Show_msg("Update book " + this.data_book_cur["name"].ToString() + " success!");
            if (box_paragraphs != null) box_paragraphs.close();
            bible.carrot.delay_function(1.5f, () =>
            {
                if (BoxMsg != null) BoxMsg.close();
                bible.book.View(this.data_book_cur, this.IndexBookEdit);
            });
        });

        box_paragraphs.create_btn_menu_header(this.bible.carrot.icon_carrot_write,false).set_act(() =>
        {
            Carrot_Window_Input inp=bible.carrot.Show_input("Create Data by json", "Enter Content json");
            inp.set_act_done(sText =>
            {
                IList listC = Json.Deserialize(inp.get_val()) as IList;
                for(int i=0;i<listC.Count;i++)
                {
                    Carrot_Box_Item item_p_new = box_paragraphs.create_item();
                    item_p_new.set_icon(this.bible.icon_chapter);
                    item_p_new.set_type(Box_Item_Type.box_value_input);
                    item_p_new.set_title("Paragraph " + (i+1));
                    item_p_new.set_tip("New Paragraph");
                    item_p_new.set_val(listC[i].ToString());
                    this.AddBtnDelParagraphItem(item_p_new);
                }
                inp.close();
            });
        });
    }

    private Carrot_Box_Btn_Item AddBtnDelParagraphItem(Carrot_Box_Item item_p)
    {
        item_p.create_item().set_icon(this.bible.icon_up).set_act(() =>
        {
            int indexCur = item_p.transform.GetSiblingIndex();
            indexCur--;
            item_p.transform.SetSiblingIndex(indexCur);
            this.bible.carrot.play_sound_click();
        });

        item_p.create_item().set_icon(this.bible.icon_down).set_act(() =>
        {
            int indexCur = item_p.transform.GetSiblingIndex();
            indexCur++;
            item_p.transform.SetSiblingIndex(indexCur);
            this.bible.carrot.play_sound_click();
        });

        Carrot_Box_Btn_Item btn_del = item_p.create_item();
        btn_del.set_icon(this.bible.carrot.sp_icon_del_data);
        btn_del.set_icon_color(Color.white);
        btn_del.set_color(Color.red);
        btn_del.set_act(() =>
        {
            Destroy(item_p.gameObject);
        });
        return btn_del;
    }

    public void View_paragraphs_list(int indexBook, int indexChapter)
    {
        this.IndexBookEdit = indexBook;
        this.data_book_cur=list_data_Bible[indexBook] as IDictionary;
        IList contents = (IList)data_book_cur["contents"];
        IDictionary chapter = (IDictionary)contents[indexChapter];
        chapter["index"] = indexChapter;
        View_paragraphs_list(chapter);
    }

    public void View_paragraphs_list(IDictionary chapter)
    {
        index_show_chapter = int.Parse(chapter["index"].ToString());
        type_view_page = false;
        data_chapter_cur = chapter;
        bible.carrot.play_sound_click();
        Carrot.Carrot_Box box_paragraphs = Box_view(data_book_cur["name"].ToString() + " (" + this.bible.carrot.L("chapter", "Chapter") + " " + (index_show_chapter + 1) + ")");
        IList paragraphs = (IList)chapter["paragraphs"];
        for (int i = 0; i < paragraphs.Count; i++)
        {
            var index_item = i;
            string s_paragraph = paragraphs[i].ToString();
            var s_txt = s_paragraph;
            Carrot_Box_Item item_p = box_paragraphs.create_item("p_" + i);
            item_p.set_title(s_paragraph);
            item_p.set_tip(bible.carrot.L("sentence","Sentence")+" "+ (i + 1).ToString());
            item_p.set_icon(bible.icon_paragraph);

            Carrot_Box_Btn_Item btn_copy = item_p.create_item();
            btn_copy.set_icon(bible.icon_copy);
            btn_copy.set_color(bible.carrot.color_highlight);
            btn_copy.set_act(() => Show_copy(s_paragraph));

            Carrot_Box_Btn_Item btn_speech = item_p.create_item();
            btn_speech.set_icon(bible.icon_speech);
            btn_speech.set_color(bible.carrot.color_highlight);
            btn_speech.set_act(() =>
            {
                this.Speak(s_txt, this.bible.carrot.lang.Get_key_lang());
            });
        }

        Nav_page(box_paragraphs);
    }

    public void Speak(string text, string language = "en")
    {
        StartCoroutine(DownloadAndPlay(text, language));
    }

    IEnumerator DownloadAndPlay(string text, string language)
    {
        string url = $"https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&q={UnityWebRequest.EscapeURL(text)}&tl={language}";

        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            this.audioSpeaker.clip = clip;
            this.audioSpeaker.Play();
        }
    }

    public void View_paragraphs_page(IDictionary chapter)
    {
        index_show_chapter = int.Parse(chapter["index"].ToString());
        type_view_page = true;
        data_chapter_cur = chapter;
        bible.carrot.play_sound_click();
        Carrot.Carrot_Box box_paragraphs = Box_view(data_book_cur["name"].ToString() + " (" + this.bible.carrot.L("chapter", "Chapter") + " " + (index_show_chapter + 1) + ")");
        IList paragraphs = (IList)chapter["paragraphs"];
        string s_page = "";
        for (int i = 0; i < paragraphs.Count; i++)
        {
            s_page = s_page + (i + 1) + " " + paragraphs[i].ToString() + " ";
        }

        GameObject obj_txt;
        if (bible.carrot.lang.Get_key_lang() == "ko")
        {
            obj_txt = Instantiate(bible.prefab_paragraph_item_ko);
        }
        else if (bible.carrot.lang.Get_key_lang() == "zh-CN")
        {
            obj_txt = Instantiate(bible.prefab_paragraph_item_zh);
        }
        else
        {
            obj_txt = Instantiate(bible.prefab_paragraph_item);
        }

        textPro = obj_txt.GetComponent<TextMeshProUGUI>();
        textPro.text = s_page;

        obj_txt.transform.SetParent(box_paragraphs.area_all_item);
        obj_txt.transform.localPosition = new Vector3(0f, 0f, 0f);
        obj_txt.transform.localScale = new Vector3(1f, 1f, 1f);
        obj_txt.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        bible.carrot.delay_function(1f, Fix_size_paragraph);
    }

    private void Nav_page(Carrot.Carrot_Box box)
    {
        Carrot.Carrot_Box_Btn_Panel panel = box.create_panel_btn();
        Carrot.Carrot_Button_Item btn_prev = panel.create_btn("btn_prev");
        btn_prev.set_icon(bible.icon_prev_page);
        btn_prev.set_label(this.bible.carrot.L("prev", "Previous"));
        btn_prev.set_bk_color(bible.carrot.color_highlight);
        btn_prev.set_act_click(() => Prev_page());

        Carrot.Carrot_Button_Item btn_next = panel.create_btn("btn_next");
        btn_next.set_icon(bible.icon_next_page);
        btn_next.set_label(this.bible.carrot.L("next", "Next"));
        btn_next.set_bk_color(bible.carrot.color_highlight);
        btn_next.set_act_click(() => Nex_page());
    }

    private Carrot.Carrot_Box Box_view(string s_title)
    {
        bible.ads.show_ads_Interstitial();
        if (box_paragraphs_view != null) box_paragraphs_view.close();
        box_paragraphs_view = bible.carrot.Create_Box();
        box_paragraphs_view.set_icon_white(bible.icon_chapter);
        box_paragraphs_view.set_title(s_title);

        Carrot.Carrot_Box_Btn_Item btn_page = box_paragraphs_view.create_btn_menu_header(bible.icon_book_open);
        btn_page.set_act(() => View_paragraphs_page_chapter_cur());
        if (type_view_page == true) btn_page.set_icon_color(bible.carrot.color_highlight);
        Carrot.Carrot_Box_Btn_Item btn_list = box_paragraphs_view.create_btn_menu_header(icon_list);
        btn_list.set_act(() => View_paragraphs_list_chapter_cur());
        if (type_view_page == false) btn_list.set_icon_color(bible.carrot.color_highlight);

        return box_paragraphs_view;
    }

    private void View_paragraphs_page_chapter_cur()
    {
        View_paragraphs_page(data_chapter_cur);
    }

    private void View_paragraphs_list_chapter_cur()
    {
        View_paragraphs_list(data_chapter_cur);
    }

    private void Fix_size_paragraph()
    {
        textPro.overflowMode = TextOverflowModes.Masking;
        Nav_page(box_paragraphs_view);
    }

    public Carrot_Box_Item Item_book(IDictionary data)
    {
        Carrot_Box_Item item_book = bible.Create_item();
        if (data["name"] != null)
        {
            string s_type = data["type"].ToString();
            item_book.set_title(data["name"].ToString());
            int countChapter = 0;
            if (data["contents"] != null)
            {
                IList listContent = data["contents"] as IList;
                countChapter = listContent.Count;
            }
            item_book.set_tip(this.bible.carrot.L(s_type, s_type) + " - (" + countChapter + " " + this.bible.carrot.L("chapter", "Chapter") + ")");
        }

        if (data["type"] != null)
        {
            string s_type_book = data["type"].ToString();
            if (s_type_book == "old_testament")
                item_book.set_icon_white(bible.icon_book_old_testament);
            else
                item_book.set_icon_white(bible.icon_book_new_Testament);
        }
        return item_book;
    }

    private void Nex_page()
    {
        index_show_chapter++;
        Show_chapter();
    }

    private void Prev_page()
    {
        index_show_chapter--;
        Show_chapter();
    }

    private void Show_chapter()
    {
        IList contents = (IList)data_book_cur["contents"];
        IDictionary chapter = (IDictionary)contents[index_show_chapter];
        if (type_view_page)
            View_paragraphs_page(chapter);
        else
            View_paragraphs_list(chapter);
    }

    private void Show_copy(string s_text)
    {
        bible.carrot.Show_input(this.bible.carrot.L("copy", "Copy"), this.bible.carrot.L("copy_tip", "Copy this bible passage"), s_text);
    }

    public void Set_data_book_cur(IDictionary data)
    {
        this.data_book_cur = data;
    }

    public void ShowAddBook(IDictionary dataBookEdit = null)
    {
        Carrot_Box box_add = this.bible.carrot.Create_Box();
        if (dataBookEdit != null)
        {
            this.data_book_cur = dataBookEdit;
            box_add.set_title("Update Book ("+this.data_book_cur["name"].ToString()+")");
        }
        else
        {
            box_add.set_title("Add Book");
        }
        box_add.set_icon(this.bible.carrot.icon_carrot_add);

        Carrot_Box_Item item_name = box_add.create_item();
        item_name.set_icon(this.bible.carrot.user.icon_user_name);
        item_name.set_title("Name");
        item_name.set_tip("Set name for book");
        item_name.set_type(Box_Item_Type.box_value_input);
        if (dataBookEdit != null) item_name.set_val(dataBookEdit["name"].ToString());

        Carrot_Box_Item item_type = box_add.create_item();
        item_type.set_icon(this.bible.carrot.icon_carrot_all_category);
        item_type.set_title("Type");
        item_type.set_tip("Set type for book");
        item_type.set_type(Box_Item_Type.box_value_dropdown);
        item_type.dropdown_val.ClearOptions();
        item_type.dropdown_val.options.Add(new Dropdown.OptionData { text = "New" });
        item_type.dropdown_val.options.Add(new Dropdown.OptionData { text = "Old" });
        if (dataBookEdit != null)
        {
            string s_type = dataBookEdit["type"].ToString();
            if (s_type == "new_testament")
                item_type.set_val("0");
            else
                item_type.set_val("1");
        }
        item_type.dropdown_val.RefreshShownValue();

        Carrot_Box_Item item_order = box_add.create_item();
        item_order.set_icon(this.bible.carrot.user.icon_user_name);
        item_order.set_title("Order");
        item_order.set_tip("Set order for book");
        item_order.set_type(Box_Item_Type.box_value_input);
        item_order.set_val(this.list_data_Bible.Count.ToString());
        if (dataBookEdit != null) item_order.set_val(dataBookEdit["order"].ToString());

        box_add.CreatePanelCancelDone(() =>
        {
            IDictionary DataBookNew = Json.Deserialize("{}") as IDictionary;
            DataBookNew["name"] = item_name.get_val(); 
            if (item_type.dropdown_val.value == 0)
                DataBookNew["type"] = "new_testament";
            else
                DataBookNew["type"] = "old_testament";
            DataBookNew["order"] = item_order.get_val();

            if (dataBookEdit != null)
            {
                DataBookNew["contents"] = data_book_cur["contents"];
                int index_data = int.Parse(dataBookEdit["index_data"].ToString());
                this.list_data_Bible[index_data] = DataBookNew;
                this.bible.carrot.Show_msg("Update bible " + this.data_book_cur["name"].ToString() + " success!");
                box_add.close();
            }
            else
            {
                this.list_data_Bible.Add(DataBookNew);
            }

            this.UpdateDataFile();
            this.bible.carrot.delay_function(1f, () => this.bible.Act_load());
        });
    }

    public void UpdateDataFile()
    {
        FileBrowserHelpers.WriteTextToFile(System.IO.Path.Combine(this.s_path_data, "bible_" + this.bible.carrot.lang.Get_key_lang() + ".json"), Json.Serialize(this.list_data_Bible));
    }

    public void DeleteEbook(int index)
    {
        this.list_data_Bible.RemoveAt(index);
        this.UpdateDataFile();
    }

    public IDictionary GetPassage()
    {
        IDictionary Passage = Json.Deserialize("{}") as IDictionary;

        string sPassage = "";
        string sLocation = "";
        int indexBook = 0;
        int indexContent = 0;
        if (this.list_data_Bible.Count > 0)
        {
            indexBook = UnityEngine.Random.Range(0, this.list_data_Bible.Count);
            IDictionary dataBook = list_data_Bible[indexBook] as IDictionary;
            IList listContents = dataBook["contents"] as IList;
            indexContent = UnityEngine.Random.Range(0, listContents.Count);
            IDictionary cContent = listContents[indexContent] as IDictionary;
            IList paragraphs = cContent["paragraphs"] as IList;
            int indexP = UnityEngine.Random.Range(0, paragraphs.Count);
            sPassage = paragraphs[indexP].ToString();
            sLocation += dataBook["name"].ToString() + " - " + (indexContent + 1) + "," + (indexP + 1);
            bible.txtPassageLocation_l.text = sLocation;
            bible.txtPassageLocation_p.text = sLocation;
        }
        Passage["text"] = sPassage;
        Passage["location"] = sLocation;
        Passage["indexBook"] = indexBook;
        Passage["indexContent"] = indexContent;
        return Passage;
    }

    public void GetAndShowNewPassage()
    {
        IDictionary dataPassage = this.GetPassage();
        string msgPassage = dataPassage["text"].ToString();
        msgPassage += "\n\n" + dataPassage["location"].ToString();
        if (msgPassage != "") bible.carrot.Show_msg(bible.carrot.L("bible_passage", "Bible passage"), msgPassage);
        bible.txtPassage_l.text = dataPassage["text"].ToString();
        bible.txtPassage_p.text = dataPassage["text"].ToString();
        bible.txtPassageLocation_l.text = dataPassage["location"].ToString();
        bible.txtPassageLocation_p.text=dataPassage["location"].ToString();
    }

}
