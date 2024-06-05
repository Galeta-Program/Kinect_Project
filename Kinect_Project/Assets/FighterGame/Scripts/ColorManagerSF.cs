using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;

public class ColorManagerSF : MonoBehaviour
{
    private KinectSensor kinectSensor;
    public RawImage outputImage;
    private Texture2D texture;
    private byte[] colorData;
    private ColorFrameReader colorFrameReader;

    void Start()
    {
        // 初始化 Kinect
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            // 啟動 Kinect
            kinectSensor.Open();

            // 初始化紋理和相關數據
            texture = new Texture2D(kinectSensor.ColorFrameSource.FrameDescription.Width,
                                    kinectSensor.ColorFrameSource.FrameDescription.Height,
                                    TextureFormat.RGBA32, false);
            colorData = new byte[kinectSensor.ColorFrameSource.FrameDescription.LengthInPixels * 4];

            // 初始化 ColorFrameReader
            colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();
        }
        else
        {
            Debug.LogError("No Kinect sensor found!");
        }
    }

    void Update()
    {
        if (kinectSensor != null && !kinectSensor.IsOpen)
        {
            kinectSensor.Open();
        }

        if (colorFrameReader != null)
        {          
            // 獲取彩色影像幀
            using (var frame = colorFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    // 將影像數據轉換為 Unity 支持的格式
                    frame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Rgba);

                    // 更新紋理
                    texture.LoadRawTextureData(colorData);
                    texture.Apply();

                    if (outputImage != null)
                        outputImage.texture = texture;
                }
            }
        }
    }

    void OnGUI()
    {
        if (texture != null && outputImage == null)
        {
            // 在 GUI 中顯示 Kinect 彩色影像
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture, ScaleMode.ScaleToFit);
        }
    }

    void OnApplicationQuit()
    {
        // 關閉 Kinect
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }
}
