using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using System;

public class CameraPublisher : MonoBehaviour
{
    public Camera cam;
    public string topicName = "/camera/image_raw";
    private ROSConnection ros;
    private Texture2D texture2D;

    void Start()
    {
        ros = ROSConnection.instance;
        ros.RegisterPublisher<ImageMsg>(topicName);

        int width = cam.targetTexture.width;
        int height = cam.targetTexture.height;
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
    }

    void Update()
    {
        RenderTexture rt = cam.targetTexture;
        RenderTexture.active = rt;

        texture2D.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        texture2D.Apply();
        FlipTexture(texture2D);

        byte[] imageBytes = texture2D.GetRawTextureData();


        var imageMsg = new ImageMsg
        {
            header = new RosMessageTypes.Std.HeaderMsg
            {
                stamp = new RosMessageTypes.BuiltinInterfaces.TimeMsg
                {
                    sec = (int)Time.time,
                    nanosec = (uint)((Time.time % 1) * 1e9)
                },
                frame_id = "camera_link"
            },
            height = (uint)rt.height,
            width = (uint)rt.width,
            encoding = "rgb8",
            is_bigendian = (byte)0,
            step = (uint)(rt.width * 3),
            data = imageBytes
        };

        ros.Publish(topicName, imageMsg);
    }

    void FlipTexture(Texture2D texture)/* Unity gorselleri asagidan yukari okur, fakat rviz yukaridan assagi bekler */
    {
        Color[] pixels = texture.GetPixels();
        Color[] flipped = new Color[pixels.Length];
        int width = texture.width;
        int height = texture.height;

        for (int y = 0; y < height; y++)
        {
            Array.Copy(pixels, y * width, flipped, (height - y - 1) * width, width);
        }

        texture.SetPixels(flipped);
        texture.Apply();
    }

}
