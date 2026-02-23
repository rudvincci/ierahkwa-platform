const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const VisitorBook = sequelize.define('VisitorBook', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    visitorName: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    phone: DataTypes.STRING(50),
    email: DataTypes.STRING(200),
    idNumber: DataTypes.STRING(50),
    purpose: DataTypes.STRING(500),
    personToMeet: DataTypes.STRING(200),
    checkInTime: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW
    },
    checkOutTime: DataTypes.DATE,
    badge: DataTypes.STRING(50),
    notes: DataTypes.TEXT,
    studentId: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'visitor_books',
    timestamps: true
  });

  return VisitorBook;
};
