require('dotenv').config({ path: require('path').join(__dirname, '../../.env') });
const bcrypt = require('bcryptjs');
const { sequelize, Tenant, User, Role, UserRole, Grade, ClassRoom, Material, SchoolYear, Account } = require('../models');

const seedDatabase = async () => {
  try {
    console.log('Starting database seed...');
    
    await sequelize.sync({ force: true });
    console.log('Database synchronized');

    // Create Roles
    const roles = await Role.bulkCreate([
      { name: 'Admin', description: 'System Administrator', isSystemRole: true },
      { name: 'SchoolAdmin', description: 'School Administrator', isSystemRole: true },
      { name: 'Accountant', description: 'School Accountant', isSystemRole: true },
      { name: 'Teacher', description: 'Teacher', isSystemRole: true },
      { name: 'Student', description: 'Student', isSystemRole: true },
      { name: 'Parent', description: 'Parent/Guardian', isSystemRole: true },
      { name: 'Receptionist', description: 'Front Desk Staff', isSystemRole: true },
      { name: 'Librarian', description: 'Library Staff', isSystemRole: true }
    ]);
    console.log('Roles created');

    // Create Default Tenant
    const tenant = await Tenant.create({
      name: 'Demo School',
      code: 'DEMO001',
      address: '123 Education Street',
      phone: '+1-555-0100',
      email: 'info@demoschool.com',
      currency: 'USD',
      taxRate: 0
    });
    console.log('Tenant created');

    // Create Admin User
    const adminPassword = await bcrypt.hash(process.env.DEFAULT_ADMIN_PASSWORD || 'changeme-dev', 10);
    const adminUser = await User.create({
      username: 'admin',
      email: 'admin@smartschool.com',
      password: adminPassword,
      firstName: 'System',
      lastName: 'Administrator',
      isActive: true,
      emailConfirmed: true
    });

    const adminRole = roles.find(r => r.name === 'Admin');
    await UserRole.create({ userId: adminUser.id, roleId: adminRole.id });
    console.log('Admin user created');

    // Create School Admin
    const schoolAdminUser = await User.create({
      username: 'schooladmin',
      email: 'schooladmin@demoschool.com',
      password: adminPassword,
      firstName: 'School',
      lastName: 'Admin',
      isActive: true,
      emailConfirmed: true,
      tenantId: tenant.id
    });

    const schoolAdminRole = roles.find(r => r.name === 'SchoolAdmin');
    await UserRole.create({ userId: schoolAdminUser.id, roleId: schoolAdminRole.id });
    console.log('School admin created');

    // Create School Year
    const currentYear = new Date().getFullYear();
    await SchoolYear.create({
      name: `${currentYear}-${currentYear + 1}`,
      startDate: `${currentYear}-09-01`,
      endDate: `${currentYear + 1}-06-30`,
      isCurrent: true,
      tenantId: tenant.id
    });
    console.log('School year created');

    // Create Grades
    const grades = await Grade.bulkCreate([
      { name: 'Grade 1', orderIndex: 1, tenantId: tenant.id },
      { name: 'Grade 2', orderIndex: 2, tenantId: tenant.id },
      { name: 'Grade 3', orderIndex: 3, tenantId: tenant.id },
      { name: 'Grade 4', orderIndex: 4, tenantId: tenant.id },
      { name: 'Grade 5', orderIndex: 5, tenantId: tenant.id },
      { name: 'Grade 6', orderIndex: 6, tenantId: tenant.id }
    ]);
    console.log('Grades created');

    // Create Classrooms
    for (const grade of grades) {
      await ClassRoom.bulkCreate([
        { name: `${grade.name} - A`, gradeId: grade.id, capacity: 30, tenantId: tenant.id },
        { name: `${grade.name} - B`, gradeId: grade.id, capacity: 30, tenantId: tenant.id }
      ]);
    }
    console.log('Classrooms created');

    // Create Materials (Subjects)
    await Material.bulkCreate([
      { name: 'Mathematics', code: 'MATH', tenantId: tenant.id },
      { name: 'English Language', code: 'ENG', tenantId: tenant.id },
      { name: 'Science', code: 'SCI', tenantId: tenant.id },
      { name: 'Social Studies', code: 'SOC', tenantId: tenant.id },
      { name: 'Art', code: 'ART', tenantId: tenant.id },
      { name: 'Physical Education', code: 'PE', tenantId: tenant.id },
      { name: 'Computer Science', code: 'CS', tenantId: tenant.id },
      { name: 'Music', code: 'MUS', tenantId: tenant.id }
    ]);
    console.log('Materials created');

    // Create Chart of Accounts
    const accountTypes = [
      { code: '1000', name: 'Assets', type: 'Asset', isSystemAccount: true },
      { code: '1100', name: 'Cash', type: 'Asset', parentCode: '1000' },
      { code: '1200', name: 'Accounts Receivable', type: 'Asset', parentCode: '1000' },
      { code: '2000', name: 'Liabilities', type: 'Liability', isSystemAccount: true },
      { code: '2100', name: 'Accounts Payable', type: 'Liability', parentCode: '2000' },
      { code: '3000', name: 'Equity', type: 'Equity', isSystemAccount: true },
      { code: '3100', name: 'Retained Earnings', type: 'Equity', parentCode: '3000' },
      { code: '4000', name: 'Revenue', type: 'Revenue', isSystemAccount: true },
      { code: '4100', name: 'Tuition Fees', type: 'Revenue', parentCode: '4000' },
      { code: '4200', name: 'Registration Fees', type: 'Revenue', parentCode: '4000' },
      { code: '5000', name: 'Expenses', type: 'Expense', isSystemAccount: true },
      { code: '5100', name: 'Salaries', type: 'Expense', parentCode: '5000' },
      { code: '5200', name: 'Utilities', type: 'Expense', parentCode: '5000' },
      { code: '5300', name: 'Supplies', type: 'Expense', parentCode: '5000' }
    ];

    const accountMap = {};
    for (const acc of accountTypes) {
      const parentId = acc.parentCode ? accountMap[acc.parentCode] : null;
      const created = await Account.create({
        code: acc.code,
        name: acc.name,
        type: acc.type,
        parentId,
        level: parentId ? 2 : 1,
        isSystemAccount: acc.isSystemAccount || false,
        tenantId: tenant.id
      });
      accountMap[acc.code] = created.id;
    }
    console.log('Chart of accounts created');

    console.log('\n========================================');
    console.log('Database seeded successfully!');
    console.log('========================================');
    console.log('\nLogin Credentials:');
    console.log('  System Admin: admin / P@ssw0rd');
    console.log('  School Admin: schooladmin / P@ssw0rd');
    console.log('========================================\n');

    process.exit(0);
  } catch (error) {
    console.error('Seed error:', error);
    process.exit(1);
  }
};

seedDatabase();
