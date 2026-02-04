using UnityEngine;

public enum GalleryFilter { All, Odd, Even, Non }

[CreateAssetMenu(menuName = "Gallery/GalleryConfig")]
public class GalleryConfig : ScriptableObject
{
    [Header("Remote")]
    public string _baseUrl = "http://data.ikppbb.com/test-task-unity-data/pics/";
    [SerializeField] private int _minImages = 1;
    [SerializeField] public int _totalImages = 66;

    public int MinImages => _minImages;
    public int TotalImages => _totalImages;

    public string GetUrl(int id) =>
        $"{_baseUrl}{id}.jpg";
}
