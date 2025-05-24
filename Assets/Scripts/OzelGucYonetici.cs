// OzelGucYonetici.cs
using UnityEngine;

public class OzelGucYonetici : MonoBehaviour
{
    // Bu fonksiyon, bir kart oynandığında OyunYonetici tarafından çağrılacak.
    public void GucUygula(Kart kart, OyunYonetici yonetici, bool oyuncuTarafindanOynandi)
    {
        if (kart.OzelGuc == OzelGucTipi.Yok)
        {
            return; // Güç yoksa bir şey yapma.
        }

        Debug.Log($"ÖZEL GÜÇ TETİKLENDİ: {kart.Isim} ({kart.OzelGuc}) - Oynayan: {(oyuncuTarafindanOynandi ? "Oyuncu" : "AI")}");

        // Güçleri bir switch-case yapısı ile yönetelim.
        switch (kart.OzelGuc)
        {
            case OzelGucTipi.CanYenile:
                yonetici.CanDegistir(oyuncuTarafindanOynandi, kart.OzelGucDegeri);
                yonetici.DurumGuncelle($"{(oyuncuTarafindanOynandi ? "Oyuncu" : "AI")} {kart.OzelGucDegeri} can yeniledi!");
                break;

            case OzelGucTipi.DirektHasar:
                // Rakibe hasar ver. Oyuncu oynadıysa AI'ya, AI oynadıysa Oyuncu'ya.
                yonetici.CanDegistir(!oyuncuTarafindanOynandi, -kart.OzelGucDegeri);
                yonetici.DurumGuncelle($"{(oyuncuTarafindanOynandi ? "Oyuncu" : "AI")} rakibe {kart.OzelGucDegeri} direkt hasar verdi!");
                break;

            case OzelGucTipi.KalkanGucu:
                yonetici.GeciciEtkiEkle(oyuncuTarafindanOynandi, "def", kart.OzelGucDegeri);
                yonetici.DurumGuncelle($"{(oyuncuTarafindanOynandi ? "Oyuncu" : "AI")} sonraki tur {kart.OzelGucDegeri} ekstra savunma kazandı!");
                break;

            case OzelGucTipi.AtakGucu:
                yonetici.GeciciEtkiEkle(oyuncuTarafindanOynandi, "atk", kart.OzelGucDegeri);
                yonetici.DurumGuncelle($"{(oyuncuTarafindanOynandi ? "Oyuncu" : "AI")} sonraki tur {kart.OzelGucDegeri} ekstra saldırı kazandı!");
                break;

            case OzelGucTipi.Yansit:
                yonetici.GeciciEtkiEkle(oyuncuTarafindanOynandi, "yansit", kart.OzelGucDegeri); // OzelGucDegeri yansıtılacak hasar miktarı olsun.
                yonetici.DurumGuncelle($"{(oyuncuTarafindanOynandi ? "Oyuncu" : "AI")} sonraki saldırıda {kart.OzelGucDegeri} hasar yansıtacak!");
                break;

            case OzelGucTipi.DesteDegisim:
                yonetici.ElDegistir();
                yonetici.DurumGuncelle("DESTELER DEĞİŞTİRİLDİ!");
                break;

            // Gelecekte eklenecek diğer güçler buraya gelecek...

            default:
                Debug.LogWarning($"Bilinmeyen veya uygulanmamış özel güç: {kart.OzelGuc}");
                break;
        }
    }
}