# Setup
This project relies on several external repositories and datasets which will need to be set up before you can fully utilize FAR.
* [Unity3D v2020.3.18](https://unity3d.com/get-unity/download/archive) - for augmented reality app
* [nVidia Cuda v11.4.120](https://docs.nvidia.com/cuda/cuda-installation-guide-linux/index.html) - for training DeepLab network
* [Python v2.7](https://www.python.org/download/releases/2.7/) - for Deeplab network and prepping dataset

## General Setup
All commands assume you are running from the root FAR directory with the python virtual environment activated and python dependencies installed.
Many commands have run configurations setup in PyCharm. These configurations are given where they exist. In general, 
you must complete these set up steps in the order that they appear. Also, be sure you install the exact versions of 
dependencies in `requirements.txt`. There are several dependencies that must be exact or else errors will occur 
(flatbuffers==1.12 and tensorflow==1.15.0 for example).

Initial Session:
```
pip install virtualenv
virtualenv --python=python2.7 venv
source venv/bin/activate
pip install -r requirements.txt
```
Subsequent Sessions:
```
source venv/bin/activate
```



## OpenSurfaces Dataset Setup
The OpenSurfaces Dataset is not included in the repo. You must download and process the dataset to use it for training.
To set up the Dataset do the following:

`Setup/Setup OpenSurfaces`:
```
python datasets/opensurfaces/setup_opensurfaces.py
```

## ADE20K Dataset Setup
Follow these steps to download and set up the ADE20K dataset.
1. Register to download the dataset by filling out [this](http://groups.csail.mit.edu/vision/datasets/ADE20K/request_data/register) request form.
2. Once your access has been approved, log in and download the dataset.
3. Move and rename the zip file to `datasets/ade20k/ade20k.zip`
4. Run the following command from the root FAR directory: 
```
python datasets/ade20k/setup_ade20k.py
```


## DeepLab Project Setup
The DeepLab network is part of the Tensorflow Models repository, so to train the segmentation network, we will need to 
clone that repository and follow their setup instructions. You can do that with the following:

`Setup DeepLab`:
```
sh deeplab/setup_deeplab.sh
```

Once the deeplab repository and the datasets have been set up, you can train the deeplab network on our dataset. The
following command will convert our dataset to tfrecord shards, train the network on the dataset, visualize the 
segmentation output, and export a tflite model for on device inference. It will then copy that model to the Assets 
folder of the Android AAR project.

`Create Network`:
```
sh deeplab/create_network.sh
```

## Far Plugin
Semantic segmentation and image processing for the app are done in an Android AAR library/plugin. To build the plugin
and export it to the Unity project, run the following command:

From Android Studio `far_plugin [exportPlugin]`
```
cd ./far_plugin
./gradlew exportPlugin
```