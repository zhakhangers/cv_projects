#if !(PLATFORM_LUMIN && !UNITY_EDITOR)

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.UnityUtils.Helper;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using OpenCVForUnity.ObjdetectModule;


namespace OpenCVForUnityExample
{

    [RequireComponent(typeof(WebCamTextureToMatHelper))]
    public class QrReader : MonoBehaviour
    {

        public ResolutionPreset requestedResolution = ResolutionPreset._640x480;
        public FPSPreset requestedFPS = FPSPreset._30;
        public Toggle rotate90DegreeToggle;
        public Toggle flipVerticalToggle;
        public Toggle flipHorizontalToggle;
        Texture2D texture;
        WebCamTextureToMatHelper webCamTextureToMatHelper;

        public RawImage inputImage;

        public Text qrText;
        Mat grayMat;
        QRCodeDetector detector;
        Mat points;

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
                RectTransform rt = inputImage.GetComponent<RectTransform>();
                //rt.sizeDelta = new Vector2(720, 1280);
                //rt.localScale = new Vector3(1.6f, 1.6f, 1f);
                rt.sizeDelta = new Vector2(480, 640);
                rt.localScale = new Vector3(2.75f, 2.75f, 1f);
            }

            detector = new QRCodeDetector();
            grayMat = new Mat();
            points = new Mat();

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

            // if WebCamera is frontFaceing, flip Mat.
            webCamTextureToMatHelper.flipHorizontal = webCamTextureToMatHelper.GetWebCamDevice().isFrontFacing;



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
        public void OnBackButtonClick()
        {
            SceneManager.LoadScene("OpenCVForUnityExample");
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

                ////////////////////////////////////////////////////////////////////////////

                Imgproc.cvtColor(img, grayMat, Imgproc.COLOR_RGBA2GRAY);
                bool result = detector.detect(grayMat, points);

   
                if (result)
                {
                    //print(points.dump());

                    float[] pointsA = new float[8];
                    points.get(0, 0, pointsA);

                    Imgproc.line(img, new Point(pointsA[0], pointsA[1]), new Point(pointsA[2], pointsA[3]),
                                                                            new Scalar(255, 0, 0, 255), 2);
                    Imgproc.line(img, new Point(pointsA[2], pointsA[3]), new Point(pointsA[4], pointsA[5]),
                                                                            new Scalar(255, 0, 0, 255), 2);
                    Imgproc.line(img, new Point(pointsA[4], pointsA[5]), new Point(pointsA[6], pointsA[7]),
                                                                            new Scalar(255, 0, 0, 255), 2);
                    Imgproc.line(img, new Point(pointsA[6], pointsA[7]), new Point(pointsA[0], pointsA[1]),
                                                                            new Scalar(255, 0, 0, 255), 2);

                    string decode_info = detector.decode(grayMat, points);
                    //print(decode_info);
                    qrText.text = decode_info;
                }

                ////////////////////////////////////////////////////////////////////////////


                Utils.fastMatToTexture2D(img, texture);
                inputImage.texture = texture;
            }
        }

        public void openWebsite() 
        {

            Application.OpenURL(qrText.text);

        }


    }
}

#endif