"""Core background removal processor using rembg."""

import io
import logging
from typing import Optional, Union
from PIL import Image
from rembg import remove, new_session

logger = logging.getLogger(__name__)


class BackgroundRemovalProcessor:
    """Handles background removal using rembg AI models."""
    
    def __init__(self, model_name: str = "u2net"):
        """
        Initialize the processor with a specific model.
        
        Args:
            model_name: The rembg model to use (u2net, u2net_human_seg, etc.)
        """
        self.model_name = model_name
        self._session = None
    
    def _get_session(self):
        """Get or create the rembg session (lazy loading)."""
        if self._session is None:
            logger.info(f"Initializing rembg session with model: {self.model_name}")
            self._session = new_session(self.model_name)
        return self._session
    
    def _validate_image(self, image: Image.Image) -> Image.Image:
        """
        Validate and prepare image for processing.
        
        Args:
            image: PIL Image to validate
            
        Returns:
            Validated PIL Image
            
        Raises:
            ValueError: If image is invalid or too large
        """
        if image is None:
            raise ValueError("Image cannot be None")
        
        # Check image size
        max_size = 1024  # Default max size
        if max(image.size) > max_size:
            logger.warning(f"Image size {image.size} exceeds maximum {max_size}, resizing...")
            image.thumbnail((max_size, max_size), Image.Resampling.LANCZOS)
        
        # Convert to RGB if necessary
        if image.mode not in ('RGB', 'RGBA'):
            logger.info(f"Converting image from {image.mode} to RGB")
            image = image.convert('RGB')
        
        return image
    
    def remove_background(
        self, 
        input_data: Union[bytes, Image.Image], 
        output_format: str = "PNG"
    ) -> bytes:
        """
        Remove background from an image.
        
        Args:
            input_data: Image data as bytes or PIL Image
            output_format: Output format (PNG, JPEG, etc.)
            
        Returns:
            Processed image as bytes with transparent background
            
        Raises:
            ValueError: If input data is invalid
            Exception: If processing fails
        """
        try:
            # Convert input to PIL Image if needed
            if isinstance(input_data, bytes):
                image = Image.open(io.BytesIO(input_data))
            elif isinstance(input_data, Image.Image):
                image = input_data
            else:
                raise ValueError("Input must be bytes or PIL Image")
            
            # Validate and prepare image
            image = self._validate_image(image)
            
            logger.info(f"Processing image: {image.size}, mode: {image.mode}")
            
            # Get rembg session
            session = self._get_session()
            
            # Convert PIL Image to bytes for rembg
            img_bytes = io.BytesIO()
            image.save(img_bytes, format='PNG')
            img_bytes.seek(0)
            
            # Process with rembg
            result_bytes = remove(img_bytes.getvalue(), session=session)
            
            # Convert result back to PIL Image for format conversion if needed
            if output_format.upper() != "PNG":
                result_image = Image.open(io.BytesIO(result_bytes))
                
                # Convert to desired format
                output_bytes = io.BytesIO()
                if output_format.upper() == "JPEG":
                    # JPEG doesn't support transparency, so we need to add a white background
                    white_bg = Image.new('RGB', result_image.size, (255, 255, 255))
                    white_bg.paste(result_image, mask=result_image.split()[-1] if result_image.mode == 'RGBA' else None)
                    white_bg.save(output_bytes, format='JPEG', quality=95)
                else:
                    result_image.save(output_bytes, format=output_format)
                
                return output_bytes.getvalue()
            
            return result_bytes
            
        except Exception as e:
            logger.error(f"Error processing image: {str(e)}")
            raise Exception(f"Failed to remove background: {str(e)}")
    
    def remove_background_from_file(
        self, 
        input_path: str, 
        output_path: Optional[str] = None,
        output_format: str = "PNG"
    ) -> str:
        """
        Remove background from an image file.
        
        Args:
            input_path: Path to input image file
            output_path: Path for output file (if None, auto-generates)
            output_format: Output format (PNG, JPEG, etc.)
            
        Returns:
            Path to the output file
        """
        try:
            # Read input image
            with open(input_path, 'rb') as f:
                input_data = f.read()
            
            # Process image
            result_data = self.remove_background(input_data, output_format)
            
            # Generate output path if not provided
            if output_path is None:
                base_name = input_path.rsplit('.', 1)[0]
                output_path = f"{base_name}_no_bg.{output_format.lower()}"
            
            # Write result
            with open(output_path, 'wb') as f:
                f.write(result_data)
            
            logger.info(f"Background removed successfully: {input_path} -> {output_path}")
            return output_path
            
        except Exception as e:
            logger.error(f"Error processing file {input_path}: {str(e)}")
            raise Exception(f"Failed to process file: {str(e)}")
    
    def get_supported_models(self) -> list:
        """Get list of supported rembg models."""
        return [
            "u2net",
            "u2net_human_seg", 
            "u2netp",
            "u2net_cloth_seg",
            "silueta",
            "isnet-general-use"
        ]
    
    def __del__(self):
        """Cleanup session on destruction."""
        if self._session is not None:
            del self._session

