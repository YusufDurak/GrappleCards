// OyunYonetici.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI elemanları için (İleride kullanılacak)
using TMPro;
using System.Collections; // Zamanlayıcı ve gecikmeler için

public class OyunYonetici : MonoBehaviour
{
    public KartVeritabani kartVeritabani; // Inspector'dan sürükleyip bırakın

    public List<Kart> oyuncuEli = new List<Kart>();
    public List<Kart> aiEli = new List<Kart>();
    public int oyuncuCan = 30;
    public int aiCan = 30;
    private int maxCan = 30; // Max canı bilmek slider için iyi olur
    private int turSayaci = 0;
    private bool oyuncuBaslar = true; // Başlangıçta oyuncu başlar

    // Oynanan kartları tutacak alanlar
    public Kart oyuncuOynananKart;
    public Kart aiOynananKart;

    // Oyun durumunu yönetmek için enum
    public enum OyunDurumu { Hazirlik, OyuncuSirasi, OyuncuOynadi, AISirasi, Savas, Bitis }
    public OyunDurumu mevcutDurum;

    // Oyuncu zamanlayıcısı
    public float turSuresi = 20.0f;
    private float mevcutSure;
    private bool zamanlayiciAktif = false;

    // --- YENİ UI REFERANSLARI ---
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
    public KartUI oyuncuSavasSlotu; // Savaş alanındaki kartı göstermek için
    public KartUI aiSavasSlotu;     // Savaş alanındaki kartı göstermek için
    public GameObject kartPrefab;   // Oluşturduğumuz KartUI Prefab'ı
    public GameObject kartArkasiPrefab; // AI kartları için basit bir prefab


    void Start() // Oyun başladığında çalışır (Awake'den sonra)
    {
        // KartVeritabani'nı bul (eğer atanmadıysa)
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

        // Başlangıç ellerini temizle ve dağıt
        oyuncuEli.Clear();
        aiEli.Clear();
        KartCek(oyuncuEli, 4);
        KartCek(aiEli, 4);
        UpdateTumUI(); // Başlangıçta tüm UI'ı güncelle

        Debug.Log("--- Oyuncunun Başlangıç Eli ---");
        foreach (Kart kart in oyuncuEli)
        {
            Debug.Log(kart.ToString());
        }

        Debug.Log("--- AI'nın Başlangıç Eli ---");
        foreach (Kart kart in aiEli)
        {
            Debug.Log(kart.ToString());
        }

        // İlk turu başlat
        StartCoroutine(YeniTurBaslat());

    }

    // Belirtilen ele kart çeken fonksiyon
    public void KartCek(List<Kart> el, int adet)
    {
        for (int i = 0; i < adet; i++)
        {
            Kart cekilenKart = kartVeritabani.RastgeleKartCek();
            if (cekilenKart != null)
            {
                el.Add(cekilenKart);
            }
        }
    }

    void Update()
    {
        // Zamanlayıcıyı sadece oyuncu sırasındayken çalıştır
        if (mevcutDurum == OyunDurumu.OyuncuSirasi && zamanlayiciAktif)
        {
            mevcutSure -= Time.deltaTime;
            zamanlayiciText.text = Mathf.CeilToInt(mevcutSure).ToString(); // UI Güncellemesi

            if (mevcutSure <= 0)
            {
                zamanlayiciAktif = false;
                Debug.Log("Süre doldu! Rastgele kart oynanıyor...");
                OyuncuRastgeleKartOyna();
            }
        }

        // --- TEST İÇİN KLAVYE GİRİŞİ ---
        // UI olmadığında test etmek için 1-5 arası tuşlara basarak kart oynama
        /* if (mevcutDurum == OyunDurumu.OyuncuSirasi)
         {
             if (Input.GetKeyDown(KeyCode.Alpha1)) OyuncuKartOyna(0);
             if (Input.GetKeyDown(KeyCode.Alpha2)) OyuncuKartOyna(1);
             if (Input.GetKeyDown(KeyCode.Alpha3)) OyuncuKartOyna(2);
             if (Input.GetKeyDown(KeyCode.Alpha4)) OyuncuKartOyna(3);
             if (Input.GetKeyDown(KeyCode.Alpha5)) OyuncuKartOyna(4);
         }
         // --- /TEST İÇİN KLAVYE GİRİŞİ --- */
    }

    IEnumerator YeniTurBaslat()
    {
        mevcutDurum = OyunDurumu.Hazirlik;
        turSayaci++;
        Debug.Log($"--- TUR {turSayaci} BAŞLIYOR ---");

        oyuncuOynananKart = null;
        aiOynananKart = null;

        UpdateSavasAlaniUI(); // Savaş alanını temizle

        // 5 turda bir başlangıç sırasını değiştir
        if ((turSayaci - 1) % 5 == 0 && turSayaci > 1)
        {
            oyuncuBaslar = !oyuncuBaslar;
            Debug.Log($"Başlangıç sırası değişti! Şimdi {(oyuncuBaslar ? "Oyuncu" : "AI")} başlıyor.");
        }

        // Herkes 1 kart çeker (Elde 5 kart olacak)
        KartCek(oyuncuEli, 1);
        KartCek(aiEli, 1);
        UpdateTumUI(); // Kartları ve tur bilgisini güncelle
        Debug.Log("Kartlar çekildi.");
        EliGoster(oyuncuEli, "Oyuncu"); // Elleri konsola yazdır (Test için)
        // EliGoster(aiEli, "AI"); // AI elini normalde göstermeyiz.

        yield return new WaitForSeconds(1.0f); // Kısa bir bekleme

        if (oyuncuBaslar) OyuncuSirasiniBaslat();
        else StartCoroutine(AISirasiniBaslat());

    }

    void OyuncuSirasiniBaslat()
    {
        Debug.Log("Sıra Oyuncuda.");
        mevcutDurum = OyunDurumu.OyuncuSirasi;
        mevcutSure = turSuresi;
        zamanlayiciAktif = true;
        durumText.text = "Sıra Sende! Kartını Seç.";
        UpdateElUI(); // Butonları aktif etmek için eli güncelle

    }

    IEnumerator AISirasiniBaslat()
    {
        Debug.Log("Sıra Yapay Zekada.");
        mevcutDurum = OyunDurumu.AISirasi;
        durumText.text = "Rakip Düşünüyor...";
        yield return new WaitForSeconds(1.5f); // AI'nın düşünme süresi simülasyonu
        AIHamlesiYap();
    }

    // UI butonları veya test tuşları bu fonksiyonu çağıracak
    public void OyuncuKartOyna(int kartIndex)
    {
        // Sadece oyuncu sırasındaysa ve geçerli bir index ise oyna
        if (mevcutDurum == OyunDurumu.OyuncuSirasi && kartIndex >= 0 && kartIndex < oyuncuEli.Count)
        {
            zamanlayiciAktif = false;
            oyuncuOynananKart = oyuncuEli[kartIndex];
            oyuncuEli.RemoveAt(kartIndex);
            Debug.Log($"Oyuncu oynadı: {oyuncuOynananKart}");

            mevcutDurum = OyunDurumu.OyuncuOynadi;
            durumText.text = "Oyuncu Kart Oynadı.";
            UpdateTumUI(); // Eli ve savaş alanını güncelle

            if (oyuncuBaslar) StartCoroutine(AISirasiniBaslat());
            else StartCoroutine(SavasBaslat());
        }
        else if (mevcutDurum != OyunDurumu.OyuncuSirasi)
        {
            Debug.LogWarning("Şu an oyuncu sırası değil!");
        }
        else
        {
            Debug.LogWarning("Geçersiz kart indexi!");
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
        if (aiEli.Count == 0) return; // Eli boşsa oynayamaz

        Kart enIyiKart = aiEli[0];
        int enYuksekStat = 0;

        // En yüksek tekil stat'a sahip kartı bul
        foreach (Kart kart in aiEli)
        {
            if (kart.Atk > enYuksekStat) { enYuksekStat = kart.Atk; enIyiKart = kart; }
            if (kart.C_Atk > enYuksekStat) { enYuksekStat = kart.C_Atk; enIyiKart = kart; }
            if (kart.Savunma > enYuksekStat) { enYuksekStat = kart.Savunma; enIyiKart = kart; }
        }

        aiOynananKart = enIyiKart;
        aiEli.Remove(enIyiKart);
        Debug.Log($"AI oynadı: {aiOynananKart}");
        durumText.text = "Rakip Kart Oynadı.";
        UpdateTumUI(); // AI elini ve savaş alanını güncelle

        if (!oyuncuBaslar) OyuncuSirasiniBaslat();
        else StartCoroutine(SavasBaslat());

    }

    IEnumerator SavasBaslat()
    {
        mevcutDurum = OyunDurumu.Savas;
        durumText.text = $"{oyuncuOynananKart.Isim} vs {aiOynananKart.Isim}";
        Debug.Log("--- SAVAŞ BAŞLIYOR! ---");
        Debug.Log($"{oyuncuOynananKart.Isim} vs {aiOynananKart.Isim}");

        // Kartların savaş alanında göründüğünden emin ol (UpdateSavasAlaniUI çağrılmalı)
        // UpdateSavasAlaniUI(); // Bu genellikle kartlar oynandığında zaten çağrılır.

        yield return new WaitForSeconds(1.5f); // Kartların çarpışması / animasyon için zaman

        // Formülleri uygula
        int p_eff_atk = Mathf.Max(0, oyuncuOynananKart.Atk - aiOynananKart.C_Atk);
        int ai_eff_atk = Mathf.Max(0, aiOynananKart.Atk - oyuncuOynananKart.C_Atk);

        int ai_hasar = Mathf.Max(0, p_eff_atk - aiOynananKart.Savunma);
        int oyuncu_hasar = Mathf.Max(0, ai_eff_atk - oyuncuOynananKart.Savunma);

        Debug.Log("Hasarlar hesaplandı...");
        durumText.text = "Hasarlar Hesaplanıyor...";

        yield return new WaitForSeconds(1.0f); // Hesaplama efekti / beklemesi

        // Hasarları uygula
        aiCan -= ai_hasar;
        oyuncuCan -= oyuncu_hasar;

        // Hasarları UI'da göster (Basit Yöntem)
        string hasarMetni = "";
        if (ai_hasar > 0) hasarMetni += $"Rakip {ai_hasar} hasar aldı! ";
        if (oyuncu_hasar > 0) hasarMetni += $" {oyuncu_hasar} hasar aldın!";
        if (string.IsNullOrEmpty(hasarMetni)) hasarMetni = "Kimse hasar almadı!";

        durumText.text = hasarMetni;
        Debug.Log($"Oyuncu {ai_hasar} hasar verdi. AI Can: {aiCan}");
        Debug.Log($"AI {oyuncu_hasar} hasar verdi. Oyuncu Can: {oyuncuCan}");

        // Can UI'ını güncelle
        UpdateCanUI();

        // (İleri Seviye: Burada hasar sayılarını ekranda gösterme kodu olabilir)

        yield return new WaitForSeconds(2.0f); // Oyuncunun sonucu görmesi için daha uzun bekleme

        // Oyun sonu kontrolü
        if (oyuncuCan <= 0 && aiCan <= 0)
        {
            Debug.Log("BERABERE!");
            durumText.text = "Oyun Bitti: BERABERE!";
            mevcutDurum = OyunDurumu.Bitis;
            // (İleri Seviye: Oyun sonu ekranını açma kodu)
        }
        else if (aiCan <= 0)
        {
            Debug.Log("OYUNCU KAZANDI!");
            durumText.text = "Oyun Bitti: KAZANDIN!";
            mevcutDurum = OyunDurumu.Bitis;
            // (İleri Seviye: Oyun sonu ekranını açma kodu)
        }
        else if (oyuncuCan <= 0)
        {
            Debug.Log("AI KAZANDI!");
            durumText.text = "Oyun Bitti: KAYBETTİN!";
            mevcutDurum = OyunDurumu.Bitis;
            // (İleri Seviye: Oyun sonu ekranını açma kodu)
        }
        else
        {
            // Oyun bitmediyse yeni tura başla
            durumText.text = "Yeni Tur Başlıyor...";
            yield return new WaitForSeconds(0.5f); // Yeni tur öncesi kısa bekleme
            StartCoroutine(YeniTurBaslat());
        }
    }
    // EliGoster fonksiyonu (Test için)
    public void EliGoster(List<Kart> el, string kimin)
    {
        string elStr = "";
        for (int i = 0; i < el.Count; i++)
        {
            elStr += $"[{i + 1}] {el[i].Isim}  ";
        }
        Debug.Log($"--- {kimin} Eli ({el.Count} Kart) --- \n {elStr}");
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
        if(oyuncuCanText != null) oyuncuCanText.text = $"CAN: {oyuncuCan}";
        if(aiCanText != null) aiCanText.text = $"CAN: {aiCan}";
        if(oyuncuCanSlider != null) oyuncuCanSlider.value = (float)oyuncuCan / maxCan;
        if(aiCanSlider != null) aiCanSlider.value = (float)aiCan / maxCan;
    }

    void UpdateElUI()
    {
        // Oyuncu Elini Temizle ve Yeniden Oluştur
        foreach (Transform child in oyuncuElAlani) { Destroy(child.gameObject); }
        for (int i = 0; i < oyuncuEli.Count; i++)
        {
            GameObject kartGO = Instantiate(kartPrefab, oyuncuElAlani);
            KartUI kartUI = kartGO.GetComponent<KartUI>();
            kartUI.KartBilgileriniAyarla(oyuncuEli[i], i, this);
            // Kartların butonlarını sadece oyuncu sırasındayken aktif et
            kartUI.kartButton.interactable = (mevcutDurum == OyunDurumu.OyuncuSirasi);
        }

        // AI Elini Temizle ve Yeniden Oluştur (Sadece Kart Arkası)
        foreach (Transform child in aiElAlani) { Destroy(child.gameObject); }
        for (int i = 0; i < aiEli.Count; i++)
        {
            Instantiate(kartArkasiPrefab, aiElAlani);
        }
    }

    void UpdateSavasAlaniUI()
    {
        // Oyuncu Slotu
        if (oyuncuOynananKart != null)
        {
            oyuncuSavasSlotu.gameObject.SetActive(true);
            oyuncuSavasSlotu.KartBilgileriniAyarla(oyuncuOynananKart, -1, this); // -1: Oynanamaz
            oyuncuSavasSlotu.kartButton.interactable = false;
        }
        else
        {
            oyuncuSavasSlotu.gameObject.SetActive(false);
        }

        // AI Slotu
        if (aiOynananKart != null)
        {
            aiSavasSlotu.gameObject.SetActive(true);
            aiSavasSlotu.KartBilgileriniAyarla(aiOynananKart, -1, this); // -1: Oynanamaz
            aiSavasSlotu.kartButton.interactable = false;
        }
        else
        {
            aiSavasSlotu.gameObject.SetActive(false);
        }
    }

     void UpdateTurUI()
     {
         if(turText != null) turText.text = $"TUR: {turSayaci}";
         if(zamanlayiciText != null && mevcutDurum == OyunDurumu.OyuncuSirasi)
            zamanlayiciText.gameObject.SetActive(true);
         else if(zamanlayiciText != null)
            zamanlayiciText.gameObject.SetActive(false);
     }

}