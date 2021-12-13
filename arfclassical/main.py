import argparse
import loader
import os
import cv2
import numpy as np
import watershed
import lighting
from tqdm import tqdm

def overlay(input, mask):
    mask = (mask > 0) * 255
    mask = np.stack([mask, np.zeros(mask.shape), np.zeros(mask.shape)], axis=2).astype(np.uint8)
    output = cv2.addWeighted(input, 1, mask, 1, 0)
    return output

def show(title, image):
    cv2.imshow(title, image)
    cv2.imwrite("test.png", image)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("-i", "--input-path",
                        type=str,
                        default="./data/exp/inputs/image*.png",
                        help="The path to the dataset of input images to segment")
    parser.add_argument("-o", "--output-path",
                        type=str,
                        default="./output/",
                        help="The path where to put the segmented output images")
    ARGS, unparsed = parser.parse_known_args()

    os.makedirs(ARGS.output_path, exist_ok=True)
    loader = loader.Loader(ARGS.input_path)
    segmenter = watershed.WatershedSegmenter()
    lighting_sim = lighting.LightingSimulator()
    for input_file, input_image, label_file, label_image in tqdm(loader.enumerate(), total=loader.count):

        print(input_file)
        lighting_sim.sim_lighting(input_image, label_image)
        # output_image = segmenter.segment(input_image, label_image)

        # output_image = overlay(input_image, output_image)
        # filename = os.path.basename(input_file)
        # output_file = os.path.join(ARGS.output_path, filename)
        # show("output", output_image)
