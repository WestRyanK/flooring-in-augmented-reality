import numpy as np
from PIL import Image
from PIL import ImageOps
import math
from tqdm import tqdm
import os
import glob

above_threshold_count = 0
threshold = 0.08

INPUT_DIR = ""
OUTPUT_DIR = "../data"
SRC_LABELS_DIR = os.path.join(INPUT_DIR, "photos-labels")
SRC_INPUTS_DIR = os.path.join(INPUT_DIR, "photos")
VAL = "val"
TEST = "test"
TRAIN = "train"
INPUTS = "inputs"
LABELS = "labels"
INPUT_EXT = ".jpg"
LABEL_EXT = ".png"

MATERIAL_CHANNEL = 1
VALUE_FLOOR = 83
DST_SIZE = 1024


def fuzz_copy(src, dst, dst_ext, resample_type, convert, pad_color):
    image = Image.open(src)

    image.thumbnail((DST_SIZE, DST_SIZE), resample=resample_type)
    image = pad(image, (DST_SIZE, DST_SIZE), resample_type, color=pad_color)
    if convert:
        image_array = np.array(image)
        h, w = image_array.shape[:2]
        image_array = (image_array[:, :, MATERIAL_CHANNEL] == VALUE_FLOOR).astype(np.uint8) * 255
        image = Image.fromarray(image_array)
    image.save(dst + dst_ext)
    image_mirror = ImageOps.mirror(image)
    image_mirror.save(dst + "_mirror" + dst_ext)


def fuzz_copy_all(paths, folder_name):
    dst_inputs_dir = os.path.join(OUTPUT_DIR, folder_name, INPUTS)
    dst_labels_dir = os.path.join(OUTPUT_DIR, folder_name, LABELS)
    os.path.join(OUTPUT_DIR, folder_name, INPUTS)
    if not os.path.exists(dst_inputs_dir):
        os.makedirs(dst_inputs_dir)
    if not os.path.exists(dst_labels_dir):
        os.makedirs(dst_labels_dir)
    print("Fuzz copying {}...".format(folder_name))
    for i, path in tqdm(enumerate(paths), total=len(paths)):
        filename = os.path.splitext(os.path.basename(path))[0]

        src_input = os.path.join(SRC_INPUTS_DIR, filename + INPUT_EXT)
        src_label = os.path.join(SRC_LABELS_DIR, filename + LABEL_EXT)
        dst_input = os.path.join(dst_inputs_dir, filename)
        dst_label = os.path.join(dst_labels_dir, filename)

        if not os.path.exists(src_input):
            print("'{}' doesn't exist!".format(src_input))
        elif not os.path.exists(src_label):
            print("'{}' doesn't exist!".format(src_label))
        else:
            fuzz_copy(src_input, dst_input, LABEL_EXT, resample_type=Image.BICUBIC, convert=False, pad_color=(128, 128, 128))
            fuzz_copy(src_label, dst_label, LABEL_EXT, resample_type=Image.NEAREST, convert=True, pad_color=0)


def filter_images(paths):
    filtered_paths = []
    print("Filtering...")
    for i, path in tqdm(enumerate(paths), total=len(paths)):
        image = Image.open(path)
        image_array = np.array(image)[:, :, MATERIAL_CHANNEL]
        floor_mask = image_array == VALUE_FLOOR
        floor_count = np.count_nonzero(floor_mask)
        total_count = image_array.shape[0] * image_array.shape[1]
        percent_floor = float(floor_count) / total_count
        if percent_floor > threshold:
            filtered_paths.append(path)
    return filtered_paths


def split_train_val_test(paths, val_percent, test_percent):
    val_count = int(math.floor(len(paths) * val_percent))
    test_count = int(math.floor(len(paths) * test_percent))
    if val_count > 0:
        val = paths[:val_count]
    else:
        val = []
    if test_count > 0:
        test = paths[val_count:val_count + test_count]
    else:
        test = []
    train = paths[val_count + test_count:]
    return train, val, test

def filter_copy_opensurfaces():
    search = os.path.join(SRC_LABELS_DIR, "*" + LABEL_EXT)
    input_paths = glob.glob(search)
    filtered_paths = filter_images(input_paths)
    train, val, test = split_train_val_test(filtered_paths, val_percent=0.02, test_percent=0.0)

    fuzz_copy_all(train, TRAIN)
    fuzz_copy_all(val, VAL)
    fuzz_copy_all(test, TEST)


# This is a modified version of PIL.ImageOps.pad(). That function had several bugs in 2.7 related to integer division
def pad(image, size, method=Image.NEAREST, color=None, centering=(0.5, 0.5)):
    """
    Returns a sized and padded version of the image, expanded to fill the
    requested aspect ratio and size.

    :param image: The image to size and crop.
    :param size: The requested output size in pixels, given as a
                 (width, height) tuple.
    :param method: What resampling method to use. Default is
                   :py:attr:`PIL.Image.NEAREST`.
    :param color: The background color of the padded image.
    :param centering: Control the position of the original image within the
                      padded version.
                          (0.5, 0.5) will keep the image centered
                          (0, 0) will keep the image aligned to the top left
                          (1, 1) will keep the image aligned to the bottom
                          right
    :return: An image.
    """

    im_ratio = float(image.width) / image.height
    dest_ratio = float(size[0]) / size[1]

    if im_ratio == dest_ratio:
        out = image.resize(size, resample=method)
    else:
        out = Image.new(image.mode, size, color)
        if im_ratio > dest_ratio:
            new_height = int(float(image.height) / image.width * size[0])
            if new_height != size[1]:
                image = image.resize((size[0], new_height), resample=method)

            y = int((size[1] - new_height) * max(0, min(centering[1], 1)))
            out.paste(image, (0, y))
        else:
            new_width = int(float(image.width) / image.height * size[1])
            if new_width != size[0]:
                image = image.resize((new_width, size[1]), resample=method)

            x = int((size[0] - new_width) * max(0, min(centering[0], 1)))
            out.paste(image, (x, 0))
    return out
