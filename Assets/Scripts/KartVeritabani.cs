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
        anaKartHavuzu.Add(new Kart("Sıradan Asker", Nadirlik.CokYaygin, 2, 1, 1));
        anaKartHavuzu.Add(new Kart("Tahta Kalkan", Nadirlik.CokYaygin, 1, 0, 3));
        anaKartHavuzu.Add(new Kart("Sinsi Hançer", Nadirlik.CokYaygin, 3, 0, 1));
        anaKartHavuzu.Add(new Kart("Acemi Büyücü", Nadirlik.CokYaygin, 2, 2, 0));
        anaKartHavuzu.Add(new Kart("Kaya Fırlatıcı", Nadirlik.CokYaygin, 2, 1, 2));

        anaKartHavuzu.Add(new Kart("Demir Zırhlı", Nadirlik.Yaygin, 3, 1, 5));
        anaKartHavuzu.Add(new Kart("Savaşçı Kılıcı", Nadirlik.Yaygin, 5, 2, 2));
        anaKartHavuzu.Add(new Kart("Parazit Vuruşu", Nadirlik.Yaygin, 4, 4, 1));
        anaKartHavuzu.Add(new Kart("Şifacı Keşiş", Nadirlik.Yaygin, 1, 3, 4));
        anaKartHavuzu.Add(new Kart("Ork Baltacısı", Nadirlik.Yaygin, 6, 1, 2));

        anaKartHavuzu.Add(new Kart("Dev Kalkanı", Nadirlik.Nadir, 2, 3, 8));
        anaKartHavuzu.Add(new Kart("Ejderha Nefesi", Nadirlik.Nadir, 8, 2, 3));
        anaKartHavuzu.Add(new Kart("Ayna Büyüsü", Nadirlik.Nadir, 3, 8, 2));
        anaKartHavuzu.Add(new Kart("Efsanevi Savaşçı", Nadirlik.Nadir, 6, 5, 6));
        anaKartHavuzu.Add(new Kart("Kara Şövalye", Nadirlik.Nadir, 7, 4, 5));

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