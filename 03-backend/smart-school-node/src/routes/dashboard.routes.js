const express = require('express');
const router = express.Router();
const { addTenantFilter } = require('../middleware/tenant.middleware');
const config = require('../config/config');
const { 
  Student, Teacher, ClassRoom, Grade, Invoice, Fees, 
  Homework, LiveClass, BorrowTransaction, Complain, AdmissionEnquiry 
} = require('../models');
const { Op } = require('sequelize');

// Main dashboard statistics
router.get('/', async (req, res, next) => {
  try {
    const filter = addTenantFilter(req);
    
    const [
      studentCount,
      teacherCount,
      classRoomCount,
      gradeCount
    ] = await Promise.all([
      Student.count({ where: filter }),
      Teacher.count({ where: filter }),
      ClassRoom.count({ where: filter }),
      Grade.count({ where: filter })
    ]);
    
    // Financial summary
    const invoices = await Invoice.findAll({
      where: { ...filter, type: 'FeesInvoice' },
      attributes: ['totalAmount', 'paidAmount', 'paymentStatus']
    });
    
    const totalRevenue = invoices.reduce((sum, inv) => sum + parseFloat(inv.totalAmount || 0), 0);
    const totalReceived = invoices.reduce((sum, inv) => sum + parseFloat(inv.paidAmount || 0), 0);
    const pendingAmount = totalRevenue - totalReceived;
    
    // Recent activities
    const recentHomework = await Homework.count({
      where: { ...filter, createdAt: { [Op.gte]: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000) } }
    });
    
    const upcomingClasses = await LiveClass.count({
      where: { ...filter, status: 'Scheduled', startDateTime: { [Op.gte]: new Date() } }
    });
    
    const openComplaints = await Complain.count({
      where: { ...filter, status: { [Op.in]: ['Open', 'InProgress'] } }
    });
    
    const newEnquiries = await AdmissionEnquiry.count({
      where: { ...filter, status: 'New' }
    });
    
    res.json({
      success: true,
      data: {
        counts: {
          students: studentCount,
          teachers: teacherCount,
          classRooms: classRoomCount,
          grades: gradeCount
        },
        financial: {
          totalRevenue,
          totalReceived,
          pendingAmount
        },
        activities: {
          recentHomework,
          upcomingClasses,
          openComplaints,
          newEnquiries
        }
      }
    });
  } catch (error) { next(error); }
});

// Financial dashboard
router.get('/financial', async (req, res, next) => {
  try {
    const filter = addTenantFilter(req);
    
    // Monthly revenue
    const sixMonthsAgo = new Date();
    sixMonthsAgo.setMonth(sixMonthsAgo.getMonth() - 6);
    
    const invoices = await Invoice.findAll({
      where: { 
        ...filter, 
        type: 'FeesInvoice',
        invoiceDate: { [Op.gte]: sixMonthsAgo }
      },
      attributes: ['invoiceDate', 'totalAmount', 'paidAmount', 'paymentStatus']
    });
    
    // Group by month
    const monthlyData = {};
    invoices.forEach(inv => {
      const month = new Date(inv.invoiceDate).toISOString().slice(0, 7);
      if (!monthlyData[month]) {
        monthlyData[month] = { revenue: 0, collected: 0, pending: 0 };
      }
      monthlyData[month].revenue += parseFloat(inv.totalAmount || 0);
      monthlyData[month].collected += parseFloat(inv.paidAmount || 0);
      monthlyData[month].pending += parseFloat(inv.totalAmount || 0) - parseFloat(inv.paidAmount || 0);
    });
    
    // Payment status breakdown
    const statusCounts = {
      paid: invoices.filter(i => i.paymentStatus === 'Paid').length,
      partial: invoices.filter(i => i.paymentStatus === 'PartiallyPaid').length,
      pending: invoices.filter(i => i.paymentStatus === 'Pending').length
    };
    
    res.json({
      success: true,
      data: {
        monthly: monthlyData,
        statusBreakdown: statusCounts
      }
    });
  } catch (error) { next(error); }
});

// Academic dashboard
router.get('/academic', async (req, res, next) => {
  try {
    const filter = addTenantFilter(req);
    
    // Students per grade
    const grades = await Grade.findAll({
      where: filter,
      include: [{
        model: ClassRoom,
        include: [{ model: Student, attributes: ['id'] }]
      }]
    });
    
    const studentsPerGrade = grades.map(g => ({
      grade: g.name,
      count: g.ClassRooms.reduce((sum, c) => sum + (c.Students?.length || 0), 0)
    }));
    
    // Recent homeworks
    const homeworks = await Homework.findAll({
      where: filter,
      order: [['createdAt', 'DESC']],
      limit: 5
    });
    
    // Upcoming live classes
    const classes = await LiveClass.findAll({
      where: { ...filter, status: 'Scheduled', startDateTime: { [Op.gte]: new Date() } },
      order: [['startDateTime', 'ASC']],
      limit: 5
    });
    
    res.json({
      success: true,
      data: {
        studentsPerGrade,
        recentHomeworks: homeworks,
        upcomingClasses: classes
      }
    });
  } catch (error) { next(error); }
});

// Library dashboard
router.get('/library', async (req, res, next) => {
  try {
    const filter = addTenantFilter(req);
    const { Book, LibraryMember } = require('../models');
    
    const [bookCount, memberCount, overdueCount, borrowedCount] = await Promise.all([
      Book.count({ where: filter }),
      LibraryMember.count({ where: filter }),
      BorrowTransaction.count({ 
        where: { ...filter, status: 'Borrowed', dueDate: { [Op.lt]: new Date() } } 
      }),
      BorrowTransaction.count({ where: { ...filter, status: 'Borrowed' } })
    ]);
    
    res.json({
      success: true,
      data: {
        totalBooks: bookCount,
        totalMembers: memberCount,
        overdueBooks: overdueCount,
        borrowedBooks: borrowedCount
      }
    });
  } catch (error) { next(error); }
});

module.exports = router;
