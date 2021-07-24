import cv2
import time
import numpy as np
import HandTrackingModule as htm
import math
import osascript



#################
wCam, hCam = 640, 480

#################


cap = cv2.VideoCapture(0)
cap.set(3, wCam)
cap.set(4, hCam)
pTime = 0

detector = htm.handDetector()
vol = 0
volBar = 0
volPer = 0
while True:
    success, img = cap.read()

    img = detector.findHands(img)
    lmList = detector.findPosition(img, draw=False)
    if len(lmList) != 0:
        # print(lmList[4], lmList[8])

        x1, y1 = lmList[4][1], lmList[4][2]
        x2, y2 = lmList[8][1], lmList[8][2]
        cx, cy = (x1 + x2) // 2, (y1 + y2) // 2

        cv2.circle(img, (x1, y1), 15, (30, 30, 130), cv2.FILLED)
        cv2.circle(img, (x2, y2), 15, (30, 30, 130), cv2.FILLED)
        cv2.line(img, (x1, y1), (x2, y2), (30, 130, 130), 3)
        cv2.circle(img, (cx, cy), 15, (30, 30, 130), cv2.FILLED)

        length = math.hypot(x2 - x1, y2 - y1)
        # print(length)

        # Hand range 50 - 300
        # Volume range 0 - 100

        vol = np.interp(length, [50, 400], [0, 100])
        volBar = np.interp(length, [50, 400], [400, 150])
        volPer = np.interp(length, [50, 400], [0, 100])
        print(vol)
        target_volume = vol
        osascript.osascript("set volume output volume {}".format(target_volume))

        if length < 50:
            cv2.circle(img, (cx, cy), 15, (30, 255, 10), cv2.FILLED)

    cv2.rectangle(img, (50, 150), (85, 400), (0, 255, 0), 3)
    cv2.rectangle(img, (50, int(volBar)), (85, 400), (0, 255, 0), cv2.FILLED)
    cv2.putText(img, f'{int(volPer)} %', (40, 450), cv2.FONT_HERSHEY_PLAIN, 3, (200, 200, 200), 3)

    cTime = time.time()
    fps = 1/(cTime-pTime)
    pTime = cTime

    cv2.putText(img, f'FPS: {int(fps)}', (40, 70), cv2.FONT_HERSHEY_PLAIN, 3, (200, 100, 200), 3)

    cv2.imshow("Image", img)
    cv2.waitKey(1)