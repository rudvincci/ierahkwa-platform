# Foundation Services Registration

services {
  name = "pupitre-users"
  id   = "pupitre-users-1"
  port = 60001
  tags = ["foundation", "users", "api"]
  
  check {
    http     = "http://localhost:60001/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-gles"
  id   = "pupitre-gles-1"
  port = 60002
  tags = ["foundation", "gles", "api"]
  
  check {
    http     = "http://localhost:60002/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-curricula"
  id   = "pupitre-curricula-1"
  port = 60003
  tags = ["foundation", "curricula", "api"]
  
  check {
    http     = "http://localhost:60003/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-lessons"
  id   = "pupitre-lessons-1"
  port = 60004
  tags = ["foundation", "lessons", "api"]
  
  check {
    http     = "http://localhost:60004/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-assessments"
  id   = "pupitre-assessments-1"
  port = 60005
  tags = ["foundation", "assessments", "api"]
  
  check {
    http     = "http://localhost:60005/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-ieps"
  id   = "pupitre-ieps-1"
  port = 60006
  tags = ["foundation", "ieps", "api"]
  
  check {
    http     = "http://localhost:60006/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-rewards"
  id   = "pupitre-rewards-1"
  port = 60007
  tags = ["foundation", "rewards", "api"]
  
  check {
    http     = "http://localhost:60007/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-notifications"
  id   = "pupitre-notifications-1"
  port = 60008
  tags = ["foundation", "notifications", "api"]
  
  check {
    http     = "http://localhost:60008/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-credentials"
  id   = "pupitre-credentials-1"
  port = 60009
  tags = ["foundation", "credentials", "api"]
  
  check {
    http     = "http://localhost:60009/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}

services {
  name = "pupitre-analytics"
  id   = "pupitre-analytics-1"
  port = 60010
  tags = ["foundation", "analytics", "api"]
  
  check {
    http     = "http://localhost:60010/health"
    interval = "10s"
    timeout  = "5s"
  }

  meta {
    version = "1.0.0"
    team    = "foundation"
  }
}
