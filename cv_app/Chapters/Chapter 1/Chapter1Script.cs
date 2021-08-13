using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class Chapter1Script : MonoBehaviour
{
    public RawImage imgDisplay;
    // Start is called before the first frame update
    void Start()
    {
        // Load Image 
        Texture2D inputTexture = Resources.Load("shapes") as Texture2D;
        // Create an empty Mat 
        Mat img = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);
        // Convert texture to Mat 
        Utils.texture2DToMat(inputTexture, img);

        /////////////////////////////////
        ////////////////////////////////
        ////////// CODE /////////////////
        /////////////////////////////////
        /////////////////////////////////
        ///

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
