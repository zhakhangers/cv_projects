using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class Chapter3Script : MonoBehaviour
{
    public RawImage imgDisplay;
    public RawImage imgDisplayResize;
    public RawImage imgDisplayCrop;



    // Start is called before the first frame update
    void Start()
    {
        // Load Image 
        Texture2D inputTexture = Resources.Load("test") as Texture2D;
        // Create an empty Mat 
        Mat img = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);
        // Convert texture to Mat 
        Utils.texture2DToMat(inputTexture, img);

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        Mat imgResize = new Mat();
        Mat imgCrop = new Mat();

        // Resize
        Size newSize = new Size(85, 85);
        Imgproc.resize(img, imgResize, newSize, 0, 0);

        //Crop
        OpenCVForUnity.CoreModule.Rect rectCrop = new OpenCVForUnity.CoreModule.Rect(250, 250, 300, 300);
        imgCrop = new Mat(img, rectCrop);


        ///////////////////////////////////////////////////////////////////////////////////////////////////


        // Create New 2D Texture
        Texture2D outputTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
        Texture2D resizeTexture = new Texture2D((int) newSize.width, (int) newSize.height, TextureFormat.RGBA32, false);
        Texture2D cropTexture = new Texture2D(300, 300, TextureFormat.RGBA32, false);
  
        // Convert Mat to texture 
        Utils.matToTexture2D(img, outputTexture);
        Utils.matToTexture2D(imgResize, resizeTexture);
        Utils.matToTexture2D(imgCrop, cropTexture, true, 1);

        // Display the texture on the Raw Image 
        imgDisplay.texture = outputTexture;
        imgDisplayResize.texture = resizeTexture;
        imgDisplayCrop.texture = cropTexture;
 



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
