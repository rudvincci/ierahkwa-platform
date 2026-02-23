"""FastAPI routes for background removal API."""

import logging
from typing import List, Optional
from fastapi import APIRouter, File, UploadFile, HTTPException, Form, Depends, Body
from fastapi.responses import Response
from PIL import Image
import io

from ..core.processor import BackgroundRemovalProcessor
from ..core.config import settings

logger = logging.getLogger(__name__)

# Create router
router = APIRouter(prefix="/api", tags=["background-removal"])

# Global processor instance
_processor: Optional[BackgroundRemovalProcessor] = None


def get_processor() -> BackgroundRemovalProcessor:
    """Get or create the background removal processor."""
    global _processor
    if _processor is None:
        _processor = BackgroundRemovalProcessor(model_name=settings.model_name)
    return _processor


@router.get("/health")
async def health_check():
    """Health check endpoint."""
    return {"status": "healthy", "service": "mamey-image-background-removal"}


@router.post("/remove-background")
async def remove_background(
    file: UploadFile = File(..., description="Image file to process"),
    output_format: str = Form(default="PNG", description="Output format (PNG, JPEG)"),
    processor: BackgroundRemovalProcessor = Depends(get_processor)
):
    """
    Remove background from a single image.
    
    Args:
        file: Image file to process
        output_format: Output format (PNG, JPEG)
        processor: Background removal processor
        
    Returns:
        Processed image with transparent background
    """
    try:
        # Validate file type
        if not file.content_type or not file.content_type.startswith('image/'):
            raise HTTPException(
                status_code=400, 
                detail="File must be an image"
            )
        
        # Validate output format
        if output_format.upper() not in ['PNG', 'JPEG']:
            raise HTTPException(
                status_code=400,
                detail="Output format must be PNG or JPEG"
            )
        
        # Read file content
        content = await file.read()
        
        # Process image
        result_data = processor.remove_background(content, output_format.upper())
        
        # Determine content type
        content_type = f"image/{output_format.lower()}"
        
        logger.info(f"Successfully processed image: {file.filename}")
        
        return Response(
            content=result_data,
            media_type=content_type,
            headers={
                "Content-Disposition": f"attachment; filename=no_bg_{file.filename}"
            }
        )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error processing image {file.filename}: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"Failed to process image: {str(e)}"
        )


@router.post("/remove-background/bytes")
async def remove_background_bytes(
    request_data: dict = Body(..., description="JSON with base64 encoded image data"),
    processor: BackgroundRemovalProcessor = Depends(get_processor)
):
    """
    Remove background from base64 encoded image data.
    
    Args:
        request_data: JSON containing 'image_data' (base64 string) and 'output_format' (string)
        processor: Background removal processor
        
    Returns:
        Raw bytes of processed image with transparent background
    """
    try:
        import base64
        
        # Extract data from request
        image_data_b64 = request_data.get('image_data')
        output_format = request_data.get('output_format', 'PNG')
        
        # Validate input
        if not image_data_b64:
            raise HTTPException(
                status_code=400,
                detail="No image_data provided in request"
            )
        
        # Validate output format
        if output_format.upper() not in ['PNG', 'JPEG']:
            raise HTTPException(
                status_code=400,
                detail="Output format must be PNG or JPEG"
            )
        
        # Decode base64 data
        try:
            image_data = base64.b64decode(image_data_b64)
        except Exception as e:
            raise HTTPException(
                status_code=400,
                detail=f"Invalid base64 image data: {str(e)}"
            )
        
        # Process image
        result_data = processor.remove_background(image_data, output_format.upper())
        
        # Determine content type
        content_type = f"image/{output_format.lower()}"
        
        logger.info(f"Successfully processed base64 encoded image bytes")
        
        return Response(
            content=result_data,
            media_type=content_type,
            headers={
                "Content-Disposition": f"attachment; filename=no_bg_image.{output_format.lower()}"
            }
        )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error processing base64 image bytes: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"Failed to process image: {str(e)}"
        )


@router.post("/remove-background/batch")
async def remove_background_batch(
    files: List[UploadFile] = File(..., description="Image files to process"),
    output_format: str = Form(default="PNG", description="Output format (PNG, JPEG)"),
    processor: BackgroundRemovalProcessor = Depends(get_processor)
):
    """
    Remove background from multiple images.
    
    Args:
        files: List of image files to process
        output_format: Output format (PNG, JPEG)
        processor: Background removal processor
        
    Returns:
        ZIP file containing processed images
    """
    try:
        import zipfile
        import tempfile
        import os
        
        # Validate files
        if len(files) == 0:
            raise HTTPException(status_code=400, detail="No files provided")
        
        if len(files) > 10:  # Limit batch size
            raise HTTPException(status_code=400, detail="Maximum 10 files allowed per batch")
        
        # Validate output format
        if output_format.upper() not in ['PNG', 'JPEG']:
            raise HTTPException(
                status_code=400,
                detail="Output format must be PNG or JPEG"
            )
        
        # Process images
        processed_files = []
        for file in files:
            if not file.content_type or not file.content_type.startswith('image/'):
                logger.warning(f"Skipping non-image file: {file.filename}")
                continue
            
            try:
                content = await file.read()
                result_data = processor.remove_background(content, output_format.upper())
                
                # Store in temporary file
                temp_file = tempfile.NamedTemporaryFile(delete=False, suffix=f".{output_format.lower()}")
                temp_file.write(result_data)
                temp_file.close()
                
                processed_files.append({
                    'path': temp_file.name,
                    'filename': f"no_bg_{file.filename}"
                })
                
            except Exception as e:
                logger.error(f"Error processing {file.filename}: {str(e)}")
                continue
        
        if not processed_files:
            raise HTTPException(status_code=400, detail="No valid images processed")
        
        # Create ZIP file
        zip_buffer = io.BytesIO()
        with zipfile.ZipFile(zip_buffer, 'w', zipfile.ZIP_DEFLATED) as zip_file:
            for file_info in processed_files:
                zip_file.write(file_info['path'], file_info['filename'])
                os.unlink(file_info['path'])  # Clean up temp file
        
        zip_buffer.seek(0)
        
        logger.info(f"Successfully processed {len(processed_files)} images in batch")
        
        return Response(
            content=zip_buffer.getvalue(),
            media_type="application/zip",
            headers={
                "Content-Disposition": "attachment; filename=processed_images.zip"
            }
        )
        
    except HTTPException:
        raise
    except Exception as e:
        logger.error(f"Error processing batch: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"Failed to process batch: {str(e)}"
        )


@router.get("/models")
async def get_available_models(processor: BackgroundRemovalProcessor = Depends(get_processor)):
    """Get list of available background removal models."""
    try:
        models = processor.get_supported_models()
        return {
            "models": models,
            "current_model": settings.model_name
        }
    except Exception as e:
        logger.error(f"Error getting models: {str(e)}")
        raise HTTPException(
            status_code=500,
            detail=f"Failed to get models: {str(e)}"
        )

