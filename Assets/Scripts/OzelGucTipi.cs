// OzelGucTipi.cs
public enum OzelGucTipi
{
    Yok,           // Özel gücü olmayan standart kart
    CanYenile,     // Oynandığında kendi canını yeniler
    DirektHasar,   // Oynandığında rakibe direkt hasar verir (savunmayı geçer)
    KalkanGucu,    // Oynandığında bir sonraki tur ekstra savunma sağlar
    KartCek,       // Oynandığında ekstra kart çektirir
    AtakGucu       // Oynandığında bir sonraki tur ekstra saldırı sağlar
    // Gelecekte daha fazla güç eklenebilir...
}