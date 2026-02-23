"""
Mamey Image Background Removal

AI-powered background removal service for images using rembg.
Supports both REST API and CLI interfaces.
"""

from .core.processor import BackgroundRemovalProcessor
from .core.config import Settings

__version__ = "1.0.0"
__all__ = ["BackgroundRemovalProcessor", "Settings"]

