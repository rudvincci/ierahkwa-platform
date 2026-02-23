#!/usr/bin/env python3
"""
Comprehensive test script for background removal on all test images.
This script tests the Python service with all available models and formats.
"""

import os
import sys
import time
from pathlib import Path
from PIL import Image
import io

# Add the src directory to the Python path
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), 'src')))

# Import modules for testing
try:
    from mamey_image_bg.core.config import Settings
    from mamey_image_bg.core.processor import BackgroundRemovalProcessor
    from mamey_image_bg.api.main import app as fastapi_app
    from mamey_image_bg.cli.commands import cli
    print("✅ All modules imported successfully")
except Exception as e:
    print(f"❌ Failed to import modules: {e}")
    sys.exit(1)

def test_all_images():
    """Test background removal on all images in the testImages directory."""
    
    # Define paths
    test_images_dir = Path("/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages")
    output_dir = Path("/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out")
    
    # Create output directory if it doesn't exist
    output_dir.mkdir(exist_ok=True)
    
    # Get all image files
    image_extensions = {'.jpg', '.jpeg', '.png', '.bmp', '.tiff', '.webp'}
    image_files = []
    
    if test_images_dir.exists():
        for file_path in test_images_dir.iterdir():
            if file_path.is_file() and file_path.suffix.lower() in image_extensions:
                image_files.append(file_path)
    
    if not image_files:
        print("❌ No image files found in testImages directory")
        return False
    
    print(f"📁 Found {len(image_files)} image files to process")
    
    # Initialize processor
    try:
        processor = BackgroundRemovalProcessor()
        print("✅ Background removal processor initialized")
    except Exception as e:
        print(f"❌ Failed to initialize processor: {e}")
        return False
    
    # Get available models from FastAPI
    try:
        from fastapi.testclient import TestClient
        client = TestClient(fastapi_app)
        response = client.get("/api/models")
        if response.status_code == 200:
            models_data = response.json()
            models = models_data.get('models', [])
            print(f"✅ Available models: {', '.join(models)}")
        else:
            print(f"❌ Failed to get models from API: {response.status_code}")
            models = ['u2net']  # Default fallback
    except Exception as e:
        print(f"⚠️  Could not get models from API: {e}")
        models = ['u2net']  # Default fallback
    
    # Test each image with different models and formats
    results = []
    
    for i, image_file in enumerate(image_files, 1):
        print(f"\n🖼️  Processing image {i}/{len(image_files)}: {image_file.name}")
        
        try:
            # Read image
            with open(image_file, 'rb') as f:
                image_data = f.read()
            
            # Test with default model (u2net)
            start_time = time.time()
            result_data = processor.remove_background(image_data, "PNG")
            processing_time = time.time() - start_time
            
            # Save result
            output_file = output_dir / f"{image_file.stem}_u2net.png"
            with open(output_file, 'wb') as f:
                f.write(result_data)
            
            # Get image dimensions
            original_img = Image.open(io.BytesIO(image_data))
            result_img = Image.open(io.BytesIO(result_data))
            
            print(f"   ✅ Processed with u2net in {processing_time:.2f}s")
            print(f"   📏 Original: {original_img.size[0]}x{original_img.size[1]}")
            print(f"   📏 Result: {result_img.size[0]}x{result_img.size[1]}")
            print(f"   💾 Saved to: {output_file}")
            
            results.append({
                'file': image_file.name,
                'model': 'u2net',
                'time': processing_time,
                'success': True,
                'output': str(output_file)
            })
            
            # Test with human segmentation model if available
            if 'u2net_human_seg' in models:
                try:
                    start_time = time.time()
                    result_data_human = processor.remove_background(image_data, "PNG", model_name="u2net_human_seg")
                    processing_time_human = time.time() - start_time
                    
                    output_file_human = output_dir / f"{image_file.stem}_human_seg.png"
                    with open(output_file_human, 'wb') as f:
                        f.write(result_data_human)
                    
                    print(f"   ✅ Processed with u2net_human_seg in {processing_time_human:.2f}s")
                    print(f"   💾 Saved to: {output_file_human}")
                    
                    results.append({
                        'file': image_file.name,
                        'model': 'u2net_human_seg',
                        'time': processing_time_human,
                        'success': True,
                        'output': str(output_file_human)
                    })
                except Exception as e:
                    print(f"   ⚠️  Human seg model failed: {e}")
                    results.append({
                        'file': image_file.name,
                        'model': 'u2net_human_seg',
                        'time': 0,
                        'success': False,
                        'error': str(e)
                    })
            
            # Test JPEG output format
            try:
                start_time = time.time()
                result_data_jpeg = processor.remove_background(image_data, "JPEG")
                processing_time_jpeg = time.time() - start_time
                
                output_file_jpeg = output_dir / f"{image_file.stem}_u2net.jpg"
                with open(output_file_jpeg, 'wb') as f:
                    f.write(result_data_jpeg)
                
                print(f"   ✅ Processed as JPEG in {processing_time_jpeg:.2f}s")
                print(f"   💾 Saved to: {output_file_jpeg}")
                
                results.append({
                    'file': image_file.name,
                    'model': 'u2net_jpeg',
                    'time': processing_time_jpeg,
                    'success': True,
                    'output': str(output_file_jpeg)
                })
            except Exception as e:
                print(f"   ⚠️  JPEG format failed: {e}")
                results.append({
                    'file': image_file.name,
                    'model': 'u2net_jpeg',
                    'time': 0,
                    'success': False,
                    'error': str(e)
                })
                
        except Exception as e:
            print(f"   ❌ Failed to process {image_file.name}: {e}")
            results.append({
                'file': image_file.name,
                'model': 'u2net',
                'time': 0,
                'success': False,
                'error': str(e)
            })
    
    # Print summary
    print(f"\n📊 TEST SUMMARY")
    print(f"=" * 50)
    
    successful = [r for r in results if r['success']]
    failed = [r for r in results if not r['success']]
    
    print(f"✅ Successful: {len(successful)}")
    print(f"❌ Failed: {len(failed)}")
    
    if successful:
        avg_time = sum(r['time'] for r in successful) / len(successful)
        print(f"⏱️  Average processing time: {avg_time:.2f}s")
    
    if failed:
        print(f"\n❌ Failed tests:")
        for result in failed:
            print(f"   - {result['file']} ({result['model']}): {result.get('error', 'Unknown error')}")
    
    print(f"\n📁 Output directory: {output_dir}")
    print(f"📁 Total files created: {len([f for f in output_dir.iterdir() if f.is_file()])}")
    
    return len(failed) == 0

def test_fastapi_endpoints():
    """Test FastAPI endpoints using TestClient."""
    print(f"\n🌐 Testing FastAPI endpoints...")
    
    try:
        from fastapi.testclient import TestClient
        
        client = TestClient(fastapi_app)
        
        # Test health endpoint
        response = client.get("/api/health")
        if response.status_code == 200:
            print("✅ Health endpoint working")
        else:
            print(f"❌ Health endpoint failed: {response.status_code}")
            return False
        
        # Test models endpoint
        response = client.get("/api/models")
        if response.status_code == 200:
            models = response.json()
            print(f"✅ Models endpoint working: {len(models.get('models', []))} models available")
        else:
            print(f"❌ Models endpoint failed: {response.status_code}")
            return False
        
        return True
        
    except Exception as e:
        print(f"❌ FastAPI test failed: {e}")
        return False

def test_cli_commands():
    """Test CLI commands."""
    print(f"\n💻 Testing CLI commands...")
    
    try:
        from click.testing import CliRunner
        
        runner = CliRunner()
        
        # Test help command
        result = runner.invoke(cli, ["--help"])
        if result.exit_code == 0:
            print("✅ CLI help command working")
        else:
            print(f"❌ CLI help command failed: {result.exit_code}")
            return False
        
        # Test models command
        result = runner.invoke(cli, ["models"])
        if result.exit_code == 0:
            print("✅ CLI models command working")
        else:
            print(f"❌ CLI models command failed: {result.exit_code}")
            return False
        
        return True
        
    except Exception as e:
        print(f"❌ CLI test failed: {e}")
        return False

if __name__ == "__main__":
    print("🚀 Starting comprehensive background removal test...")
    print("=" * 60)
    
    # Test all components
    all_tests_passed = True
    
    # Test image processing
    if not test_all_images():
        all_tests_passed = False
    
    # Test FastAPI
    if not test_fastapi_endpoints():
        all_tests_passed = False
    
    # Test CLI
    if not test_cli_commands():
        all_tests_passed = False
    
    print(f"\n🎯 FINAL RESULT")
    print("=" * 60)
    
    if all_tests_passed:
        print("🎉 ALL TESTS PASSED! Background removal service is fully functional.")
    else:
        print("⚠️  Some tests failed. Check the output above for details.")
    
    print(f"\n📁 Check the output directory for processed images:")
    print(f"   {Path('/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out').absolute()}")
