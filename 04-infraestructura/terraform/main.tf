terraform {
  required_version = ">= 1.5.0"
  backend "s3" {
    bucket = "soberana-terraform"
    key    = "state/production.tfstate"
    region = "us-east-1"
  }
}

provider "aws" { region = var.region }

# VPC
module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  name    = "soberana-vpc"
  cidr    = "10.0.0.0/16"
  azs     = ["${var.region}a", "${var.region}b", "${var.region}c"]
  private_subnets = ["10.0.1.0/24", "10.0.2.0/24", "10.0.3.0/24"]
  public_subnets  = ["10.0.101.0/24", "10.0.102.0/24", "10.0.103.0/24"]
  enable_nat_gateway = true
}

# EKS Cluster
module "eks" {
  source          = "terraform-aws-modules/eks/aws"
  cluster_name    = "soberana-cluster"
  cluster_version = "1.28"
  vpc_id          = module.vpc.vpc_id
  subnet_ids      = module.vpc.private_subnets
  eks_managed_node_groups = {
    api = { instance_types = ["m6i.xlarge"]; min_size = 3; max_size = 20; desired_size = 3 }
    blockchain = { instance_types = ["r6i.2xlarge"]; min_size = 3; max_size = 6; desired_size = 3 }
    ai = { instance_types = ["g5.4xlarge"]; min_size = 2; max_size = 8; desired_size = 2 }
  }
}

# RDS PostgreSQL
resource "aws_db_instance" "soberana" {
  identifier        = "soberana-db"
  engine            = "postgres"
  engine_version    = "16"
  instance_class    = "db.r6g.xlarge"
  allocated_storage = 100
  db_name           = "soberana"
  username          = "soberano"
  password          = var.db_password
  vpc_security_group_ids = [aws_security_group.db.id]
  db_subnet_group_name   = aws_db_subnet_group.soberana.name
  multi_az               = true
  storage_encrypted      = true
  backup_retention_period = 30
}

# ElastiCache Redis
resource "aws_elasticache_cluster" "soberana" {
  cluster_id      = "soberana-redis"
  engine          = "redis"
  node_type       = "cache.r6g.large"
  num_cache_nodes = 1
}

# S3 for media storage
resource "aws_s3_bucket" "media" {
  bucket = "soberana-media"
}

resource "aws_s3_bucket_server_side_encryption_configuration" "media" {
  bucket = aws_s3_bucket.media.id
  rule { apply_server_side_encryption_by_default { sse_algorithm = "aws:kms" } }
}

# Security Group for RDS
resource "aws_security_group" "db" {
  name_prefix = "soberana-db-"
  vpc_id      = module.vpc.vpc_id

  ingress {
    from_port       = 5432
    to_port         = 5432
    protocol        = "tcp"
    security_groups = [module.eks.cluster_security_group_id]
    description     = "PostgreSQL from EKS cluster"
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = { Name = "soberana-db-sg" }
}

# DB Subnet Group
resource "aws_db_subnet_group" "soberana" {
  name       = "soberana-db-subnet"
  subnet_ids = module.vpc.private_subnets

  tags = { Name = "soberana-db-subnet-group" }
}

# Variables
variable "region" { default = "us-east-1" }
variable "db_password" {
  sensitive   = true
  description = "PostgreSQL master password"
}

# Outputs
output "cluster_endpoint" { value = module.eks.cluster_endpoint }
output "db_endpoint" { value = aws_db_instance.soberana.endpoint }
output "vpc_id" { value = module.vpc.vpc_id }
output "redis_endpoint" { value = aws_elasticache_cluster.soberana.cache_nodes[0].address }
