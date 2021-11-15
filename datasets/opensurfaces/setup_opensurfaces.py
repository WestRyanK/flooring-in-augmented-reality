import urllib2
import os
import zipfile
import process_opensurfaces
import filter_opensurfaces


def download_extract_opensurfaces_dataset():
    datasets_dir = os.path.join('datasets', 'opensurfaces')
    opensurfaces_zip_path = os.path.join(datasets_dir, 'opensurfaces.zip')
    opensurfaces_url = 'http://labelmaterial.s3.amazonaws.com/release/opensurfaces-release-0.zip'
    opensurfaces_root_dir = os.path.join(datasets_dir, 'opensurfaces')
    if not os.path.exists(opensurfaces_zip_path):
        with open(opensurfaces_zip_path, 'wb') as zip_file:
            print "Downloading OpenSurfaces Dataset..."
            zip_file.write(urllib2.urlopen(opensurfaces_url).read())
            print "Download Complete"
    else:
        print "OpenSurfaces already downloaded"
    if not os.path.exists(opensurfaces_root_dir):
        os.makedirs(opensurfaces_root_dir)
        with zipfile.ZipFile(opensurfaces_zip_path, 'r') as zip_file:
            print "Unzipping OpenSurfaces Dataset..."
            zip_file.extractall(datasets_dir)
            print "Unzip Complete"
    else:
        print "OpenSurfaces already unzipped"


download_extract_opensurfaces_dataset()
cwd = os.getcwd()
os.chdir(os.path.join('datasets', 'opensurfaces', 'opensurfaces'))
if not os.path.exists('photos') and not os.path.exists('photos-labels'):
    print "Downloading and processing OpenSurfaces images"
    process_opensurfaces.process_opensurfaces()
    print "Downloading and processing complete"
else:
    print "OpenSurfaces images already downloaded"

if not os.path.exists("../data"):
    print "Filtering OpenSurfaces to only images with flooring..."
    filter_opensurfaces.filter_copy_opensurfaces()
    print "Flooring filter complete"
else:
    print "Initial filtering already complete"
