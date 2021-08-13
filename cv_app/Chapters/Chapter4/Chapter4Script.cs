using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class Chapter4Script : MonoBehaviour
{
    public RawImage imgDisplay;
    // Start is called before the first frame update
    void Start()
    {
        // Create an empty Mat 
        Mat img = new Mat(512, 512, CvType.CV_8UC3, new Scalar(255, 255, 255));
        // Draw Circle
        Imgproc.circle(img, new Point(256, 256), 150, new Scalar(255, 69, 0), Imgproc.FILLED);
        // Rectangle
        Imgproc.rectangle(img, new Point(130, 226), new Point(382, 286), new Scalar(255, 255, 255), Imgproc.FILLED);
        // Line
        Imgproc.line(img, new Point(130, 296), new Point(382, 296), new Scalar(255, 255, 255), 2);
        // Text
        Imgproc.putText(img, "Murtaza's Workshop", new Point(137, 262), Imgproc.FONT_HERSHEY_DUPLEX, 0.75, new Scalar(255, 69, 0), 2);



        // Create New 2D Texture
        Texture2D outputTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
        // Convert Mat to texture 
        Utils.matToTexture2D(img, outputTexture);
        // Display the texture on the Raw Image 
        imgDisplay.texture = outputTexture;



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
