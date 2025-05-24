// OzelGucTipi.cs
public enum OzelGucTipi
{
    Yok,           // Özel gücü olmayan standart kart
    CanYenile,     // Oynandığında kendi canını yeniler
    DirektHasar,   // Oynandığında rakibe direkt hasar verir (savunmayı geçer)
    KalkanGucu,    // Oynandığında bir sonraki tur ekstra savunma sağlar
    Yansit,        // Oynandığında hasarı karşıya yansıtır
    DesteDegisim,  // Oynandığında rakip ile oyuncu destesi değişir
    AtakGucu,      // Oynandığında 1 tur ekstra saldırı sağlar
    
    // Gelecekte daha fazla güç eklenebilir...
}