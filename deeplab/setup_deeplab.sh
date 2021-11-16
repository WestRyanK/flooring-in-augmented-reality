#!/bin/bash

cd deeplab
git clone --depth 1 https://github.com/tensorflow/models

# Add folders to PYTHONPATH in virtualenv
cd ..
ROOT_DIR=$PWD
SITE_DIR=`python -c "from distutils.sysconfig import get_python_lib; print(get_python_lib())"`
echo $ROOT_DIR/deeplab/models > $SITE_DIR/deeplab_models.pth
echo $ROOT_DIR/deeplab/models/research/slim > $SITE_DIR/deeplab_models_slim.pth


# Copy modified data_generator.py into repo to add Flooring dataset
cp deeplab/data_generator.py deeplab/models/research/deeplab/datasets/data_generator.py