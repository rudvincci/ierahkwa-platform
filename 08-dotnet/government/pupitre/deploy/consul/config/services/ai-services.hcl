# AI Services Registration

services {
  name = "pupitre-ai-tutors"
  id   = "pupitre-ai-tutors-1"
  port = 60011
  tags = ["ai", "tutors", "api", "llm"]
  
  check {
    http     = "http://localhost:60011/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
    gpu_required = "true"
  }
}

services {
  name = "pupitre-ai-assessments"
  id   = "pupitre-ai-assessments-1"
  port = 60012
  tags = ["ai", "assessments", "api", "llm"]
  
  check {
    http     = "http://localhost:60012/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-content"
  id   = "pupitre-ai-content-1"
  port = 60013
  tags = ["ai", "content", "api", "llm"]
  
  check {
    http     = "http://localhost:60013/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-speech"
  id   = "pupitre-ai-speech-1"
  port = 60014
  tags = ["ai", "speech", "api", "whisper", "tts"]
  
  check {
    http     = "http://localhost:60014/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
    gpu_required = "true"
  }
}

services {
  name = "pupitre-ai-adaptive"
  id   = "pupitre-ai-adaptive-1"
  port = 60015
  tags = ["ai", "adaptive", "api", "ml"]
  
  check {
    http     = "http://localhost:60015/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-behavior"
  id   = "pupitre-ai-behavior-1"
  port = 60016
  tags = ["ai", "behavior", "api", "ml"]
  
  check {
    http     = "http://localhost:60016/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-safety"
  id   = "pupitre-ai-safety-1"
  port = 60017
  tags = ["ai", "safety", "api", "moderation"]
  
  check {
    http     = "http://localhost:60017/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-recommendations"
  id   = "pupitre-ai-recommendations-1"
  port = 60018
  tags = ["ai", "recommendations", "api", "ml"]
  
  check {
    http     = "http://localhost:60018/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-translation"
  id   = "pupitre-ai-translation-1"
  port = 60019
  tags = ["ai", "translation", "api", "llm"]
  
  check {
    http     = "http://localhost:60019/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
  }
}

services {
  name = "pupitre-ai-vision"
  id   = "pupitre-ai-vision-1"
  port = 60020
  tags = ["ai", "vision", "api", "ml"]
  
  check {
    http     = "http://localhost:60020/health"
    interval = "15s"
    timeout  = "10s"
  }

  meta {
    version = "1.0.0"
    team    = "ai"
    gpu_required = "true"
  }
}
