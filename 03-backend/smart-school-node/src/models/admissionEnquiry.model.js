const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const AdmissionEnquiry = sequelize.define('AdmissionEnquiry', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    studentName: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    parentName: DataTypes.STRING(200),
    phone: DataTypes.STRING(50),
    email: DataTypes.STRING(200),
    address: DataTypes.TEXT,
    gradeId: DataTypes.INTEGER,
    enquiryDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    source: DataTypes.STRING(100),
    notes: DataTypes.TEXT,
    status: {
      type: DataTypes.ENUM('New', 'InProgress', 'Converted', 'Lost', 'Closed'),
      defaultValue: 'New'
    },
    followUpDate: DataTypes.DATEONLY,
    assignedTo: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'admission_enquiries',
    timestamps: true,
    paranoid: true
  });

  return AdmissionEnquiry;
};
