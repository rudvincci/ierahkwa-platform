import qrcode
import barcode
from barcode.writer import ImageWriter
import logging
import io
from PIL import Image
import requests
from pdf417gen import encode, render_image as render_pdf417_image, render_svg
from pystrich.datamatrix import DataMatrixEncoder
from pystrich.code128 import Code128Encoder
import sys
import base64
import argparse

logging.basicConfig(level=logging.INFO)

def save_image(image, filename, format='PNG'):
    output = io.BytesIO()
    image.save(output, format=format)
    output.seek(0)
    with open(filename, 'wb') as f:
        f.write(output.getvalue())
    logging.info(f"Image saved as {filename}")
    return output.getvalue()

def handle_error(reason):
    logging.error(f"Error: {reason}")
    sys.exit(1)

def generate_pdf417(data, content):
    try:
        codes = encode(data, columns=content.get('columns', 6), security_level=content.get('security_level', 2))
        scale = content.get('scale', 3)
        ratio = content.get('ratio', 3)
        fg_color = content.get('fg_color', '#000000')
        bg_color = content.get('bg_color', '#FFFFFF')
        padding = content.get('padding', 20)
        filename = content.get('filename', 'pdf417.png')

        if content.get('format', 'png').lower() == 'svg':
            svg = render_svg(codes, scale=scale, ratio=ratio, color=fg_color)
            with open(filename, 'wb') as output:
                output.write(svg)
            logging.info(f"SVG saved as {filename}")
        else:
            return save_image(render_pdf417_image(codes, scale=scale, ratio=ratio, padding=padding, fg_color=fg_color, bg_color=bg_color), filename)
    except Exception as e:
        handle_error(f"Failed to generate PDF417 barcode: {str(e)}")

def generate_datamatrix(data, content):
    try:
        encoder = DataMatrixEncoder(data)
        filename = content.get('filename', 'datamatrix.png')
        return save_image(encoder.get_image(), filename)
    except Exception as e:
        handle_error(f"Failed to generate DataMatrix barcode: {str(e)}")

def generate_code128(data, content):
    try:
        encoder = Code128Encoder(data)
        filename = content.get('filename', 'code128.png')
        return save_image(encoder.get_image(), filename)
    except Exception as e:
        handle_error(f"Failed to generate Code128 barcode: {str(e)}")

def generate_qrcode(data, content):
    try:
        qr = qrcode.QRCode(
            version=content.get('version', 1),
            error_correction=qrcode.constants.ERROR_CORRECT_L,
            box_size=content.get('box_size', 10),
            border=content.get('border', 4)
        )
        qr.add_data(data)
        qr.make(fit=True)
        img = qr.make_image(fill_color=content.get('fg_color', 'black'), back_color=content.get('bg_color', 'white'))
        filename = content.get('filename', 'qrcode.png')
        
        # Optionally handle embedded images
        image_url = content.get('image_url')
        if image_url:
            try:
                image_url = image_url.strip()
                if image_url:  # Proceed only if image_url is not empty
                    icon = Image.open(requests.get(image_url, stream=True).raw)
                    img.paste(icon, (int((img.size[0] - icon.size[0]) / 2), int((img.size[1] - icon.size[1]) / 2)))
            except requests.RequestException as e:
                handle_error(f"Failed to load embedded image: {str(e)}")
            except Exception as e:
                handle_error(f"Error processing embedded image: {str(e)}")
        
        return save_image(img, filename)
    except Exception as e:
        handle_error(f"Failed to generate QR code: {str(e)}")

def generate_code39(data, content):
    try:
        barcode_class = barcode.get_barcode_class('code39')
        barcode_obj = barcode_class(data, writer=ImageWriter(), add_checksum=False)
        filename = content.get('filename', 'code39.png')
        output = io.BytesIO()
        barcode_obj.write(output)
        with open(filename, 'wb') as f:
            f.write(output.getvalue())
        logging.info(f"Image saved as {filename}")
        return output.getvalue()
    except Exception as e:
        handle_error(f"Failed to generate Code39 barcode: {str(e)}")

def generate_ean13(data, content):
    try:
        barcode_class = barcode.get_barcode_class('ean13')
        barcode_obj = barcode_class(data, writer=ImageWriter())
        filename = content.get('filename', 'ean13.png')
        output = io.BytesIO()
        barcode_obj.write(output)
        with open(filename, 'wb') as f:
            f.write(output.getvalue())
        logging.info(f"Image saved as {filename}")
        return output.getvalue()
    except Exception as e:
        handle_error(f"Failed to generate EAN13 barcode: {str(e)}")

def main():
    parser = argparse.ArgumentParser(description="Generate various types of barcodes and QR codes.")
    parser.add_argument("data", help="The data to encode in the barcode.")
    parser.add_argument("barcode_type", help="The type of barcode to generate (pdf417, datamatrix, code128, qrcode, code39, ean13).")
    parser.add_argument("output_format", help="The output format (file or byte).")
    parser.add_argument("--filename", help="The filename (with path) to save the barcode image if output format is file.", default="barcode.png")
    
    # Adding additional settings for different barcode types
    parser.add_argument("--columns", type=int, help="Number of columns for pdf417.", default=6)
    parser.add_argument("--security_level", type=int, help="Security level for pdf417.", default=2)
    parser.add_argument("--scale", type=int, help="Scale for pdf417.", default=3)
    parser.add_argument("--ratio", type=int, help="Ratio for pdf417.", default=3)
    parser.add_argument("--fg_color", help="Foreground color for barcodes.", default="#000000")
    parser.add_argument("--bg_color", help="Background color for barcodes.", default="#FFFFFF")
    parser.add_argument("--padding", type=int, help="Padding for pdf417.", default=20)
    parser.add_argument("--version", type=int, help="Version for qrcode.", default=1)
    parser.add_argument("--box_size", type=int, help="Box size for qrcode.", default=10)
    parser.add_argument("--border", type=int, help="Border size for qrcode.", default=4)
    parser.add_argument("--image_url", help="URL of the image to embed in qrcode.", default=None)
    
    args = parser.parse_args()

    data = args.data
    barcode_type = args.barcode_type.lower()
    output_format = args.output_format.lower()

    content = {
        'filename': args.filename,
        'columns': args.columns,
        'security_level': args.security_level,
        'scale': args.scale,
        'ratio': args.ratio,
        'fg_color': args.fg_color,
        'bg_color': args.bg_color,
        'padding': args.padding,
        'version': args.version,
        'box_size': args.box_size,
        'border': args.border,
        'image_url': args.image_url
    }

    barcode_functions = {
        "pdf417": generate_pdf417,
        "datamatrix": generate_datamatrix,
        "code128": generate_code128,
        "qrcode": generate_qrcode,
        "code39": generate_code39,
        "ean13": generate_ean13,
    }
    
    try:
        if barcode_type in barcode_functions:
            image_data = barcode_functions[barcode_type](data, content)
            if output_format == 'byte':
                encoded_image = base64.b64encode(image_data).decode('utf-8')
                print(encoded_image)
        else:
            handle_error("Unsupported barcode type")
    except KeyError as e:
        handle_error(f"Missing required field: {str(e)}")
    except Exception as e:
        handle_error(f"Failed to generate barcode: {str(e)}")

if __name__ == '__main__':
    main()
