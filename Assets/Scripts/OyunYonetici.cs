// OyunYonetici.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Slider için gerekli
using TMPro;
using System.Collections;
// using UnityEngine.EventSystems; // Bu script'te doğrudan gerekmiyor ama UI çalışması için sahnede olmalı.

public class OyunYonetici : MonoBehaviour
{
    public KartVeritabani kartVeritabani;

    public List<Kart> oyuncuEli = new List<Kart>();
    public List<Kart> aiEli = new List<Kart>();

    public int oyuncuCan = 30;
    public int aiCan = 30;
    private int maxCan = 30;

    private int turSayaci = 0;
    private bool oyuncuBaslar = true;

    public Kart oyuncuOynananKart;
    public Kart aiOynananKart;

    public enum OyunDurumu { Hazirlik, KartSecimi, OyuncuSirasi, OyuncuOynadi, AISirasi, Savas, Bitis }
    public OyunDurumu mevcutDurum;

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
    public Transform aiElAlani;
    public KartUI oyuncuSavasSlotu;
    public KartUI aiSavasSlotu;
    public GameObject kartPrefab;
    public GameObject kartArkasiPrefab;

    [Header("Seçim Paneli")]
    public GameObject secimPaneli;
    public KartUI[] secimKartSlotlari = new KartUI[3];
    private List<Kart> secimIcinKartlar = new List<Kart>();

    void Start()
    {
        if (kartVeritabani == null)
        {
            kartVeritabani = FindObjectOfType<KartVeritabani>();
        }

        if (kartVeritabani != null)
        {
            OyunuBaslat();
        }
        else
        {
            Debug.LogError("Kart Veritabanı bulunamadı!");
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

        oyuncuEli.Clear();
        aiEli.Clear();

        // Başlangıçta AI için 4 kart çek (Oyuncu seçerek alacak)
        KartCekAI(4); // Sadece AI için kart çeken yeni fonksiyon
        // Oyuncu için başlangıçta 4 kart seçtir
        OyuncuBaslangicKartSeciminiBaslat(4); // Oyuncuya başlangıçta 4 kart seçtiren fonksiyon
    }

    // AI için kart çeken basit fonksiyon
    void KartCekAI(int adet)
    {
        for (int i = 0; i < adet; i++)
        {
             Kart cekilenKart = kartVeritabani.RastgeleKartCek();
             if (cekilenKart != null) aiEli.Add(cekilenKart);
        }
    }

    // Oyuncunun başlangıçta veya tur başında kart seçmesini yönetir.
    // Başlangıçta 4, tur başında 1 kart seçer.
    void OyuncuBaslangicKartSeciminiBaslat(int kartSayisi)
    {
        // Bu fonksiyon şimdilik sadece 1 kart seçtiriyor,
        // başlangıçta 4 kart seçtirme daha karmaşık bir UI gerektirebilir.
        // Şimdilik başlangıçta da 1 seçtirip 4'e tamamlayalım veya direkt 4 verelim.
        // En kolayı: Başlangıçta 4 kartı direkt verelim, tur başında 1 seçtirelim.
        Debug.Log("Başlangıç kartları oyuncuya veriliyor (şimdilik rastgele).");
        oyuncuEli.Clear();
        for(int i = 0; i < 4; i++) {
            oyuncuEli.Add(kartVeritabani.RastgeleKartCek());
        }
        UpdateTumUI();
        StartCoroutine(YeniTurBaslat()); // Direkt ilk turu başlat
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
                Debug.Log("Süre doldu! Rastgele kart oynanıyor...");
                OyuncuRastgeleKartOyna();
            }
        }
    }

    IEnumerator YeniTurBaslat()
    {
        // Eğer oyun bittiyse yeni tur başlatma
        if(mevcutDurum == OyunDurumu.Bitis) yield break;

        mevcutDurum = OyunDurumu.Hazirlik;
        turSayaci++;
        Debug.Log($"--- TUR {turSayaci} BAŞLIYOR ---");

        oyuncuOynananKart = null;
        aiOynananKart = null;
        UpdateSavasAlaniUI();

        if ((turSayaci - 1) % 5 == 0 && turSayaci > 1)
        {
            oyuncuBaslar = !oyuncuBaslar;
            Debug.Log($"Başlangıç sırası değişti! Şimdi {(oyuncuBaslar ? "Oyuncu" : "AI")} başlıyor.");
        }

        // AI kartını her zaman çeker
        KartCekAI(1);
        Debug.Log("AI kart çekti.");
        UpdateElUI(); // AI el sayısını güncellemek için

        Debug.Log("Oyuncu kart seçme aşaması başlıyor...");
        OyuncuKartSeciminiBaslat(); // Oyuncu 3 karttan 1'ini seçecek

        // Akış KartSecildi fonksiyonundan devam edecek.
        yield return null;
    }

    void OyuncuKartSeciminiBaslat()
    {
        mevcutDurum = OyunDurumu.KartSecimi;
        durumText.text = "3 Karttan Birini Seç!";
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
                secimKartSlotlari[i].SetInteractable(true); // <-- DEĞİŞTİ
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

        // Tur sırasına göre devam et
        if (oyuncuBaslar)
        {
            OyuncuSirasiniBaslat();
        }
        else
        {
            // Eğer AI başladıysa ve oyuncu şimdi kart seçtiyse,
            // AI zaten oynamış olmalı. Bu akış gözden geçirilmeli.
            // Şimdiki akış: Her tur başında AI çeker, sonra oyuncu seçer.
            // Sonra kim başlıyorsa o oynar.
            StartCoroutine(AISirasiniBaslat());
        }
    }

    void OyuncuSirasiniBaslat()
    {
        Debug.Log("Sıra Oyuncuda.");
        mevcutDurum = OyunDurumu.OyuncuSirasi;
        mevcutSure = turSuresi;
        zamanlayiciAktif = true;
        durumText.text = "Sıra Sende! Kartını Seç.";
        UpdateElUI(); // Kartları tıklanabilir yapmak için.
    }

    IEnumerator AISirasiniBaslat()
    {
        Debug.Log("Sıra Yapay Zekada.");
        mevcutDurum = OyunDurumu.AISirasi;
        durumText.text = "Rakip Düşünüyor...";
        UpdateElUI(); // Oyuncu kartlarını tıklanamaz yapmak için.
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

            mevcutDurum = OyunDurumu.OyuncuOynadi;
            durumText.text = "Oyuncu Kart Oynadı.";
            UpdateTumUI();

            if (oyuncuBaslar) StartCoroutine(AISirasiniBaslat());
            else StartCoroutine(SavasBaslat());
        }
        // ... (Hata mesajları) ...
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
             // AI kart oynayamıyorsa ne olacak? Şimdilik sırayı oyuncuya verelim veya savaşı başlatalım.
             // Eğer oyuncu başladıysa savaşı başlatalım (AI pas geçmiş gibi)
             if (oyuncuBaslar) StartCoroutine(SavasBaslat());
             else OyuncuSirasiniBaslat(); // Eğer AI başladıysa sıra oyuncuya geçer.
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
        durumText.text = "Rakip Kart Oynadı.";
        UpdateTumUI();

        if (!oyuncuBaslar) OyuncuSirasiniBaslat();
        else StartCoroutine(SavasBaslat());
    }

    IEnumerator SavasBaslat()
    {
       // Eğer bir taraf kart oynamadıysa (örneğin AI'nın kartı bitti)
       // bunu burada ele almamız gerekebilir. Şimdilik ikisinin de oynadığını varsayıyoruz.
       if(oyuncuOynananKart == null || aiOynananKart == null)
       {
           Debug.LogError("Savaş başlatılamadı, bir oyuncu kart oynamadı!");
           StartCoroutine(YeniTurBaslat()); // Hata durumunda yeni tura geç.
           yield break;
       }

        mevcutDurum = OyunDurumu.Savas;
        durumText.text = $"{oyuncuOynananKart.Isim} vs {aiOynananKart.Isim}";
        Debug.Log("--- SAVAŞ BAŞLIYOR! ---");

        yield return new WaitForSeconds(1.5f);

        int ai_hasar = Mathf.Max(0, oyuncuOynananKart.Atk - aiOynananKart.Savunma);
        int oyuncu_hasar = Mathf.Max(0, aiOynananKart.Atk - oyuncuOynananKart.Savunma);

        durumText.text = "Hasarlar Hesaplanıyor...";
        yield return new WaitForSeconds(1.0f);

        aiCan -= ai_hasar;
        oyuncuCan -= oyuncu_hasar;

        string hasarMetni = $"Rakip {ai_hasar} hasar aldı! {oyuncu_hasar} hasar aldın!";
        durumText.text = hasarMetni;
        Debug.Log($"Oyuncu {ai_hasar} vurdu. AI Can: {aiCan} | AI {oyuncu_hasar} vurdu. Oyuncu Can: {oyuncuCan}");

        UpdateCanUI();
        yield return new WaitForSeconds(2.0f);

        // Oyun sonu kontrolü
        if (oyuncuCan <= 0 || aiCan <= 0)
        {
             if (oyuncuCan <= 0 && aiCan <= 0) { durumText.text = "Oyun Bitti: BERABERE!"; }
             else if (aiCan <= 0) { durumText.text = "Oyun Bitti: KAZANDIN!"; }
             else { durumText.text = "Oyun Bitti: KAYBETTİN!"; }
             mevcutDurum = OyunDurumu.Bitis;
             // Burada oyun sonu panelini aktif edebiliriz.
        }
        else
        {
            durumText.text = "Yeni Tur Başlıyor...";
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(YeniTurBaslat());
        }
    }

    void UpdateTumUI()
    {
        UpdateCanUI();
        UpdateElUI();
        UpdateSavasAlaniUI();
        UpdateTurUI();
    }

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
            kartUI.SetInteractable(mevcutDurum == OyunDurumu.OyuncuSirasi); // <-- DEĞİŞTİ
        }

        foreach (Transform child in aiElAlani) { Destroy(child.gameObject); }
        for (int i = 0; i < aiEli.Count; i++)
        {
            Instantiate(kartArkasiPrefab, aiElAlani);
        }
    }

    void UpdateSavasAlaniUI()
    {
        if (oyuncuOynananKart != null)
        {
            oyuncuSavasSlotu.gameObject.SetActive(true);
            oyuncuSavasSlotu.KartBilgileriniAyarla(oyuncuOynananKart, -1, this);
            oyuncuSavasSlotu.SetInteractable(false); // <-- DEĞİŞTİ
        }
        else
        {
            oyuncuSavasSlotu.gameObject.SetActive(false);
        }

        if (aiOynananKart != null)
        {
            aiSavasSlotu.gameObject.SetActive(true);
            aiSavasSlotu.KartBilgileriniAyarla(aiOynananKart, -1, this);
            aiSavasSlotu.SetInteractable(false); // <-- DEĞİŞTİ
        }
        else
        {
            aiSavasSlotu.gameObject.SetActive(false);
        }
    }

    void UpdateTurUI()
    {
        if (turText != null) turText.text = $"TUR: {turSayaci}";
        if (zamanlayiciText != null)
        {
             zamanlayiciText.gameObject.SetActive(mevcutDurum == OyunDurumu.OyuncuSirasi);
        }
    }

    // EliGoster fonksiyonu (Debug için)
    public void EliGoster(List<Kart> el, string kimin)
    {
        string elStr = "";
        for(int i = 0; i < el.Count; i++)
        {
            elStr += $"[{i+1}] {el[i].Isim}  ";
        }
         Debug.Log($"--- {kimin} Eli ({el.Count} Kart) --- \n {elStr}");
    }
}