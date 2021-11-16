#!/bin/bash

# This script is used to train DeepLab-Mobilenet-v2 using the Flooring dataset and export it to a tflite file.

# Settings
CLASS_COUNT=21
OUTPUT_SIZE=513
OUTPUT_STRIDE=32 # https://github.com/tensorflow/models/issues/6464
DATASET_NAME="flooring"
INIT_LAST_LAYER=true
NUM_ITERATIONS = 30000
MODEL_VARIANT="mobilenet_v2"

# Exit immediately if a command has an error.
set -e

# Set up the working environment
PROJECT_ROOT_DIR=$(PWD)
cd deeplab
#cd deeplab/models/research
#CURRENT_DIR=$(PWD)
#WORK_DIR="${CURRENT_DIR}/deeplab"

# Go to datasets folder and create Flooring dataset
DATASET_DIR="datasets"
cd "${WORK_DIR}/${DATASET_DIR}"
sh convert_flooring.sh

# Go back to original directory
cd "${CURRENT_DIR}"

# Set up directories
EXP_FOLDER="exp/mnv2_size${OUTPUT_SIZE}_stride${OUTPUT_STRIDE}"
INIT_FOLDER="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/init_models"
TRAIN_LOGDIR="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/${EXP_FOLDER}/train"
EVAL_LOGDIR="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/${EXP_FOLDER}/eval"
VIS_LOGDIR="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/${EXP_FOLDER}/vis"
EXPORT_DIR="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/${EXP_FOLDER}/export"
TFLITE_DIR="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/${EXP_FOLDER}/tflite"
REPRESENTATIVE_DATA_PATTERN="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/flooring_data/images/train/*.png"
METADATA_TFLITE_DIR="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/${EXP_FOLDER}/metadata_tflite"
mkdir -p "${INIT_FOLDER}"
mkdir -p "${TRAIN_LOGDIR}"
mkdir -p "${EVAL_LOGDIR}"
mkdir -p "${VIS_LOGDIR}"
mkdir -p "${EXPORT_DIR}"
mkdir -p "${TFLITE_DIR}"
mkdir -p "${METADATA_TFLITE_DIR}"

# Download initial checkpoint.
TF_INIT_ROOT="http://download.tensorflow.org/models"
CKPT_NAME="deeplabv3_mnv2_pascal_train_aug"
TF_INIT_CKPT="${CKPT_NAME}_2018_01_29.tar.gz"
cd "${INIT_FOLDER}"
wget -nd -c "${TF_INIT_ROOT}/${TF_INIT_CKPT}"
tar -xf "${TF_INIT_CKPT}"
cd "${CURRENT_DIR}"

# Train network
MY_DATASET="${WORK_DIR}/${DATASET_DIR}/${DATASET_NAME}/tfrecord"
python "${WORK_DIR}"/train.py \
  --logtostderr \
  --train_split="train" \
  --model_variant=${MODEL_VARIANT} \
  --dataset="${DATASET_NAME}" \
  --output_stride=${OUTPUT_STRIDE} \
  --train_batch_size=12 \
  --training_number_of_steps="${NUM_ITERATIONS}" \
  --fine_tune_batch_norm=true \
  --initialize_last_layer=${INIT_LAST_LAYER} \
  --tf_initial_checkpoint="${INIT_FOLDER}/${CKPT_NAME}/model.ckpt-30000" \
  --train_logdir="${TRAIN_LOGDIR}" \
  --dataset_dir="${MY_DATASET}" \
  --train_crop_size="${OUTPUT_SIZE},${OUTPUT_SIZE}" \
  --log_steps=100 \
  --save_interval_secs=240 \
  --save_summaries_secs=240

# Visualize results.
python "${WORK_DIR}"/vis.py \
  --logtostderr \
  --vis_split="val" \
  --model_variant=${MODEL_VARIANT} \
  --dataset="${DATASET_NAME}" \
  --checkpoint_dir="${TRAIN_LOGDIR}" \
  --vis_logdir="${VIS_LOGDIR}" \
  --dataset_dir="${MY_DATASET}" \
  --max_number_of_iterations=1 \
  --vis_crop_size="${OUTPUT_SIZE},${OUTPUT_SIZE}"

# Export the trained checkpoint.
CKPT_PATH="${TRAIN_LOGDIR}/model.ckpt-${NUM_ITERATIONS}"
EXPORT_PATH="${EXPORT_DIR}/frozen_inference_graph.pb"
python "${WORK_DIR}"/export_model.py \
  --logtostderr \
  --checkpoint_path="${CKPT_PATH}" \
  --export_path="${EXPORT_PATH}" \
  --output_stride=${OUTPUT_STRIDE} \
  --model_variant=${MODEL_VARIANT} \
  --num_classes=${CLASS_COUNT} \
  --inference_scales=1.0 \
  --crop_size=${OUTPUT_SIZE} \
  --crop_size=${OUTPUT_SIZE}

# Convert to tflite.
TFLITE_PATH="${TFLITE_DIR}/model.tflite"
python "${WORK_DIR}"/convert_tflite.py \
  --frozen_graph="${EXPORT_PATH}" \
  --tflite_path="${TFLITE_PATH}" \
  --dataset_pattern="${REPRESENTATIVE_DATA_PATTERN}" \
  --crop_size=${OUTPUT_SIZE} \
  --crop_size=${OUTPUT_SIZE}

# Add metadata to tflite model.
python "${WORK_DIR}"/add_metadata.py \
  --model_file="${TFLITE_PATH}" \
  --export_directory="${METADATA_TFLITE_DIR}"
