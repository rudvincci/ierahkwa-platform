"""Tests for CLI commands."""

import pytest
import tempfile
import os
from pathlib import Path
from unittest.mock import patch, MagicMock
from click.testing import CliRunner
from PIL import Image

from mamey_image_bg.cli.commands import cli


class TestCLICommands:
    """Test cases for CLI commands."""
    
    def setup_method(self):
        """Set up test fixtures."""
        self.runner = CliRunner()
        self.temp_dir = tempfile.mkdtemp()
        self.temp_path = Path(self.temp_dir)
    
    def teardown_method(self):
        """Clean up test fixtures."""
        import shutil
        shutil.rmtree(self.temp_dir, ignore_errors=True)
    
    def create_test_image(self, name="test.png", size=(100, 100)):
        """Create a test image file."""
        image = Image.new('RGB', size, color='red')
        image_path = self.temp_path / name
        image.save(image_path)
        return str(image_path)
    
    def test_cli_help(self):
        """Test CLI help command."""
        result = self.runner.invoke(cli, ['--help'])
        assert result.exit_code == 0
        assert "Mamey Image Background Removal CLI Tool" in result.output
    
    def test_cli_verbose(self):
        """Test CLI with verbose flag."""
        result = self.runner.invoke(cli, ['--verbose', '--help'])
        assert result.exit_code == 0
    
    @patch('mamey_image_bg.cli.commands.BackgroundRemovalProcessor')
    def test_remove_command_success(self, mock_processor_class):
        """Test successful remove command."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.remove_background_from_file.return_value = "output.png"
        mock_processor_class.return_value = mock_processor
        
        # Create test image
        input_path = self.create_test_image()
        output_path = str(self.temp_path / "output.png")
        
        result = self.runner.invoke(cli, [
            'remove', input_path, output_path
        ])
        
        assert result.exit_code == 0
        assert "Background removed successfully!" in result.output
        mock_processor.remove_background_from_file.assert_called_once()
    
    @patch('mamey_image_bg.cli.commands.BackgroundRemovalProcessor')
    def test_remove_command_with_format(self, mock_processor_class):
        """Test remove command with specific format."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.remove_background_from_file.return_value = "output.jpg"
        mock_processor_class.return_value = mock_processor
        
        # Create test image
        input_path = self.create_test_image()
        output_path = str(self.temp_path / "output.jpg")
        
        result = self.runner.invoke(cli, [
            'remove', input_path, output_path, '--format', 'JPEG'
        ])
        
        assert result.exit_code == 0
        mock_processor.remove_background_from_file.assert_called_once_with(
            input_path, output_path, 'JPEG'
        )
    
    def test_remove_command_nonexistent_file(self):
        """Test remove command with non-existent file."""
        result = self.runner.invoke(cli, [
            'remove', 'nonexistent.png', 'output.png'
        ])
        
        assert result.exit_code == 1
        assert "Input file does not exist" in result.output
    
    def test_remove_command_invalid_image(self):
        """Test remove command with invalid image file."""
        # Create a text file instead of image
        invalid_file = self.temp_path / "invalid.txt"
        invalid_file.write_text("not an image")
        
        result = self.runner.invoke(cli, [
            'remove', str(invalid_file), 'output.png'
        ])
        
        assert result.exit_code == 1
        assert "Invalid image file" in result.output
    
    @patch('mamey_image_bg.cli.commands.BackgroundRemovalProcessor')
    def test_remove_command_processing_error(self, mock_processor_class):
        """Test remove command with processing error."""
        # Mock processor to raise exception
        mock_processor = MagicMock()
        mock_processor.remove_background_from_file.side_effect = Exception("Processing failed")
        mock_processor_class.return_value = mock_processor
        
        # Create test image
        input_path = self.create_test_image()
        output_path = str(self.temp_path / "output.png")
        
        result = self.runner.invoke(cli, [
            'remove', input_path, output_path
        ])
        
        assert result.exit_code == 1
        assert "Processing failed" in result.output
    
    @patch('mamey_image_bg.cli.commands.BackgroundRemovalProcessor')
    def test_batch_command_success(self, mock_processor_class):
        """Test successful batch command."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.remove_background_from_file.return_value = "output.png"
        mock_processor_class.return_value = mock_processor
        
        # Create test images
        input_dir = self.temp_path / "input"
        input_dir.mkdir()
        output_dir = self.temp_path / "output"
        
        # Create multiple test images
        for i in range(3):
            self.create_test_image(f"input/test{i}.png")
        
        result = self.runner.invoke(cli, [
            'batch', str(input_dir), str(output_dir)
        ])
        
        assert result.exit_code == 0
        assert "Processing complete:" in result.output
        assert "Successful: 3" in result.output
        assert "Failed: 0" in result.output
    
    @patch('mamey_image_bg.cli.commands.BackgroundRemovalProcessor')
    def test_batch_command_no_images(self, mock_processor_class):
        """Test batch command with no image files."""
        # Create empty directory
        input_dir = self.temp_path / "input"
        input_dir.mkdir()
        output_dir = self.temp_path / "output"
        
        result = self.runner.invoke(cli, [
            'batch', str(input_dir), str(output_dir)
        ])
        
        assert result.exit_code == 0
        assert "No image files found" in result.output
    
    @patch('mamey_image_bg.cli.commands.BackgroundRemovalProcessor')
    def test_models_command(self, mock_processor_class):
        """Test models command."""
        # Mock processor
        mock_processor = MagicMock()
        mock_processor.get_supported_models.return_value = ["u2net", "u2net_human_seg"]
        mock_processor_class.return_value = mock_processor
        
        result = self.runner.invoke(cli, ['models'])
        
        assert result.exit_code == 0
        assert "Available background removal models:" in result.output
        assert "u2net" in result.output
        assert "u2net_human_seg" in result.output
    
    @patch('mamey_image_bg.cli.commands.uvicorn')
    def test_serve_command(self, mock_uvicorn):
        """Test serve command."""
        result = self.runner.invoke(cli, [
            'serve', '--host', '127.0.0.1', '--port', '5001'
        ])
        
        # The command should start uvicorn (in a real scenario)
        # For testing, we just check that it doesn't fail immediately
        assert result.exit_code == 0 or "Starting" in result.output

