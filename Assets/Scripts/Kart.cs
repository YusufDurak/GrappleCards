// Kart.cs
using UnityEngine; // [System.Serializable] için gerekli

[System.Serializable] // Bu satır, Kart nesnelerinin Unity Inspector'da görünmesini sağlar.
public class Kart
{
    public string Isim;
    public Nadirlik NadirlikSeviyesi;
    public int Atk;
    public int Savunma;
    public OzelGucTipi OzelGuc;
    public int OzelGucDegeri;

    // Yapıcı Metot (Constructor) - Kod içinden kart oluşturmak için.
public Kart(string isim, Nadirlik nadirlik, int atk, int savunma, OzelGucTipi guc = OzelGucTipi.Yok, int gucDegeri = 0)
    {
        this.Isim = isim;
        this.NadirlikSeviyesi = nadirlik;
        this.Atk = atk;
        this.Savunma = savunma;
        this.OzelGuc = guc; // Parametreden gelen 'guc' kullanıldı.
        this.OzelGucDegeri = gucDegeri; // Parametreden gelen 'gucDegeri' kullanıldı.
    }

    // Kart bilgilerini kolayca yazdırmak için ToString() metodunu override ediyoruz.
    public override string ToString()
    {
        string gucStr = (OzelGuc != OzelGucTipi.Yok) ? $" GÜÇ:{OzelGuc}({OzelGucDegeri})" : "";
        return $"{Isim} ({NadirlikSeviyesi}) [ATK:{Atk} DEF:{Savunma}{gucStr}]";
    }
}