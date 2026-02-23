const { DataTypes } = require('sequelize');

module.exports = (sequelize) => {
  const Teacher = sequelize.define('Teacher', {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true
    },
    userId: {
      type: DataTypes.INTEGER,
      allowNull: false
    },
    employeeId: DataTypes.STRING(50),
    firstName: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    lastName: {
      type: DataTypes.STRING(100),
      allowNull: false
    },
    email: DataTypes.STRING(200),
    phone: DataTypes.STRING(50),
    address: DataTypes.TEXT,
    dateOfBirth: DataTypes.DATEONLY,
    gender: DataTypes.STRING(20),
    qualification: DataTypes.STRING(200),
    experience: DataTypes.STRING(100),
    joiningDate: DataTypes.DATEONLY,
    profileImage: DataTypes.STRING(500),
    isActive: {
      type: DataTypes.BOOLEAN,
      defaultValue: true
    },
    tenantId: {
      type: DataTypes.INTEGER,
      allowNull: false
    }
  }, {
    tableName: 'teachers',
    timestamps: true,
    paranoid: true,
    getterMethods: {
      fullName() {
        return `${this.firstName} ${this.lastName}`.trim();
      }
    }
  });

  return Teacher;
};
