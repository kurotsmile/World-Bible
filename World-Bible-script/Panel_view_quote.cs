using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Panel_view_quote : MonoBehaviour
{
    public Text txt_quote;
    public Text txt_chap;
    public Skybox skybox;
    public Button btn_close;

    private string id_chap_share;
    private string lang_chap_share;
    public void show_quote(string s_quote,Image img_bk,string id_chap_share,string lang_chap_share)
    {
        this.set_txt_and_img(s_quote,"", img_bk);
        this.id_chap_share=id_chap_share;
        this.lang_chap_share=lang_chap_share;
        this.gameObject.SetActive(true);
        this.btn_close.onClick.RemoveAllListeners();
        this.btn_close.onClick.AddListener(this.close);
    }

    public void show_quote_in_view_p(string s_quote,string s_chap, Image img_bk,string id_chap_share,string lang_chap_share)
    {
        this.set_txt_and_img(s_quote,s_chap, img_bk);
        this.id_chap_share=id_chap_share;
        this.lang_chap_share=lang_chap_share;
        this.gameObject.SetActive(true);
        this.btn_close.onClick.RemoveAllListeners();
        this.btn_close.onClick.AddListener(this.close);
        this.btn_close.onClick.AddListener(GameObject.Find("Bible").GetComponent<Bible>().panel_view.close_slide_list_select_p);
    }

    private void set_txt_and_img(string s_txt,string s_chap, Image img)
    {
        this.txt_quote.text = s_txt;
        this.txt_chap.text = s_chap;
        GameObject.Find("Bible").GetComponent<Bible>().panel_main.SetActive(false);
        GameObject.Find("Bible").GetComponent<Bible>().Sound_Click.Play();
        this.set_skybox_Texture(img.mainTexture);
    }


    public void close()
    {
        this.StopAllCoroutines();
        GameObject.Find("Bible").GetComponent<Bible>().panel_main.SetActive(true);
        this.gameObject.SetActive(false);
        GameObject.Find("Bible").GetComponent<Bible>().Sound_Click.Play();

    }

    public void set_skybox_Texture(Texture textT)
    {
        Material result = new Material(Shader.Find("RenderFX/Skybox"));
        result.SetTexture("_FrontTex", textT);
        result.SetTexture("_BackTex", textT);
        result.SetTexture("_LeftTex", textT);
        result.SetTexture("_RightTex", textT);
        result.SetTexture("_UpTex", textT);
        result.SetTexture("_DownTex", textT);
        this.skybox.material = result;
    }

    public void change_bk()
    {
        WWWForm frm_load_bk=GameObject.Find("Bible").GetComponent<Bible>().carrot.frm_act("get_image_bk");
        GameObject.Find("Bible").GetComponent<Bible>().carrot.send(frm_load_bk,get_image_handle);
    }
    private void get_image_handle(string s_data)
    {
         GameObject.Find("Bible").GetComponent<Bible>().carrot.get_img(s_data,downloadBK);
    }

    public void downloadBK(Texture2D texture2)
    {
        this.set_skybox_Texture(texture2);
    }

    public void btn_share(){
        string url_share=GameObject.Find("Bible").GetComponent<Bible>().carrot.get_url_host()+"/b/"+this.id_chap_share+"/"+this.lang_chap_share;
        GameObject.Find("Bible").GetComponent<Bible>().carrot.show_share(url_share,PlayerPrefs.GetString("app_title", "Bible"));
    }

}
