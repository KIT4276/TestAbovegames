using UnityEngine;

public enum GalleryFilter { All, Odd, Even, Non }

[CreateAssetMenu(menuName = "Gallery/GalleryConfig")]
public class GalleryConfig : ScriptableObject
{
    [Header("Remote")]
    [SerializeField] private string _baseUrl = "https://data.ikppbb.com/test-task-unity-data/pics/";
    [SerializeField] private int _minImages = 1;
    [SerializeField] public int _totalImages = 66;

    public string BaseUrl => _baseUrl;
    public int MinImages => _minImages;
    public int TotalImages => _totalImages;

    public string GetUrl(int id) =>
        $"{_baseUrl}{id}.jpg";
}
