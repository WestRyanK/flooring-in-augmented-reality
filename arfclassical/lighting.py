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

class LightingSimulator:
    def clamp(self, input_img, min_value, max_value):
        _, input_img = cv2.threshold(input_img, 0, 255, cv2.THRESH_TOZERO)
        _, input_img = cv2.threshold(input_img, 255, 255, cv2.THRESH_TRUNC)
        return input_img
        pass


    def adjust_to_average_brightness(self, input_img, mask_img):
        avg_brightness = cv2.mean(input_img, mask_img)[0]
        print(avg_brightness)
        # show("a", (255 - avg_brightness) + input_img * mask_img)
        adjusted_img = (128 - avg_brightness) + input_img * mask_img
        adjusted_img = self.clamp(adjusted_img, 0, 255)
        # less_than = cv2.compare(adjusted_img, 0, cv2.CMP_LT)
        # adjusted_img[less_than > 0] = 0
        # greater_than = cv2.compare(adjusted_img, 255, cv2.CMP_GT)
        # adjusted_img[greater_than > 0] = 255

        return adjusted_img[:, :, np.newaxis].astype(np.uint8)
        # show("b", adjusted_img)


    def sim_lighting(self, input_img, label_img):
        mask_img = (label_img[:, :, 0] > 0).astype(np.uint8)
        blurred_mask_img = mask_img.astype(np.float)
        lighting_img = cv2.cvtColor(input_img, cv2.COLOR_BGR2GRAY)
        show("lighting", lighting_img)
        lighting_img[mask_img == 0] = 0
        show("masked lighting", lighting_img)
        show("mask", mask_img * 255)
        blur_kernel = (31, 31)
        cv2.blur(lighting_img, blur_kernel, lighting_img)
        cv2.blur(blurred_mask_img, blur_kernel, blurred_mask_img)
        show("blurred mask", blurred_mask_img * 255)
        show("blurred lighting", lighting_img)
        lighting_img = lighting_img.astype(np.float)
        lighting_img = cv2.divide(lighting_img, blurred_mask_img)
        lighting_img[mask_img == 0] = 0
        show("blurred lighting masked", lighting_img)
        lighting_img = self.adjust_to_average_brightness(lighting_img, mask_img)
        show("adjusted", lighting_img)
        lighting_img = cv2.cvtColor(lighting_img, cv2.COLOR_GRAY2BGR)
        combined_img = np.multiply(input_img, 1 - mask_img[:,:,np.newaxis]) + np.multiply(lighting_img, mask_img[:,:,np.newaxis])
        show("sim", combined_img)
