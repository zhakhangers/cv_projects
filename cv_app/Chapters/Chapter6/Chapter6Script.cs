using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class Chapter6Script : MonoBehaviour
{
    public RawImage imgDisplay;
    public RawImage imgDisplayMask;


    // Start is called before the first frame update
    void Start()
    {
        // Load Image 
        Texture2D inputTexture = Resources.Load("lambo") as Texture2D;
        // Create an empty Mat 
        Mat img = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);
        // Convert texture to Mat 
        Utils.texture2DToMat(inputTexture, img);

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        Mat imgHSV = new Mat();
        Mat mask = new Mat();
        Imgproc.cvtColor(img, imgHSV, Imgproc.COLOR_RGB2HSV);
        Core.inRange(imgHSV, new Scalar(0, 110, 153), new Scalar(19, 240, 255), mask);

        ///////////////////////////////////////////////////////////////////////////////////////////////////


        // Create New 2D Texture
        Texture2D outputTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
        Texture2D maskTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
  
        // Convert Mat to texture 
        Utils.matToTexture2D(img, outputTexture);
        Utils.matToTexture2D(mask, maskTexture);

        // Display the texture on the Raw Image 
        imgDisplay.texture = outputTexture;
        imgDisplayMask.texture = maskTexture;

 



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
