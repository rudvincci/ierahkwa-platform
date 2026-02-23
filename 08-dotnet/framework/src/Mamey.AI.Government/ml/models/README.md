# Government AI Models

This directory contains the trained Machine Learning models used by the Mamey.AI.Government library.

## Directory Structure

- `training/`: Python scripts for training models.
- `models/`: Output directory for trained model artifacts (.onnx, .pb, .model).

## Training Pipelines

### Document Classifier
Classifies uploaded documents into categories (Passport, ID, Utility Bill, etc.).

**Usage:**
```bash
python training/document_classifier_training.py --data /path/to/dataset --epochs 20
```

**Output:** `models/document_classifier.onnx`

### Fraud Detection
Scores applications based on risk factors.

**Usage:**
```bash
python training/fraud_model_training.py --data /path/to/fraud_history.csv
```

**Output:** `models/fraud_detection.model`

## Integration

These models are loaded by the C# services using `Microsoft.ML.OnnxRuntime` or specific model loaders defined in `Mamey.AI.Government/Services`.
