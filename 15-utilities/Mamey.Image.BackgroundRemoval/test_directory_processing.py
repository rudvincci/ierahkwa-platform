#!/usr/bin/env python3
"""
Test script for directory-based background removal processing.
This script tests processing all images in a directory and saving to an output directory.
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

def test_directory_processing():
    """Test processing all images in a directory and saving to output directory."""
    
    # Define paths
    input_dir = Path("/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages")
    output_dir = Path("/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out")
    
    print(f"📁 Input directory: {input_dir}")
    print(f"📁 Output directory: {output_dir}")
    
    # Check if input directory exists
    if not input_dir.exists():
        print(f"❌ Input directory does not exist: {input_dir}")
        return False
    
    # Create output directory if it doesn't exist
    output_dir.mkdir(exist_ok=True)
    print(f"✅ Output directory ready: {output_dir}")
    
    # Get all image files
    image_extensions = {'.jpg', '.jpeg', '.png', '.bmp', '.tiff', '.webp'}
    image_files = []
    
    for file_path in input_dir.iterdir():
        if file_path.is_file() and file_path.suffix.lower() in image_extensions:
            image_files.append(file_path)
    
    if not image_files:
        print("❌ No image files found in input directory")
        return False
    
    print(f"📁 Found {len(image_files)} image files to process")
    
    # Initialize processor
    try:
        processor = BackgroundRemovalProcessor()
        print("✅ Background removal processor initialized")
    except Exception as e:
        print(f"❌ Failed to initialize processor: {e}")
        return False
    
    # Process each image
    results = []
    start_time = time.time()
    
    for i, image_file in enumerate(image_files, 1):
        print(f"\n🖼️  Processing image {i}/{len(image_files)}: {image_file.name}")
        
        try:
            # Read image
            with open(image_file, 'rb') as f:
                image_data = f.read()
            
            # Process with u2net model
            image_start_time = time.time()
            result_data = processor.remove_background(image_data, "PNG")
            processing_time = time.time() - image_start_time
            
            # Create output filename
            output_filename = f"{image_file.stem}_no_bg.png"
            output_file = output_dir / output_filename
            
            # Save result
            with open(output_file, 'wb') as f:
                f.write(result_data)
            
            # Get image dimensions
            original_img = Image.open(io.BytesIO(image_data))
            result_img = Image.open(io.BytesIO(result_data))
            
            print(f"   ✅ Processed in {processing_time:.2f}s")
            print(f"   📏 Original: {original_img.size[0]}x{original_img.size[1]}")
            print(f"   📏 Result: {result_img.size[0]}x{result_img.size[1]}")
            print(f"   💾 Saved to: {output_file.name}")
            
            results.append({
                'input': image_file.name,
                'output': output_filename,
                'time': processing_time,
                'success': True,
                'original_size': original_img.size,
                'result_size': result_img.size
            })
            
        except Exception as e:
            print(f"   ❌ Failed to process {image_file.name}: {e}")
            results.append({
                'input': image_file.name,
                'output': None,
                'time': 0,
                'success': False,
                'error': str(e)
            })
    
    total_time = time.time() - start_time
    
    # Print summary
    print(f"\n📊 DIRECTORY PROCESSING SUMMARY")
    print(f"=" * 60)
    
    successful = [r for r in results if r['success']]
    failed = [r for r in results if not r['success']]
    
    print(f"✅ Successful: {len(successful)}")
    print(f"❌ Failed: {len(failed)}")
    print(f"⏱️  Total processing time: {total_time:.2f}s")
    
    if successful:
        avg_time = sum(r['time'] for r in successful) / len(successful)
        print(f"⏱️  Average processing time per image: {avg_time:.2f}s")
    
    if failed:
        print(f"\n❌ Failed images:")
        for result in failed:
            print(f"   - {result['input']}: {result.get('error', 'Unknown error')}")
    
    print(f"\n📁 Output directory contents:")
    output_files = list(output_dir.iterdir())
    for file in sorted(output_files):
        if file.is_file():
            file_size = file.stat().st_size
            print(f"   - {file.name} ({file_size:,} bytes)")
    
    return len(failed) == 0

def test_cli_directory_processing():
    """Test CLI directory processing command."""
    print(f"\n💻 Testing CLI directory processing...")
    
    try:
        from click.testing import CliRunner
        
        runner = CliRunner()
        
        # Test batch command
        input_dir = "/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages"
        output_dir = "/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out_cli"
        
        # Create output directory
        Path(output_dir).mkdir(exist_ok=True)
        
        result = runner.invoke(cli, ["batch", input_dir, output_dir])
        
        if result.exit_code == 0:
            print("✅ CLI batch processing successful")
            
            # Check output files
            output_path = Path(output_dir)
            if output_path.exists():
                output_files = list(output_path.iterdir())
                print(f"   📁 Created {len(output_files)} output files")
                for file in sorted(output_files):
                    if file.is_file():
                        print(f"   - {file.name}")
            return True
        else:
            print(f"❌ CLI batch processing failed: {result.exit_code}")
            print(f"   Error: {result.output}")
            return False
        
    except Exception as e:
        print(f"❌ CLI test failed: {e}")
        return False

def test_fastapi_batch_processing():
    """Test FastAPI batch processing endpoint."""
    print(f"\n🌐 Testing FastAPI batch processing...")
    
    try:
        from fastapi.testclient import TestClient
        import json
        
        client = TestClient(fastapi_app)
        
        # Prepare test data
        input_dir = Path("/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages")
        image_files = [f for f in input_dir.iterdir() if f.is_file() and f.suffix.lower() in {'.jpg', '.jpeg', '.png'}]
        
        if not image_files:
            print("❌ No test images found for API testing")
            return False
        
        # Test with first 3 images to avoid timeout
        test_files = image_files[:3]
        
        files = []
        for img_file in test_files:
            with open(img_file, 'rb') as f:
                files.append(('files', (img_file.name, f.read(), 'image/png')))
        
        # Test batch endpoint
        response = client.post("/api/remove-background/batch", files=files)
        
        if response.status_code == 200:
            print("✅ FastAPI batch processing successful")
            
            # Save the response as zip file
            output_dir = Path("/Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out_api")
            output_dir.mkdir(exist_ok=True)
            
            zip_file = output_dir / "batch_results.zip"
            with open(zip_file, 'wb') as f:
                f.write(response.content)
            
            print(f"   📁 Batch results saved to: {zip_file}")
            print(f"   📏 Response size: {len(response.content):,} bytes")
            return True
        else:
            print(f"❌ FastAPI batch processing failed: {response.status_code}")
            print(f"   Response: {response.text}")
            return False
        
    except Exception as e:
        print(f"❌ FastAPI batch test failed: {e}")
        return False

if __name__ == "__main__":
    print("🚀 Starting directory processing tests...")
    print("=" * 60)
    
    # Test all components
    all_tests_passed = True
    
    # Test core directory processing
    if not test_directory_processing():
        all_tests_passed = False
    
    # Test CLI directory processing
    if not test_cli_directory_processing():
        all_tests_passed = False
    
    # Test FastAPI batch processing
    if not test_fastapi_batch_processing():
        all_tests_passed = False
    
    print(f"\n🎯 FINAL RESULT")
    print("=" * 60)
    
    if all_tests_passed:
        print("🎉 ALL DIRECTORY PROCESSING TESTS PASSED!")
        print("✅ Core directory processing working")
        print("✅ CLI batch processing working")
        print("✅ FastAPI batch processing working")
    else:
        print("⚠️  Some directory processing tests failed. Check the output above for details.")
    
    print(f"\n📁 Check the output directories for processed images:")
    print(f"   Core processing: /Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out")
    print(f"   CLI processing: /Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out_cli")
    print(f"   API processing: /Volumes/Barracuda/mamey-io/code-cursor/Mamey/src/Mamey.Image/testImages/out_api")

