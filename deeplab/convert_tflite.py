
import os
import numpy as np
import tensorflow as tf
import PIL
from cv2 import cv2
import glob

flags = tf.app.flags
FLAGS = flags.FLAGS

flags.DEFINE_string('frozen_graph', None, 'Path to frozen graph to convert to tflite.')

flags.DEFINE_string('tflite_path', None,
                    'Path to output Tensorflow tflite file.')

flags.DEFINE_string('dataset_pattern', None, 'Path to images for representative dataset')

flags.DEFINE_multi_integer('crop_size', [129, 129],
                           'Crop size [height, width].')


def representative_dataset():
    pattern = FLAGS.dataset_pattern
    count = 100
    image_filenames = glob.glob(pattern)
    take_count = min(count, len(image_filenames) - 1)
    image_filenames = image_filenames[:take_count]
    images = []
    for image_filename in image_filenames:
        image = np.asarray(PIL.Image.open(image_filename))
        image = cv2.resize(image, (FLAGS.crop_size[0], FLAGS.crop_size[1]))
        # image = tf.convert_to_tensor(image, dtype=tf.float32)
        image = image.astype(np.float32)
        image = image[np.newaxis, ...]
        yield [image]
    #     images.append(image)
    # images = tf.stack(images)
    # for data in tf.data.Dataset.from_tensor_slices((images)).batch(1).take(take_count):
    #     yield [tf.dtypes.cast(data, tf.float32)]
#
# tf.enable_eager_execution()
# Load the TensorFlow model
converter = tf.compat.v1.lite.TFLiteConverter.from_frozen_graph(
    graph_def_file = FLAGS.frozen_graph,
    input_arrays = ['sub_2'], # For the Xception model it needs to be `sub_7`, for MobileNet it would be `sub_2`
    output_arrays = ['ResizeBilinear_2'],
    input_shapes={'sub_2':[1,FLAGS.crop_size[0],FLAGS.crop_size[1],3]}
)
# converter.optimizations = [tf.lite.Optimize.DEFAULT]

# converter.representative_dataset = representative_dataset
# converter.target_spec.supported_ops = [tf.lite.OpsSet.TFLITE_BUILTINS_INT8]
# converter.inference_input_type = tf.uint8
# converter.inference_output_type = tf.uint8

# Convert to TFLite Model
tflite_model = converter.convert()
# Save Model as tflite format
# tflite_path = "deeplab/datasets/pascal_voc_seg/exp/train_on_trainval_set_mobilenetv2/tflite/model.tflite"
if not os.path.exists(os.path.dirname(FLAGS.tflite_path)):
    os.makedirs(os.path.dirname(FLAGS.tflite_path))
tflite_model_size = open(FLAGS.tflite_path, 'wb').write(tflite_model)




