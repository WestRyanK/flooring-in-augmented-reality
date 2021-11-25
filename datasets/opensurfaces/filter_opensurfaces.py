import numpy as np
from PIL import Image
from PIL import ImageOps
import math
from tqdm import tqdm
import os
import glob
import process_dataset_utils

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
DST_EXT = LABEL_EXT

MATERIAL_CHANNEL = 1
VALUE_FLOOR = 83


def convert_to_opensurfaces_mask(img):
    return process_dataset_utils.convert_to_mask(img, lambda x: x[:, :, MATERIAL_CHANNEL], VALUE_FLOOR)


def get_filename_input_label(path):
    filename = os.path.splitext(os.path.basename(path))[0]
    src_input = os.path.join(SRC_INPUTS_DIR, filename + INPUT_EXT)
    src_label = os.path.join(SRC_LABELS_DIR, filename + LABEL_EXT)
    return filename, src_input, src_label


def filter_copy_opensurfaces():
    search = os.path.join(SRC_LABELS_DIR, "*" + LABEL_EXT)
    input_paths = glob.glob(search)
    filtered_paths = process_dataset_utils.filter_images(input_paths, convert_to_opensurfaces_mask)
    train, val, test = process_dataset_utils.split_train_val_test(filtered_paths, val_percent=0.02, test_percent=0.0)

    process_dataset_utils.fuzz_copy_all(
        train,
        TRAIN,
        OUTPUT_DIR,
        INPUTS,
        LABELS,
        DST_EXT,
        get_filename_input_label,
        convert_to_opensurfaces_mask)
    process_dataset_utils.fuzz_copy_all(
        val,
        VAL,
        OUTPUT_DIR,
        INPUTS,
        LABELS,
        DST_EXT,
        get_filename_input_label,
        convert_to_opensurfaces_mask)
    process_dataset_utils.fuzz_copy_all(
        test,
        TEST,
        OUTPUT_DIR,
        INPUTS,
        LABELS,
        DST_EXT,
        get_filename_input_label,
        convert_to_opensurfaces_mask)
