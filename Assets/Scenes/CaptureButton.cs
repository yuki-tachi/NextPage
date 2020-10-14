using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CaptureButton : MonoBehaviour
{
    private Camera cam;
    private GameObject canvas;
    public Image m_DrawTex;

    public GameObject bgPaper;
    public GameObject page1;
    public GameObject page2;

    private bool isPaging = false;

    public void Botton()
    {
        StartCoroutine("Capture");
    }

    private void changePage()
    {
        this.page1.SetActive(!this.page1.activeSelf);
        this.page2.SetActive(!this.page2.activeSelf);
    }
    IEnumerator Capture()
    {

        // bgPaper.SetActive(false);
        //ReadPicxelsがこの後でないと使えないので必ず書く
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cam.targetTexture = renderTexture;
        //スクリーンの大きさのSpriteを作る
        var texture = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false);

        //スクリーンを取得する
        texture.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        //適応する
        texture.Apply();

        //取得した画像をSpriteに入るように変換する
        byte[] pngdata = texture.EncodeToPNG();
        texture.LoadImage(pngdata);

        //先ほど作ったSpriteに画像をいれる
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), new Vector2(0.5f, 0.5f));
        Debug.Log($"screen: w:{cam.targetTexture.width} h:{cam.targetTexture.height}");

        //Spriteを使用するオブジェクトに指定する
        //     今回はUIのImage
        m_DrawTex.GetComponent<Image>().sprite = sprite;

        // // サイズ変更
        // m_DrawTex.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);

        //imageをアクティブにする
        m_DrawTex.gameObject.SetActive(true);

        this.isPaging = true;

        cam.targetTexture = null;

        bgPaper.SetActive(true);
        this.GetComponent<Button>().interactable = false;
        this.changePage();
    }

    void Awake()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        canvas = GameObject.Find("Canvas");
    }

    void Start()
    {
        m_DrawTex.material.SetFloat("_Flip", 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.isPaging)
        {
            return;
        }

        float flipValue = m_DrawTex.material.GetFloat("_Flip");

        if (flipValue > -1.0f)
        {
            flipValue -= 2.5f * Time.deltaTime;
        }
        else
        {
            this.isPaging = false;
            flipValue = 1.0f;
            this.GetComponent<Button>().interactable = true;
            m_DrawTex.gameObject.SetActive(false);
        }

        m_DrawTex.material.SetFloat("_Flip", flipValue);
    }
}
