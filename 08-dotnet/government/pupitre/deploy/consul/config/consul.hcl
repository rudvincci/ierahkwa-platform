datacenter = "pupitre-dc1"
data_dir = "/consul/data"
log_level = "INFO"

server = true
bootstrap_expect = 1

ui_config {
  enabled = true
}

client_addr = "0.0.0.0"
bind_addr = "0.0.0.0"

ports {
  http = 8500
  dns = 8600
  grpc = 8502
}

connect {
  enabled = true
}

# ACL Configuration (disabled for dev)
acl {
  enabled = false
  default_policy = "allow"
  enable_token_persistence = true
}

# Service Mesh
service_mesh {
  enabled = true
}
