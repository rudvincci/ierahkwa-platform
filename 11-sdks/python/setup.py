from setuptools import setup, find_packages

setup(
    name="mamey-sdk",
    version="1.0.0",
    description="Official MameyNode Python SDK for Ierahkwa Sovereign Blockchain",
    author="Mamey-io",
    author_email="dev@mamey.io",
    packages=find_packages(),
    install_requires=[
        "requests>=2.28.0",
    ],
    python_requires=">=3.8",
    classifiers=[
        "Development Status :: 4 - Beta",
        "Intended Audience :: Developers",
        "License :: OSI Approved :: Apache Software License",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "Programming Language :: Python :: 3.10",
        "Programming Language :: Python :: 3.11",
    ],
)
