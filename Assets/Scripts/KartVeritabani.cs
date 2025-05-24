// KartVeritabani.cs
using System.Collections.Generic;
using UnityEngine;

public class KartVeritabani : MonoBehaviour
{
    // Listeyi public yapıyoruz. Unity Inspector'dan bu listeyi dolduracağız.
    public List<Kart> anaKartHavuzu = new List<Kart>();

    void Awake()
    {
        if (anaKartHavuzu == null || anaKartHavuzu.Count == 0)
        {
            Debug.LogError("DİKKAT: Kart Veritabanı (anaKartHavuzu) Inspector'dan doldurulmamış!");
            // İsteğe bağlı: Hata durumunda test kartları yüklemek için
            // DoldurTestKartlari(); // şeklinde bir fonksiyon çağrılabilir.
        }
    }

    // Rastgele kart çekme fonksiyonu (Değişiklik yok)
    public Kart RastgeleKartCek()
    {
        if (anaKartHavuzu == null || anaKartHavuzu.Count == 0)
        {
            Debug.LogError("Kart havuzu boş!");
            return null; // Veya bir varsayılan kart döndür
        }
        int randomIndex = Random.Range(0, anaKartHavuzu.Count);
        return anaKartHavuzu[randomIndex];
    }
}