const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const HomeworkQuestion = sequelize.define('HomeworkQuestion', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    homeworkId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    question: {
      type: DataTypes.TEXT,
      allowNull: false
    },
    type: {
      type: DataTypes.ENUM('Text', 'MultipleChoice', 'TrueFalse', 'FileUpload'),
      defaultValue: 'Text'
    },
    options: DataTypes.JSON,
    correctAnswer: DataTypes.TEXT,
    points: {
      type: DataTypes.DECIMAL(5, 2),
      defaultValue: 10
    },
    orderIndex: {
      type: DataTypes.INTEGER,
      defaultValue: 0
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'homework_questions',
    timestamps: true
  });

  return HomeworkQuestion;
};
