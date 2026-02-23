const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const HomeworkContent = sequelize.define('HomeworkContent', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    homeworkId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    title: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    type: {
      type: DataTypes.ENUM('Video', 'PDF', 'Word', 'Image', 'Link'),
      allowNull: false
    },
    filePath: {
      type: DataTypes.STRING(500),
      allowNull: false
    },
    description: DataTypes.TEXT,
    orderIndex: {
      type: DataTypes.INTEGER,
      defaultValue: 0
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'homework_contents',
    timestamps: true
  });

  return HomeworkContent;
};
