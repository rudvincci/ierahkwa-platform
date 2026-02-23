const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Fees = sequelize.define('Fees', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    name: {
      type: DataTypes.STRING(200),
      allowNull: false
    },
    description: DataTypes.TEXT,
    amount: {
      type: DataTypes.DECIMAL(18, 2),
      allowNull: false
    },
    schoolYearId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    gradeId: DataTypes.INTEGER,
    type: {
      type: DataTypes.ENUM('Tuition', 'Registration', 'Library', 'Laboratory', 'Sports', 
                           'Transportation', 'Uniform', 'Books', 'Examination', 'Other'),
      defaultValue: 'Tuition'
    },
    isMandatory: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    dueDate: DataTypes.DATEONLY,
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'fees',
    timestamps: true,
    paranoid: true
  });

  return Fees;
};
