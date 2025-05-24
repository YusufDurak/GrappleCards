// OyunYonetici.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class OyunYonetici : MonoBehaviour
{
    [Header("Yöneticiler")]
    public KartVeritabani kartVeritabani;
    public OzelGucYonetici ozelGucYonetici;

    [Header("Kart Listeleri")]
    public List<Kart> oyuncuEli = new List<Kart>();
    public List<Kart> aiEli = new List<Kart>();

    [Header("Can Bilgileri")]
    public int oyuncuCan = 30;
    public int aiCan = 30;
    private int maxCan = 30;

    [Header("Tur Bilgileri")]
    private int turSayaci = 0;
    private bool oyuncuBaslar = true;

    [Header("Oyun Alanı")]
    public Kart oyuncuOynananKart;
    public Kart aiOynananKart;

    // --- ENUM TANIMI ---
    // Header buradan KALDIRILDI. Enum kendi başına bir tanımdır.
    public enum OyunDurumu { Hazirlik, KartSecimi, OyuncuSirasi, OyuncuOynadi, AISirasi, Savas, Bitis }

    [Header("Oyun Durumu")] // <-- Header 6 - DOĞRU YER: Alanın (field) hemen üzeri.
    public OyunDurumu mevcutDurum;

    [Header("Zamanlayıcı")]
    public float turSuresi = 20.0f;
    private float mevcutSure;
    private bool zamanlayiciAktif = false;

    [Header("UI Referansları")]
    public TextMeshProUGUI oyuncuCanText;
    public Slider oyuncuCanSlider;
    public TextMeshProUGUI aiCanText;
    public Slider aiCanSlider;
    public TextMeshProUGUI turText;
    public TextMeshProUGUI zamanlayiciText;
    public TextMeshProUGUI durumText;

    [Header("Alan Referansları")]
    public Transform oyuncuElAlani;

    //public Transform aiElAlani;
    public KartUI oyuncuSavasSlotu;
    public KartUI aiSavasSlotu;
    public GameObject kartPrefab;
    //public GameObject kartArkasiPrefab;

    [Header("Seçim Paneli")]
    public GameObject secimPaneli;
    public KartUI[] secimKartSlotlari = new KartUI[3];
    private List<Kart> secimIcinKartlar = new List<Kart>();

    [Header("Geçici Etkiler")]
    public int oyuncuEkstraAtk = 0;
    public int oyuncuEkstraDef = 0;
    public int oyuncuYansitHasari = 0;
    public int aiEkstraAtk = 0;
    public int aiEkstraDef = 0;
    public int aiYansitHasari = 0;

    // --- Metotlar Başlıyor ---

    void Start()
    {
        if (kartVeritabani == null)
            kartVeritabani = Object.FindAnyObjectByType<KartVeritabani>();

        if (ozelGucYonetici == null)
            ozelGucYonetici = Object.FindAnyObjectByType<OzelGucYonetici>();

        if (kartVeritabani != null && ozelGucYonetici != null)
        {
            OyunuBaslat();
        }
        else
        {
            Debug.LogError("Kart Veritabanı veya OzelGucYonetici bulunamadı!");
        }
    }

    void OyunuBaslat()
    {
        Debug.Log("Oyun Başlıyor...");
        mevcutDurum = OyunDurumu.Hazirlik;
        oyuncuCan = 30;
        aiCan = 30;
        turSayaci = 0;
        oyuncuBaslar = true;
        oyuncuEkstraAtk = 0; oyuncuEkstraDef = 0; oyuncuYansitHasari = 0;
        aiEkstraAtk = 0; aiEkstraDef = 0; aiYansitHasari = 0;

        oyuncuEli.Clear();
        aiEli.Clear();

        KartCekAI(4);
        oyuncuEli.Clear();
        for (int i = 0; i < 4; i++)
        {
            oyuncuEli.Add(kartVeritabani.RastgeleKartCek());
        }
        UpdateTumUI();
        StartCoroutine(YeniTurBaslat());
    }

    void KartCekAI(int adet)
    {
        for (int i = 0; i < adet; i++)
        {
            Kart cekilenKart = kartVeritabani.RastgeleKartCek();
            if (cekilenKart != null) aiEli.Add(cekilenKart);
        }
    }

    void Update()
    {
        if (mevcutDurum == OyunDurumu.OyuncuSirasi && zamanlayiciAktif)
        {
            mevcutSure -= Time.deltaTime;
            zamanlayiciText.text = Mathf.CeilToInt(mevcutSure).ToString();

            if (mevcutSure <= 0)
            {
                zamanlayiciAktif = false;
                DurumGuncelle("Süre doldu! Rastgele oynanıyor...");
                OyuncuRastgeleKartOyna();
            }
        }
    }

    IEnumerator YeniTurBaslat()
    {
        if (mevcutDurum == OyunDurumu.Bitis) yield break;

        mevcutDurum = OyunDurumu.Hazirlik;
        turSayaci++;
        DurumGuncelle($"TUR {turSayaci}");

        oyuncuOynananKart = null;
        aiOynananKart = null;
        UpdateSavasAlaniUI();

        if ((turSayaci - 1) % 5 == 0 && turSayaci > 1)
        {
            oyuncuBaslar = !oyuncuBaslar;
            DurumGuncelle($"Başlangıç sırası değişti! {(oyuncuBaslar ? "Oyuncu" : "AI")} başlıyor.");
            yield return new WaitForSeconds(1f);
        }

        KartCekAI(1);
        UpdateElUI();
        OyuncuKartSeciminiBaslat();

        yield return null;
    }

    void OyuncuKartSeciminiBaslat()
    {
        mevcutDurum = OyunDurumu.KartSecimi;
        DurumGuncelle("3 Karttan Birini Seç!");
        secimIcinKartlar.Clear();

        for (int i = 0; i < 3; i++)
        {
            secimIcinKartlar.Add(kartVeritabani.RastgeleKartCek());
        }

        secimPaneli.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            if (i < secimIcinKartlar.Count && secimKartSlotlari[i] != null)
            {
                secimKartSlotlari[i].gameObject.SetActive(true);
                secimKartSlotlari[i].SecimKartBilgileriniAyarla(secimIcinKartlar[i], this);
                secimKartSlotlari[i].SetInteractable(true);
            }
            else if (secimKartSlotlari[i] != null)
            {
                secimKartSlotlari[i].gameObject.SetActive(false);
            }
        }
    }

    public void KartSecildi(Kart secilenKart)
    {
        if (mevcutDurum != OyunDurumu.KartSecimi) return;

        oyuncuEli.Add(secilenKart);
        Debug.Log($"Oyuncu seçti: {secilenKart.Isim}");

        secimPaneli.SetActive(false);
        UpdateTumUI();

        if (oyuncuBaslar) OyuncuSirasiniBaslat();
        else StartCoroutine(AISirasiniBaslat());
    }

    void OyuncuSirasiniBaslat()
    {
        mevcutDurum = OyunDurumu.OyuncuSirasi;
        mevcutSure = turSuresi;
        zamanlayiciAktif = true;
        DurumGuncelle("Sıra Sende! Kartını Seç.");
        UpdateElUI();
    }

    IEnumerator AISirasiniBaslat()
    {
        mevcutDurum = OyunDurumu.AISirasi;
        DurumGuncelle("Rakip Düşünüyor...");
        UpdateElUI();
        yield return new WaitForSeconds(1.5f);
        AIHamlesiYap();
    }

    public void OyuncuKartOyna(int kartIndex)
    {
        if (mevcutDurum == OyunDurumu.OyuncuSirasi && kartIndex >= 0 && kartIndex < oyuncuEli.Count)
        {
            zamanlayiciAktif = false;
            oyuncuOynananKart = oyuncuEli[kartIndex];
            oyuncuEli.RemoveAt(kartIndex);
            Debug.Log($"Oyuncu oynadı: {oyuncuOynananKart}");

            ozelGucYonetici.GucUygula(oyuncuOynananKart, this, true);

            mevcutDurum = OyunDurumu.OyuncuOynadi;
            UpdateTumUI();

            if (oyuncuBaslar) StartCoroutine(AISirasiniBaslat());
            else StartCoroutine(SavasBaslat());
        }
    }

    void OyuncuRastgeleKartOyna()
    {
        if (oyuncuEli.Count > 0)
        {
            int randomIndex = Random.Range(0, oyuncuEli.Count);
            OyuncuKartOyna(randomIndex);
        }
    }

    void AIHamlesiYap()
    {
        if (aiEli.Count == 0)
        {
            Debug.LogWarning("AI'nın oynayacak kartı yok!");
            aiOynananKart = null;
            if (oyuncuBaslar) StartCoroutine(SavasBaslat());
            else OyuncuSirasiniBaslat();
            return;
        }

        Kart enIyiKart = aiEli[0];
        int enYuksekStat = 0;
        foreach (Kart kart in aiEli)
        {
            if (kart.Atk > enYuksekStat) { enYuksekStat = kart.Atk; enIyiKart = kart; }
            if (kart.Savunma > enYuksekStat) { enYuksekStat = kart.Savunma; enIyiKart = kart; }
        }

        aiOynananKart = enIyiKart;
        aiEli.Remove(enIyiKart);
        Debug.Log($"AI oynadı: {aiOynananKart}");

        ozelGucYonetici.GucUygula(aiOynananKart, this, false);

        UpdateTumUI();

        if (!oyuncuBaslar) OyuncuSirasiniBaslat();
        else StartCoroutine(SavasBaslat());
    }

    IEnumerator SavasBaslat()
    {
        if (oyuncuOynananKart == null && aiOynananKart == null)
        {
            DurumGuncelle("Kimse kart oynamadı!");
            yield return new WaitForSeconds(1f);
            StartCoroutine(YeniTurBaslat());
            yield break;
        }

        mevcutDurum = OyunDurumu.Savas;
        string savasBaslik = "";
        if (oyuncuOynananKart != null && aiOynananKart != null) savasBaslik = $"{oyuncuOynananKart.Isim} vs {aiOynananKart.Isim}";
        else if (oyuncuOynananKart != null) savasBaslik = $"{oyuncuOynananKart.Isim} saldırıyor!";
        else savasBaslik = $"{aiOynananKart.Isim} saldırıyor!";
        DurumGuncelle(savasBaslik);

        yield return new WaitForSeconds(1.5f);

        int oyuncuAnlikAtk = (oyuncuOynananKart != null ? oyuncuOynananKart.Atk : 0) + oyuncuEkstraAtk;
        int oyuncuAnlikDef = (oyuncuOynananKart != null ? oyuncuOynananKart.Savunma : 0) + oyuncuEkstraDef;
        int aiAnlikAtk = (aiOynananKart != null ? aiOynananKart.Atk : 0) + aiEkstraAtk;
        int aiAnlikDef = (aiOynananKart != null ? aiOynananKart.Savunma : 0) + aiEkstraDef;

        int ai_hasar = Mathf.Max(0, oyuncuAnlikAtk - aiAnlikDef);
        int oyuncu_hasar = Mathf.Max(0, aiAnlikAtk - oyuncuAnlikDef);

        DurumGuncelle("Hasarlar Hesaplanıyor...");
        yield return new WaitForSeconds(1.0f);

        int oyuncuya_yansiyan_hasar = (oyuncuYansitHasari > 0 && ai_hasar > 0) ? oyuncuYansitHasari : 0;
        int aiye_yansiyan_hasar = (aiYansitHasari > 0 && oyuncu_hasar > 0) ? aiYansitHasari : 0;

        CanDegistir(false, -ai_hasar);
        CanDegistir(true, -oyuncu_hasar);
        CanDegistir(true, -oyuncuya_yansiyan_hasar);
        CanDegistir(false, -aiye_yansiyan_hasar);

        string hasarMetni = $"Rakip {ai_hasar} aldı! Sen {oyuncu_hasar} aldın!";
        if (aiye_yansiyan_hasar > 0) hasarMetni += $" R {aiye_yansiyan_hasar} yansıdı!";
        if (oyuncuya_yansiyan_hasar > 0) hasarMetni += $" S {oyuncuya_yansiyan_hasar} yansıdı!";
        DurumGuncelle(hasarMetni);

        oyuncuEkstraAtk = 0; oyuncuEkstraDef = 0; oyuncuYansitHasari = 0;
        aiEkstraAtk = 0; aiEkstraDef = 0; aiYansitHasari = 0;

        yield return new WaitForSeconds(2.0f);

        if (oyuncuCan <= 0 || aiCan <= 0)
        {
            mevcutDurum = OyunDurumu.Bitis;
            if (oyuncuCan <= 0 && aiCan <= 0) DurumGuncelle("Oyun Bitti: BERABERE!");
            else if (aiCan <= 0) DurumGuncelle("Oyun Bitti: KAZANDIN!");
            else DurumGuncelle("Oyun Bitti: KAYBETTİN!");
        }
        else
        {
            StartCoroutine(YeniTurBaslat());
        }
    }

    void UpdateTumUI() { UpdateCanUI(); UpdateElUI(); UpdateSavasAlaniUI(); UpdateTurUI(); }

    void UpdateCanUI()
    {
        if (oyuncuCanText != null) oyuncuCanText.text = $"CAN: {oyuncuCan}";
        if (aiCanText != null) aiCanText.text = $"CAN: {aiCan}";
        if (oyuncuCanSlider != null) oyuncuCanSlider.value = (float)Mathf.Max(0, oyuncuCan) / maxCan;
        if (aiCanSlider != null) aiCanSlider.value = (float)Mathf.Max(0, aiCan) / maxCan;
    }

    void UpdateElUI()
    {
        foreach (Transform child in oyuncuElAlani) { Destroy(child.gameObject); }
        for (int i = 0; i < oyuncuEli.Count; i++)
        {
            GameObject kartGO = Instantiate(kartPrefab, oyuncuElAlani);
            KartUI kartUI = kartGO.GetComponent<KartUI>();
            kartUI.KartBilgileriniAyarla(oyuncuEli[i], i, this);
            kartUI.SetInteractable(mevcutDurum == OyunDurumu.OyuncuSirasi);
        }

        //foreach (Transform child in aiElAlani) { Destroy(child.gameObject); }
        //for (int i = 0; i < aiEli.Count; i++)
        {
            //Instantiate(kartArkasiPrefab, aiElAlani);
        }
    }

    void UpdateSavasAlaniUI()
    {
        if (oyuncuOynananKart != null)
        {
            oyuncuSavasSlotu.gameObject.SetActive(true);
            oyuncuSavasSlotu.KartBilgileriniAyarla(oyuncuOynananKart, -1, this);
            oyuncuSavasSlotu.SetInteractable(false);
        }
        else { oyuncuSavasSlotu.gameObject.SetActive(false); }

        if (aiOynananKart != null)
        {
            aiSavasSlotu.gameObject.SetActive(true);
            aiSavasSlotu.KartBilgileriniAyarla(aiOynananKart, -1, this);
            aiSavasSlotu.SetInteractable(false);
        }
        else { aiSavasSlotu.gameObject.SetActive(false); }
    }

    void UpdateTurUI()
    {
        if (turText != null) turText.text = $"TUR: {turSayaci}";
        if (zamanlayiciText != null)
        {
            zamanlayiciText.gameObject.SetActive(mevcutDurum == OyunDurumu.OyuncuSirasi);
        }
    }

    public void CanDegistir(bool oyuncuyaMi, int miktar)
    {
        if (oyuncuyaMi) { oyuncuCan = Mathf.Clamp(oyuncuCan + miktar, 0, maxCan); }
        else { aiCan = Mathf.Clamp(aiCan + miktar, 0, maxCan); }
        UpdateCanUI();
    }

    public void GeciciEtkiEkle(bool oyuncuyaMi, string etkiTipi, int deger)
    {
        if (oyuncuyaMi)
        {
            if (etkiTipi == "atk") oyuncuEkstraAtk += deger;
            else if (etkiTipi == "def") oyuncuEkstraDef += deger;
            else if (etkiTipi == "yansit") oyuncuYansitHasari += deger;
        }
        else
        {
            if (etkiTipi == "atk") aiEkstraAtk += deger;
            else if (etkiTipi == "def") aiEkstraDef += deger;
            else if (etkiTipi == "yansit") aiYansitHasari += deger;
        }
    }

    public void ElDegistir()
    {
        List<Kart> tempEl = new List<Kart>(oyuncuEli);
        oyuncuEli = new List<Kart>(aiEli);
        aiEli = tempEl;
        UpdateElUI();
    }

    public void DurumGuncelle(string metin)
    {
        if (durumText != null) durumText.text = metin;
        Debug.Log($"DURUM: {metin}");
    }
}