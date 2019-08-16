using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
//using Svg;
using System.IO;
using System.Drawing;
//using System.Drawing.Imaging;
using System.Linq;

namespace ScreepsViewer
{
    public class BadgeController : MonoBehaviour
    {
        public string url;
        public Dictionary<string, Badge> badges;
        public Texture myTexture;
        public string userName;

        public void DownloadBadge()
        {
            StartCoroutine(GetBadge());
        }
        IEnumerator GetBadge()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://screeps.com/api/user/badge-svg?username=smitt33");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                byte[] thisTextureBin = www.downloadHandler.data;
                using (Stream stream = new MemoryStream(thisTextureBin))
                {
                    //SvgDocument svgDocument = SvgDocument.Open<SvgDocument>(stream);
                    //Bitmap bitmap = svgDocument.Draw();
                    //ImageExtensions.SaveJpeg(bitmap, Application.persistentDataPath + "/" + userName + "badge.jpeg", 10);
                    //Sprite badge = LoadSprite(Application.persistentDataPath + "/" + userName + "badge.jpeg");
                    //myTexture = badge.texture;
                }
            }
        }
        private Sprite LoadSprite(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                return sprite;
            }
            return null;
        }
        [System.Serializable]
        public class Badge
        {
            public int type; // 2,
            public string color1; // #000000,
            public string color2; // #028300,
            public string color3; // #8b5c00,
            public int param; // 0,
            public bool flip; // false
        }
    }
    //public static class ImageExtensions
    //{
    //    public static void SaveJpeg(this System.Drawing.Image img, string filePath, long quality)
    //    {
    //        var encoderParameters = new EncoderParameters(1);
    //        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
    //        img.Save(filePath, GetEncoder(ImageFormat.Jpeg), encoderParameters);
    //    }

    //    public static void SaveJpeg(this System.Drawing.Image img, Stream stream, long quality)
    //    {
    //        var encoderParameters = new EncoderParameters(1);
    //        encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, quality);
    //        img.Save(stream, GetEncoder(ImageFormat.Jpeg), encoderParameters);
    //    }

    //    static ImageCodecInfo GetEncoder(ImageFormat format)
    //    {
    //        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
    //        return codecs.Single(codec => codec.FormatID == format.Guid);
    //    }
    //}
}
