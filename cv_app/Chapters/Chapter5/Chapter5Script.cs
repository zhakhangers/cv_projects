using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class Chapter5Script : MonoBehaviour
{
    public RawImage imgDisplay;
    public RawImage imgDisplayWarp;

    float w = 500, h = 700; 

    // Start is called before the first frame update
    void Start()
    {
        // Load Image 
        Texture2D inputTexture = Resources.Load("cards") as Texture2D;
        // Create an empty Mat 
        Mat img = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);
        // Convert texture to Mat 
        Utils.texture2DToMat(inputTexture, img);

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        Mat imgWarp = img.clone();

        Mat src = new Mat(4, 1, CvType.CV_32FC2);
        Mat dst = new Mat(4, 1, CvType.CV_32FC2);

        // Values of Source and Destination points
        src.put(0, 0, 529.0, 142.0, 771.0, 190.0, 405.0, 395.0, 674.0, 457.0);
        dst.put(0, 0, 0.0, 0.0, w, 0, 0, h, w, h);

        // Find the Transformation matrix 
        Mat matrix = Imgproc.getPerspectiveTransform(src, dst);
        // Warp the Image
        Imgproc.warpPerspective(img, imgWarp, matrix, new Size((int)w, (int)h));

        ///////////////////////////////////////////////////////////////////////////////////////////////////


        // Create New 2D Texture
        Texture2D outputTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
        Texture2D warpTexture = new Texture2D((int) w, (int) h, TextureFormat.RGBA32, false);

        // Change Raw image size to match the warp image size
        imgDisplayWarp.GetComponent<RectTransform>().sizeDelta = new Vector2((int)w, (int)h);
  
        // Convert Mat to texture 
        Utils.matToTexture2D(img, outputTexture);
        Utils.matToTexture2D(imgWarp, warpTexture);

        // Display the texture on the Raw Image 
        imgDisplay.texture = outputTexture;
        imgDisplayWarp.texture = warpTexture;

 



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
