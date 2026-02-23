const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const LiveClass = sequelize.define('LiveClass', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    title: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    description: DataTypes.TEXT,
    teacherId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    classRoomId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    materialId: DataTypes.INTEGER,
    startDateTime: {
      type: DataTypes.DATE,
      allowNull: false
    },
    durationMinutes: {
      type: DataTypes.INTEGER,
      defaultValue: 60
    },
    zoomMeetingId: DataTypes.STRING(100),
    zoomJoinUrl: DataTypes.STRING(500),
    zoomStartUrl: DataTypes.TEXT,
    zoomPassword: DataTypes.STRING(50),
    status: {
      type: DataTypes.ENUM('Scheduled', 'InProgress', 'Completed', 'Cancelled'),
      defaultValue: 'Scheduled'
    },
    recordingUrl: DataTypes.STRING(500),
    isRecorded: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'live_classes',
    timestamps: true,
    paranoid: true
  });

  return LiveClass;
};
