# Mamey Barcode Generator

`mamey-barcode.console.py` is a versatile script for generating various types of barcodes and QR codes. It supports multiple barcode formats and allows customization of barcode settings.

## Features

- Generate different types of barcodes and QR codes
- Customizable settings for barcode generation
- Save barcode images to a file or output as a byte array
- Embed images into QR codes

## Supported Barcode Types

- PDF417
- DataMatrix
- Code128
- QRCode
- Code39
- EAN13

## Prerequisites

- Python 3.x
- pip (Python package installer)

## Setup Instructions

### Windows

1. **Install Python**

   Download and install Python from the [official Python website](https://www.python.org/downloads/windows/). Ensure you check the box "Add Python to PATH" during installation.

2. **Install Required Packages**

   Open Command Prompt and run the following command:

   ```sh
   pip install "qrcode[pil]" python-barcode pdf417gen pystrich requests Pillow
   ```

3. **Download the Script**

   Download `mamey-barcode.console.py` from the repository.

4. **Add Script to PATH**

   - Copy the script to a directory (e.g., `C:\barcode-generator`).
   - Add this directory to the system PATH:
     1. Open Control Panel > System and Security > System.
     2. Click on "Advanced system settings".
     3. Click on "Environment Variables".
     4. In the "System variables" section, find the `Path` variable and click "Edit".
     5. Click "New" and add the path to your script directory (e.g., `C:\barcode-generator`).

5. **Run the Script**

   Open Command Prompt and run:

   ```sh
   python mamey-barcode.console.py <data> <barcode_type> <output_format> [options]
   ```

### Mac

1. **Install Homebrew**

   Open Terminal and run the following command to install Homebrew:

   ```sh
   /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
   ```

2. **Install Python**

   Run the following command to install Python using Homebrew:

   ```sh
   brew install python
   ```

3. **Install Required Packages**

   Run the following command to install the required packages:

   ```sh
   pip3 install "qrcode[pil]" python-barcode pdf417gen pystrich requests Pillow
   ```

4. **Download the Script**

   Download `mamey-barcode.console.py` from the repository.

5. **Add Script to PATH**

   - Copy the script to a directory (e.g., `~/barcode-generator`).
   - Add this directory to the system PATH by adding the following line to your `~/.bash_profile` or `~/.zshrc` file:

     ```sh
     export PATH="$PATH:~/barcode-generator"
     ```

   - Reload the configuration file:

     ```sh
     source ~/.bash_profile   # For Bash users
     source ~/.zshrc          # For Zsh users
     ```

6. **Run the Script**

   Open Terminal and run:

   ```sh
   python3 mamey-barcode.console.py <data> <barcode_type> <output_format> [options]
   ```

### Linux

1. **Install Python**

   Open Terminal and run the following command to install Python:

   ```sh
   sudo apt-get update
   sudo apt-get install python3 python3-pip
   ```

2. **Install Required Packages**

   Run the following command to install the required packages:

   ```sh
   pip3 install "qrcode[pil]" python-barcode pdf417gen pystrich requests Pillow
   ```

3. **Download the Script**

   Download `mamey-barcode.console.py` from the repository.

4. **Add Script to PATH**

   - Copy the script to a directory (e.g., `~/barcode-generator`).
   - Add this directory to the system PATH by adding the following line to your `~/.bashrc` or `~/.profile` file:

     ```sh
     export PATH="$PATH:~/barcode-generator"
     ```

   - Reload the configuration file:

     ```sh
     source ~/.bashrc
     source ~/.profile
     ```

5. **Run the Script**

   Open Terminal and run:

   ```sh
   python3 mamey-barcode.console.py <data> <barcode_type> <output_format> [options]
   ```

## Usage

### Basic Usage

```sh
python3 mamey-barcode.console.py <data> <barcode_type> <output_format> [options]
```

### Arguments

- `<data>`: The data to encode in the barcode.
- `<barcode_type>`: The type of barcode to generate (`pdf417`, `datamatrix`, `code128`, `qrcode`, `code39`, `ean13`).
- `<output_format>`: The output format (`file` or `byte`).

### Options

- `--filename`: The filename (with path) to save the barcode image if output format is `file`. Default is `barcode.png`.
- `--columns`: Number of columns for `pdf417`. Default is `6`.
- `--security_level`: Security level for `pdf417`. Default is `2`.
- `--scale`: Scale for `pdf417`. Default is `3`.
- `--ratio`: Ratio for `pdf417`. Default is `3`.
- `--fg_color`: Foreground color for barcodes. Default is `#000000`.
- `--bg_color`: Background color for barcodes. Default is `#FFFFFF`.
- `--padding`: Padding for `pdf417`. Default is `20`.
- `--version`: Version for `qrcode`. Default is `1`.
- `--box_size`: Box size for `qrcode`. Default is `10`.
- `--border`: Border size for `qrcode`. Default is `4`.
- `--image_url`: URL of the image to embed in `qrcode`. Default is `None`.

### Examples

Generate a QR code and save it to a file:

```sh
python3 mamey-barcode.console.py "Hello, World!" "qrcode" "file" --filename "/path/to/qrcode.png"
```

Generate a DataMatrix barcode and print the byte array:

```sh
python3 mamey-barcode.console.py "Hello, World!" "datamatrix" "byte"
```

Generate a PDF417 barcode with custom settings:

```sh
python3 mamey-barcode.console.py "Hello, World!" "pdf417" "file" --filename "/path/to/pdf417.png" --columns 8 --security_level 3 --scale 5 --fg_color "#FF0000" --bg_color "#FFFFFF"
```

