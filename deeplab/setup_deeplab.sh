#!/bin/bash

cd deeplab
rm -r models
git clone --depth 1 https://github.com/tensorflow/models

echo "Adding deeplab folders to PYTHONPATH in virtualenv"
cd ..
ROOT_DIR=$PWD
SITE_DIR=`python -c "from distutils.sysconfig import get_python_lib; print(get_python_lib())"`
echo $ROOT_DIR/deeplab/models/research > $SITE_DIR/deeplab_models_research.pth
echo $ROOT_DIR/deeplab/models/research/slim > $SITE_DIR/deeplab_models_slim.pth


echo "Copying modified data_generator.py into repo to add Flooring dataset"
cp deeplab/data_generator.py deeplab/models/research/deeplab/datasets/data_generator.py