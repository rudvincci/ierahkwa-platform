"""Configuration settings for the background removal service."""

import os
from typing import Optional
from pydantic_settings import BaseSettings


class Settings(BaseSettings):
    """Application settings."""
    
    # Server configuration
    host: str = "0.0.0.0"
    port: int = 5000
    debug: bool = False
    
    # Background removal configuration
    model_name: str = "u2net"  # rembg model to use
    max_image_size: int = 1024  # Maximum image dimension in pixels
    quality: int = 95  # Output image quality (1-100)
    
    # Logging
    log_level: str = "INFO"
    
    class Config:
        env_file = ".env"
        env_prefix = "MAMEY_IMAGE_BG_"


# Global settings instance
settings = Settings()
