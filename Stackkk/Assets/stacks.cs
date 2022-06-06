using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class stacks : MonoBehaviour
{
    int stack_uzunlugu;
    int count = 4;
    int skor = 0;
    GameObject[] go_stack;
    int stack_index;
    bool stack_alindi=false;
    const float max_deger = 7f;
    const float hiz_degeri = 0.20f;
    float hiz = hiz_degeri;
    bool x_ekseninde_hareket;
    const float buyukluk = 4f;
    Vector2 stack_boyut = new Vector2(buyukluk, buyukluk);
    Vector3 Camera_pos;
    Vector3 eski_stack_pos;
    float hassasiyet = -0.5f;
    bool dead = false;
    float hata_payi = 0.2f;
    int combo = 0;
    Color32 renk;
    public Color32 renk1;
    public Color32 renk2;
    public Color32 renk3;
    public Color32 renk4;
    public Text textimiz;
    public Text high_score_Text;
    public Text hedeftext;
    public Text texthyaz;
    int sayac = 0;
    Camera camera;
    public GameObject g_panel;
    int high_score;
    public int hedef = 10;

    //Bloklarý tam üstüne denk getirirsek büyüme ve hedef kodu çalýþýyo
    //Denk getiremezsek o if in içine girmediði için
    //Blok büyümesi yapmýyor 
    //Bunu çöz...
    //-->Sorun çözüldü...

    //---------------------------------------------------------------------
    //Oyunumuza menü eklenecek: Baþla - Ayarlar - Hakkýnda - Çýkýþ
    //Ayarlarda zorluk ayarý bulanacak, hýz ayarý seçilecek...

    // Start is called before the first frame update
    void Start()
    {
        high_score = PlayerPrefs.GetInt("highscore"); //High Skoru tutuyoruz.
        high_score_Text.text = high_score.ToString();
        textimiz.text = skor.ToString();    //Anlýk skorumuz.
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        camera.backgroundColor = renk2;
        renk = renk1;
        stack_uzunlugu = transform.childCount;
        go_stack = new GameObject[stack_uzunlugu];
        for (int i = 0; i < stack_uzunlugu; i++)
        {
            go_stack[i] = transform.GetChild(i).gameObject;
            go_stack[i].GetComponent<Renderer>().material.color = renk;
        }
        stack_index = stack_uzunlugu - 1;  //Stacklari alttan alýp üste ekliyoruz
        //Yeni bir stack oluþturmuyoruz.
    }
    void ArtikParcaOl(Vector3 konum,Vector3 scale,Color32 renkde)
    {
        //Stackten kopan parçalarý oluþturma...
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.localScale = scale;
        go.transform.position = konum;
        go.GetComponent<Renderer>().material.color = renkde;
        go.AddComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        //Android kontrollerini tanýmlama...
        //Touch kontrolü
        if (!dead)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Oyun();
                }
                Hareketlendir();
                transform.position = Vector3.Lerp(transform.position, Camera_pos, 01f); //Her týkladýðýmýzda cameranýn da hareket etmesini saðlar.
            }
            else if(Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount>0 && Input.GetTouch(0).phase==TouchPhase.Began)
                {
                    Oyun();
                }
                Hareketlendir();
                transform.position = Vector3.Lerp(transform.position, Camera_pos, 01f);
            }
        }
    }

    public void Oyun()
    {
        if (Stack_kontrol())
        {
            Stack_Al_Koy();
            count++;
            skor++;
            textimiz.text = skor.ToString();
            if (skor > high_score)
            {
                high_score = skor;
            }
            byte deger = 25;
            renk = new Color32((byte)(renk.r + deger), (byte)(renk.g + deger), (byte)(renk.b + deger), renk.a);
            renk2 = new Color32((byte)(renk2.r + deger), (byte)(renk2.g + deger), (byte)(renk2.b + deger), renk2.a);
            if (sayac > 3) //Renklerin sürekli deðiþmesini saðlýyoruz..
            {
                sayac = 0;
                renk3 = renk4;
                renk4 = renk1;
                renk1 = renk2;
                renk2 = renk3;
                renk = renk1;
            }
            sayac++;
        }
        else
        {
            Bitir(); //oyunuu bitir...
        }
    }

    void Stack_Al_Koy()
    {
        eski_stack_pos = go_stack[stack_index].transform.localPosition;
        if (stack_index<=0)
        {
            stack_index = stack_uzunlugu;
        }
        stack_alindi = false;
        stack_index--;
        x_ekseninde_hareket = !x_ekseninde_hareket;
        Camera_pos = new Vector3(0,-count+3,0);
        go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);
        go_stack[stack_index].GetComponent<Renderer>().material.color = Color32.Lerp(go_stack[stack_index].GetComponent<Renderer>().material.color,renk,0.5f);
        camera.backgroundColor = Color32.Lerp(camera.backgroundColor, renk2, 0.1f);
    }

    void Hareketlendir()
    {
        //Stacklerin x ve y koordinatlarýnda hareketin saðlanmasý...
        if(x_ekseninde_hareket)
        {
            if (!stack_alindi)
            {
                //go_stack[stack_index].transform.localPosition = new Vector3(-10, count, hassasiyet); //1.3f
                go_stack[stack_index].transform.localPosition = new Vector3(-10, count, hassasiyet); //1.3f
                stack_alindi = true;
            }
            if (go_stack[stack_index].transform.localPosition.x > max_deger)//Max deðer: Bloðun gideceði son koordinat.
            {
                hiz = hiz_degeri * -1;
            }
            else if (go_stack[stack_index].transform.localPosition.x < -max_deger)
            {
                hiz = hiz_degeri;
            }
            go_stack[stack_index].transform.localPosition += new Vector3(hiz, 0, 0);
        }
        else
        {
            if (!stack_alindi)
            {
                go_stack[stack_index].transform.localPosition = new Vector3(hassasiyet, count, -13); //-0.5f //Buraya bi bak-13tü enson (-2 orj)
                stack_alindi = true;
            }
            if (go_stack[stack_index].transform.localPosition.z > max_deger)
            {
                hiz = hiz_degeri * -1;
            }
            else if (go_stack[stack_index].transform.localPosition.z < -max_deger)
            {
                hiz = hiz_degeri;
            }
            go_stack[stack_index].transform.localPosition += new Vector3(0, 0, hiz);
        }
     
    }
    bool Stack_kontrol()
    {
      
        if (x_ekseninde_hareket)
        {
            float fark = eski_stack_pos.x - go_stack[stack_index].transform.localPosition.x;
            if (Mathf.Abs(fark) > hata_payi) //Yeni koyduðumuz blok belirli bir uzaklýða kadar hata kabul ediyor, ve tam üstüne yerleþtiriyor.
            {
                combo = 0;
                Vector3 konum;
                if (go_stack[stack_index].transform.localPosition.x > eski_stack_pos.x)
                {
                    konum = new Vector3(go_stack[stack_index].transform.position.x + go_stack[stack_index].transform.localScale.x / 2, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z);
                }
                else
                {
                    konum = new Vector3(go_stack[stack_index].transform.position.x - go_stack[stack_index].transform.localScale.x / 2, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z);
                }
                Vector3 boyut = new Vector3(fark, 1, stack_boyut.y);
                stack_boyut.x -= Mathf.Abs(fark);
                if (stack_boyut.x < 0)
                {
                    return false;
                }
                go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);
                float mid = go_stack[stack_index].transform.localPosition.x / 2 + eski_stack_pos.x / 2;
                go_stack[stack_index].transform.localPosition = new Vector3(mid, count, eski_stack_pos.z);
                hassasiyet = go_stack[stack_index].transform.localPosition.x;
                ArtikParcaOl(konum, boyut, go_stack[stack_index].GetComponent<Renderer>().material.color);
                //-------------Aþaðýsý yeni eklendi--------------------
                combo++;  //-----X ekseninde Combo ----------------
                if ((skor+1) >= hedef)
                {
                    //textimiz.text = skor.ToString();
                    hedef += 10;
                    texthyaz.text = hedef.ToString();
                    stack_boyut.x += 1f;
                    stack_boyut.y += 1f;//fazla
                    if (stack_boyut.x > buyukluk)
                    {
                        stack_boyut.x = buyukluk;
                    }
                    go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                else
                {
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                hassasiyet = go_stack[stack_index].transform.localPosition.x;



            }
            else
            {
                combo++;  //-----X ekseninde Combo ----------------
                if ((skor+1) >= hedef) //Hedef e ulaþtýðýmýzda Stack imiz büyüyecek...
                {
                    //textimiz.text = skor.ToString();
                    hedef += 10;
                    texthyaz.text = hedef.ToString();                   
                    stack_boyut.x += 1f;
                    stack_boyut.y += 1f;//fazla
                    if (stack_boyut.x > buyukluk)
                    {
                        stack_boyut.x = buyukluk;                      
                    }
                    go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                else
                {
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                hassasiyet = go_stack[stack_index].transform.localPosition.x;
            }
        }
        else
        {

            float fark = eski_stack_pos.z - go_stack[stack_index].transform.localPosition.z;
            if (Mathf.Abs(fark) > hata_payi)
            {
                combo = 0;
                Vector3 konum;
                if (go_stack[stack_index].transform.localPosition.z > eski_stack_pos.z)
                {
                    konum = new Vector3(go_stack[stack_index].transform.position.x, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z + go_stack[stack_index].transform.localScale.z / 2);
                }
                else
                {
                    konum = new Vector3(go_stack[stack_index].transform.position.x, go_stack[stack_index].transform.position.y, go_stack[stack_index].transform.position.z - go_stack[stack_index].transform.localScale.z / 2);
                }
                Vector3 boyut = new Vector3(stack_boyut.x, 1, fark);
                stack_boyut.y -= Mathf.Abs(fark);
                if (stack_boyut.y < 0)
                {
                    return false;
                }
                go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);
                float mid = go_stack[stack_index].transform.localPosition.z / 2 + eski_stack_pos.z / 2;
                go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, mid);
                hassasiyet = go_stack[stack_index].transform.localPosition.z;
                ArtikParcaOl(konum, boyut, go_stack[stack_index].GetComponent<Renderer>().material.color);
                combo++;
                //Aþaðýsý yeni eklendi-----------------------------------------
                combo++;  //----------Y ekseninde Combo -----------------
                if ((skor+1) >= hedef)
                {
                    hedef += 10;
                    texthyaz.text = hedef.ToString();
                    stack_boyut.y += 1f;
                    stack_boyut.x += 1f;
                    if (stack_boyut.y > buyukluk)
                    {
                        stack_boyut.y = buyukluk;
                    }
                    go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                else
                {
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                hassasiyet = go_stack[stack_index].transform.localPosition.z;
            }

            else
            {
                combo++;  //----------Y ekseninde Combo -----------------
                if ((skor+1) >= hedef) //Hedefe ulaþtýðýmýzda Stack büyümesi gerçekleþecek...
                {
                    hedef += 10;
                    texthyaz.text = hedef.ToString();
                    stack_boyut.y += 1f;
                    stack_boyut.x += 1f;
                    if (stack_boyut.y > buyukluk)
                    {
                        stack_boyut.y = buyukluk;                        
                    }
                    go_stack[stack_index].transform.localScale = new Vector3(stack_boyut.x, 1, stack_boyut.y);                    
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                else
                {
                    go_stack[stack_index].transform.localPosition = new Vector3(eski_stack_pos.x, count, eski_stack_pos.z);
                }
                hassasiyet = go_stack[stack_index].transform.localPosition.z;
            }
        }
        return true;
    }

    void Bitir()
    {
        dead = true;
        go_stack[stack_index].AddComponent<Rigidbody>();
        g_panel.SetActive(true);
        PlayerPrefs.SetInt("highscore", high_score);
        high_score_Text.text = high_score.ToString(); 
        textimiz.text = "";
    }


    public void Yeni_Oyun()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
       
}
