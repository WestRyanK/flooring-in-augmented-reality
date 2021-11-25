import numpy as np
import os
from PIL import Image
import glob
import shutil
import process_dataset_utils
import json
from tqdm import tqdm

FLOORING_CATEGORIES = {
    "floor, flooring": 976,
    "rug, carpet, carpeting": 2178
}
FLOORING_KEYS = set(FLOORING_CATEGORIES.keys())
FLOORING_VALUES = list(FLOORING_CATEGORIES.values())

OUTPUT_DIR = "../data"
VAL = "val"
TEST = "test"
TRAIN = "train"
INPUTS = "inputs"
LABELS = "labels"

JSON_EXT = ".json"
INPUT_EXT = ".jpg"
SEG_EXT = "_seg"
DST_EXT = ".png"
LABEL_EXT = SEG_EXT + DST_EXT


def filter_flooring_by_json(categories):
    flooring_list_file = "flooring.txt"
    flooring_paths = process_dataset_utils.load_cached_list(flooring_list_file)
    if flooring_paths is not None:
        print "Found {} skipping json parsing".format(flooring_list_file)
        return flooring_paths

    search_pattern = os.path.join("images", "ADE", "training", "*", "*", "ADE_train_*.json")
    paths = glob.glob(search_pattern)
    flooring_paths = set()
    print "Parsing json to find flooring images..."
    for i, path in tqdm(enumerate(paths), total=len(paths)):
        with open(path, 'r') as f:
            file_text = f.read()
            file_json = json.loads(file_text.decode("utf-8", "ignore"))
            objects_info = file_json["annotation"]["object"]
            for object_info in objects_info:
                try:
                    object_name = str(object_info["name"])
                    if object_name in categories:
                        flooring_paths.add(path)
                except UnicodeEncodeError as e:
                    print "Bad object name: {}".format(object_name)
    print "Json parsing complete"
    process_dataset_utils.save_list(flooring_paths, flooring_list_file)
    return flooring_paths


def convert_to_ade20k_mask(img):
    # https://github.com/CSAILVision/ADE20K/blob/main/utils/utils_ade20k.py
    def class_converter(x):
        return (x[:, :, 0] / 10).astype(np.int32) * 256 + (x[:, :, 1].astype(np.int32))

    return process_dataset_utils.convert_to_mask(img, class_converter, FLOORING_VALUES)


def get_filename_input_label(path):
    path_dir = os.path.dirname(path)
    filename = os.path.basename(path).replace(LABEL_EXT, "")
    src_input = os.path.join(path_dir, filename + INPUT_EXT)
    src_label = os.path.join(path_dir, filename + LABEL_EXT)
    return filename, src_input, src_label


def filter_copy_ade20k():
    flooring_json_paths = filter_flooring_by_json(FLOORING_KEYS)
    flooring_paths = []
    for path in flooring_json_paths:
        path = path.replace(JSON_EXT, LABEL_EXT)
        flooring_paths.append(path)

    filtered_paths = process_dataset_utils.filter_images(flooring_paths, convert_to_ade20k_mask)
    train, val, test = process_dataset_utils.split_train_val_test(filtered_paths, val_percent=0.02, test_percent=0.0)
    process_dataset_utils.fuzz_copy_all(
        train,
        TRAIN,
        OUTPUT_DIR,
        INPUTS,
        LABELS,
        DST_EXT,
        get_filename_input_label,
        convert_to_ade20k_mask)
    process_dataset_utils.fuzz_copy_all(
        val,
        VAL,
        OUTPUT_DIR,
        INPUTS,
        LABELS,
        DST_EXT,
        get_filename_input_label,
        convert_to_ade20k_mask)
    process_dataset_utils.fuzz_copy_all(
        test,
        TEST,
        OUTPUT_DIR,
        INPUTS,
        LABELS,
        DST_EXT,
        get_filename_input_label,
        convert_to_ade20k_mask)
