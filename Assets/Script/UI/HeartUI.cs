using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetFull()
    {
        image.sprite = fullHeart;
    }

    public void SetEmpty()
    {
        image.sprite = emptyHeart;
    }
}