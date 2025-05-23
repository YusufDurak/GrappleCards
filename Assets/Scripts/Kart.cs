// Kart.cs
using UnityEngine; // [System.Serializable] için gerekli

[System.Serializable] // Bu satır, Kart nesnelerinin Unity Inspector'da görünmesini sağlar.
public class Kart
{
    public string Isim;
    public Nadirlik NadirlikSeviyesi;
    public int Atk;
    public int C_Atk;
    public int Savunma; // 'def' C#'da anahtar kelime olabileceğinden 'Savunma' kullandık.

    // Yapıcı Metot (Constructor) - Kod içinden kart oluşturmak için.
    public Kart(string isim, Nadirlik nadirlik, int atk, int c_atk, int savunma)
    {
        this.Isim = isim;
        this.NadirlikSeviyesi = nadirlik;
        this.Atk = atk;
        this.C_Atk = c_atk;
        this.Savunma = savunma;
    }

    // Kart bilgilerini kolayca yazdırmak için ToString() metodunu override ediyoruz.
    public override string ToString()
    {
        return $"{Isim} ({NadirlikSeviyesi}) [ATK:{Atk} C-ATK:{C_Atk} DEF:{Savunma}]";
    }
}