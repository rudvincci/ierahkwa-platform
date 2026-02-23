const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Complain = sequelize.define('Complain', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    complainNumber: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    complainerName: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    phone: DataTypes.STRING(50),
    email: DataTypes.STRING(200),
    type: {
      type: DataTypes.ENUM('Academic', 'Behavioral', 'Facility', 'Staff', 'Transport', 'Food', 'Other'),
      allowNull: false
    },
    subject: {
      type: DataTypes.STRING(500),
      allowNull: false
    },
    description: {
      type: DataTypes.TEXT,
      allowNull: false
    },
    complainDate: {
      type: DataTypes.DATEONLY,
      defaultValue: DataTypes.NOW
    },
    status: {
      type: DataTypes.ENUM('Open', 'InProgress', 'Resolved', 'Closed', 'Rejected'),
      defaultValue: 'Open'
    },
    resolution: DataTypes.TEXT,
    resolvedDate: DataTypes.DATEONLY,
    assignedTo: DataTypes.INTEGER,
    studentId: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'complains',
    timestamps: true,
    paranoid: true
  });

  return Complain;
};
