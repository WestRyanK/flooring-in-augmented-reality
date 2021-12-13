import glob
import os
import cv2


class Loader:
    def __init__(self, input_path):
        self.input_path = input_path
        self.input_files = glob.glob(input_path)
        self.input_files = sorted(self.input_files)
        self.count = len(self.input_files)

    def enumerate(self):
        for i, input_file in enumerate(self.input_files):

            filename = os.path.splitext(os.path.basename(input_file))[0]
            base_folder = os.path.dirname(os.path.dirname(input_file))
            label_filename = os.path.join(base_folder, "labels", filename + ".png")
            label_image = cv2.imread(label_filename)
            input_image = cv2.imread(input_file)
            yield input_file, input_image, label_filename, label_image



