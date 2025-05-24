// KartVeritabani.cs
using System.Collections.Generic;
using UnityEngine;

public class KartVeritabani : MonoBehaviour
{
    // public List<Kart> anaKartHavuzu; // Editörden doldurmak için

    // VEYA kod içinde doldurmak için (Python örneğindeki gibi):
    public List<Kart> anaKartHavuzu = new List<Kart>();

    void Awake() // Oyun başladığında ilk çalışan fonksiyonlardan biri
    {
        // Eğer listeyi kod içinde dolduracaksak burada yapabiliriz.
        // Eğer editörden doldurduysak bu kısma gerek yok.
        if (anaKartHavuzu.Count == 0)
        {
            Doldur();
        }
    }

    void Doldur()
    {
        anaKartHavuzu.Clear(); // Önce temizle

        // Çok Yaygın
        anaKartHavuzu.Add(new Kart("Sıradan Asker", Nadirlik.CokYaygin, 3, 2)); // ATK:3, DEF:2, Güç: Yok
        anaKartHavuzu.Add(new Kart("Tahta Kalkan", Nadirlik.CokYaygin, 1, 4)); // ATK:1, DEF:4, Güç: Yok
        anaKartHavuzu.Add(new Kart("Küçük İksir", Nadirlik.CokYaygin, 0, 1, OzelGucTipi.CanYenile, 3)); // ATK:0, DEF:1, Güç: 3 Can Yenile

        // Yaygın
        anaKartHavuzu.Add(new Kart("Demir Zırhlı", Nadirlik.Yaygin, 4, 6)); // ATK:4, DEF:6, Güç: Yok
        anaKartHavuzu.Add(new Kart("Savaşçı Kılıcı", Nadirlik.Yaygin, 6, 3)); // ATK:6, DEF:3, Güç: Yok
        anaKartHavuzu.Add(new Kart("Ateş Topu", Nadirlik.Yaygin, 2, 2, OzelGucTipi.DirektHasar, 4)); // ATK:2, DEF:2, Güç: 4 Direkt Hasar

        // Nadir
        anaKartHavuzu.Add(new Kart("Dev Kalkanı", Nadirlik.Nadir, 3, 9)); // ATK:3, DEF:9, Güç: Yok
        anaKartHavuzu.Add(new Kart("Ejderha Nefesi", Nadirlik.Nadir, 9, 4)); // ATK:9, DEF:4, Güç: Yok
        anaKartHavuzu.Add(new Kart("Kutsal Işık", Nadirlik.Nadir, 1, 3, OzelGucTipi.CanYenile, 8)); // ATK:1, DEF:3, Güç: 8 Can Yenile
        anaKartHavuzu.Add(new Kart("Büyü Kitabı", Nadirlik.Nadir, 2, 2, OzelGucTipi.KartCek, 1)); // ATK:2, DEF:2, Güç: 1 Kart Çek


        Debug.Log("Kart Veritabanı Dolduruldu!");
    }

    // Rastgele kart çekme fonksiyonunu da buraya ekleyebiliriz.
    public Kart RastgeleKartCek()
    {
        if (anaKartHavuzu == null || anaKartHavuzu.Count == 0)
        {
            Debug.LogError("Kart havuzu boş veya atanmamış!");
            return null;
        }

        // Ağırlıklı çekme sonra eklenebilir, şimdilik tamamen rastgele.
        int randomIndex = Random.Range(0, anaKartHavuzu.Count);
        return anaKartHavuzu[randomIndex];
    }
}