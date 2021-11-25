import os
import zipfile
import glob
import filter_ade20k

def glob_folder(pattern, root_dir):
    path_pattern = os.path.join(root_dir, pattern)
    paths = glob.glob(path_pattern)
    path_pattern_upper = os.path.join(root_dir, pattern.upper())
    paths_upper = glob.glob(path_pattern_upper)
    paths.extend(paths_upper)
    for path in paths:
        if os.path.isdir(path):
            return path
    return None

def extract_ade20k_dataset():
    datasets_dir = os.path.join('datasets', 'ade20k')
    ade20k_zip_path = os.path.join(datasets_dir, 'ade20k.zip')
    ade20k_root_dir = os.path.join(datasets_dir, 'ade20k')

    if not os.path.exists(ade20k_zip_path):
        return False
    else:
        if not os.path.exists(ade20k_root_dir):
            with zipfile.ZipFile(ade20k_zip_path, 'r') as zip_file:
                print "Unzipping ADE20K dataset..."
                zip_file.extractall(datasets_dir)
                print "Unzip complete"
                extracted_folder = glob_folder("ade20k*", datasets_dir)
                os.rename(extracted_folder, ade20k_root_dir)
        else:
            print "ADE20K already unzipped"
        return True

if not extract_ade20k_dataset():
    print "ade20k.zip is missing. Follow the ADE20K setup instructions in SETUP.md to download the dataset."
    exit(1)

cwd = os.getcwd()
os.chdir(os.path.join('datasets', 'ade20k', 'ade20k'))

# filter_ade20k.filter_ade20k()
filter_ade20k.filter_copy_ade20k()
# filter_ade20k.print_categories()
