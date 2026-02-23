const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const QuestionAnswer = sequelize.define('QuestionAnswer', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    homeworkAnswerId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    questionId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    answer: DataTypes.TEXT,
    filePath: DataTypes.STRING(500),
    points: DataTypes.DECIMAL(5, 2),
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'question_answers',
    timestamps: true
  });

  return QuestionAnswer;
};
