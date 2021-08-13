using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.Features2dModule;
using OpenCVForUnity.UnityUtils;

namespace OpenCVForUnityExample
{
    /// <summary>
    /// Feature2D Example
    /// An example of descriptor extraction and descriptor matching.
    /// http://docs.opencv.org/3.1.0/d5/dde/tutorial_feature_description.html
    /// </summary>
    public class ArBasics : MonoBehaviour
    {
        // Use this for initialization
        void Start ()
        {
            
            //// Load Images
            Texture2D imgTexture = Resources.Load ("test") as Texture2D;
            Texture2D imgTexture2 = Resources.Load("test2") as Texture2D;
            Mat img1 = new Mat (imgTexture.height, imgTexture.width, CvType.CV_8UC3);
            Mat img2 = new Mat(imgTexture2.height, imgTexture2.width, CvType.CV_8UC3);
            Utils.texture2DToMat (imgTexture, img1);
            Utils.texture2DToMat (imgTexture2, img2);


            /// Initialize Detector 
            ORB detector = ORB.create ();
            ORB extractor = ORB.create ();
            MatOfKeyPoint keypoints1 = new MatOfKeyPoint ();
            MatOfKeyPoint keypoints2 = new MatOfKeyPoint();
            Mat descriptors1 = new Mat();
            Mat descriptors2 = new Mat ();


            //// Get Features
            detector.detect(img1, keypoints1);
            detector.detect(img2, keypoints2);
            //// Get Descriptors
            extractor.compute(img1, keypoints1, descriptors1);
            extractor.compute (img2, keypoints2, descriptors2);

            //// Match Descriptors
            DescriptorMatcher matcher = DescriptorMatcher.create (DescriptorMatcher.BRUTEFORCE_HAMMINGLUT);
            MatOfDMatch matches = new MatOfDMatch ();
            matcher.match (descriptors1, descriptors2, matches);

            //// Draw Results on Image 
            Mat resultImg = new Mat ();
            Features2d.drawMatches (img1, keypoints1, img2, keypoints2, matches, resultImg);

            //// Display Output
            Texture2D texture = new Texture2D (resultImg.cols (), resultImg.rows (), TextureFormat.RGBA32, false);
            Utils.matToTexture2D (resultImg, texture);
            gameObject.GetComponent<Renderer> ().material.mainTexture = texture;
        }
    
    }
}