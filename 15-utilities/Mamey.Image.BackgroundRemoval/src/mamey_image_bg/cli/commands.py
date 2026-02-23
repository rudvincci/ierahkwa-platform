"""CLI commands for background removal."""

import os
import sys
import logging
from pathlib import Path
from typing import Optional
import click
from PIL import Image

from ..core.processor import BackgroundRemovalProcessor
from ..core.config import settings

# Configure logging for CLI
logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s - %(levelname)s - %(message)s"
)
logger = logging.getLogger(__name__)


@click.group()
@click.option('--model', default=settings.model_name, help='Background removal model to use')
@click.option('--verbose', '-v', is_flag=True, help='Enable verbose logging')
@click.pass_context
def cli(ctx, model, verbose):
    """Mamey Image Background Removal CLI Tool."""
    if verbose:
        logging.getLogger().setLevel(logging.DEBUG)
    
    ctx.ensure_object(dict)
    ctx.obj['model'] = model
    ctx.obj['verbose'] = verbose


@cli.command()
@click.argument('input_path', type=click.Path(exists=True))
@click.argument('output_path', type=click.Path())
@click.option('--format', 'output_format', default='PNG', 
              type=click.Choice(['PNG', 'JPEG'], case_sensitive=False),
              help='Output format')
@click.option('--model', help='Override model for this command')
@click.pass_context
def remove(ctx, input_path, output_path, output_format, model):
    """Remove background from an image file.
    
    INPUT_PATH: Path to input image file
    OUTPUT_PATH: Path for output image file
    """
    try:
        # Use command-specific model or context model
        model_name = model or ctx.obj['model']
        
        # Initialize processor
        processor = BackgroundRemovalProcessor(model_name=model_name)
        
        # Validate input file
        input_path = Path(input_path)
        if not input_path.is_file():
            click.echo(f"Error: Input file does not exist: {input_path}", err=True)
            sys.exit(1)
        
        # Check if input is a valid image
        try:
            with Image.open(input_path) as img:
                click.echo(f"Input image: {img.size}, mode: {img.mode}")
        except Exception as e:
            click.echo(f"Error: Invalid image file: {e}", err=True)
            sys.exit(1)
        
        # Create output directory if needed
        output_path = Path(output_path)
        output_path.parent.mkdir(parents=True, exist_ok=True)
        
        # Process image
        click.echo(f"Processing image with model '{model_name}'...")
        result_path = processor.remove_background_from_file(
            str(input_path), 
            str(output_path), 
            output_format.upper()
        )
        
        click.echo(f"✅ Background removed successfully!")
        click.echo(f"Input:  {input_path}")
        click.echo(f"Output: {result_path}")
        
    except Exception as e:
        click.echo(f"Error: {e}", err=True)
        sys.exit(1)


@cli.command()
@click.argument('input_dir', type=click.Path(exists=True, file_okay=False))
@click.argument('output_dir', type=click.Path())
@click.option('--format', 'output_format', default='PNG',
              type=click.Choice(['PNG', 'JPEG'], case_sensitive=False),
              help='Output format')
@click.option('--pattern', default='*', help='File pattern to match (e.g., "*.jpg")')
@click.option('--model', help='Override model for this command')
@click.option('--recursive', '-r', is_flag=True, help='Process subdirectories recursively')
@click.pass_context
def batch(ctx, input_dir, output_dir, output_format, pattern, model, recursive):
    """Remove background from multiple images in a directory.
    
    INPUT_DIR: Directory containing input images
    OUTPUT_DIR: Directory for output images
    """
    try:
        # Use command-specific model or context model
        model_name = model or ctx.obj['model']
        
        # Initialize processor
        processor = BackgroundRemovalProcessor(model_name=model_name)
        
        input_dir = Path(input_dir)
        output_dir = Path(output_dir)
        
        # Create output directory
        output_dir.mkdir(parents=True, exist_ok=True)
        
        # Find image files
        if recursive:
            image_files = list(input_dir.rglob(pattern))
        else:
            image_files = list(input_dir.glob(pattern))
        
        # Filter for image files
        image_extensions = {'.jpg', '.jpeg', '.png', '.bmp', '.tiff', '.webp'}
        image_files = [f for f in image_files if f.suffix.lower() in image_extensions]
        
        if not image_files:
            click.echo(f"No image files found in {input_dir} with pattern '{pattern}'")
            return
        
        click.echo(f"Found {len(image_files)} image files to process")
        
        # Process each file
        successful = 0
        failed = 0
        
        for i, input_file in enumerate(image_files, 1):
            try:
                # Calculate relative path to maintain directory structure
                if recursive:
                    rel_path = input_file.relative_to(input_dir)
                else:
                    rel_path = Path(input_file.name)
                
                # Create output path
                output_file = output_dir / rel_path.with_suffix(f'.{output_format.lower()}')
                output_file.parent.mkdir(parents=True, exist_ok=True)
                
                click.echo(f"[{i}/{len(image_files)}] Processing: {input_file.name}")
                
                # Process image
                processor.remove_background_from_file(
                    str(input_file),
                    str(output_file),
                    output_format.upper()
                )
                
                successful += 1
                
            except Exception as e:
                click.echo(f"  ❌ Failed: {e}", err=True)
                failed += 1
                continue
        
        # Summary
        click.echo(f"\n📊 Processing complete:")
        click.echo(f"  ✅ Successful: {successful}")
        click.echo(f"  ❌ Failed: {failed}")
        click.echo(f"  📁 Output directory: {output_dir}")
        
    except Exception as e:
        click.echo(f"Error: {e}", err=True)
        sys.exit(1)


@cli.command()
@click.pass_context
def models(ctx):
    """List available background removal models."""
    try:
        processor = BackgroundRemovalProcessor()
        models = processor.get_supported_models()
        
        click.echo("Available background removal models:")
        for model in models:
            current = " (current)" if model == ctx.obj['model'] else ""
            click.echo(f"  • {model}{current}")
        
        click.echo(f"\nCurrent model: {ctx.obj['model']}")
        
    except Exception as e:
        click.echo(f"Error: {e}", err=True)
        sys.exit(1)


@cli.command()
@click.option('--host', default=settings.host, help='Host to bind to')
@click.option('--port', default=settings.port, help='Port to bind to')
@click.option('--reload', is_flag=True, help='Enable auto-reload for development')
@click.pass_context
def serve(ctx, host, port, reload):
    """Start the background removal API server."""
    try:
        import uvicorn
        from ..api.main import app
        
        click.echo(f"Starting Mamey Image Background Removal API server...")
        click.echo(f"Host: {host}")
        click.echo(f"Port: {port}")
        click.echo(f"Model: {ctx.obj['model']}")
        click.echo(f"API docs: http://{host}:{port}/docs")
        
        uvicorn.run(
            app,
            host=host,
            port=port,
            reload=reload,
            log_level="info"
        )
        
    except Exception as e:
        click.echo(f"Error starting server: {e}", err=True)
        sys.exit(1)


if __name__ == '__main__':
    cli()
