// KartVeritabani.cs
using System.Collections.Generic;
using UnityEngine;

public class KartVeritabani : MonoBehaviour
{
    // Listeyi public yapıyoruz. Unity Inspector'dan bu listeyi dolduracağız.
    public List<Kart> anaKartHavuzu = new List<Kart>();
    public Kart defaultKart; // Reference to a default card ScriptableObject

    private void Awake()
    {
        ValidateCardPool();
    }

    private void ValidateCardPool()
    {
        if (anaKartHavuzu == null || anaKartHavuzu.Count == 0)
        {
            Debug.LogError("DİKKAT: Kart Veritabanı (anaKartHavuzu) Inspector'dan doldurulmamış!");
            CreateDefaultCardPool();
        }
        else
        {
            // Validate each card
            for (int i = 0; i < anaKartHavuzu.Count; i++)
            {
                if (!anaKartHavuzu[i].IsValid())
                {
                    Debug.LogError($"Geçersiz kart bulundu: {anaKartHavuzu[i].Isim}");
                    if (defaultKart != null)
                    {
                        anaKartHavuzu[i] = defaultKart;
                    }
                    else
                    {
                        Debug.LogError("Default kart atanmamış! Lütfen Inspector'dan bir default kart atayın.");
                    }
                }
            }
        }
    }

    private void CreateDefaultCardPool()
    {
        if (defaultKart == null)
        {
            Debug.LogError("Default kart atanmamış! Lütfen Inspector'dan bir default kart atayın.");
            return;
        }

        anaKartHavuzu = new List<Kart>
        {
            defaultKart,
            defaultKart,
            defaultKart
        };
        Debug.Log("Varsayılan kart havuzu oluşturuldu.");
    }

    // Rastgele kart çekme fonksiyonu (Değişiklik yok)
    public Kart RastgeleKartCek()
    {
        if (anaKartHavuzu == null || anaKartHavuzu.Count == 0)
        {
            Debug.LogError("Kart havuzu boş!");
            return defaultKart;
        }

        try
        {
            int randomIndex = Random.Range(0, anaKartHavuzu.Count);
            var card = anaKartHavuzu[randomIndex];

            if (!card.IsValid())
            {
                Debug.LogWarning($"Geçersiz kart çekildi: {card.Isim}. Varsayılan kart kullanılıyor.");
                return defaultKart;
            }

            return card;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Kart çekilirken hata oluştu: {e.Message}");
            return defaultKart;
        }
    }
}