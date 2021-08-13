import cv2
import numpy as np
import time
import  HandTrackingModule as htm


###################
brushThickness = 15
eraserThickness = 100


###################

drawColor = (255, 0, 255)

cap = cv2.VideoCapture(0)

detector = htm.handDetector(detectionCon=0.5)
xp, yp = 0, 0
imgCanvas = np.zeros((720, 1280, 3), np.uint8)

while True:
    # 1. Import image
    success, img = cap.read()
    img = cv2.flip(img, 1)

    color1 = cv2.circle(img, (250, 100), 40, (30, 30, 130), cv2.FILLED)
    color2 = cv2.circle(img, (500, 100), 40, (30, 130, 30), cv2.FILLED)
    color3 = cv2.circle(img, (750, 100), 40, (230, 30, 30), cv2.FILLED)
    eraser = cv2.rectangle(img, (1000, 60), (1040, 140), (0, 0, 0), cv2.FILLED)

    # 2. Find Hand Landmarks
    img = detector.findHands(img)
    lmList = detector.findPosition(img, draw=False)

    if len(lmList) != 0:
        # print(lmList)

        # tip of index and middle fingers
        x1, y1 = lmList[8][1:]
        x2, y2 = lmList[12][1:]

        # 3. Check which fingers are up

        fingers = detector.fingersUp()
        print(fingers)

        # 4. If Selection Mode - Two fingers are up
        if fingers[1] and fingers[2]:
            xp, yp = 0, 0

            print("Selection Mode")
            # Checking fore the click
            if y1 < 200:
                if 210 < x1 < 290:
                    cv2.circle(img, (250, 100), 80, (30, 30, 130), cv2.FILLED)
                    drawColor = (30, 30, 130)
                elif 460 < x1 < 540:
                    cv2.circle(img, (500, 100), 80, (30, 130, 30), cv2.FILLED)
                    drawColor = (30, 130, 30)
                elif 710 < x1 < 790:
                    cv2.circle(img, (750, 100), 80, (230, 30, 30), cv2.FILLED)
                    drawColor = (230, 30, 30)
                elif 1000 < x1 < 1040:
                    cv2.rectangle(img, (960, 45), (1080, 155), (0, 0, 0), cv2.FILLED)
                    drawColor = (0, 0, 0)
            cv2.rectangle(img, (x1, y1 - 25), (x2, y2 + 25), drawColor, cv2.FILLED)


        # 5. If Drawing Mode - Index finger is up
        if fingers[1] and fingers[2]==False:
            cv2.circle(img, (x1, y1), 15, drawColor, cv2.FILLED)
            print("Drawing Mode")
            if xp == 0 and yp == 0:
                xp, yp = x1, y1

            if drawColor == (0, 0, 0):
                cv2.line(img, (xp, yp), (x1, y1), drawColor, eraserThickness)
                cv2.line(imgCanvas, (xp, yp), (x1, y1), drawColor, eraserThickness)
            else:
                cv2.line(img, (xp, yp), (x1, y1), drawColor, brushThickness)
                cv2.line(imgCanvas, (xp, yp), (x1, y1), drawColor, brushThickness)

            xp, yp = x1, y1

    imgGray = cv2.cvtColor(imgCanvas, cv2.COLOR_BGR2GRAY)
    _, imgInv = cv2.threshold(imgGray, 50, 255, cv2.THRESH_BINARY_INV)
    imgInv = cv2.cvtColor(imgInv, cv2.COLOR_GRAY2BGR)
    img = cv2.bitwise_and(img, imgInv)
    img = cv2.bitwise_or(img, imgCanvas)




    # img = cv2.addWeighted(img, 0.5, imgCanvas, 0.5, 0)
    cv2.imshow("Image", img)
    cv2.waitKey(1)