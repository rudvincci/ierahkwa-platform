const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const HomeworkAnswer = sequelize.define('HomeworkAnswer', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    homeworkId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    studentId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    submittedAt: {
      type: DataTypes.DATE,
      defaultValue: DataTypes.NOW
    },
    score: DataTypes.DECIMAL(5, 2),
    feedback: DataTypes.TEXT,
    isGraded: {
      type: DataTypes.BOOLEAN,
      defaultValue: false
    },
    gradedAt: DataTypes.DATE,
    gradedBy: DataTypes.INTEGER,
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'homework_answers',
    timestamps: true,
    indexes: [
      { unique: true, fields: ['homeworkId', 'studentId'] }
    ]
  });

  return HomeworkAnswer;
};
