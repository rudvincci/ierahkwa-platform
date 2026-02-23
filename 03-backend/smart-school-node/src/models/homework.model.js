const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Homework = sequelize.define('Homework', {
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
    materialId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    classRoomId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    dueDate: {
      type: DataTypes.DATE,
      allowNull: false
    },
    maxScore: {
      type: DataTypes.DECIMAL(5, 2),
      defaultValue: 100
    },
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'homeworks',
    timestamps: true,
    paranoid: true
  });

  return Homework;
};
