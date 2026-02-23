"""Tests for the FastAPI endpoints."""

import pytest
import io
from fastapi.testclient import TestClient
from unittest.mock import patch, MagicMock
from PIL import Image

from mamey_image_bg.api.main import app

client = TestClient(app)


class TestHealthEndpoint:
    """Test cases for health check endpoint."""
    
    def test_health_check(self):
        """Test health check endpoint."""
        response = client.get("/api/health")
        assert response.status_code == 200
        data = response.json()
        assert data["status"] == "healthy"
        assert data["service"] == "mamey-image-background-removal"


class TestRemoveBackgroundEndpoint:
    """Test cases for single image background removal endpoint."""
    
    def create_test_image(self, format='PNG'):
        """Create a test image in memory."""
        image = Image.new('RGB', (100, 100), color='red')
        img_bytes = io.BytesIO()
        image.save(img_bytes, format=format)
        img_bytes.seek(0)
        return img_bytes.getvalue()
    
    @patch('mamey_image_bg.api.routes.get_processor')
    def test_remove_background_success(self, mock_get_processor):
        """Test successful background removal."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.remove_background.return_value = b"processed_image_data"
        mock_get_processor.return_value = mock_processor
        
        # Create test image
        image_data = self.create_test_image()
        
        response = client.post(
            "/api/remove-background",
            files={"file": ("test.png", image_data, "image/png")},
            data={"output_format": "PNG"}
        )
        
        assert response.status_code == 200
        assert response.content == b"processed_image_data"
        assert response.headers["content-type"] == "image/png"
        mock_processor.remove_background.assert_called_once()
    
    @patch('mamey_image_bg.api.routes.get_processor')
    def test_remove_background_jpeg_format(self, mock_get_processor):
        """Test background removal with JPEG output format."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.remove_background.return_value = b"processed_image_data"
        mock_get_processor.return_value = mock_processor
        
        # Create test image
        image_data = self.create_test_image()
        
        response = client.post(
            "/api/remove-background",
            files={"file": ("test.png", image_data, "image/png")},
            data={"output_format": "JPEG"}
        )
        
        assert response.status_code == 200
        assert response.headers["content-type"] == "image/jpeg"
    
    def test_remove_background_invalid_file_type(self):
        """Test background removal with non-image file."""
        response = client.post(
            "/api/remove-background",
            files={"file": ("test.txt", b"not an image", "text/plain")},
            data={"output_format": "PNG"}
        )
        
        assert response.status_code == 400
        data = response.json()
        assert "File must be an image" in data["detail"]
    
    def test_remove_background_invalid_format(self):
        """Test background removal with invalid output format."""
        image_data = self.create_test_image()
        
        response = client.post(
            "/api/remove-background",
            files={"file": ("test.png", image_data, "image/png")},
            data={"output_format": "INVALID"}
        )
        
        assert response.status_code == 400
        data = response.json()
        assert "Output format must be PNG or JPEG" in data["detail"]
    
    @patch('mamey_image_bg.api.routes.get_processor')
    def test_remove_background_processing_error(self, mock_get_processor):
        """Test background removal with processing error."""
        # Mock processor to raise exception
        mock_processor = MagicMock()
        mock_processor.remove_background.side_effect = Exception("Processing failed")
        mock_get_processor.return_value = mock_processor
        
        image_data = self.create_test_image()
        
        response = client.post(
            "/api/remove-background",
            files={"file": ("test.png", image_data, "image/png")},
            data={"output_format": "PNG"}
        )
        
        assert response.status_code == 500
        data = response.json()
        assert "Failed to process image" in data["detail"]


class TestBatchRemoveBackgroundEndpoint:
    """Test cases for batch background removal endpoint."""
    
    def create_test_image(self, format='PNG'):
        """Create a test image in memory."""
        image = Image.new('RGB', (100, 100), color='red')
        img_bytes = io.BytesIO()
        image.save(img_bytes, format=format)
        img_bytes.seek(0)
        return img_bytes.getvalue()
    
    @patch('mamey_image_bg.api.routes.get_processor')
    def test_batch_remove_background_success(self, mock_get_processor):
        """Test successful batch background removal."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.remove_background.return_value = b"processed_image_data"
        mock_get_processor.return_value = mock_processor
        
        # Create test images
        image_data = self.create_test_image()
        
        response = client.post(
            "/api/remove-background/batch",
            files=[
                ("files", ("test1.png", image_data, "image/png")),
                ("files", ("test2.png", image_data, "image/png"))
            ],
            data={"output_format": "PNG"}
        )
        
        assert response.status_code == 200
        assert response.headers["content-type"] == "application/zip"
        assert response.headers["content-disposition"] == "attachment; filename=processed_images.zip"
    
    def test_batch_remove_background_no_files(self):
        """Test batch background removal with no files."""
        response = client.post(
            "/api/remove-background/batch",
            files=[],
            data={"output_format": "PNG"}
        )
        
        assert response.status_code == 400
        data = response.json()
        assert "No files provided" in data["detail"]
    
    def test_batch_remove_background_too_many_files(self):
        """Test batch background removal with too many files."""
        image_data = self.create_test_image()
        files = [("files", (f"test{i}.png", image_data, "image/png")) for i in range(11)]
        
        response = client.post(
            "/api/remove-background/batch",
            files=files,
            data={"output_format": "PNG"}
        )
        
        assert response.status_code == 400
        data = response.json()
        assert "Maximum 10 files allowed per batch" in data["detail"]


class TestModelsEndpoint:
    """Test cases for models endpoint."""
    
    @patch('mamey_image_bg.api.routes.get_processor')
    def test_get_models_success(self, mock_get_processor):
        """Test successful models retrieval."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.get_supported_models.return_value = ["u2net", "u2net_human_seg"]
        mock_get_processor.return_value = mock_processor
        
        response = client.get("/api/models")
        
        assert response.status_code == 200
        data = response.json()
        assert "models" in data
        assert "current_model" in data
        assert "u2net" in data["models"]
    
    @patch('mamey_image_bg.api.routes.get_processor')
    def test_get_models_error(self, mock_get_processor):
        """Test models retrieval with error."""
        # Mock processor to raise exception
        mock_processor = MagicMock()
        mock_processor.get_supported_models.side_effect = Exception("Model error")
        mock_get_processor.return_value = mock_processor
        
        response = client.get("/api/models")
        
        assert response.status_code == 500
        data = response.json()
        assert "Failed to get models" in data["detail"]

