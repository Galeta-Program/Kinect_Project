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
        // ��l�� Kinect
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            // �Ұ� Kinect
            kinectSensor.Open();

            // ��l�Ư��z�M�����ƾ�
            texture = new Texture2D(kinectSensor.ColorFrameSource.FrameDescription.Width,
                                    kinectSensor.ColorFrameSource.FrameDescription.Height,
                                    TextureFormat.RGBA32, false);
            colorData = new byte[kinectSensor.ColorFrameSource.FrameDescription.LengthInPixels * 4];

            // ��l�� ColorFrameReader
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
            // ����m��v���V
            using (var frame = colorFrameReader.AcquireLatestFrame())
            {
                if (frame != null)
                {
                    // �N�v���ƾ��ഫ�� Unity ������榡
                    frame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Rgba);

                    // ��s���z
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
            // �b GUI ����� Kinect �m��v��
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture, ScaleMode.ScaleToFit);
        }
    }

    void OnApplicationQuit()
    {
        // ���� Kinect
        if (kinectSensor != null)
        {
            kinectSensor.Close();
        }
    }
}
