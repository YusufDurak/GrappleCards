// Kart.cs
using UnityEngine;

[System.Serializable]
public class Kart
{
    public string Isim;
    public Sprite Gorsel; // <-- YENİ: Kartın görselini tutacak alan.
    public Nadirlik NadirlikSeviyesi;
    public int Atk;
    public int Savunma;
    public OzelGucTipi OzelGuc;
    public int OzelGucDegeri;

    // Yapıcı Metot (Constructor) - Sprite eklendi.
    // Sprite null olabilir, çünkü Inspector'dan atama yapacağız.
    public Kart(string isim, Sprite gorsel, Nadirlik nadirlik, int atk, int savunma, OzelGucTipi guc = OzelGucTipi.Yok, int gucDegeri = 0)
    {
        this.Isim = isim;
        this.Gorsel = gorsel; // <-- YENİ
        this.NadirlikSeviyesi = nadirlik;
        this.Atk = atk;
        this.Savunma = savunma;
        this.OzelGuc = guc;
        this.OzelGucDegeri = gucDegeri;
    }

    // ToString() metodunu değiştirmeye gerek yok, ama isterseniz görsel adını ekleyebilirsiniz.
    public override string ToString()
    {
        string gucStr = (OzelGuc != OzelGucTipi.Yok) ? $" GÜÇ:{OzelGuc}({OzelGucDegeri})" : "";
        return $"{Isim} ({NadirlikSeviyesi}) [ATK:{Atk} DEF:{Savunma}{gucStr}]";
    }
}