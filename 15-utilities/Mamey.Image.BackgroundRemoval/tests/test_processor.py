"""Tests for the background removal processor."""

import pytest
import io
from PIL import Image
from unittest.mock import patch, MagicMock

from mamey_image_bg.core.processor import BackgroundRemovalProcessor


class TestBackgroundRemovalProcessor:
    """Test cases for BackgroundRemovalProcessor."""
    
    def test_init(self):
        """Test processor initialization."""
        processor = BackgroundRemovalProcessor("u2net")
        assert processor.model_name == "u2net"
        assert processor._session is None
    
    def test_get_supported_models(self):
        """Test getting supported models."""
        processor = BackgroundRemovalProcessor()
        models = processor.get_supported_models()
        
        assert isinstance(models, list)
        assert len(models) > 0
        assert "u2net" in models
        assert "u2net_human_seg" in models
    
    def test_validate_image_valid(self):
        """Test image validation with valid image."""
        processor = BackgroundRemovalProcessor()
        
        # Create a test image
        image = Image.new('RGB', (100, 100), color='red')
        
        result = processor._validate_image(image)
        assert result.mode == 'RGB'
        assert result.size == (100, 100)
    
    def test_validate_image_large(self):
        """Test image validation with large image (should resize)."""
        processor = BackgroundRemovalProcessor()
        
        # Create a large test image
        image = Image.new('RGB', (2000, 2000), color='red')
        
        result = processor._validate_image(image)
        assert max(result.size) <= 1024  # Should be resized
    
    def test_validate_image_invalid_mode(self):
        """Test image validation with invalid mode (should convert)."""
        processor = BackgroundRemovalProcessor()
        
        # Create image in L mode
        image = Image.new('L', (100, 100), color=128)
        
        result = processor._validate_image(image)
        assert result.mode == 'RGB'
    
    def test_validate_image_none(self):
        """Test image validation with None input."""
        processor = BackgroundRemovalProcessor()
        
        with pytest.raises(ValueError, match="Image cannot be None"):
            processor._validate_image(None)
    
    @patch('mamey_image_bg.core.processor.remove')
    def test_remove_background_bytes(self, mock_remove):
        """Test background removal with bytes input."""
        processor = BackgroundRemovalProcessor()
        
        # Mock rembg response
        mock_result = b"fake_png_data"
        mock_remove.return_value = mock_result
        
        # Create test image
        image = Image.new('RGB', (100, 100), color='red')
        img_bytes = io.BytesIO()
        image.save(img_bytes, format='PNG')
        
        result = processor.remove_background(img_bytes.getvalue())
        
        assert result == mock_result
        mock_remove.assert_called_once()
    
    @patch('mamey_image_bg.core.processor.remove')
    def test_remove_background_pil_image(self, mock_remove):
        """Test background removal with PIL Image input."""
        processor = BackgroundRemovalProcessor()
        
        # Mock rembg response
        mock_result = b"fake_png_data"
        mock_remove.return_value = mock_result
        
        # Create test image
        image = Image.new('RGB', (100, 100), color='red')
        
        result = processor.remove_background(image)
        
        assert result == mock_result
        mock_remove.assert_called_once()
    
    @patch('mamey_image_bg.core.processor.remove')
    def test_remove_background_invalid_input(self, mock_remove):
        """Test background removal with invalid input."""
        processor = BackgroundRemovalProcessor()
        
        with pytest.raises(ValueError, match="Input must be bytes or PIL Image"):
            processor.remove_background("invalid_input")
    
    @patch('mamey_image_bg.core.processor.remove')
    def test_remove_background_processing_error(self, mock_remove):
        """Test background removal with processing error."""
        processor = BackgroundRemovalProcessor()
        
        # Mock rembg to raise exception
        mock_remove.side_effect = Exception("Processing failed")
        
        image = Image.new('RGB', (100, 100), color='red')
        
        with pytest.raises(Exception, match="Failed to remove background"):
            processor.remove_background(image)
    
    @patch('mamey_image_bg.core.processor.remove')
    @patch('builtins.open', create=True)
    def test_remove_background_from_file(self, mock_open, mock_remove):
        """Test background removal from file."""
        processor = BackgroundRemovalProcessor()
        
        # Mock file operations
        mock_file = MagicMock()
        mock_file.read.return_value = b"fake_image_data"
        mock_open.return_value.__enter__.return_value = mock_file
        
        # Mock rembg response
        mock_result = b"fake_png_data"
        mock_remove.return_value = mock_result
        
        # Mock file write
        with patch('builtins.open', create=True) as mock_write:
            mock_write.return_value.__enter__.return_value = MagicMock()
            
            result = processor.remove_background_from_file("input.jpg", "output.png")
            
            assert result == "output.png"
            mock_remove.assert_called_once()
    
    @patch('mamey_image_bg.core.processor.remove')
    @patch('builtins.open', create=True)
    def test_remove_background_from_file_auto_output(self, mock_open, mock_remove):
        """Test background removal from file with auto-generated output path."""
        processor = BackgroundRemovalProcessor()
        
        # Mock file operations
        mock_file = MagicMock()
        mock_file.read.return_value = b"fake_image_data"
        mock_open.return_value.__enter__.return_value = mock_file
        
        # Mock rembg response
        mock_result = b"fake_png_data"
        mock_remove.return_value = mock_result
        
        # Mock file write
        with patch('builtins.open', create=True) as mock_write:
            mock_write.return_value.__enter__.return_value = MagicMock()
            
            result = processor.remove_background_from_file("input.jpg")
            
            assert result == "input_no_bg.png"
            mock_remove.assert_called_once()
    
    def test_remove_background_from_file_nonexistent(self):
        """Test background removal from non-existent file."""
        processor = BackgroundRemovalProcessor()
        
        with pytest.raises(Exception, match="Failed to process file"):
            processor.remove_background_from_file("nonexistent.jpg")

