using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tools : MonoBehaviour
{

    public string get_url_audio(string url_audio)
    {
        if (Application.isEditor)
        {
            return "file://"+Application.dataPath + "/" + url_audio;
        }
        else
        {
            return "file://"+Application.persistentDataPath + "/" + url_audio;
        }
    }

    public void save_audio(string name_file_img, byte[] data)
    {
        if (Application.isEditor)
        {
            System.IO.File.WriteAllBytes(Application.dataPath + "/" + name_file_img, data);
        }
        else
        {
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + name_file_img, data);
        }
    }

    public Sprite load_image(string name_file_image)
    {
        string name_file_bk = "";
        if (Application.isEditor)
        {
            name_file_bk = Application.dataPath + "/" + name_file_image;
        }
        else
        {
            name_file_bk = Application.persistentDataPath + "/" + name_file_image;
        }

        if (System.IO.File.Exists(name_file_bk))
        {
            Texture2D load_s01_texture;
            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(name_file_bk);

            load_s01_texture = new Texture2D(1, 1);
            load_s01_texture.LoadImage(bytes);

            return Sprite.Create(load_s01_texture, new Rect(0.0f, 0.0f, load_s01_texture.width, load_s01_texture.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        else
        {
            return null;
        }
    }


    public void delete_file(string name_file)
    {
        string name_file_del = "";
        if (Application.isEditor)
        {
            name_file_del = Application.dataPath + "/" + name_file;
        }
        else
        {
            name_file_del = Application.persistentDataPath + "/" + name_file;
        }

        System.IO.File.Delete(name_file_del);
    }
}
