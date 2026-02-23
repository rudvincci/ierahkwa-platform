import qrcode
from flask import Flask, request, send_file, jsonify, Response
import barcode  # Import the python-barcode library
from barcode.writer import ImageWriter
import logging
import io
from PIL import Image
import requests  # For handling external images

app = Flask(__name__)
logging.basicConfig(level=logging.INFO)

def render_response(image, format='PNG'):
    output = io.BytesIO()
    image.save(output, format=format)
    output.seek(0)
    return send_file(output, mimetype=f'image/{format.lower()}')

def handle_error(status_code, code, reason):
    return jsonify({"httpStatus": status_code, "code": code, "reason": reason}), status_code

def generate_pdf417(data, content):
    from pdf417gen import encode, render_image as render_pdf417_image, render_svg
    codes = encode(data, columns=content.get('columns', 6), security_level=content.get('security_level', 2))
    scale = content.get('scale', 3)
    ratio = content.get('ratio', 3)
    fg_color = content.get('fg_color', '#000000')
    bg_color = content.get('bg_color', '#FFFFFF')
    padding = content.get('padding', 20)

    if content.get('format', 'png').lower() == 'svg':
        svg = render_svg(codes, scale=scale, ratio=ratio, color=fg_color)
        output = io.BytesIO()
        svg.write(output)
        output.seek(0)
        return Response(output.getvalue(), mimetype='image/svg+xml')
    else:
        return render_response(render_pdf417_image(codes, scale=scale, ratio=ratio, padding=padding, fg_color=fg_color, bg_color=bg_color))

def generate_datamatrix(data):
    from pystrich.datamatrix import DataMatrixEncoder
    encoder = DataMatrixEncoder(data)
    return render_response(encoder.get_image())

def generate_code128(data):
    from pystrich.code128 import Code128Encoder
    encoder = Code128Encoder(data)
    return render_response(encoder.get_image())

def generate_qrcode(data, content):
    qr = qrcode.QRCode(
        version=content.get('version', 1),
        error_correction=qrcode.constants.ERROR_CORRECT_L,
        box_size=content.get('box_size', 10),
        border=content.get('border', 4)
    )
    qr.add_data(data)
    qr.make(fit=True)
    img = qr.make_image(fill_color=content.get('fg_color', 'black'), back_color=content.get('bg_color', 'white'))
    
    # Optionally handle embedded images
    image_url = content.get('image_url')
    if image_url:
        try:
            image_url = image_url.strip()
            if image_url:  # Proceed only if image_url is not empty
                icon = Image.open(requests.get(image_url, stream=True).raw)
                img.paste(icon, (int((img.size[0] - icon.size[0]) / 2), int((img.size[1] - icon.size[1]) / 2)))
        except Exception as e:
            return handle_error(500, "MBG001", f"Failed to load embedded image: {str(e)}")
    
    return render_response(img)

def generate_code39(data):
    barcode_class = barcode.get_barcode_class('code39')
    barcode_obj = barcode_class(data, writer=ImageWriter(), add_checksum=False)
    output = io.BytesIO()
    barcode_obj.write(output)
    output.seek(0)
    return send_file(output, mimetype='image/png')

def generate_ean13(data):
    barcode_class = barcode.get_barcode_class('ean13')
    barcode_obj = barcode_class(data, writer=ImageWriter())
    output = io.BytesIO()
    barcode_obj.write(output)
    output.seek(0)
    return send_file(output, mimetype='image/png')

@app.route('/generate-barcode', methods=['POST'])
def generate_barcode():
    try:
        content = request.json
        data = content['data']
        barcode_type = content['type'].lower()

        barcode_functions = {
            "pdf417": generate_pdf417,
            "datamatrix": generate_datamatrix,
            "code128": generate_code128,
            "qrcode": generate_qrcode,
            "code39": generate_code39,
            "ean13": generate_ean13,
        }
        
        if barcode_type in barcode_functions:
            return barcode_functions[barcode_type](data, content)
        else:
            return handle_error(400, "MBG002", "Unsupported barcode type")
    except KeyError as e:
        logging.exception("Missing required fields")
        return handle_error(400, "MBG003", f"Missing required field: {str(e)}")
    except Exception as e:
        logging.exception("Failed to generate barcode")
        return handle_error(500, "MBG004", "Internal server error")

if __name__ == '__main__':
    app.run(debug=True, port=5000)
