// KartUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KartUI : MonoBehaviour
{
    public TextMeshProUGUI isimText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI cAtkText;
    public TextMeshProUGUI defText;
    public Button kartButton;

    private Kart _kartVerisi;
    private int _elIndex; // Elimizdeki kaçıncı kart olduğunu bilmek için
    private OyunYonetici _oyunYonetici;

    public void KartBilgileriniAyarla(Kart kart, int index, OyunYonetici yonetici)
    {
        _kartVerisi = kart;
        _elIndex = index;
        _oyunYonetici = yonetici;

        isimText.text = kart.Isim;
        atkText.text = $"ATK: {kart.Atk}";
        cAtkText.text = $"C-ATK: {kart.C_Atk}";
        defText.text = $"DEF: {kart.Savunma}";

        // Butonun tıklama olayını ayarla
        kartButton.onClick.RemoveAllListeners(); // Önceki listenerları temizle
        kartButton.onClick.AddListener(KartOynaTiklandi);
    }

    void KartOynaTiklandi()
    {
        // Oyun Yöneticisine bu kartın oynanmak istendiğini bildir.
        _oyunYonetici.OyuncuKartOyna(_elIndex);
    }
}