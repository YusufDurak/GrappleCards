// KartUI.cs
using UnityEngine;
using UnityEngine.UI;           // Image için gerekli
using TMPro;                  // TextMeshPro için gerekli
using UnityEngine.EventSystems; // Tıklama olaylarını yakalamak için EKLENDİ

// IPointerClickHandler arayüzünü ekliyoruz. Bu, nesnenin tıklamaları algılamasını sağlar.
public class KartUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI isimText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI gucText;
    
    public Image kartGrafik;      // İsteğe bağlı: Kartın ana görselini atayabilirsiniz.
    public Image raycastHedefi;   // ÖNEMLİ: Tıklamaları algılayacak Image. Inspector'da Raycast Target açık olmalı.

    // Button referansı KALDIRILDI.

    private Kart _kartVerisi;
    private int _index;
    private OyunYonetici _oyunYonetici;
    private bool _secimModu = false;
    private bool _interactable = true; // Kartın tıklanabilir olup olmadığını tutar.

    // El kartları için
    public void KartBilgileriniAyarla(Kart kart, int index, OyunYonetici yonetici)
    {
        _secimModu = false;
        Ayarla(kart, index, yonetici);
    }

    // Seçim kartları için
    public void SecimKartBilgileriniAyarla(Kart kart, OyunYonetici yonetici)
    {
        _secimModu = true;
        Ayarla(kart, -1, yonetici);
    }

    // Ortak ayarlama fonksiyonu
    private void Ayarla(Kart kart, int index, OyunYonetici yonetici)
    {
        _kartVerisi = kart;
        _index = index;
        _oyunYonetici = yonetici;

        if (kart == null)
        {
            Debug.LogError("Ayarla fonksiyonuna null kart geldi!");
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        isimText.text = kart.Isim;
        atkText.text = $"ATK: {kart.Atk}";
        defText.text = $"DEF: {kart.Savunma}";
        gucText.text = (kart.OzelGuc != OzelGucTipi.Yok) ? $"GÜÇ: {kart.OzelGuc}" : ""; 
        

        // Button ile ilgili satırlar KALDIRILDI.
        // Tıklanabilirliği başlangıçta true yapalım, Yönetici gerektiğinde değiştirir.
        SetInteractable(true);
    }

    /// <summary>
    /// Bu kartın tıklanabilir olup olmadığını ayarlar ve görsel geri bildirim verir.
    /// </summary>
    /// <param name="durum">True ise tıklanabilir, False ise tıklanamaz.</param>
    public void SetInteractable(bool durum)
    {
        _interactable = durum;

        // Tıklamaları yakalayan Image elemanının rengini değiştirerek
        // oyuncuya görsel geri bildirim sağlıyoruz.
        if (raycastHedefi != null)
        {
            // Tıklanabilirse normal renk, değilse soluk renk.
            raycastHedefi.color = durum ? Color.white : new Color(0.7f, 0.7f, 0.7f, 0.8f);
        }
    }

    /// <summary>
    /// IPointerClickHandler arayüzünden gelen bu fonksiyon,
    /// Event Sistemi tarafından bu UI elemanına tıklandığında çağrılır.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Sadece tıklanabilir (_interactable == true) ise ve
        // OyunYonetici referansı varsa devam et.
        if (!_interactable || _oyunYonetici == null)
        {
            return;
        }

        // Tıklandığını konsola yazarak test edebiliriz.
        Debug.Log($"{_kartVerisi.Isim} tıklandı! Mod: {(_secimModu ? "Seçim" : "Oynama")}");

        // Mod'a göre OyunYonetici'deki ilgili fonksiyonu çağır.
        if (_secimModu)
        {
            _oyunYonetici.KartSecildi(_kartVerisi); // Seçim panelindeyse bunu çağır.
        }
        else
        {
            _oyunYonetici.OyuncuKartOyna(_index); // Oyuncu elindeyse bunu çağır.
        }
    }
}