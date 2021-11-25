import numpy as np
from PIL import Image
from PIL import ImageOps
from tqdm import tqdm
import os
import math

THRESHOLD = 0.08
DST_SIZE = 1024


def fuzz_copy(src, dst, mirror_dst, resample_type, pad_color, conversion):
    image = Image.open(src)

    image.thumbnail((DST_SIZE, DST_SIZE), resample=resample_type)
    image = pad(image, (DST_SIZE, DST_SIZE), resample_type, color=pad_color)
    if conversion is not None:
        image_array = np.array(image)
        image_array = conversion(image_array)
        image = Image.fromarray(image_array)
    image.save(dst)
    image_mirror = ImageOps.mirror(image)
    image_mirror.save(mirror_dst)


def fuzz_copy_all(paths, folder_name, output_dir, inputs_dir, labels_dir, dst_ext, get_filename_input_label, conversion):
    dst_inputs_dir = os.path.join(output_dir, inputs_dir, folder_name)
    dst_labels_dir = os.path.join(output_dir, labels_dir, folder_name)
    if not os.path.exists(dst_inputs_dir):
        os.makedirs(dst_inputs_dir)
    if not os.path.exists(dst_labels_dir):
        os.makedirs(dst_labels_dir)
    print("Fuzz copying {}...".format(folder_name))
    for i, path in tqdm(enumerate(paths), total=len(paths)):
        filename, src_input, src_label = get_filename_input_label(path)
        dst_input = os.path.join(dst_inputs_dir, filename)
        dst_label = os.path.join(dst_labels_dir, filename)

        if not os.path.exists(src_input):
            print("'{}' doesn't exist!".format(src_input))
        elif not os.path.exists(src_label):
            print("'{}' doesn't exist!".format(src_label))
        else:
            if os.path.exists(dst_input + dst_ext) or os.path.exists(dst_label + dst_ext):
                print("'{}' already exists. Skipping".format(dst_input))
            else:
                input_dst_path = dst_input + dst_ext
                input_mirror_dst_path = dst_input + "_mirror" + dst_ext
                label_dst_path = dst_label + dst_ext
                label_mirror_dst_path = dst_label + "_mirror" + dst_ext

                try:
                    fuzz_copy(
                        src_input,
                        input_dst_path,
                        input_mirror_dst_path,
                        resample_type=Image.BICUBIC,
                        pad_color=(128, 128, 128),
                        conversion=None)
                    fuzz_copy(
                        src_label,
                        label_dst_path,
                        label_mirror_dst_path,
                        resample_type=Image.NEAREST,
                        pad_color=0,
                        conversion=conversion)
                except Exception as e:
                    print("Error processing '{}' and '{}'".format(src_input, src_label))
                    try_delete_all([input_dst_path, input_mirror_dst_path, label_dst_path, label_mirror_dst_path])


def try_delete_all(paths):
    for path in paths:
        if os.path.exists(path):
            os.remove(path)


def load_cached_list(list_name):
    if os.path.exists(list_name):
        with open(list_name, 'r') as f:
            list_items = f.readlines()
            list_items_stripped = []
            for list_item in list_items:
                list_items_stripped.append(list_item.strip())
            return list_items_stripped
    return None


def save_list(in_list, filename):
    in_list = sorted(list(in_list))
    with open(filename, "w") as output_file:
        for list_item in in_list:
            output_file.write(list_item + "\n")


def convert_to_mask(image_array, conversion, filter_values):
    if not isinstance(filter_values, list):
        filter_values = [filter_values]

    image_array = conversion(image_array)
    total_mask = np.zeros(image_array.shape)
    for filter_value in filter_values:
        mask = image_array == filter_value
        total_mask = np.logical_or(mask, total_mask)
    return total_mask


def filter_images(paths, conversion):
    filtered_list_file = "filtered.txt"
    filtered_paths = load_cached_list(filtered_list_file)
    if filtered_paths is not None:
        print "Found {} skipping filtering".format(filtered_list_file)
        return filtered_paths

    filtered_paths = []
    print("Filtering...")
    for i, path in tqdm(enumerate(paths), total=len(paths)):
        image = Image.open(path)
        image_array = np.array(image)
        total_floor_mask = conversion(image_array)
        floor_count = np.count_nonzero(total_floor_mask)
        total_count = image_array.shape[0] * image_array.shape[1]
        percent_floor = float(floor_count) / total_count
        if percent_floor > THRESHOLD:
            filtered_paths.append(path)
    save_list(filtered_paths, filtered_list_file)
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

            y = int((size[1] - new_height) * max(0.0, min(centering[1], 1)))
            out.paste(image, (0, y))
        else:
            new_width = int(float(image.width) / image.height * size[1])
            if new_width != size[0]:
                image = image.resize((new_width, size[1]), resample=method)

            x = int((size[0] - new_width) * max(0.0, min(centering[0], 1)))
            out.paste(image, (x, 0))
    return out
