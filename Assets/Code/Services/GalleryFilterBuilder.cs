using System.Collections.Generic;

public class GalleryFilterBuilder
{
    public static List<int> BuildIds(int min, int total, GalleryFilter filter)
    {
        List<int> ids = new List<int>();

        int i;
        for (i = min; i <= total; i++)
        {
            bool ok = false;

            switch (filter)
            {
                case GalleryFilter.All:
                    ok = true;
                    break;
                case GalleryFilter.Odd:
                    if ((i % 2) == 1)
                        ok = true;
                    break;
                case GalleryFilter.Even:
                    if ((i % 2) == 0)
                        ok = true;
                    break;
                default:
                    ok = true;
                    break;
            }

            if (ok)
                ids.Add(i);
        }

        return ids;
    }
}

