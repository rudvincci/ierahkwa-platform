const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const LiveClassAttendance = sequelize.define('LiveClassAttendance', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    liveClassId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    studentId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    joinedAt: DataTypes.DATE,
    leftAt: DataTypes.DATE,
    durationMinutes: {
      type: DataTypes.INTEGER,
      defaultValue: 0
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'live_class_attendances',
    timestamps: true,
    indexes: [
      { unique: true, fields: ['liveClassId', 'studentId'] }
    ]
  });

  return LiveClassAttendance;
};
