// KartUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class KartUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elemanları")]
    public TextMeshProUGUI isimText;
    public Image kartResmi;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI gucText;
    public Image raycastHedefi;

    private Kart _kartVerisi;
    private int _index;
    private OyunYonetici _oyunYonetici;
    private bool _secimModu = false;
    private bool _interactable = true;
    public bool oyuncuElindeMi = false;
    private Coroutine scaleCoroutine;

    public Animator animator; // Inspector’dan atayacaksın

    void Awake()
    {
        Debug.Log($"KartUI Awake: {gameObject.name}, raycastHedefi: {(raycastHedefi != null ? "Var" : "Yok")}");
        ValidateUIElements();
    }

    private void ValidateUIElements()
    {
        if (isimText == null)
            Debug.LogError($"isimText is not assigned on {gameObject.name}");
        if (kartResmi == null)
            Debug.LogError($"kartResmi is not assigned on {gameObject.name}");
        if (atkText == null)
            Debug.LogError($"atkText is not assigned on {gameObject.name}");
        if (defText == null)
            Debug.LogError($"defText is not assigned on {gameObject.name}");
        if (gucText == null)
            Debug.LogError($"gucText is not assigned on {gameObject.name}");
        if (raycastHedefi == null)
            Debug.LogError($"raycastHedefi is not assigned on {gameObject.name}");
    }

    public void KartBilgileriniAyarla(Kart kart, int index, OyunYonetici yonetici)
    { 
        _secimModu = false; 
        Ayarla(kart, index, yonetici); 
    }

    public void SecimKartBilgileriniAyarla(Kart kart, OyunYonetici yonetici)
    { 
        _secimModu = true; 
        Ayarla(kart, -1, yonetici); 
    }

    private void Ayarla(Kart kart, int index, OyunYonetici yonetici)
    {
        _kartVerisi = kart;
        _index = index;
        _oyunYonetici = yonetici;

        if (kart == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (isimText != null)
            isimText.text = kart.Isim;
        
        if (atkText != null)
            atkText.text = $"ATK: {kart.Atk}";
        
        if (defText != null)
            defText.text = $"DEF: {kart.Savunma}";
        
        if (gucText != null)
            gucText.text = (kart.OzelGuc != OzelGucTipi.Yok) ? $"GÜÇ: {kart.OzelGuc}" : "";

        if (kartResmi != null)
        {
            if (kart.Gorsel != null)
            {
                kartResmi.sprite = kart.Gorsel;
                kartResmi.color = Color.white;
            }
            else
            {
                kartResmi.sprite = null;
                kartResmi.color = Color.clear;
            }
        }

        SetInteractable(true);
    }

    public void SetInteractable(bool durum)
    {
        _interactable = durum;
        if (raycastHedefi != null)
        {
            raycastHedefi.raycastTarget = durum;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Kart tıklandı! Interactable: {_interactable}, Kart: {_kartVerisi?.Isim}, Index: {_index}");
        if (!_interactable || _kartVerisi == null || _oyunYonetici == null) return;

        if (_secimModu)
        {
            _oyunYonetici.KartSecildi(_kartVerisi);
        }
        else if (_index >= 0)
        {
            _oyunYonetici.OyuncuKartOyna(_index);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (oyuncuElindeMi)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleTo(Vector3.one * 1.1f, 0.2f));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (oyuncuElindeMi)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
            scaleCoroutine = StartCoroutine(ScaleTo(Vector3.one, 0.2f));
        }
    }

    private IEnumerator ScaleTo(Vector3 target, float duration)
    {
        Vector3 initial = transform.localScale;
        float time = 0f;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(initial, target, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localScale = target;
    }
}