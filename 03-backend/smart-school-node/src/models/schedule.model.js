const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Schedule = sequelize.define('Schedule', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    classRoomId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    teacherId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    materialId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    dayOfWeek: {
      type: DataTypes.INTEGER,
      allowNull: false,
      validate: { min: 0, max: 6 }
    },
    startTime: {
      type: DataTypes.TIME,
      allowNull: false
    },
    endTime: {
      type: DataTypes.TIME,
      allowNull: false
    },
    room: DataTypes.STRING(50),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'schedules',
    timestamps: true,
    paranoid: true
  });

  return Schedule;
};
