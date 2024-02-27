using UnityEngine;

public class Manager_Search : MonoBehaviour
{
    [Header("Obj Main")]
    public Bible bible;

    [Header("Obj Search")]
    public Sprite icon_search_return;

    public void show_search()
    {
        this.bible.carrot.ads.show_ads_Interstitial();
        this.bible.carrot.show_search(null, PlayerPrefs.GetString("search_tip", "You can search for any biblical content here!"));
    }

    public void list()
    {
        this.bible.carrot.clear_contain(this.bible.tr_all_item_book);
        Carrot.Carrot_Box_Item item_inp_search=this.bible.create_item();
        item_inp_search.set_icon(this.bible.icon_search);
        item_inp_search.set_type(Carrot.Box_Item_Type.box_value_input);
        item_inp_search.check_type();
        item_inp_search.set_title(PlayerPrefs.GetString("search", "Search"));
        item_inp_search.set_tip(PlayerPrefs.GetString("search_tip", "You can search for any biblical content here!"));
        item_inp_search.inp_val.onSubmit.RemoveAllListeners();
        item_inp_search.inp_val.onSubmit.AddListener(this.Submit_search);
    }

    private void Submit_search(string s_key)
    {
        string s_data = PlayerPrefs.GetString("data_bible_" +bible.carrot.lang.get_key_lang());
        if (s_data != "")
        {
            
        }
        else
        {
            bible.carrot.show_msg("No data");
        }
    }
}
