// KartUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class KartUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI isimText;
    public Image kartResmi; // <-- YENİ: Kart görselini gösterecek Image.
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI gucText;
    public Image raycastHedefi;

    // ... (Diğer değişkenler) ...
    private Kart _kartVerisi;
    private int _index;
    private OyunYonetici _oyunYonetici;
    private bool _secimModu = false;
    private bool _interactable = true;

    // ... (KartBilgileriniAyarla ve SecimKartBilgileriniAyarla) ...
    public void KartBilgileriniAyarla(Kart kart, int index, OyunYonetici yonetici)
    { _secimModu = false; Ayarla(kart, index, yonetici); }
    public void SecimKartBilgileriniAyarla(Kart kart, OyunYonetici yonetici)
    { _secimModu = true; Ayarla(kart, -1, yonetici); }


    private void Ayarla(Kart kart, int index, OyunYonetici yonetici)
    {
        // ... (Mevcut kod) ...
        _kartVerisi = kart;
        _index = index;
        _oyunYonetici = yonetici;

        if (kart == null) { /* Hata ve gizleme */ gameObject.SetActive(false); return; }

        gameObject.SetActive(true);
        isimText.text = kart.Isim;
        atkText.text = $"ATK: {kart.Atk}";
        defText.text = $"DEF: {kart.Savunma}";
        gucText.text = (kart.OzelGuc != OzelGucTipi.Yok) ? $"GÜÇ: {kart.OzelGuc}" : "";

        // --- YENİ: Görseli Ayarla ---
        if (kartResmi != null && kart.Gorsel != null)
        {
            kartResmi.sprite = kart.Gorsel;
            kartResmi.color = Color.white; // Görünür yap
        }
        else if(kartResmi != null)
        {
            // Eğer görsel atanmamışsa, boş veya varsayılan bir sprite ata
            // veya Image'ı gizle/şeffaf yap.
            kartResmi.sprite = null;
            kartResmi.color = Color.clear; // Şeffaf yap
        }
        // -----------------------------

        SetInteractable(true);
    }

    // ... (SetInteractable ve OnPointerClick) ...
     public void SetInteractable(bool durum) { /* ... */ }
     public void OnPointerClick(PointerEventData eventData) { /* ... */ }
}