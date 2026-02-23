module.exports = {
  // JWT Configuration
  jwt: {
    secret: process.env.JWT_SECRET || 'your-super-secret-key-change-in-production',
    expiresIn: process.env.JWT_EXPIRES_IN || '1h',
    refreshExpiresIn: process.env.JWT_REFRESH_EXPIRES_IN || '7d'
  },
  
  // Email Configuration
  email: {
    host: process.env.SMTP_HOST || 'smtp.gmail.com',
    port: process.env.SMTP_PORT || 587,
    user: process.env.SMTP_USER || '',
    password: process.env.SMTP_PASSWORD || '',
    from: process.env.SMTP_FROM || 'noreply@smartschool.com'
  },
  
  // Zoom Configuration
  zoom: {
    apiKey: process.env.ZOOM_API_KEY || '',
    apiSecret: process.env.ZOOM_API_SECRET || '',
    accountId: process.env.ZOOM_ACCOUNT_ID || ''
  },
  
  // File Upload Configuration
  upload: {
    path: process.env.UPLOAD_PATH || 'uploads',
    maxFileSize: parseInt(process.env.MAX_FILE_SIZE) || 52428800,
    allowedImages: ['.jpg', '.jpeg', '.png', '.gif', '.webp'],
    allowedDocuments: ['.pdf', '.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx'],
    allowedVideos: ['.mp4', '.avi', '.mkv', '.mov', '.webm']
  },
  
  // Roles
  roles: {
    ADMIN: 'Admin',
    SCHOOL_ADMIN: 'SchoolAdmin',
    ACCOUNTANT: 'Accountant',
    TEACHER: 'Teacher',
    STUDENT: 'Student',
    PARENT: 'Parent',
    RECEPTIONIST: 'Receptionist',
    LIBRARIAN: 'Librarian'
  },
  
  // Default Admin Credentials
  defaultAdmin: {
    username: 'admin',
    email: 'admin@smartschool.com',
    password: process.env.DEFAULT_ADMIN_PASSWORD || 'changeme-dev',
    firstName: 'System',
    lastName: 'Administrator'
  }
};
