// Kart.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card")]
public class Kart : ScriptableObject
{
    public string Isim;
    public Sprite Gorsel;
    public Nadirlik NadirlikSeviyesi;
    public int Atk;
    public int Savunma;
    public OzelGucTipi OzelGuc;
    public int OzelGucDegeri;

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Isim) && 
               Gorsel != null && 
               Atk >= 0 && 
               Savunma >= 0;
    }

    public override string ToString()
    {
        string gucStr = (OzelGuc != OzelGucTipi.Yok) ? $" GÜÇ:{OzelGuc}({OzelGucDegeri})" : "";
        return $"{Isim} ({NadirlikSeviyesi}) [ATK:{Atk} DEF:{Savunma}{gucStr}]";
    }
}