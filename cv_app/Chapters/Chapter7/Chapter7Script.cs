using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;

public class Chapter7Script : MonoBehaviour
{
    public RawImage imgDisplay;
    public RawImage imgDisplayEdges;
    public RawImage imgDisplayContours;



    // Start is called before the first frame update
    void Start()
    {
        // Load Image 
        Texture2D inputTexture = Resources.Load("shapes") as Texture2D;
        // Create an empty Mat 
        Mat img = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);
        // Convert texture to Mat 
        Utils.texture2DToMat(inputTexture, img);

        ///////////////////////////////////////////////////////////////////////////////////////////////////

        ///////////// Preprocessing  ////////////////

        Mat imgContours = img.clone();
        Mat imgGray = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC1);
        Mat imgBlur = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC1);
        Mat imgCanny = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC1);
        Mat imgEdges = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC1);

        // Convert Image to grayscale
        Imgproc.cvtColor(img, imgGray, Imgproc.COLOR_RGBA2GRAY);
        // Add blur
        Imgproc.GaussianBlur(imgGray, imgBlur, new Size(5, 5), 5, 0);
        // Find the Edges
        Imgproc.Canny(imgBlur, imgCanny, 20, 50);
        // Increase / Decrease edge thickness
        Mat erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(3, 3));
        Imgproc.dilate(imgCanny, imgEdges, erodeElement);


        ///////////// Find Contours  ////////////////

        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierachy = new Mat();
        Imgproc.findContours(imgEdges, contours, hierachy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

        for ( int i = 0; i < contours.Count; i++)
        {
            double area = Imgproc.contourArea(contours[i]);

            if (area > 2000)
            {
                MatOfPoint2f cntF = new MatOfPoint2f(contours[i].toArray());
                MatOfPoint2f approx = new MatOfPoint2f();

                double peri = Imgproc.arcLength(cntF, true); 
                Imgproc.approxPolyDP(cntF, approx, 0.02 * peri, true);
                print(approx.toArray().Length);

                Imgproc.drawContours(imgContours, contours, i, new Scalar(255, 0, 255, 255), 5);
                OpenCVForUnity.CoreModule.Rect bbox = Imgproc.boundingRect(approx);
                Imgproc.rectangle(imgContours, bbox.tl(), bbox.br(), new Scalar(0, 255, 0, 255), 2);

                //if (approx.toArray().Length == 4)
                //{
                //    Imgproc.drawContours(imgContours, contours, i, new Scalar(255, 0, 255, 255), 5);
                //}
             }

        }









        ///////////////////////////////////////////////////////////////////////////////////////////////////


        // Create New 2D Texture
        Texture2D outputTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
        Texture2D edgesTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
        Texture2D contoursTexture = new Texture2D(img.cols(), img.rows(), TextureFormat.RGBA32, false);
  
        // Convert Mat to texture 
        Utils.matToTexture2D(img, outputTexture);
        Utils.matToTexture2D(imgEdges, edgesTexture);
        Utils.matToTexture2D(imgContours, contoursTexture);

        // Display the texture on the Raw Image 
        imgDisplay.texture = outputTexture;
        imgDisplayEdges.texture = edgesTexture;
        imgDisplayContours.texture = contoursTexture;
 



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
