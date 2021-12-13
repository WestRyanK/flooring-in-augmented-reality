import cv2
import numpy as np

def show(title, image):
    if len(image.shape) == 2:
        display_image = cv2.cvtColor(image.astype(np.uint8), cv2.COLOR_GRAY2BGR)
    else:
        display_image = image
    cv2.imshow(title, display_image)
    cv2.imwrite("test.png", image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

class WatershedSegmenter:
    def __init__(self):
        self.kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))

    def make_markers(self, label_image):
        label_image = label_image[:, :, 2].astype(np.uint8)
        label_image = (label_image > 0).astype(np.uint8)
        show("label_image", label_image * 255)
        sure_background = cv2.dilate(label_image, self.kernel, iterations=15)
        show("sure_background", sure_background * 255)
        sure_background = cv2.absdiff(sure_background, 1)
        show("not", sure_background * 255)
        # sure_background = 1 - sure_background
        # show("background", sure_background * 255)

        sure_foreground = cv2.erode(label_image, self.kernel, iterations=5)
        show("foreground", sure_foreground * 255)

        markers = sure_foreground * 1 + sure_background * 2
        show("markers", markers * 127)

        return markers.astype(np.int32)


    def segment(self, input, label_image):
        # ret, markers = cv2.connectedComponents(input)
        width, height, depth = input.shape
        markers = self.make_markers(label_image)
        # markers = np.zeros((width, height), dtype=np.int32)
        # cv2.circle(markers, (256, 450), 40, 128, -1)
        # cv2.rectangle(markers, (0,0), (512, 150), 256, -1)
        # # cv2.imwrite("output.png", markers)
        # show("markers", markers)
        watershed_image = cv2.watershed(input, markers)
        watershed_image += 1
        show("watershed", watershed_image.astype(np.uint8) * 63)
        watershed_image = (watershed_image == 2).astype(np.uint8) * 255
        show("floor", watershed_image)
        return watershed_image