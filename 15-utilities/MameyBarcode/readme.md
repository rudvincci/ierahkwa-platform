# Mamey Barcode Generation API Documentation

This API allows users to generate various types of barcodes including PDF417, DataMatrix, Code128, QR Code, Code39, and EAN-13. This documentation covers installation, API usage, parameters, payload descriptions, and provides examples for generating each barcode type.

To run your Dockerized Flask application, you will follow a series of steps to build your Docker image and then run it using Docker Compose. This process involves using the `Dockerfile` and `docker-compose.yml` files you've created. Here are the instructions:

## Prerequisites

Ensure you have Docker and Docker Compose installed on your system. If not, you can download them from the [Docker website](https://www.docker.com/products/docker-desktop). Docker Compose comes included with Docker Desktop for Windows and Mac, but you might need to install it separately on Linux.

### Step 1: Build the Docker Image

Navigate to your project directory where your `Dockerfile` and `docker-compose.yml` are located. Open a terminal or command prompt in this directory.

Run the following command to build your Docker image using Docker Compose. This command will also pull any necessary base images, install dependencies, and set up your image based on the instructions in your `Dockerfile`.

```bash
docker-compose up --build
```

The `--build` flag tells Docker Compose to build the images before starting the containers. If you've previously built the image and made no changes to your `Dockerfile` or dependencies, you can omit the `--build` flag in subsequent runs.

### Step 2: Accessing Your Application

Once the build process is complete and the server has started, your Flask application will be running inside a Docker container on your host machine. By default, the application will be accessible through your web browser at `http://localhost:5000`, unless you've configured a different port in your `docker-compose.yml` file.

### Step 3: Stopping the Application

When you're done and want to stop your Docker container, press `CTRL+C` in the terminal where your container is running. This command stops the running container. To remove the containers along with their network, but preserve your data and images, run:

```bash
docker-compose down
```

### Additional Commands

- **View running containers**: To see all currently running Docker containers, use the command:
  ```bash
  docker ps
  ```

- **Start containers in detached mode**: To run your containers in the background, use the `-d` flag:
  ```bash
  docker-compose up -d
  ```

- **Stop containers**: To stop containers without removing them, you can use:
  ```bash
  docker-compose stop
  ```

- **Remove containers, networks, and images created by `up`**: If you wish to completely remove all containers, networks, and the default image created by `docker-compose up`, use:
  ```bash
  docker-compose down --rmi all
  ```

This setup provides a basic overview of running a Dockerized Flask application with Docker Compose. As you progress, you may need to adapt these instructions based on your deployment environment or additional services you integrate.

### 🗝️ Hotkeys

- **W:** Add Redis or database service to your Docker setup
- **S:** Configure a reverse proxy with Docker
- **D:** Scale your application with Docker Swarm or Kubernetes
- **K:** Show list of all hotkeys


## Installation on your local computer

To run this API, you'll need Python installed on your system along with the Flask framework and a few other dependencies. Here's how to get started:

1. **Install Python**: Ensure that Python 3.x is installed on your system. You can download it from [the official Python website](https://www.python.org/downloads/).

2. **Setup Virtual Environment** (Optional but recommended):
   ```bash
   python3 -m venv venv
   source venv/bin/activate  # On Windows, use `venv\Scripts\activate`
   source myenv/bin/activate
   ```


 Note: Make sure to add the environment folder in `.gitignore` and `.dockerignore` files if use other environment name than `venv`.

3. **Install Dependencies**:
   ```bash
   pip install Flask qrcode python-barcode flask-caching flask_httpauth flask_limiter pdf417gen pystrich Pillow requests
   ```

4. **Run the API**:
   ```bash
   python3 your_script_name.py
   ```

## API Usage

### Endpoint

`POST http://localhost:5000/generate-barcode`

This endpoint accepts JSON payloads with data and configuration for generating barcodes.

### Payload Structure

The JSON payload structure is as follows:

- `data` (required): The data to encode in the barcode.
- `type` (required): The type of barcode to generate. Supported types are: `pdf417`, `datamatrix`, `code128`, `qrcode`, `code39`, `ean13`.
- Additional parameters vary by barcode type and are detailed in the sections below.

### Common Parameters

- `fg_color`: Foreground color of the barcode. Default is black.
- `bg_color`: Background color of the barcode. Default is white.

### PDF417 Parameters

#### Example Payload for PDF417

```json
{
  "data": "YourDataHere",
  "type": "pdf417",
  "columns": 5,
  "security_level": 3,
  "scale": 4,
  "ratio": 3,
  "fg_color": "#000000",
  "bg_color": "#FFFFFF",
  "format": "png"
}
```

- `columns`: Number of columns in the barcode. Default is 6.
    - **Definition**: Specifies the number of columns in the PDF417 barcode. This parameter directly influences the barcode's width. A higher number of columns will result in a wider barcode.
    - **Default Value**: 6 columns.
    - **Impact**: Adjusting the number of columns allows for customization of the barcode's physical size and data density. It's important for applications that require barcodes of a specific width or are constrained by the scanning environment.
- `security_level`: Security level, affects error correction capacity. Default is 2.
    - **Definition**: Determines the error correction capacity of the barcode. The PDF417 barcode standard supports error correction levels ranging from 0 to 8, referred to here as the security level.
    - **Default Value**: 2.
    - **Impact**: A higher security level increases the barcode's resilience to damage and distortion, allowing more of the data to be recovered if part of the barcode is obscured or damaged. However, increasing the security level also increases the size of the barcode, as more error correction codewords are added.

- `scale`: Scale factor for barcode elements. Default is 3.
    - **Definition**: The scale factor controls the size of individual barcode elements (modules). Essentially, it magnifies or reduces the barcode without altering its proportions or encoded data.
    - **Default Value**: 3.
    - **Impact**: Scaling affects the overall size of the barcode, making it larger or smaller while keeping its data content constant. This is particularly useful for printing or display purposes where the barcode needs to be sized appropriately for scanners or for fitting into designated spaces.

- `ratio`: Height to width ratio of barcode elements. Default is 3.
    - **Definition**: Dictates the height-to-width ratio of the individual barcode elements. This can adjust the barcode's aspect ratio without changing the encoded data.
    - **Default Value**: 3.
    - **Impact**: Altering the ratio can help make the barcode more readable in environments where the scanner has limitations in reading barcodes of certain dimensions. It also allows for aesthetic adjustments to fit the barcode into specific designs or layouts.
- `format`: Output format, either `png` or `svg`. Default is `png`.
    - **Definition**: Specifies the output format of the generated barcode image. Available options are PNG and SVG.
    - **Default Value**: PNG.
    - **Impact**: The choice between PNG and SVG formats depends on the use case. PNG is a raster format best suited for direct use in web pages, documents, or printing where the size is fixed. SVG, being a vector format, is ideal for scenarios where the barcode needs to be resized without loss of quality, such as in large scale posters or varying print sizes.


### DataMatrix, Code128, Code39, and EAN-13 Parameters

These barcode types only use the common parameters `fg_color` and `bg_color`.

#### Example Payload for DataMatrix

```json
{
  "data": "YourDataHere",
  "type": "datamatrix",
  "fg_color": "#000000",
  "bg_color": "#FFFFFF"
}
```

### QR Code Parameters
#### Example Payload for QR Code

```json
{
  "data": "YourDataHere",
  "type": "qrcode",
  "version": 2,
  "error_correction": "M",
  "box_size": 10,
  "border": 4,
  "fg_color": "#000000",
  "bg_color": "#FFFFFF",
  "image_url": "http://example.com/icon.png"
}
```
- `version`: Version of the QR Code. Default is 1.
    - **Definition**: QR codes are standardized into versions, indicating the overall dimensions and capacity of the QR code. Versions range from 1 to 40, with 1 being the smallest and 40 the largest.
    - **Default Value**: 1.
    - **Impact**: The version determines the amount of data that can be encoded. Higher versions support more data but result in larger QR codes. Selecting the appropriate version is crucial for balancing data capacity with physical size constraints.
- `error_correction`: Error correction level. Can be `L`, `M`, `Q`, or `H`. Default is `L`.
    - **Definition**: Error correction levels in QR codes help recover data even if the code is dirty, damaged, or partially obscured. Four levels are available:
    - **L (Low)**: Recovers 7% of data.
    - **M (Medium)**: Recovers 15% of data.
    - **Q (Quartile)**: Recovers 25% of data.
    - **H (High)**: Recovers 30% of data.
    - **Default Value**: L.
    - **Impact**: Higher error correction levels increase the QR code's resilience at the cost of increasing its size. This feature is crucial for ensuring the QR code remains functional in challenging environments.
- `box_size`: Size of each box in the QR code in pixels. Default is 10.
    - **Definition**: Specifies the size of each square in the QR code grid, measured in pixels.
    - **Default Value**: 10 pixels.
    - **Impact**: Adjusting the box size changes the overall dimensions of the QR code. Larger box sizes result in larger, more easily scanned codes but may require more space for display.
- `border`: Border size around the QR code in boxes. Default is 4.
    - **Definition**: Determines the width of the white border surrounding the QR code, measured in boxes.
    - **Default Value**: 4 boxes.
    - **Impact**: The border enhances the QR code's detectability against its background, ensuring scanners can easily distinguish the code's edges. A sufficient border is crucial for reliable scanning.
- `image_url`: (optional) URL of an image to embed in the center of the QR code.
    - **Definition**: An optional parameter that specifies the URL of an image to be embedded at the center of the QR code.
    - **Impact**: Embedding an image (such as a logo) can enhance brand recognition but must be used carefully to not overly obscure the QR code data, especially with lower error correction levels.


