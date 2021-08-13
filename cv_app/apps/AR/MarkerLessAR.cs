 using OpenCVForUnity.Calib3dModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using OpenCVMarkerLessAR;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MarkerLessARExample
{
    /// <summary>
    /// WebCamTexture Markerless AR Example
    /// This code is a rewrite of https://github.com/MasteringOpenCV/code/tree/master/Chapter3_MarkerlessAR using "OpenCV for Unity".
    /// </summary>
    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class MarkerLessAR : MonoBehaviour
    {
        public RawImage patternRawImage;
        public GameObject ARGameObject;
        public Camera ARCamera;
        public bool shouldMoveARCamera;
 
        public GameObject cube;
        public string targetImage; 

        Mat patternMat;
        Texture2D texture;
        WebCamTextureToMatHelper webCamTextureToMatHelper;


        Mat grayMat;
        Mat camMatrix;
        MatOfDouble distCoeffs;
        Matrix4x4 invertYM;
        Matrix4x4 invertZM;
        Pattern pattern;
        PatternTrackingInfo patternTrackingInfo;
        PatternDetector patternDetector;

        // Use this for initialization
        void Start()
        {
            ARGameObject.gameObject.SetActive(false);
            webCamTextureToMatHelper = gameObject.GetComponent<WebCamTextureToMatHelper>();

           
            /////// Load Target Image 
            Texture2D inputTexture = Resources.Load(targetImage) as Texture2D;
            patternMat = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC4);
            Utils.texture2DToMat(inputTexture, patternMat);
            Imgproc.cvtColor(patternMat, patternMat, Imgproc.COLOR_RGB2BGR);


            Imgproc.cvtColor(patternMat, patternMat, Imgproc.COLOR_BGR2RGB);

            Texture2D patternTexture = new Texture2D(patternMat.width(), patternMat.height(), TextureFormat.RGBA32, false);

            //To reuse mat, set the flipAfter flag to true.
            Utils.matToTexture2D(patternMat, patternTexture, true, 0, true);
            Debug.Log("patternMat dst ToString " + patternMat.ToString());

            patternRawImage.texture = patternTexture;
            patternRawImage.rectTransform.localScale = new Vector3(1.0f, (float)patternMat.height() / (float)patternMat.width(), 1.0f);

            pattern = new Pattern();
            patternTrackingInfo = new PatternTrackingInfo();

            patternDetector = new PatternDetector(null, null, null, true);

            patternDetector.buildPatternFromImage(patternMat, pattern);
            patternDetector.train(pattern);


            webCamTextureToMatHelper.Initialize();

            this.gameObject.transform.localScale = new Vector3(480*4, 640*4, 1);

        }

        public void OnWebCamTextureToMatHelperInitialized()
        {
            Debug.Log("OnWebCamTextureToMatHelperInitialized");

            Mat webCamTextureMat = webCamTextureToMatHelper.GetMat();

            texture = new Texture2D(webCamTextureMat.width(), webCamTextureMat.height(), TextureFormat.RGBA32, false);
            gameObject.GetComponent<Renderer>().material.mainTexture = texture;


            grayMat = new Mat(webCamTextureMat.rows(), webCamTextureMat.cols(), CvType.CV_8UC1);


            gameObject.transform.localScale = new Vector3(webCamTextureMat.width(), webCamTextureMat.height(), 1);

            Debug.Log("Screen.width " + Screen.width + " Screen.height " + Screen.height + " Screen.orientation " + Screen.orientation);


            float width = webCamTextureMat.width();
            float height = webCamTextureMat.height();

            float imageSizeScale = 1.0f;
            float widthScale = (float)Screen.width / width;
            float heightScale = (float)Screen.height / height;
            if (widthScale < heightScale)
            {
                Camera.main.orthographicSize = (width * (float)Screen.height / (float)Screen.width) / 2;
                imageSizeScale = (float)Screen.height / (float)Screen.width;
            }
            else
            {
                Camera.main.orthographicSize = height / 2;
            }


            //set cameraparam
            int max_d = (int)Mathf.Max(width, height);
            double fx = max_d;
            double fy = max_d;
            double cx = width / 2.0f;
            double cy = height / 2.0f;
            camMatrix = new Mat(3, 3, CvType.CV_64FC1);
            camMatrix.put(0, 0, fx);
            camMatrix.put(0, 1, 0);
            camMatrix.put(0, 2, cx);
            camMatrix.put(1, 0, 0);
            camMatrix.put(1, 1, fy);
            camMatrix.put(1, 2, cy);
            camMatrix.put(2, 0, 0);
            camMatrix.put(2, 1, 0);
            camMatrix.put(2, 2, 1.0f);
            Debug.Log("camMatrix " + camMatrix.dump());


            distCoeffs = new MatOfDouble(0, 0, 0, 0);
            Debug.Log("distCoeffs " + distCoeffs.dump());


            //calibration camera
            Size imageSize = new Size(width * imageSizeScale, height * imageSizeScale);
            double apertureWidth = 0;
            double apertureHeight = 0;
            double[] fovx = new double[1];
            double[] fovy = new double[1];
            double[] focalLength = new double[1];
            Point principalPoint = new Point(0, 0);
            double[] aspectratio = new double[1];

            Calib3d.calibrationMatrixValues(camMatrix, imageSize, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

            Debug.Log("imageSize " + imageSize.ToString());
            Debug.Log("apertureWidth " + apertureWidth);
            Debug.Log("apertureHeight " + apertureHeight);
            Debug.Log("fovx " + fovx[0]);
            Debug.Log("fovy " + fovy[0]);
            Debug.Log("focalLength " + focalLength[0]);
            Debug.Log("principalPoint " + principalPoint.ToString());
            Debug.Log("aspectratio " + aspectratio[0]);


            //To convert the difference of the FOV value of the OpenCV and Unity. 
            double fovXScale = (2.0 * Mathf.Atan((float)(imageSize.width / (2.0 * fx)))) / (Mathf.Atan2((float)cx, (float)fx) + Mathf.Atan2((float)(imageSize.width - cx), (float)fx));
            double fovYScale = (2.0 * Mathf.Atan((float)(imageSize.height / (2.0 * fy)))) / (Mathf.Atan2((float)cy, (float)fy) + Mathf.Atan2((float)(imageSize.height - cy), (float)fy));

            Debug.Log("fovXScale " + fovXScale);
            Debug.Log("fovYScale " + fovYScale);


            //Adjust Unity Camera FOV https://github.com/opencv/opencv/commit/8ed1945ccd52501f5ab22bdec6aa1f91f1e2cfd4
            if (widthScale < heightScale)
            {
                ARCamera.fieldOfView = (float)(fovx[0] * fovXScale);
            }
            else
            {
                ARCamera.fieldOfView = (float)(fovy[0] * fovYScale);
            }


            invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
            Debug.Log("invertYM " + invertYM.ToString());

            invertZM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
            Debug.Log("invertZM " + invertZM.ToString());


            //if WebCamera is frontFaceing,flip Mat.
            webCamTextureToMatHelper.flipHorizontal = webCamTextureToMatHelper.GetWebCamDevice().isFrontFacing;
        }
        public void OnWebCamTextureToMatHelperDisposed()
        {
            Debug.Log("OnWebCamTextureToMatHelperDisposed");

            if (grayMat != null)
                grayMat.Dispose();
        }
        public void OnWebCamTextureToMatHelperErrorOccurred(WebCamTextureToMatHelper.ErrorCode errorCode)
        {
            Debug.Log("OnWebCamTextureToMatHelperErrorOccurred " + errorCode);
        }
        void OnDestroy()
        {
            webCamTextureToMatHelper.Dispose();

            if (patternMat != null)
                patternMat.Dispose();
        }
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("MarkerLessARExample");
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
        public void OnCapturePatternButtonClick()
        {
            SceneManager.LoadScene("CapturePattern");
        }


        void Update()
        {
            if (webCamTextureToMatHelper.IsPlaying() && webCamTextureToMatHelper.DidUpdateThisFrame())
            {

                Mat rgbaMat = webCamTextureToMatHelper.GetMat();

                Imgproc.cvtColor(rgbaMat, grayMat, Imgproc.COLOR_RGBA2GRAY);


                bool patternFound = patternDetector.findPattern(grayMat, patternTrackingInfo);

                //Debug.Log ("patternFound " + patternFound);
                if (patternFound)
                {
                    patternTrackingInfo.computePose(pattern, camMatrix, distCoeffs);

                    //Marker to Camera Coordinate System Convert Matrix
                    Matrix4x4 transformationM = patternTrackingInfo.pose3d;


                    // right-handed coordinates system (OpenCV) to left-handed one (Unity)
                    // https://stackoverflow.com/questions/30234945/change-handedness-of-a-row-major-4x4-transformation-matrix
                    Matrix4x4 ARM = invertYM * transformationM * invertYM;

                    // Apply Y-axis and Z-axis refletion matrix. (Adjust the posture of the AR object)
                    ARM = ARM * invertYM * invertZM;

                    if (shouldMoveARCamera)
                    {
                        ARM = ARGameObject.transform.localToWorldMatrix * ARM.inverse;

                        //Debug.Log("ARM " + ARM.ToString());

                        ARUtils.SetTransformFromMatrix(ARCamera.transform, ref ARM);
                    }
                    else
                    {
                        ARM = ARCamera.transform.localToWorldMatrix * ARM;

                        //Debug.Log("ARM " + ARM.ToString());

                        ARUtils.SetTransformFromMatrix(ARGameObject.transform, ref ARM);
                    }

                    ARGameObject.GetComponent<DelayableSetActive>().SetActive(true);
                }
                else
                {
                    ARGameObject.GetComponent<DelayableSetActive>().SetActive(false, 0.5f);
                }

                Utils.fastMatToTexture2D(rgbaMat, texture);
            }
        }


    }
}
