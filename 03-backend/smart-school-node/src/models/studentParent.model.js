const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const StudentParent = sequelize.define('StudentParent', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    studentId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    parentId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'student_parents',
    timestamps: true,
    indexes: [
      { unique: true, fields: ['studentId', 'parentId'] }
    ]
  });

  return StudentParent;
};
