// OyunYonetici.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI elemanları için (İleride kullanılacak)
using System.Collections; // Zamanlayıcı ve gecikmeler için

public class OyunYonetici : MonoBehaviour
{
    public KartVeritabani kartVeritabani; // Inspector'dan sürükleyip bırakın

    public List<Kart> oyuncuEli = new List<Kart>();
    public List<Kart> aiEli = new List<Kart>();

    // --- YENİ DEĞİŞKENLER ---
    public int oyuncuCan = 30;
    public int aiCan = 30;

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

    // UI Referansları (Şimdilik boş, sonra eklenecek)
    // public Text oyuncuCanText;
    // public Text aiCanText;
    // public Text turText;
    // public Text zamanlayiciText;
    // public GameObject oyuncuKartAlani;
    // public GameObject aiKartAlani;

    // --- /YENİ DEĞİŞKENLER ---

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
            // zamanlayiciText.text = Mathf.CeilToInt(mevcutSure).ToString(); // UI Güncellemesi

            if (mevcutSure <= 0)
            {
                zamanlayiciAktif = false;
                Debug.Log("Süre doldu! Rastgele kart oynanıyor...");
                OyuncuRastgeleKartOyna();
            }
        }

        // --- TEST İÇİN KLAVYE GİRİŞİ ---
        // UI olmadığında test etmek için 1-5 arası tuşlara basarak kart oynama
        if (mevcutDurum == OyunDurumu.OyuncuSirasi)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) OyuncuKartOyna(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) OyuncuKartOyna(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) OyuncuKartOyna(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) OyuncuKartOyna(3);
            if (Input.GetKeyDown(KeyCode.Alpha5)) OyuncuKartOyna(4);
        }
        // --- /TEST İÇİN KLAVYE GİRİŞİ ---
    }

    IEnumerator YeniTurBaslat()
    {
        mevcutDurum = OyunDurumu.Hazirlik;
        turSayaci++;
        Debug.Log($"--- TUR {turSayaci} BAŞLIYOR ---");

        oyuncuOynananKart = null;
        aiOynananKart = null;

        // 5 turda bir başlangıç sırasını değiştir
        if ((turSayaci - 1) % 5 == 0 && turSayaci > 1)
        {
            oyuncuBaslar = !oyuncuBaslar;
            Debug.Log($"Başlangıç sırası değişti! Şimdi {(oyuncuBaslar ? "Oyuncu" : "AI")} başlıyor.");
        }

        // Herkes 1 kart çeker (Elde 5 kart olacak)
        KartCek(oyuncuEli, 1);
        KartCek(aiEli, 1);
        Debug.Log("Kartlar çekildi.");
        EliGoster(oyuncuEli, "Oyuncu"); // Elleri konsola yazdır (Test için)
        // EliGoster(aiEli, "AI"); // AI elini normalde göstermeyiz.

        yield return new WaitForSeconds(1.0f); // Kısa bir bekleme

        if (oyuncuBaslar)
        {
            OyuncuSirasiniBaslat();
        }
        else
        {
            StartCoroutine(AISirasiniBaslat());
        }
    }

    void OyuncuSirasiniBaslat()
    {
        Debug.Log("Sıra Oyuncuda.");
        mevcutDurum = OyunDurumu.OyuncuSirasi;
        mevcutSure = turSuresi;
        zamanlayiciAktif = true;
        // UI'da oyuncuya sıra sende mesajı gösterilebilir.
    }

    IEnumerator AISirasiniBaslat()
    {
        Debug.Log("Sıra Yapay Zekada.");
        mevcutDurum = OyunDurumu.AISirasi;
        yield return new WaitForSeconds(1.5f); // AI'nın düşünme süresi simülasyonu
        AIHamlesiYap();
    }

    // ... (Önceki kodlar) ...

    // UI butonları veya test tuşları bu fonksiyonu çağıracak
    public void OyuncuKartOyna(int kartIndex)
    {
        // Sadece oyuncu sırasındaysa ve geçerli bir index ise oyna
        if (mevcutDurum == OyunDurumu.OyuncuSirasi && kartIndex >= 0 && kartIndex < oyuncuEli.Count)
        {
            zamanlayiciAktif = false; // Zamanlayıcıyı durdur
            oyuncuOynananKart = oyuncuEli[kartIndex];
            oyuncuEli.RemoveAt(kartIndex);
            Debug.Log($"Oyuncu oynadı: {oyuncuOynananKart}");
            mevcutDurum = OyunDurumu.OyuncuOynadi;

            // Sıra kimdeydi?
            if (oyuncuBaslar)
            {
                // Oyuncu başladıysa, şimdi AI oynar
                StartCoroutine(AISirasiniBaslat());
            }
            else
            {
                // AI başladıysa, şimdi Savaş zamanı
                StartCoroutine(SavasBaslat());
            }
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
        aiEli.Remove(enIyiKart); // Kartı elden çıkar
        Debug.Log($"AI oynadı: {aiOynananKart}");

        // Sıra kimdeydi?
        if (!oyuncuBaslar)
        {
            // AI başladıysa, şimdi Oyuncu oynar
            OyuncuSirasiniBaslat();
        }
        else
        {
            // Oyuncu başladıysa, şimdi Savaş zamanı
            StartCoroutine(SavasBaslat());
        }
    }
    
    // ... (Önceki kodlar) ...

    IEnumerator SavasBaslat()
    {
        mevcutDurum = OyunDurumu.Savas;
        Debug.Log("--- SAVAŞ BAŞLIYOR! ---");
        Debug.Log($"{oyuncuOynananKart.Isim} vs {aiOynananKart.Isim}");

        yield return new WaitForSeconds(1.5f); // Savaş animasyonu için zaman (varsayım)

        // Formülleri uygula
        int p_eff_atk = Mathf.Max(0, oyuncuOynananKart.Atk - aiOynananKart.C_Atk);
        int ai_eff_atk = Mathf.Max(0, aiOynananKart.Atk - oyuncuOynananKart.C_Atk);

        int ai_hasar = Mathf.Max(0, p_eff_atk - aiOynananKart.Savunma);
        int oyuncu_hasar = Mathf.Max(0, ai_eff_atk - oyuncuOynananKart.Savunma);

        // Hasarları uygula ve logla
        aiCan -= ai_hasar;
        oyuncuCan -= oyuncu_hasar;

        Debug.Log($"Oyuncu {ai_hasar} hasar verdi. AI Can: {aiCan}");
        Debug.Log($"AI {oyuncu_hasar} hasar verdi. Oyuncu Can: {oyuncuCan}");

        // UI Güncellemeleri
        // oyuncuCanText.text = oyuncuCan.ToString();
        // aiCanText.text = aiCan.ToString();

        yield return new WaitForSeconds(1.5f); // Sonucu görmek için bekleme

        // Oyun sonu kontrolü
        if (oyuncuCan <= 0 && aiCan <= 0)
        {
            Debug.Log("BERABERE!");
            mevcutDurum = OyunDurumu.Bitis;
            // Oyun sonu ekranı göster
        }
        else if (aiCan <= 0)
        {
            Debug.Log("OYUNCU KAZANDI!");
            mevcutDurum = OyunDurumu.Bitis;
            // Oyun sonu ekranı göster
        }
        else if (oyuncuCan <= 0)
        {
            Debug.Log("AI KAZANDI!");
            mevcutDurum = OyunDurumu.Bitis;
            // Oyun sonu ekranı göster
        }
        else
        {
            // Oyun bitmediyse yeni tura başla
            StartCoroutine(YeniTurBaslat());
        }
    }

    // EliGoster fonksiyonu (Test için)
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