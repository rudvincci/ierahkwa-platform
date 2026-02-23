# Support Services Registration

services {
  name = "pupitre-parents"
  id   = "pupitre-parents-1"
  port = 60021
  tags = ["support", "parents", "api"]
  
  check {
    http     = "http://localhost:60021/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-educators"
  id   = "pupitre-educators-1"
  port = 60022
  tags = ["support", "educators", "api"]
  
  check {
    http     = "http://localhost:60022/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-fundraising"
  id   = "pupitre-fundraising-1"
  port = 60023
  tags = ["support", "fundraising", "api"]
  
  check {
    http     = "http://localhost:60023/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-bookstore"
  id   = "pupitre-bookstore-1"
  port = 60024
  tags = ["support", "bookstore", "api"]
  
  check {
    http     = "http://localhost:60024/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-aftercare"
  id   = "pupitre-aftercare-1"
  port = 60025
  tags = ["support", "aftercare", "api"]
  
  check {
    http     = "http://localhost:60025/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-accessibility"
  id   = "pupitre-accessibility-1"
  port = 60026
  tags = ["support", "accessibility", "api"]
  
  check {
    http     = "http://localhost:60026/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-compliance"
  id   = "pupitre-compliance-1"
  port = 60027
  tags = ["support", "compliance", "api"]
  
  check {
    http     = "http://localhost:60027/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-ministries"
  id   = "pupitre-ministries-1"
  port = 60028
  tags = ["support", "ministries", "api"]
  
  check {
    http     = "http://localhost:60028/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}

services {
  name = "pupitre-operations"
  id   = "pupitre-operations-1"
  port = 60029
  tags = ["support", "operations", "api"]
  
  check {
    http     = "http://localhost:60029/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "support"
  }
}
