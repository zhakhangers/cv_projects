#if !(PLATFORM_LUMIN && !UNITY_EDITOR)

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System.Collections.Generic;


namespace OpenCVForUnityExample
{
    /// <summary>
    /// WebCamTextureToMatHelper Example
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class DocumentScanner : MonoBehaviour
    {

        public ResolutionPreset requestedResolution = ResolutionPreset._640x480;
        public FPSPreset requestedFPS = FPSPreset._30;
        public Toggle rotate90DegreeToggle;
        public Toggle flipVerticalToggle;
        public Toggle flipHorizontalToggle;
        Texture2D texture;
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        public RawImage imgDisplay;
        public RawImage imgDisplayWarp;
        public Image roi;

        int wi = 707, he = 1000;
        Texture2D textureWarp;
        Mat img, imgPro, imgContour, imgWarp;

        // Use this for initialization
        void Start()
        {
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();
            int width, height;
            Dimensions(requestedResolution, out width, out height);
            webCamTextureToMatHelper.requestedWidth = width;
            webCamTextureToMatHelper.requestedHeight = height;
            webCamTextureToMatHelper.requestedFPS = (int)requestedFPS;
            webCamTextureToMatHelper.Initialize();



            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                RectTransform rt = imgDisplay.GetComponent<RectTransform>();
                //rt.sizeDelta = new Vector2(720, 1280);
                //rt.localScale = new Vector3(1.6f, 1.6f, 1f);
                rt.sizeDelta = new Vector2(480, 640);
                rt.localScale = new Vector3(2.75f, 2.75f, 1f);
            }


            imgWarp = new Mat(wi, he, CvType.CV_8UC4);
            textureWarp = new Texture2D(wi, he, TextureFormat.RGBA32, false);
        }

        public void OnWebCamTextureToMatHelperInitialized()
        {
            Debug.Log("OnWebCamTextureToMatHelperInitialized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.cols(), webCamTextureMat.rows(), TextureFormat.RGBA32, false);
            Utils.fastMatToTexture2D(webCamTextureMat, texture);

            gameObject.GetComponent<Renderer>().material.mainTexture = texture;

            gameObject.transform.localScale = new Vector3(webCamTextureMat.cols(), webCamTextureMat.rows(), 1);
            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);

            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }
        }
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            if (texture != null)
            {
                Texture2D.Destroy(texture);
                texture = null;
            }
        }
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);

        }
        void OnDestroy()
        {
            webCamTextureToMatHelper.Dispose();
        }
        public void OnPlayButtonClick()
        {
            webCamTextureToMatHelper.Play();
        }
        public void OnPauseButtonClick()
        {
            webCamTextureToMatHelper.Pause();
        }
        public void OnStopButtonClick()
        {
            webCamTextureToMatHelper.Stop();
        }
        public void OnChangeCameraButtonClick()
        {
            webCamTextureToMatHelper.requestedIsFrontFacing = !webCamTextureToMatHelper.IsFrontFacing();
        }
        public void OnRequestedResolutionDropdownValueChanged(int result)
        {
            if ((int)requestedResolution != result)
            {
                requestedResolution = (ResolutionPreset)result;

                int width, height;
                Dimensions(requestedResolution, out width, out height);

                webCamTextureToMatHelper.Initialize(width, height);
            }
        }
        public void OnRequestedFPSDropdownValueChanged(int result)
        {
            string[] enumNames = Enum.GetNames(typeof(FPSPreset));
            int value = (int)System.Enum.Parse(typeof(FPSPreset), enumNames[result], true);

            if ((int)requestedFPS != value)
            {
                requestedFPS = (FPSPreset)value;

                webCamTextureToMatHelper.requestedFPS = (int)requestedFPS;
            }
        }
        public void OnRotate90DegreeToggleValueChanged()
        {
            if (rotate90DegreeToggle.isOn != webCamTextureToMatHelper.rotate90Degree)
            {
                webCamTextureToMatHelper.rotate90Degree = rotate90DegreeToggle.isOn;
            }
        }
        public void OnFlipVerticalToggleValueChanged()
        {
            if (flipVerticalToggle.isOn != webCamTextureToMatHelper.flipVertical)
            {
                webCamTextureToMatHelper.flipVertical = flipVerticalToggle.isOn;
            }


        }
        public void OnFlipHorizontalToggleValueChanged()
        {
            if (flipHorizontalToggle.isOn != webCamTextureToMatHelper.flipHorizontal)
            {
                webCamTextureToMatHelper.flipHorizontal = flipHorizontalToggle.isOn;
            }
        }
        public enum FPSPreset : int
        {
            _0 = 0,
            _1 = 1,
            _5 = 5,
            _10 = 10,
            _15 = 15,
            _30 = 30,
            _60 = 60,
        }
        public enum ResolutionPreset : byte
        {
            _50x50 = 0,
            _640x480,
            _1280x720,
            _1920x1080,
            _9999x9999,
        }
        private void Dimensions(ResolutionPreset preset, out int width, out int height)
        {
            switch (preset)
            {
                case ResolutionPreset._50x50:
                    width = 50;
                    height = 50;
                    break;
                case ResolutionPreset._640x480:
                    width = 640;
                    height = 480;
                    break;
                case ResolutionPreset._1280x720:
                    width = 1280;
                    height = 720;
                    break;
                case ResolutionPreset._1920x1080:
                    width = 1920;
                    height = 1080;
                    break;
                case ResolutionPreset._9999x9999:
                    width = 9999;
                    height = 9999;
                    break;
                default:
                    width = height = 0;
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////    CUSTOM CODE  ///////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Update is called once per frame
        void Update()
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {
                Mat img = webCamTextureToMatHelper.GetMat();
                Mat imgContour = img.clone();
               // Mat imgCrop = img.clone();

                /////////// PreProcessing (2) ///////////////
                Mat imgPro = preProcessing(img.clone());

                /////////// Get Contours (3) ///////////////
                MatOfPoint docPoints = getContours(imgPro.clone(), imgContour);


                if (docPoints.size().area() != 0)
                {
                    roi.GetComponent<Image>().color = new Color32(0, 255, 0, 200); // Green ROI

                    /////////// Reorder (4) ///////////////
                    docPoints = reorderPoints(docPoints);

                    /////////// Warp (5) ///////////////
                    imgWarp = PerspectiveTransform(img, docPoints);
                }
                else
                {
                    roi.GetComponent<Image>().color = new Color32(255, 0, 0, 200); // Red ROI
                }

                Utils.matToTexture2D(imgContour, texture);
                Utils.matToTexture2D(imgWarp, textureWarp);

                imgDisplay.texture = texture;
                imgDisplayWarp.texture = textureWarp;

            }
        }


        private Mat preProcessing(Mat imgProC)
        {

            Imgproc.cvtColor(imgProC, imgProC, Imgproc.COLOR_RGBA2GRAY);
            Imgproc.GaussianBlur(imgProC, imgProC, new Size(5, 5), 5, 0);
            Imgproc.Canny(imgProC, imgProC, 20, 50);
            Mat erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(7, 7));
            Imgproc.dilate(imgProC, imgProC, erodeElement);
            return imgProC;

        }

        private MatOfPoint getContours(Mat imgPro, Mat imgContour)
        {

            // Declaring Varibales 
            List<MatOfPoint> contours = new List<MatOfPoint>();
            Mat hierarchy = new Mat();
            MatOfPoint2f approx = new MatOfPoint2f();
            MatOfPoint bPoints = new MatOfPoint();
            double bArea = 0;

            //Find Contours 
            Imgproc.findContours(imgPro, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);

            // loop through is all contours
            for (int i = 0; i < contours.Count; i++)
            {

                // Find the area of the contour
                double area = Imgproc.contourArea(contours[i]);
                if (area > 10000)
                {
                    // Convert to FLoating Points
                    MatOfPoint2f cntF = new MatOfPoint2f(contours[i].toArray());
                    // Find the corner points 
                    double peri = Imgproc.arcLength(cntF, true);
                    Imgproc.approxPolyDP(cntF, approx, 0.02 * peri, true);

                    // Find the biggest rectangle 
                    if (approx.toArray().Length == 4 && area > bArea)
                    {
                        bArea = area;
                        approx.convertTo(bPoints, CvType.CV_32S); ;
                        Imgproc.drawContours(imgContour, contours, i, new Scalar(255, 0, 0, 255), 7);

                    }
                }
            }
            return bPoints;
        }

        private MatOfPoint reorderPoints(MatOfPoint points)
        {
            if (points.size().area() <= 0 || points.rows() < 4)
                return points;


            // rearrange the points in the order of upper left, upper right, lower right, lower left,
            using (Mat x = new Mat(points.size(), CvType.CV_32SC1))
            using (Mat y = new Mat(points.size(), CvType.CV_32SC1))
            using (Mat d = new Mat(points.size(), CvType.CV_32SC1))
            using (Mat dst = new Mat(points.size(), CvType.CV_32SC2))
            {
                Core.extractChannel(points, x, 0);
                Core.extractChannel(points, y, 1);

                // the sum of the upper left points is the smallest and the sum of the lower right points is the largest.
                Core.add(x, y, d);
                Core.MinMaxLocResult result = Core.minMaxLoc(d);
                dst.put(0, 0, points.get((int)result.minLoc.y, 0)); // (0,0)
                dst.put(2, 0, points.get((int)result.maxLoc.y, 0)); // (w,h)

                // the difference in the upper right point is the smallest, and the difference in the lower left is the largest.
                Core.subtract(y, x, d);
                result = Core.minMaxLoc(d);
                dst.put(1, 0, points.get((int)result.minLoc.y, 0));  //(w,0)
                dst.put(3, 0, points.get((int)result.maxLoc.y, 0));   // (0,h)

                dst.copyTo(points);
            }
            return points;
        }

        private Mat PerspectiveTransform(Mat image, MatOfPoint corners)
        {

            Point[] pts = corners.toArray();
            Point tl = pts[0];
            Point tr = pts[1];
            Point br = pts[2];
            Point bl = pts[3];

            Mat dst = new Mat(4, 1, CvType.CV_32FC2);
            Mat src = new Mat();

            corners.convertTo(src, CvType.CV_32FC2);
            dst.put(0, 0, 0, 0, wi, 0, wi, he, 0, he);

            Mat imgOut = new Mat(he, wi, image.type(), new Scalar(0, 0, 0, 255));
            Mat perspectiveTransform = Imgproc.getPerspectiveTransform(src, dst);
            Imgproc.warpPerspective(image, imgOut, perspectiveTransform, new Size(imgOut.cols(), imgOut.rows()));

            return imgOut;
        }

    }
}

#endif