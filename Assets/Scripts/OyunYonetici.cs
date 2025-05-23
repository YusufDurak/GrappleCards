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

        // Başlangıç ellerini dağıt
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

    // Elimizdeki kartları (örn: 5 kart) göstermek için
    public void EliGoster(List<Kart> el, string kimin)
    {
         Debug.Log($"--- {kimin} Eli ({el.Count} Kart) ---");
         foreach (Kart kart in el)
         {
            Debug.Log(kart.ToString());
         }
    }
}