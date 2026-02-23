const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const TeacherMaterial = sequelize.define('TeacherMaterial', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    teacherId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    materialId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'teacher_materials',
    timestamps: true,
    indexes: [
      { unique: true, fields: ['teacherId', 'materialId'] }
    ]
  });

  return TeacherMaterial;
};
