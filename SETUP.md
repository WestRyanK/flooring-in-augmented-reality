# Setup
This project relies on several external repositories and datasets which will need to be set up before you can fully utilize FAR.
* [Unity3D v2020.3.18](https://unity3d.com/get-unity/download/archive) - for augmented reality app
* [nVidia Cuda v11.4.120](https://docs.nvidia.com/cuda/cuda-installation-guide-linux/index.html) - for training DeepLab network
* [Python v2.7](https://www.python.org/download/releases/2.7/) - for Deeplab network and prepping dataset

# General Setup
All commands assume you are running from the root FAR directory with the python virtual environment activated and python dependencies installed.

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


# OpenSurfaces Dataset Setup
To setup the OpenSurfaces Dataset, run the following command:
```
python datasets/opensurfaces/setup_opensurfaces.py
```