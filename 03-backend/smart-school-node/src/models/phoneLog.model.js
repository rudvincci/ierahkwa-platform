const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const PhoneLog = sequelize.define('PhoneLog', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    callerName: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    phone: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    callType: {
      type: DataTypes.ENUM('Incoming', 'Outgoing', 'Missed'),
      allowNull: false
    },
    callDate: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW
    },
    duration: DataTypes.INTEGER,
    purpose: DataTypes.STRING(500),
    notes: DataTypes.TEXT,
    followUp: DataTypes.TEXT,
    receivedBy: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'phone_logs',
    timestamps: true
  });

  return PhoneLog;
};
