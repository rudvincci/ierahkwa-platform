const { Sequelize } = require('sequelize');
const dbConfig = require('../config/database');

const env = process.env.NODE_ENV || 'development';
const config = dbConfig[env];

const sequelize = new Sequelize(
  config.database,
  config.username,
  config.password,
  {
    host: config.host,
    port: config.port,
    dialect: config.dialect,
    logging: config.logging,
    pool: config.pool
  }
);

// Import models
const Tenant = require('./tenant.model')(sequelize);
const User = require('./user.model')(sequelize);
const Role = require('./role.model')(sequelize);
const UserRole = require('./userRole.model')(sequelize);
const Grade = require('./grade.model')(sequelize);
const ClassRoom = require('./classroom.model')(sequelize);
const Material = require('./material.model')(sequelize);
const Teacher = require('./teacher.model')(sequelize);
const TeacherMaterial = require('./teacherMaterial.model')(sequelize);
const Student = require('./student.model')(sequelize);
const Parent = require('./parent.model')(sequelize);
const StudentParent = require('./studentParent.model')(sequelize);
const Schedule = require('./schedule.model')(sequelize);
const Homework = require('./homework.model')(sequelize);
const HomeworkContent = require('./homeworkContent.model')(sequelize);
const HomeworkQuestion = require('./homeworkQuestion.model')(sequelize);
const HomeworkAnswer = require('./homeworkAnswer.model')(sequelize);
const QuestionAnswer = require('./questionAnswer.model')(sequelize);
const SchoolYear = require('./schoolYear.model')(sequelize);
const Fees = require('./fees.model')(sequelize);
const Unit = require('./unit.model')(sequelize);
const Category = require('./category.model')(sequelize);
const Product = require('./product.model')(sequelize);
const Supplier = require('./supplier.model')(sequelize);
const Invoice = require('./invoice.model')(sequelize);
const InvoiceItem = require('./invoiceItem.model')(sequelize);
const InvoicePayment = require('./invoicePayment.model')(sequelize);
const Account = require('./account.model')(sequelize);
const Journal = require('./journal.model')(sequelize);
const JournalEntry = require('./journalEntry.model')(sequelize);
const CostCenter = require('./costCenter.model')(sequelize);
const AdmissionEnquiry = require('./admissionEnquiry.model')(sequelize);
const VisitorBook = require('./visitorBook.model')(sequelize);
const PhoneLog = require('./phoneLog.model')(sequelize);
const PostalDispatch = require('./postalDispatch.model')(sequelize);
const PostalReceive = require('./postalReceive.model')(sequelize);
const Complain = require('./complain.model')(sequelize);
const Book = require('./book.model')(sequelize);
const BookCategory = require('./bookCategory.model')(sequelize);
const LibraryMember = require('./libraryMember.model')(sequelize);
const BorrowTransaction = require('./borrowTransaction.model')(sequelize);
const LiveClass = require('./liveClass.model')(sequelize);
const LiveClassAttendance = require('./liveClassAttendance.model')(sequelize);
const ZoomSettings = require('./zoomSettings.model')(sequelize);
const AuditLog = require('./auditLog.model')(sequelize);

// Define associations
// User - Role (Many-to-Many)
User.belongsToMany(Role, { through: UserRole, foreignKey: 'userId' });
Role.belongsToMany(User, { through: UserRole, foreignKey: 'roleId' });

// User - Tenant
User.belongsTo(Tenant, { foreignKey: 'tenantId' });
Tenant.hasMany(User, { foreignKey: 'tenantId' });

// Grade - ClassRoom
Grade.hasMany(ClassRoom, { foreignKey: 'gradeId' });
ClassRoom.belongsTo(Grade, { foreignKey: 'gradeId' });

// ClassRoom - Student
ClassRoom.hasMany(Student, { foreignKey: 'classRoomId' });
Student.belongsTo(ClassRoom, { foreignKey: 'classRoomId' });

// Teacher - Material (Many-to-Many)
Teacher.belongsToMany(Material, { through: TeacherMaterial, foreignKey: 'teacherId' });
Material.belongsToMany(Teacher, { through: TeacherMaterial, foreignKey: 'materialId' });

// Student - Parent (Many-to-Many)
Student.belongsToMany(Parent, { through: StudentParent, foreignKey: 'studentId' });
Parent.belongsToMany(Student, { through: StudentParent, foreignKey: 'parentId' });

// Schedule associations
Schedule.belongsTo(ClassRoom, { foreignKey: 'classRoomId' });
Schedule.belongsTo(Teacher, { foreignKey: 'teacherId' });
Schedule.belongsTo(Material, { foreignKey: 'materialId' });

// Homework associations
Homework.belongsTo(Teacher, { foreignKey: 'teacherId' });
Homework.belongsTo(Material, { foreignKey: 'materialId' });
Homework.belongsTo(ClassRoom, { foreignKey: 'classRoomId' });
Homework.hasMany(HomeworkContent, { foreignKey: 'homeworkId', as: 'contents' });
Homework.hasMany(HomeworkQuestion, { foreignKey: 'homeworkId', as: 'questions' });
Homework.hasMany(HomeworkAnswer, { foreignKey: 'homeworkId', as: 'answers' });

HomeworkAnswer.belongsTo(Homework, { foreignKey: 'homeworkId' });
HomeworkAnswer.belongsTo(Student, { foreignKey: 'studentId' });
HomeworkAnswer.hasMany(QuestionAnswer, { foreignKey: 'homeworkAnswerId', as: 'questionAnswers' });

QuestionAnswer.belongsTo(HomeworkQuestion, { foreignKey: 'questionId' });

// Accounting associations
Category.belongsTo(Category, { foreignKey: 'parentId', as: 'parent' });
Category.hasMany(Category, { foreignKey: 'parentId', as: 'children' });
Category.hasMany(Product, { foreignKey: 'categoryId' });

Product.belongsTo(Category, { foreignKey: 'categoryId' });
Product.belongsTo(Unit, { foreignKey: 'unitId' });

Invoice.belongsTo(Supplier, { foreignKey: 'supplierId' });
Invoice.belongsTo(Student, { foreignKey: 'studentId' });
Invoice.hasMany(InvoiceItem, { foreignKey: 'invoiceId', as: 'items' });
Invoice.hasMany(InvoicePayment, { foreignKey: 'invoiceId', as: 'payments' });

InvoiceItem.belongsTo(Product, { foreignKey: 'productId' });
InvoiceItem.belongsTo(Fees, { foreignKey: 'feesId' });

Fees.belongsTo(SchoolYear, { foreignKey: 'schoolYearId' });

Account.belongsTo(Account, { foreignKey: 'parentId', as: 'parent' });
Account.hasMany(Account, { foreignKey: 'parentId', as: 'children' });

Journal.hasMany(JournalEntry, { foreignKey: 'journalId', as: 'entries' });
JournalEntry.belongsTo(Account, { foreignKey: 'accountId' });
JournalEntry.belongsTo(CostCenter, { foreignKey: 'costCenterId' });

CostCenter.belongsTo(CostCenter, { foreignKey: 'parentId', as: 'parent' });
CostCenter.hasMany(CostCenter, { foreignKey: 'parentId', as: 'children' });

// Library associations
BookCategory.belongsTo(BookCategory, { foreignKey: 'parentId', as: 'parent' });
BookCategory.hasMany(BookCategory, { foreignKey: 'parentId', as: 'children' });
Book.belongsTo(BookCategory, { foreignKey: 'categoryId' });

BorrowTransaction.belongsTo(LibraryMember, { foreignKey: 'memberId' });
BorrowTransaction.belongsTo(Book, { foreignKey: 'bookId' });

// Live Class associations
LiveClass.belongsTo(Teacher, { foreignKey: 'teacherId' });
LiveClass.belongsTo(ClassRoom, { foreignKey: 'classRoomId' });
LiveClass.belongsTo(Material, { foreignKey: 'materialId' });
LiveClass.hasMany(LiveClassAttendance, { foreignKey: 'liveClassId', as: 'attendances' });

LiveClassAttendance.belongsTo(Student, { foreignKey: 'studentId' });

module.exports = {
  sequelize,
  Sequelize,
  Tenant,
  User,
  Role,
  UserRole,
  Grade,
  ClassRoom,
  Material,
  Teacher,
  TeacherMaterial,
  Student,
  Parent,
  StudentParent,
  Schedule,
  Homework,
  HomeworkContent,
  HomeworkQuestion,
  HomeworkAnswer,
  QuestionAnswer,
  SchoolYear,
  Fees,
  Unit,
  Category,
  Product,
  Supplier,
  Invoice,
  InvoiceItem,
  InvoicePayment,
  Account,
  Journal,
  JournalEntry,
  CostCenter,
  AdmissionEnquiry,
  VisitorBook,
  PhoneLog,
  PostalDispatch,
  PostalReceive,
  Complain,
  Book,
  BookCategory,
  LibraryMember,
  BorrowTransaction,
  LiveClass,
  LiveClassAttendance,
  ZoomSettings,
  AuditLog
};
